using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class A_Setting : MonoBehaviour
{
    [Header("세팅 파트")]
    public Button closeBtn;
    public Dropdown languageDropdown;
    public Dropdown modeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;

    private bool isFullScreen;

    void OnEnable()
    {
        if (Hub.SingletonUIManager.isLanguageDropdownDisable)
            languageDropdown.interactable = false;
        else
            languageDropdown.interactable = true;

        languageDropdown.value = PlayerPrefs.GetInt("Language", 0);
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 3);
        modeDropdown.value = PlayerPrefs.GetInt("Mode", 0);
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);

        AttatchEvents();
        #region 모드설정
        // 모드 초기설정
        switch (modeDropdown.value)
        {
            case 0:
                isFullScreen = true;
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case 1:
                isFullScreen = false;
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
        }
        #endregion
    }

    void AttatchEvents()
    {
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
        modeDropdown.onValueChanged.AddListener(ChangeScreen);
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
    }

    #region 언어설정
    private void ChangeLanguage(int index)
    {
        Hub.SoundManager.SfxSelectPlay(0);

        switch (index)
        {
            // 한국어
            case 0:
                PlayerPrefs.SetInt("Language", 0);
                Hub.TranslationManager.SetLanguage("Korean");
                break;
            // 영어
            case 1:
                PlayerPrefs.SetInt("Language", 1);
                Hub.TranslationManager.SetLanguage("English");
                break;
        }
    }
    #endregion

    #region 해상도설정
    /// <summary>
    /// 스크린의 해상도를 변경합니다.
    /// </summary>
    /// <param name="index">해상도 드롭다운 값</param>
    private void ChangeResolution(int index)
    {
        Hub.SoundManager.SfxSelectPlay(0);

        switch (index)
        {
            case 0:
                PlayerPrefs.SetInt("Resolution", 0);
                Screen.SetResolution(1280, 800, isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
                break;
            case 1:
                PlayerPrefs.SetInt("Resolution", 1);
                Screen.SetResolution(1400, 900, isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
                break;
            case 2:
                PlayerPrefs.SetInt("Resolution", 2);
                Screen.SetResolution(1680, 1050, isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
                break;
            case 3:
                PlayerPrefs.SetInt("Resolution", 3);
                Screen.SetResolution(1920, 1200, isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
                break;
        }

    }
    #endregion

    #region 모드설정
    /// <summary>
    /// 스크린의 전체 스크린 모드를 변경합니다.
    /// </summary>
    private void ChangeScreen(int index)
    {
        Hub.SoundManager.SfxSelectPlay(0);

        switch (index)
        {
            case 0:
                PlayerPrefs.SetInt("Mode", 0);
                isFullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                PlayerPrefs.SetInt("Mode", 1);
                isFullScreen = false;
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
    #endregion

    /// <summary>
    /// 시스템 볼륨을 변경하고 저장합니다.
    /// </summary>
    /// <param name="value"></param>
    #region 볼륨설정
    private void AdjustVolume(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        Hub.SoundManager.SetBGMVolume(value);
        Hub.SoundManager.SetSFXVolume(value);
    }
    #endregion
}
