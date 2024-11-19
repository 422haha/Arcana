using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;
using Map;

public class NetworkManager : MonoBehaviour
{
    #region Status Fields
    private bool emailSendCheck = false;
    private bool registerEmailCheck = false;

    [Header("Status Objects")]
    public Lean.Localization.LeanPhrase emailSendStatus;
    public Lean.Localization.LeanPhrase emailAuthStatus;
    public Lean.Localization.LeanPhrase pwConfirmStatus;
    public Lean.Localization.LeanPhrase loadingStatus;
    #endregion

    #region Authentication
    public void RequestVerifyCode(string email, TMP_Text statusText)
    {
        if (string.IsNullOrEmpty(email)) return;

        //ShowLoadingStatus(statusText, "전송중...", "Sending...");

        var email_dto = new APIManager.EmailUserDto { email = email };
        Hub.APIManager.VerifyEmail(email_dto,
            onSuccess: _ => {
                ShowSuccessStatus(statusText, emailSendStatus,
                    "인증 코드가 발송되었습니다.", "Check your email");
                emailSendCheck = true;
            },
            onError: error => {
                ShowErrorStatus(statusText, emailSendStatus,
                    "인증 코드 발송에 실패했습니다.", "Email send failed");
                emailSendCheck = false;
                Debug.LogError($"인증 코드 발송 실패: {error}");
            }
        );
    }

