using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TranslationManager : MonoBehaviour
{
    private string currentLanguage;

    public string GetLanguage()
    {
        return currentLanguage;
    }

    public void SetLanguage(string language)
    {
        Lean.Localization.LeanLocalization.SetCurrentLanguageAll(language);
        currentLanguage = language;
    }

    /// <summary>
    /// 한국어, 영어 번역기에 메시지 넣는 메서드
    /// </summary>
    /// <param name="leanTarget"></param>
    /// LeanLocallization 내부의 텍스트를 변경 or 삽입하고자 하는 목표 오브젝트
    /// <param name="text"></param>
    /// 해당 메시지들을 넣을 leanPhrase가 적용되어 있는 TMP_Text 객체
    /// <param name="korean"></param>
    /// 한국어 문자열
    /// <param name="english"></param>
    /// 영어 문자열
    public void AddTranslation(Lean.Localization.LeanPhrase leanTarget, TMP_Text text, string korean, string english)
    {
        leanTarget.AddEntry("Korean", korean, text);
        leanTarget.AddEntry("English", english, text);
        if (currentLanguage == "Korean")
            text.text = korean;
        else
            text.text = english;
    }
}
