using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class A_SkipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("스킵 버튼 오브젝트들")]
    public Image arrow1;
    public Image arrow2;
    public TMP_Text text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        arrow1.color = Color.yellow;
        arrow2.color = Color.yellow;
        text.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        arrow1.color = Color.white;
        arrow2.color = Color.white;
        text.color = Color.white;
    }
}
