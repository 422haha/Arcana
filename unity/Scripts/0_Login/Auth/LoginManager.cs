using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;
using System.Linq;
using Map;
using System.Collections.Generic;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    private bool isLogining = false;
    private bool isRegistered = false;
    private readonly string restrictedScenes = "1_Main";

    [Header("로그인 상태 오브젝트")]
    public Lean.Localization.LeanPhrase loginStatus;
    [Header("비밀번호 찾기 상태 오브젝트")]
    public Lean.Localization.LeanPhrase pwFindStatus;

    #region Authentication
    public void ProcessLogin(string email, string password, TMP_Text errorText)
    {
        if (isLogining)
        {
            Debug.Log("이미 로그인 진행 중입니다.");
            return;
        }
        isLogining = true;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowLoginError(errorText);
            return;
        }

        var request = new APIManager.LoginRequestDto
        {
            email = email,
            password = password
        };

        Hub.APIManager.Login(
            request,
            OnLoginSuccess,
            (error) => OnLoginError(error, errorText)
        );
    }

    public void ProcessLogout(string currentMapJson = null)
    {
        Hub.NetworkManager.SaveGameData(Hub.MapManager?.CurrentMap);
        ExecuteLogout();
    }

    private void ExecuteLogout()
    {
        Hub.APIManager.Logout(
            onSuccess: _ =>
            {
                ClearPlayerData();
                Debug.Log("로그아웃 됨");
            },
            onError: error =>
            {
                Debug.LogError($"로그아웃 실패: {error}");
                ClearPlayerData();
            }
        );
    }

    public void ProcessRegister(string email, string password, string nickname)
    {
        if (!Hub.NetworkManager.IsRegistrationValid())
        {
            ShowRegisterError("모든 인증 절차를 완료해주세요.", null);
            return;
        }

        var user = new APIManager.User
        {
            email = email,
            password = password,
            nickname = nickname
        };

        Hub.APIManager.RegisterUser(user,
            onSuccess: _ => {
                isRegistered = true;
                Debug.Log("회원가입 성공");
            },
            onError: error => {
                isRegistered = false;
                Debug.LogError($"회원가입 실패: {error}");
            }
        );
    }

    public void RequestEmailVerification(string email, TMP_Text statusText)
    {
        if (!string.IsNullOrEmpty(email))
        {
            // 소리
            Hub.SoundManager.SfxSelectPlay(0);
            Hub.NetworkManager.RequestVerifyCode(email, statusText);
        }
    }

    public void VerifyEmailCode(string email, string code, TMP_Text statusText)
    {
        if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
        {
            // 소리
            Hub.SoundManager.SfxSelectPlay(0);
            Hub.NetworkManager.VerifyCode(email, code, statusText);
        }
    }
    #endregion

    #region Login Success Handling
    private void OnLoginSuccess(APIManager.LoginResponseDto response)
    {
        SavePlayerData(response);
        Debug.Log("로그인 성공 - 데이터 로드 시작");
        StartCoroutine(LoadGameDataAndMoveToMain());
    }

    private IEnumerator LoadGameDataAndMoveToMain()
    {
        bool success = false;
        yield return StartCoroutine(Hub.NetworkManager.LoadGameData((result) => success = result));

        if (!success)
        {
            Debug.LogWarning("게임 데이터 로드 실패 또는 새로운 사용자");
        }

        // 씬 전환
        Hub.TransitionManager.MoveTo(1, 1, 1);

        // UI 초기화를 위한 대기 추가
        yield return new WaitForSeconds(0.5f);

        // 맵 초기화
        if (Hub.MapManager != null)
        {
            Debug.Log("로그인 후 맵 초기화 시작");
            Hub.MapManager.InitializeMapAfterLoad();
        }
    }

    public void ContinueGame()
    {
        // 게임 상태 검증
        if (Hub.ProgressManager == null || Hub.MapManager?.CurrentMap == null)
        {
            Debug.LogError("Required managers not found");
            Hub.TransitionManager.MoveTo(2, 1, 1);
            return;
        }

        try
        {
            var currentPath = Hub.MapManager.CurrentMap.path;
            if (currentPath == null)
            {
                Debug.LogError("Map path is null");
                Hub.TransitionManager.MoveTo(2, 1, 1);
                return;
            }

            int currentStageIdx = Hub.ProgressManager.CurrentStageIdx;
            if (currentStageIdx <= 0 || currentStageIdx > currentPath.Count)
            {
                Debug.LogError($"Invalid stage index: {currentStageIdx}");
                Hub.TransitionManager.MoveTo(2, 1, 1);
                return;
            }

            var currentPoint = currentPath[currentStageIdx - 1];
            var currentNode = Hub.MapManager.CurrentMap.GetNode(currentPoint);
            if (currentNode == null)
            {
                Debug.LogError("Current node not found");
                Hub.TransitionManager.MoveTo(2, 1, 1);
                return;
            }

            // 노드 타입에 따른 씬 전환
            switch (currentNode.nodeType)
            {
                case NodeType.MinorEnemy:
                case NodeType.EliteEnemy:
                case NodeType.Boss:
                    Hub.TransitionManager.MoveTo(3, 1, 1);
                    break;
                case NodeType.RestSite:
                    Hub.TransitionManager.MoveTo(5, 1, 1);
                    break;
                case NodeType.Store:
                    Hub.TransitionManager.MoveTo(6, 1, 1);
                    break;
                case NodeType.Mystery:
                    Hub.TransitionManager.MoveTo(4, 1, 1);
                    break;
                case NodeType.Treasure:
                    Hub.TransitionManager.MoveTo(2, 1, 1);
                    break;
                default:
                    Hub.TransitionManager.MoveTo(2, 1, 1);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in scene transition: {e.Message}");
            Hub.TransitionManager.MoveTo(2, 1, 1);
        }
    }
    #endregion

    #region Helper Methods
    private void SavePlayerData(APIManager.LoginResponseDto response)
    {
        PlayerPrefs.SetInt("userId", response.userId);
        PlayerPrefs.SetString("accessToken", response.accessToken);
        PlayerPrefs.SetString("refreshToken", response.refreshToken);
        PlayerPrefs.SetString("nickName", response.nickname);
        PlayerPrefs.SetString("language", response.language);
        PlayerPrefs.Save();
        Debug.Log($"사용자 데이터 저장 완료 - UserId: {response.userId}, Nickname: {response.nickname}");
    }

    private void ClearPlayerData()
    {
        isLogining = false;
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("accessToken");
        PlayerPrefs.DeleteKey("refreshToken");
        PlayerPrefs.DeleteKey("nickName");
        PlayerPrefs.Save();
        Debug.Log("사용자 데이터 삭제 완료");
    }

    private void OnLoginError(string error, TMP_Text errorText)
    {
        ShowLoginError(errorText);
        Debug.LogError($"로그인 실패: {error}");
    }

    private void ShowLoginError(TMP_Text errorText)
    {
        isLogining = false;
        if (errorText != null)
        {
            Hub.TranslationManager.AddTranslation(loginStatus, errorText,
                "로그인 실패", "Login failed");
            errorText.color = Color.red;
        }
        Debug.LogError("로그인 실패.");
    }

    private void ShowRegisterError(string message, TMP_Text errorText)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.red;
        }
        Debug.LogError(message);
    }

    public bool IsRegistered()
    {
        return isRegistered;
    }

    private void OnApplicationQuit()
    {
        Hub.NetworkManager.SaveGameData(Hub.MapManager?.CurrentMap);
        ExecuteLogout();
    }

    public void RequestPasswordReset(string email, TMP_Text statusText)
    {
        if (string.IsNullOrEmpty(email))
        {
            ShowResetPasswordError("이메일을 입력해주세요.", "email needed",statusText);
            return;
        }

        Hub.APIManager.SendPasswordResetEmail(
            email,
            _ => {
                if (statusText != null)
                {
                    statusText.color = Color.green;
                    Hub.TranslationManager.AddTranslation(pwFindStatus, statusText, "비밀번호 재설정 메일 발송완료.", "Email send success");
                }
            },
            error => {
                ShowResetPasswordError("비밀번호 재설정 메일 발송실패.", "Email send failed", statusText);
                Debug.LogError($"Password reset failed: {error}");
            }
        );
    }

    private void ShowResetPasswordError(string koMessage, string enMessage, TMP_Text errorText)
    {
        if (errorText != null)
        {
            errorText.color = Color.red;
            Hub.TranslationManager.AddTranslation(pwFindStatus, errorText, koMessage, enMessage);
        }
        Debug.LogError(koMessage);
    }
    #endregion
}