using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingletonUIManager : MonoBehaviour
{
    [Header("메인 부분")]
    public GameObject windowDescription;
    public GameObject windowDialog;
    public GameObject windowMyDeck;
    public GameObject windowMap;
    public GameObject escMenu;
    public GameObject windowHeader;

    [Header("헤더 부분")]
    public TMP_Text userName;
    public int intCurrentHP;
    public int intMaxHP;
    public TMP_Text tmpHP;
    public TMP_Text tmpGold;
    public TMP_Text tmpCurrentStage;    
    public Transform[] potionsTransform = new Transform[3];
    public GameObject potionClosePanel;
    public Transform articfactTransform;
    public GameObject artifactUnit;
    public GameObject arrowForHeader;
    public Transform arrowP1;
    public Transform arrowP2;

    [Header("덱 부분")]
    public Transform cardListTransform;


    [HideInInspector]
    // 번역 가능 여부
    public bool isLanguageDropdownDisable;

    [Header("버튼들")]
    public Button[] buttons;

    public void Awake()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => Hub.SoundManager.SfxSelectPlay(0));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonEscMenu();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환시 모든 UI 초기화
        ResetUI();

        // EventSystem 재설정
        StartCoroutine(ResetEventSystem());
    }

    private IEnumerator ResetEventSystem()
    {
        yield return new WaitForSeconds(0.1f);

        // EventSystem 찾아서 재활성화
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.enabled = false;
            eventSystem.enabled = true;
        }

        // 모든 버튼 컴포넌트 재활성화
        var buttons = GetComponentsInChildren<UnityEngine.UI.Button>(true);
        foreach (var button in buttons)
        {
            button.enabled = false;
            button.enabled = true;
        }
    }

    private void ResetUI()
    {
        if (windowDialog != null) windowDialog.SetActive(false);
        if (windowMyDeck != null) windowMyDeck.SetActive(false);
        if (escMenu != null) escMenu.SetActive(false);
        if (windowMap != null) windowMap.SetActive(false);

        // Header는 씬에 따라 설정
        if (windowHeader != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            bool showHeader = !currentScene.Equals("0_Login") &&
                            !currentScene.Equals("1_Main") &&
                            !currentScene.Equals("4_Ending");
            windowHeader.SetActive(showHeader);
        }
    }

    

    public void TurnOffFours()
    {
        windowDialog.SetActive(false);
        windowMyDeck.SetActive(false);
        escMenu.SetActive(false);
        windowMap.SetActive(false);
    }

    public void ButtonDialog() { windowDialog.SetActive(!windowDialog.activeSelf);}    

    public void ButtonMyDeck()
    {
        windowMap.SetActive(false);
        escMenu.SetActive(false);
        windowMyDeck.SetActive(!windowMyDeck.activeSelf);
    }

    public void ButtonEscMenu()
    {
        windowMyDeck.SetActive(false);
        windowMap.SetActive(false);
        escMenu.SetActive(!escMenu.activeSelf);
    }

    public void ButtonMap()
    {
        windowMyDeck.SetActive(false);
        escMenu.SetActive(false);
        windowMap.SetActive(!windowMap.activeSelf);
    }

    public void ButtonHeader()
    {
        windowHeader.SetActive(!windowHeader.activeSelf);
    }
}
