using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class A_EscMenu : MonoBehaviour
{
    // 계속 버튼
    public Button continueButton;
    // 환경설정 버튼
    public Button settingButton;
    // 메인으로 버튼
    public Button toMainButton;
    // 게임종료 버튼
    public Button exitGameButton;
    // esc 메뉴
    public GameObject escMenu;
    // 환경설정창
    public GameObject settingWindow;
    // 세팅 Close 버튼
    public Button settingEixtButton;

    private void Start()
    {
        continueButton.onClick.AddListener(CloseMenu);
        settingButton.onClick.AddListener(OpenSetting);
        toMainButton.onClick.AddListener(MoveToMain);
        exitGameButton.onClick.AddListener(ExitGame);
        settingEixtButton.onClick.AddListener(CloseMenu);
    }

    private void OnEnable()
    {
        escMenu.SetActive(true);
        settingWindow.SetActive(false);
    }

    private void CloseMenu()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.SingletonUIManager.ButtonEscMenu();
    }

    private void OpenSetting()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        escMenu.SetActive(false);
        settingWindow.SetActive(true);
    }

    private void MoveToMain()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.TransitionManager.MoveTo(1, 1, 1);
    }

    private void ExitGame()
    {
        Hub.SoundManager.SfxSelectPlay(0);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
