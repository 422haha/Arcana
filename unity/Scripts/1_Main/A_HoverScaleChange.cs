using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class A_HoverScaleChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    new Vector3 ogScale;

    private void Start()
    {
        ogScale = this.gameObject.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.transform.localScale = ogScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.transform.localScale = ogScale;
    }
}
