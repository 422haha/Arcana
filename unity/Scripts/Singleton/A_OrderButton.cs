using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class A_OrderButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text text;

    private void Start()
    {
        text = this.transform.GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = Color.black;
    }
}