    public void VerifyCode(string email, string code, TMP_Text statusText)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(email)) return;

        var code_dto = new APIManager.CodeCheckDto { email = email, authNumber = code };
        Hub.APIManager.CheckAuthNumber(code_dto,
            onSuccess: _ => {
                ShowSuccessStatus(statusText, emailAuthStatus,
                    "인증이 완료되었습니다.", "Verification completed");
                registerEmailCheck = true;
            },
            onError: error => {
                ShowErrorStatus(statusText, emailAuthStatus,
                    "인증에 실패했습니다.", "Verification failed");
                registerEmailCheck = false;
                Debug.LogError($"인증 실패: {error}");
            }
        );
    }

    public bool IsEmailSended() => emailSendCheck;
    public bool IsRegistrationValid() => registerEmailCheck;
    #endregion

    #region Game Data Management
    public IEnumerator LoadGameData(Action<bool> onComplete = null)
    {
        // 1. 진행 정보 로드
        bool progressLoaded = false;
        string progressError = null;
        APIManager.PlayerProgress loadedProgress = null;

        Hub.APIManager.LoadProgress(
            progressJson => {
                try
                {
                    if (!string.IsNullOrEmpty(progressJson))
                    {
                        var wrapper = JsonConvert.DeserializeObject<APIManager.ProgressWrapper>(progressJson);
                        if (wrapper?.progress != null)
                        {
                            loadedProgress = JsonConvert.DeserializeObject<APIManager.PlayerProgress>(wrapper.progress);

                            // 진행 정보 로드 성공 시 바로 적용
                            if (loadedProgress != null)
                            {
                                Hub.ProgressManager.LoadProgressData(loadedProgress);
                                Debug.Log($"Progress loaded - Stage: {Hub.ProgressManager.CurrentStageIdx}");
                            }
                        }
                    }
                    progressLoaded = true;
                }
                catch (Exception e)
                {
                    progressError = e.Message;
                    progressLoaded = true;
                    Debug.LogError($"진행 상황 파싱 에러: {e.Message}");
                }
            },
            error => {
                progressError = error;
                progressLoaded = true;
                Debug.LogError($"진행 상황 저장 실패: {error}");
            }
        );

        float timeout = 5f;
        float elapsed = 0f;
        while (!progressLoaded && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!progressLoaded || !string.IsNullOrEmpty(progressError))
        {
            onComplete?.Invoke(false);
            yield break;
        }

        // 2. 맵 데이터 로드
        bool mapLoaded = false;
        string mapError = null;
        Map.Map loadedMap = null;

        Hub.APIManager.LoadMapSetting(
            mapJson => {
                try
                {
                    if (!string.IsNullOrEmpty(mapJson))
                    {
                        var mapResponse = JsonConvert.DeserializeObject<APIManager.MapSettingResponse>(mapJson);
                        if (mapResponse?.mapSetting != null)
                        {
                            loadedMap = JsonConvert.DeserializeObject<Map.Map>(mapResponse.mapSetting,
                                new JsonSerializerSettings
                                {
                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                    NullValueHandling = NullValueHandling.Include
                                });

                            if (loadedMap != null)
                            {
                                loadedMap.path = loadedMap.path ?? new List<Point>();
                                PlayerPrefs.SetString("Map", mapResponse.mapSetting);
                                PlayerPrefs.Save();
                            }
                        }
                    }
                    mapLoaded = true;
                }
                catch (Exception e)
                {
                    mapError = e.Message;
                    mapLoaded = true;
                    Debug.LogError($"맵 파싱 에러: {e.Message}");
                }
            },
            error => {
                mapError = error;
                mapLoaded = true;
                Debug.LogError($"맵 로드 실패: {error}");
            }
        );

        elapsed = 0f;
        while (!mapLoaded && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!mapLoaded || !string.IsNullOrEmpty(mapError))
        {
            onComplete?.Invoke(false);
            yield break;
        }

        if (loadedMap != null && Hub.MapManager != null)
        {
            Hub.MapManager.SetCurrentMap(loadedMap);
            onComplete?.Invoke(true);
        }
        else
        {
            onComplete?.Invoke(false);
        }
    }

    #region Save Management
    private bool isSaving = false;
    private float saveDebounceTime = 1.0f;
    private float lastSaveTime = 0f;

    public void SaveGameData(Map.Map currentMap = null)
    {
        if (Time.time - lastSaveTime < saveDebounceTime)
        {
            Debug.Log("Save operation debounced");
            return;
        }

        if (!isSaving)
        {
            StartCoroutine(SaveGameDataCoroutine(currentMap));
        }
        else
        {
            Debug.Log("Save operation already in progress");
        }
    }

    private IEnumerator SaveGameDataCoroutine(Map.Map currentMap)
    {
        if (isSaving) yield break;

        isSaving = true;
        lastSaveTime = Time.time;

        try
        {
            // 1. 진행 정보 저장
            if (Hub.ProgressManager != null)
            {
                var progress = new APIManager.PlayerProgress
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
                    // 상점 관련 데이터 추가
                    currentShopCardsList = Hub.ProgressManager.currentShopCardsList,
                    currentShopArtifactsList = Hub.ProgressManager.currentShopArtifactsList,
                    currentShopPotionsList = Hub.ProgressManager.currentShopPotionsList
                };

                yield return StartCoroutine(SaveProgressCoroutine(progress));
            }

            // 2. 맵 데이터 저장
            if (currentMap != null)
            {
                yield return StartCoroutine(SaveMapCoroutine(currentMap));
            }
        }
        finally
        {
            isSaving = false;
        }
    }

    private class SaveOperation
    {
        public bool IsComplete { get; set; }
        public Exception Error { get; set; }
    }

    private IEnumerator SaveProgressCoroutine(APIManager.PlayerProgress progress)
    {
        var operation = new SaveOperation();

        Hub.APIManager.SaveProgress(progress,
            _ => operation.IsComplete = true,
            error => {
                operation.Error = new Exception(error);
                operation.IsComplete = true;
            }
        );

        yield return StartCoroutine(WaitForOperation(operation, 5f));

        if (operation.Error != null)
        {
            Debug.LogError($"Progress save failed: {operation.Error.Message}");
        }
    }

    private IEnumerator SaveMapCoroutine(Map.Map currentMap)
    {
        string mapJson = null;

        try
        {
            mapJson = JsonConvert.SerializeObject(currentMap,
                Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error serializing map data: {e.Message}");
            yield break;
        }

        if (string.IsNullOrEmpty(mapJson))
        {
            Debug.LogError("Map serialization resulted in null or empty string");
            yield break;
        }

        var operation = new SaveOperation();

        Hub.APIManager.SaveMapSetting(mapJson,
            _ => operation.IsComplete = true,
            error => {
                operation.Error = new Exception(error);
                operation.IsComplete = true;
            }
        );

        yield return StartCoroutine(WaitForOperation(operation, 5f));

        if (operation.Error != null)
        {
            Debug.LogError($"Map save failed: {operation.Error.Message}");
        }
    }

    private IEnumerator WaitForOperation(SaveOperation operation, float timeout)
    {
        float elapsed = 0f;
        while (!operation.IsComplete && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!operation.IsComplete)
        {
            Debug.LogWarning($"Operation timed out after {timeout} seconds");
        }
    }

    public void OnApplicationQuit()
    {
        if (!isSaving && Hub.MapManager?.CurrentMap != null)
        {
            string mapJson = null;
            try
            {
                mapJson = JsonConvert.SerializeObject(Hub.MapManager.CurrentMap,
                    Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                );

                if (!string.IsNullOrEmpty(mapJson))
                {
                    Hub.APIManager.SaveMapSetting(mapJson, null,
                        error => Debug.LogError($"Final save failed: {error}"));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in final save: {e.Message}");
            }
        }
    }
    #endregion

    public IEnumerator StartNewGameSession(Action<bool> onComplete = null)
    {
        bool sessionStarted = false;
        bool success = true;

        Hub.APIManager.SessionStart(
            _ => sessionStarted = true,
            error => {
                Debug.LogError($"세션 시작 실패했데이: {error}");
                sessionStarted = true;
                success = false;
            }
        );

        float timeout = 5f;
        float elapsed = 0f;
        while (!sessionStarted && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!success)
        {
            onComplete?.Invoke(false);
            yield break;
        }

        if (Hub.MapManager != null)
        {
            Hub.MapManager.GenerateNewMap();
            SaveGameData(Hub.MapManager.CurrentMap);
            onComplete?.Invoke(true);
        }
        else
        {
            onComplete?.Invoke(false);
        }
    }
    #endregion

    #region Scene Management
    public void DetermineAndLoadScene()
    {
        try
        {
            if (Hub.ProgressManager == null || Hub.MapManager?.CurrentMap == null)
            {
                Debug.LogError("필요한 매니저가 없다 안 카나");
                LoadDefaultScene();
                return;
            }

            var currentPath = Hub.MapManager.CurrentMap.path;
            if (currentPath == null || Hub.ProgressManager.CurrentStageIdx > currentPath.Count)
            {
                Debug.LogError("유효하지 않은 경로 및 인ㄷㄱ스");
                LoadDefaultScene();
                return;
            }

            var currentPoint = currentPath[Hub.ProgressManager.CurrentStageIdx - 1];
            var currentNode = Hub.MapManager.CurrentMap.GetNode(currentPoint);

            if (currentNode == null)
            {
                Debug.LogError("현재 노드 파악 안 됨");
                LoadDefaultScene();
                return;
            }

            LoadSceneByNodeType(currentNode.nodeType);
        }
        catch (Exception e)
        {
            Debug.LogError($"씬 전환 간 에러 발생: {e.Message}");
            LoadDefaultScene();
        }
    }

    private void LoadSceneByNodeType(NodeType nodeType)
    {
        switch (nodeType)
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
                LoadDefaultScene();
                break;
        }
    }

    private void LoadDefaultScene()
    {
        Hub.TransitionManager.MoveTo(2, 1, 1);
    }
    #endregion

    #region UI Helpers
    private void ShowLoadingStatus(TMP_Text statusText, string korText, string engText)
    {
        if (statusText != null)
        {
            statusText.color = Color.yellow;
            Hub.TranslationManager.AddTranslation(loadingStatus, statusText, korText, engText);
        }
    }

    private void ShowSuccessStatus(TMP_Text statusText, Lean.Localization.LeanPhrase phrase,
        string korText, string engText)
    {
        if (statusText != null)
        {
            statusText.color = Color.green;
            Hub.TranslationManager.AddTranslation(phrase, statusText, korText, engText);
        }
    }

    private void ShowErrorStatus(TMP_Text statusText, Lean.Localization.LeanPhrase phrase,
        string korText, string engText)
    {
        if (statusText != null)
        {
            statusText.color = Color.red;
            Hub.TranslationManager.AddTranslation(phrase, statusText, korText, engText);
        }
    }
    #endregion
}