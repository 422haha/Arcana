using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean;
using TMPro;

public class RestSceneManager : MonoBehaviour
{
    // 이거 20으로 잡혀 있는 거 그냥 int로 잡아놓고
    // tmp에도 되도록 해놓자고.


    [Header("휴식 이전")]
    public GameObject windowBeforeRest;
    public Button buttonBeforeRest2;

    [Header("휴식 이후")]
    public GameObject windowAfterRest;
    public Lean.Localization.LeanLocalizedTextMeshProUGUI discriptionAfterPressed;
    public TMP_Text textHealAmount;

    private void Awake()
    {
        // 브금 틀기
        Hub.SoundManager.BgmSelectPlay(6);
        Hub.ProgressManager.IsThisStageCleared = true;

        // 20 골드 이상이 아닐 경우 2번 선택지를 고를 수 없게 하기 
        try { if (Hub.ProgressManager.CurrentGold < 20) buttonBeforeRest2.interactable = false; }
        catch { }
    }

    /// <summary>
    /// 휴식 버튼 누르기
    /// 1: 1번 고름
    /// 2: 2번 고름
    /// </summary>
    /// <param buttonNum="whichButton"></param>
    public void ButtonRestPressed(int whichButton)
    {
        Hub.SoundManager.SfxSelectPlay(0);

        windowBeforeRest.SetActive(false);
        windowAfterRest.SetActive(true);
        int healAmount = 0;
        switch (whichButton)
        {
            case 1:
                healAmount = Random.Range(0, 16);
                Hub.ProgressManager.CurrentHealth += healAmount;
                if (Hub.ProgressManager.CurrentHealth > Hub.ProgressManager.MaxHealth)
                {
                    healAmount = healAmount - (Hub.ProgressManager.CurrentHealth - Hub.ProgressManager.MaxHealth);
                    Hub.ProgressManager.CurrentHealth = Hub.ProgressManager.MaxHealth;
                }
                    
                discriptionAfterPressed.TranslationName = "3_Rest/AfterRest/Discription1";
                textHealAmount.text = "+" + healAmount.ToString() + "HP"; 
                break;
            case 2:
                Hub.ProgressManager.CurrentGold -= 20;

                healAmount = Random.Range(12, 21);
                Hub.ProgressManager.CurrentHealth += healAmount;

                if (Hub.ProgressManager.CurrentHealth > Hub.ProgressManager.MaxHealth)
                {
                    healAmount = healAmount - (Hub.ProgressManager.CurrentHealth - Hub.ProgressManager.MaxHealth);
                    Hub.ProgressManager.CurrentHealth = Hub.ProgressManager.MaxHealth;
                }
                discriptionAfterPressed.TranslationName = "3_Rest/AfterRest/Discription2";
                textHealAmount.text = "+" + healAmount.ToString() + "HP";
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 다음으로 버튼 누르기
    /// </summary>
    public void ButtonToMap()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.SingletonUIManager.ButtonMap();
    }
}
