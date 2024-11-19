using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class A_Translate : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text koreanText;
    public GameObject koreanSelectImg;
    public TMP_Text englishText;
    public GameObject englishSelectImg;

    private string currentLanguage;

    private void Start()
    {
        Lean.Localization.LeanLocalization.SetCurrentLanguageAll("Korean");
        currentLanguage = Lean.Localization.LeanLocalization.GetFirstCurrentLanguage();

        Hub.TranslationManager.SetLanguage(currentLanguage);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        currentLanguage = Lean.Localization.LeanLocalization.GetFirstCurrentLanguage();

        if (currentLanguage == "Korean")
        {
            Hub.TranslationManager.SetLanguage("English");
            koreanText.color = Color.black;
            koreanSelectImg.SetActive(false);
            englishText.color = Color.white;
            englishSelectImg.SetActive(true);

            Debug.Log("영어로 변경");
            Debug.Log(currentLanguage);
        }
        else if (currentLanguage == "English")
        {
            Hub.TranslationManager.SetLanguage("Korean");
            koreanText.color = Color.white;
            koreanSelectImg.SetActive(true);
            englishText.color = Color.black;
            englishSelectImg.SetActive(false);

            Debug.Log("한국어로 변경");
            Debug.Log(currentLanguage);
        }
        else
        {
            Debug.LogError($"에러발생 : {currentLanguage}");
        }
    }

    // 마우스 오버 시 크기가 커지도록
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
