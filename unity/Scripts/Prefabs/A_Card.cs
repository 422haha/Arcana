using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


// 관련 enum EnemyInfoManager에 있음.


// 확인해야 할 필요를 모르겠어서 일단은 안 씀
public enum IsPlayers
{
    Player,
    Opp
}

public class A_Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IPointerUpHandler
{


    [Header("")]
    public IsPlayers IsPlayer;
    public int unlockLevel;
    public int price;
    public int rarity;
    [HideInInspector] public int elementIdx; // 상점에서만 사용
    [HideInInspector] public int handIdx; // 배틀 안에서 이 카드가 손에 몇 번째 카드인지




    public CurrentPositions _CurrentPosition = CurrentPositions.Idle;
    [HideInInspector]
    public CurrentPositions CurrentPosition
    {
        get { return _CurrentPosition; }
        set
        {
            _CurrentPosition = value;
            switch (value)
            {
                case CurrentPositions.Idle:
                    priceTag.SetActive(false);
                    break;
                case CurrentPositions.Shop:
                    priceTag.SetActive(true);
                    SetOriginalTransform();
                    break;
                case CurrentPositions.Battlehand:
                    SetOriginalTransform();                    
                    break;
                case CurrentPositions.BattleOther:
                    priceTag.SetActive(false);
                    if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myHandCards += CheckCardBorder;
                    break;
                case CurrentPositions.Description:
                    priceTag.SetActive(false);
                    this.transform.SetParent(Hub.SingletonUIManager.windowDescription.transform, true);
                    break;
                case CurrentPositions.Reward:
                    priceTag.SetActive(false);
                    break;
                case CurrentPositions.Deck:
                    priceTag.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
    [Header("내부 부분")]
    public GameObject explanation;
    public GameObject priceTag;
    public TMP_Text tmpPrice;
    public Transform originalTransform;         // 이 위치를 저장해 두었다가, MouseEnter할 때 여기서 나갔다가, MouseExit하면 다시 여기로 이동함.
    public float scaleDuration;                 // 스케일이 변경되는데 걸리는 시간
    [HideInInspector] public A_Hand myHand;     // 이거는 비어 있다가, 필요에 따라 여기에 담아서 사용하기 (이러지 말고 그냥 A_Hand에 있는 걸 BattleSceneManager에 옮겨도 될 거 같은데)



    public void Start()
    {
        explanation.SetActive(false);
        
    }

    public virtual void Awake()
    {
        tmpPrice.text = price.ToString();
    }


    public virtual void CheckCardBorder()
    {
        // print("이거 불러와지나");
    }

    public virtual void OnDestroy()
    {
        if (Hub.GameManager.Gamestate == GameStates.Battle) Hub.BattleSceneManager.myHandCards -= CheckCardBorder;
    }



    public void SetOriginalTransform()
    {
        originalTransform = this.transform.parent.transform;
    }

    // 크기가 커졌다가 줄어드는 것을 
    public void TranslateScale(Vector3 targetScale, Vector3 targetPos, bool isOnlyY)
    {
        StartCoroutine(TranslateScaleCor(targetScale, targetPos, isOnlyY));
    }

    public IEnumerator TranslateScaleCor(Vector3 targetScale, Vector3 targetPos, bool isOnlyY)
    {
        Vector3 startScale = transform.localScale;
        Vector3 startPos = transform.localPosition;
        if (isOnlyY) targetPos = new Vector3(startPos.x, targetPos.y, startPos.z);
        float escapledTime = 0f;

        while (escapledTime < 0.2f)
        {
            // 스케일을 시간에 따라 선형 보간
            transform.localScale = Vector3.Lerp(startScale, targetScale, escapledTime / 0.2f);
            transform.localPosition = Vector3.Lerp(startPos, targetPos, escapledTime / 0.2f);
            escapledTime += Time.deltaTime;
            yield return null;
        }

        // 최종 스케일 설정
        transform.localScale = targetScale;
        transform.localPosition = targetPos;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (CurrentPosition)
        {
            case CurrentPositions.Idle:
                break;
            case CurrentPositions.Shop:
                this.transform.SetParent(Hub.ShopSceneManager.currentSelection);
                break;
            case CurrentPositions.Battlehand:
                this.transform.SetParent(Hub.BattleSceneManager.currentSelection);
                this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                StartCoroutine(TranslateScaleCor(new Vector3(0.39f, 0.39f, 1f), new Vector3(0f, 75f, 0f), true));
                if (myHand == null) myHand = this.transform.parent.GetComponent<A_CurrentSelection>().myHand.GetComponent<A_Hand>();
                myHand.SettingPositions(true);          // 이거 나중에 리팩토링 좀;;
                break;
            case CurrentPositions.BattleOther:
                break;
            case CurrentPositions.Reward:
                break;
            case CurrentPositions.Deck:
                SetOriginalTransform();
                explanation.transform.SetParent(Hub.SingletonUIManager.windowDescription.transform);
                break;
            default:
                break;
        }
        this.transform.localScale = new Vector3(transform.localScale.x * 1.15f, transform.localScale.y * 1.15f, 1);
        // 여기다가 좌우 변경 넣기
        // 카드는 좌우 변경만 넣으면 됨.
        // explanation.transform.SetParent(Hub.SingletonUIManager.windowDescription.transform, true);
        // explanation.transform.localScale = new Vector3(1f, 1f, 1f);
        explanation.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (CurrentPosition)
        {
            case CurrentPositions.Idle:
                break;
            case CurrentPositions.Shop:
                this.transform.SetParent(originalTransform);
                break;
            case CurrentPositions.Battlehand:
                this.transform.SetParent(originalTransform);
                this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                //StopCoroutine("TranslateScaleCor");
                StartCoroutine(TranslateScaleCor(new Vector3(0.25f, 0.25f, 1f), Vector3.zero, false));       // 이거 안 해도 어차피 SettingPositions에서 해 줌.
                myHand.SettingPositions();
                break;
            case CurrentPositions.BattleOther:
                break;
            case CurrentPositions.Reward:
                break;
            case CurrentPositions.Deck:                
                explanation.transform.SetParent(originalTransform);
                break;
            default:
                break;
        }
        this.transform.localScale = new Vector3(transform.localScale.x * 0.869565f, transform.localScale.y * 0.869565f, 1);
        // explanation.transform.SetParent(this.transform, true);
        explanation.SetActive(false);
        
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentPosition == CurrentPositions.Shop)
        {
            // 물건 구매하기
            bool result = Hub.ShopSceneManager.TryBuy(price);
            if (result)
            {
                Hub.ProgressManager.GetCard(elementIdx);
                Destroy(this.gameObject);

            }
        }
        
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // 종류별로 처리
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        switch (CurrentPosition)
        {
            case CurrentPositions.Idle:
                break;
            case CurrentPositions.Header:
                break;
            case CurrentPositions.Shop:
                break;
            case CurrentPositions.Battlehand:                
                break;
            case CurrentPositions.BattleOther:
                break;
            case CurrentPositions.Reward:
                break;
            case CurrentPositions.Deck:
                break;
            default:
                break;
        }
    }



}
