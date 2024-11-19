using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A_ShopBorder : MonoBehaviour
{
    public Image borderImage; // 테두리에 적용된 Image 컴포넌트를 할당하세요
    public float blinkSpeed = 1f; // 깜빡이는 속도
    private Color baseColor = new Color(0f, 1f, 0.937f); // 00FFEF 색상

    void Start()
    {
        if (borderImage == null)
            borderImage = GetComponent<Image>();

        StartCoroutine(BlinkBorder());
    }

    IEnumerator BlinkBorder()
    {
        while (true)
        {
            // 시간에 따라 t 값을 0에서 1로 변환
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);

            // 기본 색상에서 약간 변형된 색상으로 보간
            Color newColor = Color.Lerp(baseColor * 0.85f, baseColor * 1.1f, t);

            // 테두리의 색상 적용
            borderImage.color = newColor;

            yield return null;
        }
    }
}
