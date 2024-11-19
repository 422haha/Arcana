using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;



// 관련 enum EnemyInfoManager에 있음.

public class A_PotionIcon : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler
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
                case CurrentPositions.Header:
                    priceTag.SetActive(false);
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

    public int rarity;
    public int price;    

    [Header("오브젝트들")]
    public GameObject priceTag;
    public TMP_Text tmpPrice;
    public GameObject explanation;
    [HideInInspector] public Transform originalTransform;         // 이 위치를 저장해 두었다가, MouseEnter할 때 여기서 나갔다가, MouseExit하면 다시 여기로 이동함.
    public GameObject usingItemButton;
        


    public void Awake()
    {
        explanation.transform.localScale *= 1.5f;


        tmpPrice.text = price.ToString();
        priceTag.SetActive(false);
        explanation.SetActive(false);
        usingItemButton.SetActive(false);
    }

    public void SetOriginalTransform()
    {
        originalTransform = this.transform.parent.transform;
    }




    #region 부분


    public void ButtonUse()
    {
        if (Hub.GameManager.Gamestate != GameStates.Battle)
        {
            Hub.BattleSceneManager.isDraggingOn = true;
            Hub.SingletonUIManager.arrowForHeader.SetActive(true);
            Hub.SingletonUIManager.arrowP1.position = new Vector3(this.transform.position.x, Hub.SingletonUIManager.arrowP1.position.y, Hub.SingletonUIManager.arrowP1.position.z);
            Hub.SingletonUIManager.arrowP2.position = new Vector3(this.transform.position.x, Hub.SingletonUIManager.arrowP1.position.y, Hub.SingletonUIManager.arrowP1.position.z);
        }


    }

    public void ButtonDiscard()
    {
        // 아티펙트 1: 포션을 버리면 골드 +5
        if (Hub.ProgressManager.currentArtifact.Contains(2)) Hub.ArtifactManager.Artifact2();

        int idx = -1;
        switch (this.transform.parent.name)
        {
            case "PotionPos":
                idx = 0;
                break;
            case "PotionPos2":
                idx = 1;
                break;
            case "PotionPos3":
                idx = 2;
                break;
            default:
                idx = -1;
                print("에러");
                break;
        }
        Hub.ProgressManager.DiscardPotion(idx);
        Hub.SingletonUIManager.potionClosePanel.SetActive(false);
    }

    public void ButtonClose()
    {
        usingItemButton.SetActive(false);
        Hub.SingletonUIManager.potionClosePanel.SetActive(false);
    }





    #endregion








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
        if (!Hub.SingletonUIManager.potionClosePanel.activeSelf)
        {
            
            explanation.SetActive(true);
        }
    }

    public void OnPointerMove(PointerEventData evnetData)
    {
        // header에 있는 경우는 y값 고정
        explanation.transform.position = Input.mousePosition;
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
        switch (CurrentPosition)
        {            
            case CurrentPositions.Header:
                if (elementIdx != 0)
                {
                    usingItemButton.SetActive(true);
                    Hub.SingletonUIManager.potionClosePanel.SetActive(true);
                    Hub.SingletonUIManager.potionClosePanel.GetComponent<Button>().onClick.AddListener(ButtonClose);
                    explanation.SetActive(false);
                }
                break;
            case CurrentPositions.Shop:
                // 물건 구매하기
                bool result = Hub.ShopSceneManager.TryBuy(price, Hub.ProgressManager.CheckAmountOfPotions());
                if (result)
                {
                    Hub.ProgressManager.AddPotion(elementIdx);
                    Destroy(this.gameObject);

                }
                break;
            case CurrentPositions.Reward:
                break;
            default:
                break;
        }
    }


}
