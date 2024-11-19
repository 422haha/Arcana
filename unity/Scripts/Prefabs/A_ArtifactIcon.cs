using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


// 관련 enum EnemyInfoManager에 있음.


public class A_ArtifactIcon : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector] public int elementIdx; // 상점에서만 사용
    private CurrentPositions _CurrentPosition = CurrentPositions.Idle;
    public CurrentPositions CurrentPosition
    {
        get { return _CurrentPosition; }
        set
        {
            _CurrentPosition = value;
            switch (value)
            {
                case CurrentPositions.Idle:
                    break;
                case CurrentPositions.Shop:
                    priceTag.SetActive(true);
                    break;
                case CurrentPositions.Battlehand:
                    break;
                case CurrentPositions.Deck:
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 희귀도 부분
    /// 0: 언커먼
    /// 1: 커먼
    /// 2: 레어
    /// </summary>
    public int rarity;
    public int price;

    [Header("내부 부분")]
    public GameObject priceTag;
    public TMP_Text tmpPrice;
    public GameObject explanation;
    public Transform originalTransform;         // 이 위치를 저장해 두었다가, MouseEnter할 때 여기서 나갔다가, MouseExit하면 다시 여기로 이동함.

    public void Awake()
    {
        explanation.transform.localScale *= 1.5f;

        tmpPrice.text = price.ToString();
        priceTag.SetActive(false);
        explanation.SetActive(false);
    }

    public void SetOriginalTransform()
    {
        originalTransform = this.transform.parent.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (CurrentPosition)
        {
            case CurrentPositions.Idle:
                break;
            case CurrentPositions.Header:
                break;
            case CurrentPositions.Shop:
                this.transform.SetParent(Hub.ShopSceneManager.currentSelection);
                break;
            case CurrentPositions.Reward:
                break;
            default:
                break;
        }
        this.transform.localScale = new Vector3(transform.localScale.x * 1.15f, transform.localScale.y * 1.15f, 1);
        // 여기다가 좌우 변경 넣기
        // 카드는 좌우 변경만 넣으면 됨.
        explanation.SetActive(true);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (CurrentPosition == CurrentPositions.Shop)
        {
            explanation.transform.position = Input.mousePosition;
        }
        else if (CurrentPosition == CurrentPositions.Header)
        {
            Vector3 offset = new Vector3(100, 0, 0);
            explanation.transform.position = Input.mousePosition + offset;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (CurrentPosition)
        {
            case CurrentPositions.Idle:
                break;
            case CurrentPositions.Header:
                break;
            case CurrentPositions.Shop:
                this.transform.SetParent(originalTransform);
                break;
            case CurrentPositions.Reward:
                break;
            default:
                break;
        }
        this.transform.localScale = new Vector3(transform.localScale.x * 0.869565f, transform.localScale.y * 0.869565f, 1);
        explanation.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentPosition == CurrentPositions.Shop)
        {
            // 물건 구매하기
            bool result = Hub.ShopSceneManager.TryBuy(price);
            if (result)
            {
                // 아티펙트 1: 최대 체력 +3
                if (elementIdx == 1) Hub.ArtifactManager.Artifact1();

                // 아티펙트 7: 이 아티펙트를 얻을 때, 카드 3장도 같이
                if (elementIdx == 7) Hub.ArtifactManager.Artifact7();

                Hub.ProgressManager.GetArtifact(elementIdx);
                Destroy(this.gameObject);

            }
        }
        else if (CurrentPosition == CurrentPositions.Header)
        {

        }
    }
}
