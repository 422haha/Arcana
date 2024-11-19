using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class APIManager : MonoBehaviour
{
    #region DTO Classes
    [System.Serializable]
    public class ApiResponse<T>
    {
        public string message;
        public T data;
    }

    [System.Serializable]
    public class User
    {
        public string email;
        public string nickname;
        public string password;
    }

    [System.Serializable]
    public class LoginRequestDto
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class LoginResponseDto
    {
        public string accessToken;
        public string refreshToken;
        public int userId;
        public string nickname;
        public string language;
    }

    [System.Serializable]
    public class EmailUserDto
    {//비밀번호 찾기 + 회원 탈퇴 용
        public string email;
    }

    [System.Serializable]
    public class CodeCheckDto
    {
        public string email;
        public string authNumber;
    }

    [System.Serializable]
    public class MapSettingRequest
    {
        public string mapSetting;
    }

    [System.Serializable]
    public class MapSettingResponse
    {
        public string mapSetting;
    }

    [System.Serializable]
    public class ProgressResponse
    {
        public string progress;  // S3 저장 경로
    }

    [System.Serializable]
    public class ProgressRequest
    {
        public string progress;  // 저장할 진행 정보 JSON
    }

    [System.Serializable]
    public class PlayerProgress
    {
        public int currentStageIdx;
        public int battleCount;
        public int currentHealth;
        public int maxHealth;
        public int currentGold;
        public List<int> currentDeck;
        public int[] currentPotions;
        public List<int> currentArtifact;
        public bool isThisStageCleared;
        public int currentEnemyIdx;
        public int unlockLevel;
        public int formalStageIdx;
        public int shuffleNumber;
        public List<ProgressManager.Rewards> rewardsList;
        public int currentEventIdx;
        public bool[] isEventShown;
        public int[] currentShopCardsList;
        public int[] currentShopArtifactsList;
        public int[] currentShopPotionsList;
    }

    [System.Serializable]
    public class ProgressWrapper
    {
        public string progress;  // S3 URL
    }
    #endregion

    private static string ec2URL = "https://k11d103.p.ssafy.io/";
    private static string baseURL = "https://localhost:8080/";

    #region Generic API 호출부분
    private IEnumerator SendPostRequest<TRequest, TResponse>(
        string endpoint,
        TRequest requestData,
        Action<TResponse> onSuccess = null,
        Action<string> onError = null,
        bool requiresAuth = false) where TResponse : class
    {
        var jsonData = JsonConvert.SerializeObject(requestData);
        Debug.Log($"Request Data: {jsonData}");

        var webRequest = new UnityWebRequest($"{ec2URL}{endpoint}", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        if (requiresAuth)
        {
            string token = PlayerPrefs.GetString("accessToken", "");
            if (!string.IsNullOrEmpty(token))
            {
                webRequest.SetRequestHeader("Authorization", $"Bearer {token}");
            }
        }

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"API Error: {webRequest.error}");
            Debug.LogError($"Response content: {webRequest.downloadHandler.text}");
            onError?.Invoke(webRequest.error);
            webRequest.Dispose();
            yield break;
        }

        try
        {
            var response = JsonConvert.DeserializeObject<TResponse>(webRequest.downloadHandler.text);
            Debug.Log($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
            onSuccess?.Invoke(response);
        }
        catch (Exception e)
        {
            Debug.LogError($"Response parsing error: {e.Message}");
            Debug.LogError($"Raw response: {webRequest.downloadHandler.text}");
            onError?.Invoke($"Response parsing failed: {e.Message}");
        }
        finally
        {
            webRequest.Dispose();
        }
    }

    private IEnumerator SendGetRequest<TResponse>(
        string endpoint,
        Action<TResponse> onSuccess = null,
        Action<string> onError = null,
        bool requiresAuth = false) where TResponse : class
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}{endpoint}"))
        {
            if (requiresAuth)
            {
                string token = PlayerPrefs.GetString("accessToken", "");
                if (!string.IsNullOrEmpty(token))
                {
                    webRequest.SetRequestHeader("Authorization", $"Bearer {token}");
                }
            }

            yield return webRequest.SendWebRequest();

            HandleResponse(webRequest, onSuccess, onError);
        }
    }

    private void HandleResponse<T>(UnityWebRequest webRequest, Action<T> onSuccess, Action<string> onError) where T : class
    {
        try
        {
            Debug.Log($"Raw Response Content: {webRequest.downloadHandler.text}");

            // 404 상태 코드 특별 처리
            if (webRequest.responseCode == 404)
            {
                Debug.Log("Resource not found (404) - handling as normal case");
                onSuccess?.Invoke(null);
                return;
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error: {webRequest.error}");
                onError?.Invoke(webRequest.error);
                return;
            }

            string responseText = webRequest.downloadHandler.text;
            T response = JsonConvert.DeserializeObject<T>(responseText);

            if (response == null)
            {
                Debug.LogWarning("Empty response data");
                onSuccess?.Invoke(null);  // null 응답도 정상 케이스로 처리
                return;
            }

            onSuccess?.Invoke(response);
        }
        catch (Exception e)
        {
            Debug.LogError($"Response handling error: {e.Message}");
            onError?.Invoke(e.Message);
        }
    }

    private IEnumerator DownloadMapJsonFromS3(string s3Url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(s3Url))
        {
            Debug.Log($"S3에서 맵 데이터 다운로드 시작: {s3Url}");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"S3 맵 데이터 다운로드 실패: {webRequest.error}");
                Debug.LogError($"응답 내용: {webRequest.downloadHandler.text}");
                onError?.Invoke(webRequest.error);
                yield break;
            }

            try
            {
                string jsonContent = webRequest.downloadHandler.text;

                // JSON 유효성 검사
                if (string.IsNullOrEmpty(jsonContent))
                {
                    Debug.LogError("S3에서 받은 JSON 데이터가 비어있습니다.");
                    onError?.Invoke("Empty JSON data from S3");
                    yield break;
                }

                // JSON 형식 검증
                try
                {
                    var testParse = JsonConvert.DeserializeObject(jsonContent);
                    if (testParse == null)
                    {
                        Debug.LogError("잘못된 JSON 형식입니다.");
                        onError?.Invoke("Invalid JSON format");
                        yield break;
                    }
                }
                catch (JsonReaderException e)
                {
                    Debug.LogError($"JSON 파싱 테스트 실패: {e.Message}");
                    onError?.Invoke($"JSON parsing failed: {e.Message}");
                    yield break;
                }

                Debug.Log($"S3에서 맵 데이터 다운로드 성공. 데이터 길이: {jsonContent.Length}");
                Debug.Log($"받은 JSON 데이터: {jsonContent}");
                onSuccess?.Invoke(jsonContent);
            }
            catch (Exception e)
            {
                Debug.LogError($"맵 데이터 처리 중 오류 발생: {e.Message}\n스택 트레이스: {e.StackTrace}");
                onError?.Invoke(e.Message);
            }
        }
    }

    private IEnumerator DownloadProgressFromS3(string s3Url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(s3Url))
        {
            Debug.Log($"Downloading progress from S3: {s3Url}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"S3 download failed: {webRequest.error}");
                onError?.Invoke(webRequest.error);
                yield break;
            }

            try
            {
                string jsonContent = webRequest.downloadHandler.text;
                Debug.Log($"Raw progress data from S3: {jsonContent}");

                if (string.IsNullOrEmpty(jsonContent))
                {
                    Debug.LogError("Empty progress data from S3");
                    onError?.Invoke("Empty progress data");
                    yield break;
                }

                // JSON 파싱 테스트
                var testParse = JsonConvert.DeserializeObject<PlayerProgress>(jsonContent);
                if (testParse == null)
                {
                    Debug.LogError("Invalid progress data format");
                    onError?.Invoke("Invalid data format");
                    yield break;
                }

                onSuccess?.Invoke(jsonContent);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing progress data: {e.Message}");
                onError?.Invoke(e.Message);
            }
        }
    }

    private IEnumerator SendEmptyPostRequest(string endpoint, Action<object> onSuccess = null, Action<string> onError = null, bool requiresAuth = false)
    {
        var webRequest = new UnityWebRequest($"{ec2URL}{endpoint}", "POST");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        if (requiresAuth)
        {
            string token = PlayerPrefs.GetString("accessToken", "");
            if (!string.IsNullOrEmpty(token))
            {
                webRequest.SetRequestHeader("Authorization", $"Bearer {token}");
            }
        }

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"API Error: {webRequest.error}");
            Debug.LogError($"Response content: {webRequest.downloadHandler.text}");
            onError?.Invoke(webRequest.error);
            webRequest.Dispose();
            yield break;
        }

        try
        {
            onSuccess?.Invoke(null);
        }
        catch (Exception e)
        {
            Debug.LogError($"Response handling error: {e.Message}");
            onError?.Invoke(e.Message);
        }
        finally
        {
            webRequest.Dispose();
        }
    }
    #endregion

    #region API 모듈
    public void Login(LoginRequestDto request, Action<LoginResponseDto> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<LoginRequestDto, ApiResponse<LoginResponseDto>>(
            "user/login",
            request,
            response => {
                Debug.Log($"Login Success: {JsonConvert.SerializeObject(response)}");
                onSuccess(response.data);
            },
            error => {
                Debug.LogError($"Login Error: {error}");
                onError(error);
            }
        ));
    }

    public void Logout(Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<object, object>("user/logout", new object(), onSuccess, onError, true));
    }

    public void RegisterUser(User user, Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<User, object>("user/register", user, onSuccess, onError));
    }

    public void VerifyEmail(EmailUserDto request, Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<object, object>("user/verify-email", request, onSuccess, onError));
    }

    public void CheckAuthNumber(CodeCheckDto request, Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<object, object>("user/register/authnumber", request, onSuccess, onError));
    }

    public void DeleteUser(EmailUserDto request, Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<EmailUserDto, object>("user/delete", request, onSuccess, onError, true));
    }

    public void FindUserPassword(EmailUserDto request, Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<EmailUserDto, object>("user/forgot-password", request, onSuccess, onError, true));
    }

    public void SessionStart(Action<object> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendPostRequest<object, ApiResponse<object>>(
            "game-data/start",
            new object(),
            response => {
                Debug.Log("Game session started successfully");
                onSuccess?.Invoke(null);
            },
            error => {
                Debug.LogError($"Failed to start game session: {error}");
                onError?.Invoke(error);
            },
            true
        ));
    }

    public void SaveMapSetting(string mapSettingJson, Action<object> onSuccess, Action<string> onError)
    {
        var request = new MapSettingRequest { mapSetting = mapSettingJson };
        StartCoroutine(SendPostRequest<MapSettingRequest, ApiResponse<object>>(
            "game-data/mapsetting",
            request,
            response => {
                Debug.Log("맵 데이터 저장 성공");
                onSuccess?.Invoke(null);
            },
            error => {
                Debug.LogError($"맵 데이터 저장 실패: {error}");
                onError?.Invoke(error);
            },
            true
        ));
    }

    private IEnumerator UploadMapJson(string mapJson, Action<object> onSuccess, Action<string> onError)
    {
        string tempPath = System.IO.Path.Combine(Application.temporaryCachePath, "mapdata.json");
        UnityWebRequest webRequest = null;

        try
        {
            // JSON 데이터를 파일로 저장
            System.IO.File.WriteAllText(tempPath, mapJson);

            // 파일 업로드를 위한 form 데이터 생성
            WWWForm form = new WWWForm();
            byte[] fileData = System.IO.File.ReadAllBytes(tempPath);
            form.AddBinaryData("file", fileData, "mapdata.json", "application/json");

            // UnityWebRequest 생성
            webRequest = UnityWebRequest.Post($"{ec2URL}game-data/mapsetting", form);
            string token = PlayerPrefs.GetString("accessToken", "");
            if (!string.IsNullOrEmpty(token))
            {
                webRequest.SetRequestHeader("Authorization", $"Bearer {token}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error preparing map file upload: {e.Message}");
            if (System.IO.File.Exists(tempPath))
            {
                try
                {
                    System.IO.File.Delete(tempPath);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to delete temp file: {ex.Message}");
                }
            }
            onError?.Invoke(e.Message);
            yield break;
        }

        // 웹 요청 실행 (try-catch 밖에서)
        yield return webRequest.SendWebRequest();

        // 임시 파일 삭제
        try
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to delete temp file: {e.Message}");
        }

        // 응답 처리
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Map file uploaded successfully");
            onSuccess?.Invoke(null);
        }
        else
        {
            Debug.LogError($"Failed to upload map file: {webRequest.error}");
            onError?.Invoke(webRequest.error);
        }

        // 웹 요청 정리
        webRequest.Dispose();
    }

    public void LoadMapSetting(Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendGetRequest<ApiResponse<MapSettingResponse>>(
            "game-data/mapsetting",
            response => {
                if (!string.IsNullOrEmpty(response?.data?.mapSetting))
                {
                    Debug.Log($"맵 데이터 S3 URL 수신: {response.data.mapSetting}");
                    // S3 URL을 받았으면 해당 URL에서 JSON 다운로드
                    StartCoroutine(DownloadMapJsonFromS3(
                        response.data.mapSetting,
                        onSuccess,
                        onError
                    ));
                }
                else
                {
                    Debug.Log("맵 데이터가 없습니다.");
                    onSuccess(null);
                }
            },
            error => {
                Debug.LogError($"맵 데이터 URL 로드 실패: {error}");
                onError(error);
            },
            true  // 서버 API 호출시에만 인증 필요
        ));
    }

    public void LoadProgress(Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendGetRequest<ApiResponse<ProgressWrapper>>(
            "game-data/progress",
            response => {
                // 404는 정상적인 "데이터 없음" 상태로 처리
                if (response?.data?.progress == null)
                {
                    Debug.Log("No progress data exists - normal state for new players");
                    onSuccess?.Invoke(null);
                    return;
                }

                StartCoroutine(DownloadProgressFromS3(
                    response.data.progress,
                    onSuccess,
                    onError
                ));
            },
            error => {
                // 404는 정상 처리, 다른 에러만 에러로 처리
                if (error.Contains("404"))
                {
                    Debug.Log("No progress data exists - normal state for new players");
                    onSuccess?.Invoke(null);
                }
                else
                {
                    onError?.Invoke(error);
                }
            },
            true
        ));
    }

    public void SaveProgress(PlayerProgress progress, Action<object> onSuccess, Action<string> onError)
    {
        try
        {
            var progressJson = JsonConvert.SerializeObject(progress);
            Debug.Log($"Saving progress JSON: {progressJson}");

            var request = new ProgressRequest { progress = progressJson };

            StartCoroutine(SendPostRequest<ProgressRequest, ApiResponse<object>>(
                "game-data/progress",
                request,
                response => {
                    Debug.Log("Progress saved successfully");
                    onSuccess?.Invoke(null);
                },
                error => {
                    Debug.LogError($"Failed to save progress: {error}");
                    onError?.Invoke(error);
                },
                true
            ));
        }
        catch (Exception e)
        {
            Debug.LogError($"Error preparing progress data: {e.Message}");
            onError?.Invoke(e.Message);
        }
    }

    public void SaveCurrentProgress()
    {
        if (Hub.ProgressManager == null)
        {
            Debug.LogError("ProgressManager is null");
            return;
        }

        var progress = new PlayerProgress
        {
            currentStageIdx = Hub.ProgressManager.CurrentStageIdx,
            battleCount = Hub.ProgressManager.battleCount,
            currentHealth = Hub.ProgressManager.CurrentHealth,
            maxHealth = Hub.ProgressManager.MaxHealth,
            currentGold = Hub.ProgressManager.CurrentGold,
            currentDeck = Hub.ProgressManager.currentDeck,
            currentPotions = Hub.ProgressManager.currentPotions,
            currentArtifact = Hub.ProgressManager.currentArtifact,
            isThisStageCleared = Hub.ProgressManager.IsThisStageCleared,
            currentEnemyIdx = Hub.ProgressManager.currentEnemyIdx,
            unlockLevel = Hub.ProgressManager.unlockLevel,
            formalStageIdx = Hub.ProgressManager.formalStageIdx,
            shuffleNumber = Hub.ProgressManager.shuffleNumber,
            rewardsList = Hub.ProgressManager.rewardsList,
            currentEventIdx = Hub.ProgressManager.currentEventIdx,
            isEventShown = Hub.ProgressManager.isEventShown,
            currentShopCardsList = Hub.ProgressManager.currentShopCardsList,
            currentShopArtifactsList = Hub.ProgressManager.currentShopArtifactsList,
            currentShopPotionsList = Hub.ProgressManager.currentShopPotionsList
        };

        SaveProgress(progress,
            _ => Debug.Log("Current progress saved successfully"),
            error => Debug.LogError($"Failed to save current progress: {error}")
        );
    }

    public void SendPasswordResetEmail(string email, Action<object> onSuccess, Action<string> onError)
    {
        // query parameter로 email 전송
        string endpoint = $"user/forgot-password?email={UnityWebRequest.EscapeURL(email)}";

        StartCoroutine(SendEmptyPostRequest(
            endpoint,
            response => {
                Debug.Log("Password reset email sent successfully");
                onSuccess?.Invoke(null);
            },
            error => {
                Debug.LogError($"Failed to send password reset email: {error}");
                onError?.Invoke(error);
            }
        ));
    }

    #endregion
}