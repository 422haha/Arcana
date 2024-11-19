using System;
using System.Collections;
using System.Collections.Generic;
using Map;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [Header("카드들 부모")]
    public GameObject cardParent;

    [Header("카드 생성위치")]
    public Transform firstCardPos;
    public Transform secondCardPos;
    public Transform thirdCardPos;

    [Header("새로 시작 카드")]
    public GameObject windowNewGame;

    [Header("이어서 하기 카드")]
    public GameObject windowLoadGame;

    [Header("세팅창 카드")]
    public GameObject windowSetting;

    [Header("세팅창")]
    public GameObject settingWindow;

    [Header("세팅창 닫기 버튼")]
    public Button settingCloseButton;

    [Header("크레딧창")]
    public GameObject windowCredit;

    [Header("크레딧창 토글 버튼")]
    public Button creditButton;

    [Header("주의 창")]
    public GameObject warningWindow;
    public Button yesButton;
    public Button noButton;

    [Header("메인씬 음악")]
    public AudioClip mainSceneBgm;

    private Button newGameBtn;
    private Button loadGameBtn;
    private Button settingBtn;

    // 크레딧 창 활성화 상태
    private bool isCreditOpen = false;

    void Awake()
    {
        // 브금 바꾸기
        Hub.SoundManager.BgmSelectPlay(1);
    }

    void Start()
    {
        SetupCards();
        AttachEvents();
    }

    void Update()
    {
        // 만약 크레딧 창이 켜져있고, ESC가 눌렸다면
        if (isCreditOpen && Input.GetKeyDown(KeyCode.Escape))
            // 크레딧 닫기
            ToggleCredit();
    }

    #region 크레딧
    // 크레딧 키거나 닫는 메서드
    void ToggleCredit()
    {
        Hub.SoundManager.SfxSelectPlay(0);

        // 크레딧 창이 켜져있다면 -> 닫기 위해 버튼을 눌렀다면
        // 크레딧 위치를 초기화
        if (isCreditOpen)
        {
            Transform creditPos = windowCredit.transform.GetChild(0).transform;
            creditPos.localPosition = new Vector2(0f, 0f);
        }

        windowCredit.SetActive(!windowCredit.activeSelf);
        isCreditOpen = !isCreditOpen;
    }
    #endregion

    #region 카드 배치 로직
    private void SetupCards()
    {
        GameObject setting;
        GameObject newGame;
        GameObject loadGame;

        // 환경설정은 위치 고정
        setting = Instantiate(
            windowSetting,
            thirdCardPos.position,
            Quaternion.identity,
            cardParent.transform
        );

        // 저장된 스테이지 값이 없다면 (처음 게임을 시작하는 거라면)
        if (Hub.ProgressManager.CurrentStageIdx <= 0)
        {
            // 새로 시작이 가운데로 오도록
            newGame = Instantiate(
                windowNewGame,
                secondCardPos.position,
                Quaternion.identity,
                cardParent.transform
            );
            newGame.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            loadGame = Instantiate(
                windowLoadGame,
                firstCardPos.position,
                Quaternion.identity,
                cardParent.transform
            );

            DisableLoadGame(loadGame);
        }
        else
        {
            // 이어하기가 가운데로 오도록
            loadGame = Instantiate(
                windowLoadGame,
                secondCardPos.position,
                Quaternion.identity,
                cardParent.transform
            );
            loadGame.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            newGame = Instantiate(
                windowNewGame,
                firstCardPos.position,
                Quaternion.identity,
                cardParent.transform
            );
        }

        newGameBtn = newGame.GetComponent<Button>();
        loadGameBtn = loadGame.GetComponent<Button>();
        settingBtn = setting.GetComponent<Button>();
    }

    // 이어하기 비활성화 메서드
    void DisableLoadGame(GameObject loadGame)
    {
        A_HoverScaleChange scaleChanger = loadGame.GetComponent<A_HoverScaleChange>();
        Image[] images = loadGame.GetComponentsInChildren<Image>();
        TMP_Text text = loadGame.GetComponentInChildren<TMP_Text>();
        Destroy(scaleChanger);
        text.color = Color.grey;
                foreach (Image image in images)
                {
                    image.color = Color.gray;
                }
    }
#endregion

#region 버튼 이벤트
void AttachEvents()
    {
        creditButton.onClick.AddListener(ToggleCredit);
        settingBtn.onClick.AddListener(ToggleSetting);
        settingCloseButton.onClick.AddListener(ToggleSetting);
        newGameBtn.onClick.AddListener(ToggleWarningWindow);
        yesButton.onClick.AddListener(NewGameStart);
        noButton.onClick.AddListener(ToggleWarningWindow);
        loadGameBtn.onClick.AddListener(LoadGame);
    }

    public void ToggleSetting()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        settingWindow.SetActive(!settingWindow.activeSelf);
    }

    //public void ButtonSetting()
    //{
    //    Hub.SingletonUIManager.escMenu.SetActive(true);
    //}

    private void ToggleWarningWindow()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        warningWindow.SetActive(!warningWindow.activeSelf);
    }

    #endregion

    #region 통신
    private void LoadGame()
    {

        if (Hub.ProgressManager.CurrentStageIdx <= 0)
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다. 새 게임을 시작해주세요.");
            // 선택적: 사용자에게 알림 메시지 표시
            return;
        }

        Hub.SoundManager.SfxSelectPlay(0);
        Hub.LoginManager.ContinueGame();

        Hub.ProgressManager.ClearAPC();
        Hub.ProgressManager.LoadAPC();          // 여기서 아티펙트, 포션, 카드 다시 가져 옴

    }

    //private IEnumerator ProcessLoadGame()
    //{
    //    // LoginManager의 ProcessLoadGameData 메서드 호출
    //    yield return StartCoroutine(Hub.LoginManager.ProcessLoadGameData());
    //}

    private void NewGameStart()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        StartCoroutine(ProcessNewGameStart());
    }

    private IEnumerator ProcessNewGameStart()
    {
        // 1. 기존 상태 정리
        try
        {
            if (Hub.MapManager != null)
            {
                Hub.MapManager.HideMap();
                Hub.MapManager.SetCurrentMap(null);
            }
            Hub.ProgressManager.InitializeProgress();
            Hub.ProgressManager.LoadAPC();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error clearing existing state: {e.Message}");
            yield break;
        }

        // 2. 새 게임 시작
        bool sessionSuccess = true;
        yield return StartCoroutine(
            Hub.NetworkManager.StartNewGameSession((success) => sessionSuccess = success)
        );

        if (!sessionSuccess)
        {
            Debug.LogError("Failed to start new game session");
            yield break;
        }

        // 3. 씬 전환
        try
        {
            Hub.TransitionManager.MoveTo(2, 1, 1);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error transitioning scene: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        Hub.NetworkManager.SaveGameData(Hub.MapManager?.CurrentMap);
    }
    #endregion
}
