using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A_FieldCardBorder : MonoBehaviour
{
    public GameObject useAvailable;
    public GameObject attackAvailable;
    public GameObject tauntUseAvailable;
    public GameObject tauntAttackAvailable;

    public GameObject pointerOn;
    public GameObject tauntPointerOn;

    public void ClearAvailable()
    {
        useAvailable.SetActive(false);
        attackAvailable.SetActive(false);
        tauntUseAvailable.SetActive(false);
        tauntAttackAvailable.SetActive(false);
    }

    public void ClearAll()
    {
        useAvailable.SetActive(false);
        attackAvailable.SetActive(false);
        tauntUseAvailable.SetActive(false);
        tauntAttackAvailable.SetActive(false);

        pointerOn.SetActive(false);
        tauntPointerOn.SetActive(false);
    }

}


/*
public class A_FieldCardBorder : MonoBehaviour
{
    [System.Serializable]
    public class AvailableBorder
    {
        public string name;
        public Image available;
        public Color color;
    }
    public AvailableBorder[] availableBorders;

    [System.Serializable]
    public class PointerBorder
    {
        public string name;
        public Image pointer;
        public Color color;
    }
    public PointerBorder[] pointerBorders;

    public float degree;
    public float blinkSpeed = 1f; // 깜빡이는 속도
    private Color baseColor = new Color(0f, 1f, 0.937f); // 00FFEF 색상

    void Start()
    {

        StartCoroutine(BlinkBorder());
    }


    IEnumerator BlinkBorder()
    {
        while (true)
        {
            // 시간에 따라 t 값을 0에서 1로 변환
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);

            // 기본 색상에서 약간 변형된 색상으로 보간
            Color newColor = Color.Lerp(baseColor * degree, baseColor * 1.1f, t);

            // 테두리의 색상 적용
            borderImage[i].color = newColor;

            yield return null;
        }
    }
}
*/
