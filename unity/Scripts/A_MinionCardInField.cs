using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum MySpecies
{
    Human,
    Silk,
    Browny,
    Gremlin,
    Imp,
    Kobold,
    Goblin,
    Dwarf,
    Unicorn,
    Centaur,
    Elf,
    Orc,
    Troll,
    Giant,
    Dragon
}

public class A_MinionCardInField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [HideInInspector] public Color human;
    [HideInInspector] public Color silk;
    [HideInInspector] public Color browny;
    [HideInInspector] public Color gremlin;
    [HideInInspector] public Color imp;
    [HideInInspector] public Color kobold;
    [HideInInspector] public Color goblin;
    [HideInInspector] public Color dwarf;
    [HideInInspector] public Color unicorn;
    [HideInInspector] public Color centaur;
    [HideInInspector] public Color elf;
    [HideInInspector] public Color orc;
    [HideInInspector] public Color troll;
    [HideInInspector] public Color giant;
    [HideInInspector] public Color dragon;

    public MySpecies MySpecie;
    public Image ring;
    public GameObject cardPrefab;
    private GameObject emptyPrefab;
    public Transform originalCardPos;

    [Header("Status 파트")]
    public IsPlayers IsPlayer;
    [HideInInspector] public bool isInSleep;          // 내자마자 사용 못 하게 하는 거 
    public bool IsUsed;
    // 이거를 여기에 직접 입력 말고, 카드에게서 받아오도록 하는게 좋을 듯
    // 여기에 직접 입력하면 손 안에서 변경된 값은 여기에 적용이 안 됨 
    // 그리고 이거 default로 받아서 다시 defaultcost에서 cost로 보내주는게 좋을 거 같긴 한데 일단은 
    // 그렇게 해야 PointerEnter 해서 나오는 카드의 값도 변경되긴 할텐데 
    private int _Health = 999;
    [HideInInspector] public int Health
    {
        get { return _Health; }
        set
        {
            _Health = value;
            healthTMP.text = _Health.ToString();
            if (_Health <= 0) CardRetreated();
        }
    }
    private int _Attack;
    [HideInInspector] public int Attack
    {
        get { return _Attack; }
        set
        {
            _Attack = value;
            attackTMP.text = _Attack.ToString();
        }
    }

    // 이걸 프로퍼티로 해놔야 나중에 없어지거나 부여되어도 바꿔짐. 특히나 보호막 같은 경우에
    private bool _IsTaunt;
    [HideInInspector] public bool IsTaunt
    {
        get { return _IsTaunt; }
        set
        {
            _IsTaunt = value;
            if (value) taunt.SetActive(true);
            else taunt.SetActive(false);
        }
    }
    private bool _IsShield;
    [HideInInspector] public bool IsShield
    {
        get { return _IsShield; }
        set
        {
            _IsShield = value;
            if (value) shield.SetActive(true);
            else shield.SetActive(false);
        }
    }
    private bool _IsWarCry;
    [HideInInspector] public bool IsWarCry
    {
        get { return _IsWarCry; }
        set
        {
            _IsWarCry = value;
            // if (value) warCry.SetActive(true);
            // else warCry.SetActive(false);
        }
    }

    [Header("필드카드 UI 파트")]
    public TMP_Text healthTMP; 
    public TMP_Text attackTMP;
    public GameObject shield;
    public GameObject taunt;
    public A_FieldCardBorder fieldCardBorder;
    

    [Header("Attack 관련 파트")]
    public float attackDuration = 0.7f;
    public GameObject hit;
    public TMP_Text hitTMP;

    [Header("Retreat 관련 변수")]
    public float shakeDuration = 3f; // 흔들림 지속 시간
    public float shakeMagnitude = 5f; // 흔들림 세기
    public float destroyDelay = 5f;   // 파괴 전 대기 시간
    public Vector3 originalPos;


    // Start is called before the first frame update
    void Awake()
    {
        switch (MySpecie)
        {
            case MySpecies.Human:
                ring.color = human;
                break;
            case MySpecies.Silk:
                ring.color = silk;
                break;
            case MySpecies.Browny:
                ring.color = browny;
                break;
            case MySpecies.Gremlin:
                ring.color = gremlin;
                break;
            case MySpecies.Imp:
                ring.color = imp;
                break;
            case MySpecies.Kobold:
                ring.color = kobold;
                break;
            case MySpecies.Goblin:
                ring.color = goblin;
                break;
            case MySpecies.Dwarf:
                ring.color = dwarf;
                break;
            case MySpecies.Unicorn:
                ring.color = unicorn;
                break;
            case MySpecies.Centaur:
                ring.color = centaur;
                break;
            case MySpecies.Elf:
                ring.color = elf;
                break;
            case MySpecies.Orc:
                ring.color = orc;
                break;
            case MySpecies.Troll:
                ring.color = troll;
                break;
            case MySpecies.Giant:
                ring.color = giant;
                break;
            case MySpecies.Dragon:
                ring.color = dragon;
                break;
            default:
                break;
        }
        
        
                

    }

    // Awake 대신에 여기서 함.
    public void Initialize()
    {
        if (IsTaunt)
        {
            if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myTauntAmount += 1;
            else Hub.BattleSceneManager.oppTauntAmount += 1;
        }

        // 델리게이트에 추가
        if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myFieldCards += EveryTurn;
        else Hub.BattleSceneManager.oppFieldCards += EveryTurn;

    }

    public void EveryTurn()
    {
        if ((IsPlayer == IsPlayers.Player && Hub.BattleSceneManager.BattleState == BattleStates.MyTurn) || (IsPlayer == IsPlayers.Opp && Hub.BattleSceneManager.BattleState == BattleStates.OppTurn))
        {
            isInSleep = false;
            IsUsed = false;
        }
        
        // 이런 표시는 그냥 플레이어만 하게.
        // 어차피 AI가 일일히 두드리게 해놓을 것도 아니고 
        if (IsPlayer == IsPlayers.Player)
        {
            if (Hub.BattleSceneManager.BattleState == BattleStates.OppTurn)
            {
                fieldCardBorder.ClearAvailable();
            }
            else if (!isInSleep & !IsUsed & Attack != 0)
            {
                if (!IsTaunt) fieldCardBorder.useAvailable.SetActive(true);
                else fieldCardBorder.useAvailable.SetActive(true);
            }
        }
        else if (Hub.BattleSceneManager.isDraggingOn)
        {
            if (Hub.BattleSceneManager.oppTauntAmount != 0)
            {
                if (IsTaunt) fieldCardBorder.tauntAttackAvailable.SetActive(true);
            }
            else
            {
                fieldCardBorder.attackAvailable.SetActive(true);
            }
        }
        else
        {
            if (IsTaunt) fieldCardBorder.tauntAttackAvailable.SetActive(false);
            else fieldCardBorder.attackAvailable.SetActive(false);
        }
    }

    public void CardRetreated()
    {
        // 이렇게 해놔야 큐에 넣지
        StartCoroutine(CardRetreatedCor());
        this.transform.SetParent(Hub.SingletonUIManager.windowDescription.transform);
        if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myField.GetComponent<A_BattleField>().UnitUsed();
        else Hub.BattleSceneManager.oppField.GetComponent<A_BattleField>().UnitUsed();
    }

    public IEnumerator CardRetreatedCor()
    {
        float elapsed = 0.0f;
        originalPos = this.transform.position;

        while (elapsed < shakeDuration)
        {
            // x, y 방향으로 랜덤한 흔들림 생성
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // 원래 위치로 복원
        transform.position = originalPos;        
        Hub.SoundManager.SfxSelectPlay(7);
        Destroy(this.gameObject);




    }

    public void OnDestroy()
    {
        if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myFieldCards -= EveryTurn;
        else Hub.BattleSceneManager.oppFieldCards -= EveryTurn;

        // if (IsPlayer == IsPlayers.Player) if (Hub.BattleSceneManager.myFieldCards != null) Hub.BattleSceneManager.myFieldCards -= EveryTurn;
        // else { if ( Hub.BattleSceneManager.myFieldCards != null) Hub.BattleSceneManager.oppFieldCards -= EveryTurn; }


        if (IsTaunt)
        {
            if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myTauntAmount -= 1;
            else Hub.BattleSceneManager.oppTauntAmount -= 1;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (emptyPrefab == null) emptyPrefab = Instantiate(cardPrefab, originalCardPos.transform);
        emptyPrefab.SetActive(true);
        emptyPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        emptyPrefab.GetComponent<A_MinionCard>().CurrentPosition = CurrentPositions.Description;
        // 옆에다가 카드 띄우기 
        // this.transform.localScale = new Vector3(transform.localScale.x * 1.15f, transform.localScale.y * 1.15f, 1);
        // 여기다가 좌우 변경 넣기
        // 카드는 좌우 변경만 넣으면 됨.

        if (Hub.BattleSceneManager.isDraggingOn && IsPlayer == IsPlayers.Opp)
        {
            if (Hub.BattleSceneManager.oppTauntAmount == 0 | (Hub.BattleSceneManager.oppTauntAmount != 0 & IsTaunt == true))
            {
                Hub.BattleSceneManager.draggingTo = this.gameObject;
            }
        }

        if (!IsTaunt) fieldCardBorder.pointerOn.SetActive(true);
        else fieldCardBorder.tauntPointerOn.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // emptyPrefab.SetActive(false);
        // 일단은 삭제를 해줘야 할 듯
        Destroy(emptyPrefab);
        if (Hub.BattleSceneManager.isDraggingOn && IsPlayer == IsPlayers.Opp)
        {
            Hub.BattleSceneManager.draggingTo = null;
        }

        if (!IsTaunt) fieldCardBorder.pointerOn.SetActive(false);
        else fieldCardBorder.tauntPointerOn.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInSleep & !IsUsed & Attack != 0 & Hub.BattleSceneManager.BattleState == BattleStates.MyTurn)
        {
            Hub.BattleSceneManager.isDraggingOn = true;
            Hub.BattleSceneManager.minionArrow.SetActive(true);
            Hub.BattleSceneManager.minionArrowP1.transform.position = this.transform.position;
            Hub.BattleSceneManager.minionArrowP2.transform.position = this.transform.position;
            if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.oppFieldCards?.Invoke();
            // 여기다가 영웅 관련 일단 처리
            if (Hub.BattleSceneManager.oppTauntAmount == 0) Hub.BattleSceneManager.oppHeroCardBorder.attackAvailable.SetActive(true);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInSleep & !IsUsed & Attack != 0 & Hub.BattleSceneManager.BattleState == BattleStates.MyTurn)
        {

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Hub.BattleSceneManager.minionArrow.SetActive(false);
        if (Hub.BattleSceneManager.draggingTo != null & Hub.BattleSceneManager.BattleState == BattleStates.MyTurn)
        {
            UseAttack(this.transform.position, Hub.BattleSceneManager.draggingTo.transform.position);
            IsUsed = true;
            
        }
        Hub.BattleSceneManager.isDraggingOn = false;
        Hub.BattleSceneManager.oppFieldCards?.Invoke();
        // 여기다가 영웅 관련 일단 처리
        Hub.BattleSceneManager.oppHeroCardBorder.attackAvailable.SetActive(false);
    }


    public void UseAttack(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(UseAttackCor(startPos, targetPos));
    }

    public IEnumerator UseAttackCor(Vector3 startPos, Vector3 targetPos)
    {
        // 공격 하기 전에 
        Transform originalTransform = this.transform.parent;
        this.transform.SetParent(Hub.SingletonUIManager.windowDescription.transform);

        // 공격
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration / 2)
        {
            this.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / (attackDuration * 3));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Vector3 middlePos = startPos;
        yield return new WaitForSeconds(0.25f);

        elapsedTime = 0f;
        while (elapsedTime < (attackDuration / 2))
        {
            this.transform.position = Vector3.Lerp(middlePos, targetPos, elapsedTime / (attackDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;




        // 여기서 공격 및 피격 처리
        Hub.SoundManager.SfxSelectPlay(6);
        int attackerAmount = 0;         // 공격하는 하수인이 받는 피해
        int receiverAmount = 0;

        GameObject startCard = this.gameObject;
        GameObject targetCard = Hub.BattleSceneManager.draggingTo;

        if (Hub.BattleSceneManager.draggingTo.GetComponent<A_MinionCardInField>() == null) // 영웅을 공격
        {
            // 영웅에게 공격력 넣으려면 여기에다가
            // 근데 일단 A_Hero에 공격이 없음ㅋㅋ
            if (Hub.BattleSceneManager.draggingTo.GetComponent<A_Hero>() == null)
            {
                print("심각한 에러!!! : 영웅도 하수인도 아닌 곳을 공격한다고 되어 있음");                                
            }
            else
            {
                A_Hero heroHit = Hub.BattleSceneManager.draggingTo.GetComponent<A_Hero>();
                receiverAmount = -Attack;
                heroHit.hitTMP.text = receiverAmount == 0 ? "-" + receiverAmount.ToString() : receiverAmount.ToString();
                heroHit.hit.SetActive(false);
                heroHit.hit.SetActive(true);

                // 아티펙트 4: 내 영웅이 10 피해를 입으면 카드를 한 장 뽑음.
                if (IsPlayer == IsPlayers.Opp & Hub.ProgressManager.currentArtifact.Contains(3)) Hub.ArtifactManager.Artifact3(attackerAmount);

                // 아티펙트 4: 상대 영웅에게 10 피해면 체력 +2
                if (IsPlayer == IsPlayers.Player & Hub.ProgressManager.currentArtifact.Contains(4)) Hub.ArtifactManager.Artifact4(receiverAmount);


            }

        }
        else // 하수인을 공격
        {
            ///// 근데 피격 표시를 여기서 처리하는게 아니라 health 프로퍼티에서 처리하는게 더 좋지 않으려나 


            A_MinionCardInField fieldCard = Hub.BattleSceneManager.draggingTo.GetComponent<A_MinionCardInField>();

            // 보호막이 있을 경우 보호막을
            // 그리고 여기서 피격 표시
            if (IsShield) IsShield = false;
            else
            {
                attackerAmount = -fieldCard.Attack;
            }
            hitTMP.text = attackerAmount == 0 ? "-" + attackerAmount.ToString() : attackerAmount.ToString();
            hit.SetActive(false);
            hit.SetActive(true);


            receiverAmount = 0;
            if (fieldCard.IsShield) fieldCard.IsShield = false;
            else
            {
                receiverAmount = -Attack;                
            }
            fieldCard.hitTMP.text = receiverAmount == 0 ? "-"+ receiverAmount.ToString() : receiverAmount.ToString();
            fieldCard.hit.SetActive(false);
            fieldCard.hit.SetActive(true);
        }        
        Hub.BattleSceneManager.draggingTo = null;
        fieldCardBorder.ClearAvailable();
        
        // 다시 돌아오기
        elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            this.transform.position = Vector3.Lerp(targetPos, startPos, elapsedTime / attackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
        this.transform.SetParent(originalTransform);

        // 여기서 데미지를 처리      
        if (targetCard.GetComponent<A_Hero>() != null) Hub.BattleSceneManager.OppHP = Hub.BattleSceneManager.OppHP + receiverAmount;
        else
        {
            Health = Health + attackerAmount;
            targetCard.GetComponent<A_MinionCardInField>().Health = targetCard.GetComponent<A_MinionCardInField>().Health + receiverAmount;
        }

        
    }

}
