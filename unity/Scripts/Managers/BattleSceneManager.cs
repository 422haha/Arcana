using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using TMPro;
using Lean.Localization;

public enum BattleStates
{
    idle,
    BeforeSetting,
    MyTurn,
    OppTurn,
    YouWIn,
    YouDefeated
}


public class BattleSceneManager : MonoBehaviour
{
    private BattleStates _BattleState = BattleStates.idle;
    public GameObject victoryScreen;

    [Header("디버그")]
    public GameObject aiBoard;


    [Header("게임 관련 설정들")]
    public int rewardAmount;            // 승리 시 보상
    public float drawDuration;          // 이거 0.3인데 임시적으로 0.6으로 바꿈          // 카드 뽑기 시 걸리는 시간
    [HideInInspector] public bool isDraggingOn;           // 필드카드 공격이나 스펠 사용 시
    [HideInInspector] public bool availableDraggingGroup; // 이걸로 포션이나 스펠, 공격 가능 그룹 표시할 듯    // bool이 아닌 다른 걸로
    [HideInInspector] public GameObject draggingTo;       // 필드카드 공격이나 스펠 사용되는 곳
    public delegate void FieldCards();
    public FieldCards myFieldCards;
    public FieldCards oppFieldCards;
    public delegate void HandCards();
    public HandCards myHandCards;


    [Header("턴 시작 화면 UI 요소들")]
    public GameObject battleStartBanner; // 배틀 시작 배너
    public TMP_Text bannerText;          // 배너에 표시될 텍스트
    public CanvasGroup interactionBlocker; // 클릭 방지 CanvasGroup
    public Lean.Localization.LeanToken turnNum;

    // 승리 화면 UI 요소들
    // 이거 죄다 날려도 상관 없을 듯 
    /* [Header("승리 화면 UI 요소들")]
    public TMP_Text victoryMessage;
    public TMP_Text goldRewardText;
    public Button cardRewardButton;
    public GameObject cardSelectionScreen;
    public Transform cardSelectionParent; */

    [Header("패배 화면 UI 요소들")]
    public GameObject defeatScreen;
    public TMP_Text clearedStageCount;
    public GameObject defeatBefore;
    public TMP_Text artifactsObtainedText;
    public GameObject defeatAfter;
    public TMP_Text defeatText;
    public Button defeatNextButton;
    public Button defeatMoveToMainButton;
    public TMP_Text cardsObtainedText;
    public LeanPhrase[] defeatDialogs;

    private List<int> cardOptions = new List<int>();

    public BattleStates BattleState
    {
        get { return _BattleState; }
        set
        {
            _BattleState = value;
            switch (value)
            {
                case BattleStates.idle:
                    break;
                case BattleStates.BeforeSetting: // 사전 세팅을 함 

                    // 스테이지별 상대
                    int stageIdx = Hub.ProgressManager.CurrentStageIdx;
                    if (stageIdx < 3)
                    {
                        Hub.ProgressManager.currentEnemyIdx = Random.Range(0, 3);
                    }            // 1그룹
                    else if (stageIdx < 6)
                    {
                        Hub.ProgressManager.currentEnemyIdx = Random.Range(3, 6);
                    }       // 2그룹
                    else if (stageIdx < 9)
                    {
                        Hub.ProgressManager.currentEnemyIdx = Random.Range(6, 9);
                    }       // 3그룹
                    else if (stageIdx < 12)
                    {
                        Hub.ProgressManager.currentEnemyIdx = Random.Range(9, 13);
                    }      // 4그룹
                    else Hub.ProgressManager.currentEnemyIdx = 13;
                    Hub.ProgressManager.SaveGameState();

                    // Health 넣어주기
                    playerHealthTMP.text = Hub.ProgressManager.CurrentHealth.ToString();            // 한번 넣어주기

                    // 상대
                    OppHP = Hub.EnemyInfoManager.EnemiesList[Hub.ProgressManager.currentEnemyIdx].givenHearts;
                    oppPortrait.sprite = Hub.EnemyInfoManager.EnemiesList[Hub.ProgressManager.currentEnemyIdx].portrait;

                    // 덱
                    InstantiateDecks();                    
                    DrawCard(true, 5);
                    DrawCard(false, 5);

                    // 준비가 다 되었는지 확인하는 메소드를 넣으면 좋음.
                    // 하지만 지금은 그냥 바로 둘 중 한 명에게 시작 턴 넘기는 걸로 하기 

                    // 아티펙트 5: 카드 한 장을 더 뽑음
                    if (Hub.ProgressManager.currentArtifact.Contains(5)) Hub.ArtifactManager.Artifact5();

                    // 아티펙트 8: 마나 +1을 추가로 얻은 채로
                    if (Hub.ProgressManager.currentArtifact.Contains(8)) Hub.ArtifactManager.Artifact8();

                    BattleState = BattleStates.MyTurn;
                    break;
                case BattleStates.MyTurn:
                    buttonTurn.interactable = true;

                    battleTurn += 1;
                    myTurnSignal.SetActive(true);
                    oppTurnSignal.SetActive(false);

                    // 아티펙트 9: 매턴 자기 체력 +1
                    if (Hub.ProgressManager.currentArtifact.Contains(9)) Hub.ArtifactManager.Artifact9();

                    myFieldCards?.Invoke();
                    if (battleTurn < 11) MyManaTotal += 1;
                    MyMana = MyManaTotal;
                    myHandCards?.Invoke();          // Border 표시 물어보는 거, 마나 다음에 들어가야 될 듯
                    ShowTurnBanner("나의 턴", battleTurn);
                    DrawCard(true, 1);
                    print("my turn");
                    break;
                case BattleStates.OppTurn:
                    buttonTurn.interactable = false;

                    myTurnSignal.SetActive(false);
                    oppTurnSignal.SetActive(true);

                    myFieldCards?.Invoke();
                    oppFieldCards?.Invoke();
                    myHandCards?.Invoke();          // 상대 턴일 때에도 해야지
                    if (battleTurn < 11) OppManaTotal += 1;
                    OppMana = OppManaTotal;
                    ShowTurnBanner("상대 턴", battleTurn);
                    DrawCard(false, 1);
                    StartCoroutine(GiveTerm(2.5f));                    
                    break;
                case BattleStates.YouWIn:
                    Hub.ProgressManager.IsThisStageCleared = true;

                    // 아티펙트 1: 체력 +5
                    if (Hub.ProgressManager.currentArtifact.Contains(0)) Hub.ArtifactManager.Artifact0();

                    // 분기
                    if (Hub.ProgressManager.CurrentStageIdx != 12)          // 아 이런 거 전부 int로 해야 좋은데
                    {
                        OpenVictoryScreen();
                    }
                    else
                    {
                        Hub.ProgressManager.InitializeProgress();
                        Hub.SoundManager.StopBGM();
                        Hub.TransitionManager.MoveTo(7, 1, 1);
                    }
                    break;

                case BattleStates.YouDefeated:
                    // 통계를 보여주기 위해 미리 값을 설정
                    SetStatistics();
                    // 패배 직후 게임 정보 초기화하여 강제 종료하더라도 새 게임을 시작하도록
                    Hub.ProgressManager.InitializeProgress();
                    OpenDefeatScreen();
                    break;
                default:
                    break;
            }


        }
    }
    private void Awake()
    {
        
    }

    private void Start()
    {
        // 브금 틀기
        // 기본 전투, 용 전투 다르게 틀어야 함 - 조건처리 확인필요
        if (Hub.ProgressManager.CurrentStageIdx == 12)
            Hub.SoundManager.BgmSelectPlay(4);
        else
            Hub.SoundManager.BgmSelectPlay(3);

        StartCoroutine(GameSetting());
        

    }

    public IEnumerator GameSetting()
    {
        if (!Hub.ProgressManager.IsThisStageCleared) BattleState = BattleStates.BeforeSetting;
        else BattleState = BattleStates.YouWIn;
        yield return null;
    }

    //public IEnumerator GiveTerm(float termAmount)
    //{
    //    yield return new WaitForSeconds(termAmount);
    //    TurnEnd(false);
    //}

    public IEnumerator GiveTerm(float termAmount)
    {
        yield return new WaitForSeconds(termAmount);

        // Hub를 통해 AIManager 접근
        if (Hub.AIManager != null)
        {
            Hub.AIManager.SetDifficulty(AIManager.AIDifficulty.Normal); // 난이도 설정
            yield return StartCoroutine(Hub.AIManager.ExecuteAITurn());
        }

        TurnEnd(false);
    }

    private void OpenDefeatScreen()
    {
        Hub.SoundManager.SfxSelectPlay(9);
        defeatScreen.SetActive(true);
        defeatBefore.SetActive(true);
        defeatAfter.SetActive(false);

        // 랜덤으로 대사 선택
        int dialogIndex = Random.Range(0, 5);
        // 대사 집어넣기
        LeanLocalizedTextMeshProUGUI target = defeatText.transform.GetComponent<LeanLocalizedTextMeshProUGUI>();
        target.TranslationName = defeatDialogs[dialogIndex].name;

        defeatNextButton.onClick.RemoveAllListeners();
        defeatNextButton.onClick.AddListener(ShowStatistics);
    }
        
    private void OpenVictoryScreen()
    {
        Hub.SoundManager.SfxSelectPlay(8);        
        victoryScreen.SetActive(true);

        // 승리 화면 페이드 인
        CanvasGroup victoryCanvasGroup = victoryScreen.GetComponent<CanvasGroup>() ?? victoryScreen.AddComponent<CanvasGroup>();
        victoryCanvasGroup.alpha = 0;
        victoryCanvasGroup.DOFade(1, 1f);
    }










    void Update()
    {
        if (Hub.ProgressManager.CurrentHealth <= 0 && BattleState != BattleStates.YouDefeated)
        {
            BattleState = BattleStates.YouDefeated;
        }


        if (Input.GetMouseButtonDown(1)) aiBoard.SetActive(!aiBoard.activeSelf);
    }

    [Header("게임 State")]
    public int battleTurn = 0;    
    private int _OppHP = 999;
    public int OppHP           // 내 거는 ProgressManager에 있음.
    {
        get { return _OppHP; }
        set
        {
            _OppHP = value;
            oppHealthTMP.text = _OppHP.ToString();
            if (_OppHP <= 0)
            {
                BattleState = BattleStates.YouWIn;
            }
        }
    }
    public Queue battleQueue;
    [SerializeField] private int _MyMana;
    public int MyMana
    {
        get { return _MyMana; }
        set
        {
            _MyMana = value;
            myHandCards?.Invoke();          // 마나 변경 될 때마다 Card 사용 가능한지 확인
            myManaTMP.text = _MyMana + "/" + _MyManaTotal;
            for (int i = 0; i < myManaUnitsList.Length; i++)
            {
                if (i < _MyMana) myManaUnitsList[i].GetComponent<Image>().color = starColor;
                else myManaUnitsList[i].GetComponent<Image>().color = starUsedColor;
            }
        }
    }
    [SerializeField] private int _MyManaTotal;
    public int MyManaTotal
    {
        get { return _MyManaTotal; }
        set
        {
            _MyManaTotal = value;
            myManaTMP.text = _MyMana + "/" + _MyManaTotal;
            for (int i = 0; i < 10; i++)
            {
                if (i < _MyManaTotal) myManaUnitsList[i].SetActive(true);
            }
        }
    }
    [SerializeField] private int _OppMana;
    public int OppMana
    {
        get { return _OppMana; }
        set
        {
            _OppMana = value;
            oppManaTMP.text = _OppMana + "/" + _OppManaTotal;
            for (int i = 0; i < oppManaUnitsList.Length; i++)
            {
                if (i < _OppMana) oppManaUnitsList[i].GetComponent<Image>().color = starColor;
                else oppManaUnitsList[i].GetComponent<Image>().color = starUsedColor;
            }
        }
    }
    [SerializeField] private int _OppManaTotal;
    public int OppManaTotal
    {
        get { return _OppManaTotal; }
        set
        {
            _OppManaTotal = value;
            oppManaTMP.text = _OppMana + "/" + _OppManaTotal;
            for (int i = 0; i < 10; i++)
            {
                if (i < _OppManaTotal) oppManaUnitsList[i].SetActive(true);
            }
        }
    }
    public int myTauntAmount;
    public int oppTauntAmount;



    [Header("게임 보드 파트")]
    public GameObject gameBoard;
    public A_HeroCardBorder myHeroCardBorder;
    public Button buttonTurn;


    [Header("내 파트")]
    public GameObject playerHero;
    public Image playerPortrait;
    public GameObject myTurnSignal;
    public TMP_Text playerHealthTMP;
    public A_Hand myHand;
    public List<GameObject> myHandList;
    public GameObject myManaWindow;
    public GameObject[] myManaUnitsList = new GameObject[10];
    public TMP_Text myManaTMP;
    public GameObject myField;
    public A_BattleField myBattleField;
    public A_HeroCardBorder oppHeroCardBorder;


    [Header("상대 파트")]
    public GameObject oppHero;
    public Image oppPortrait;
    public GameObject oppTurnSignal;
    public TMP_Text oppHealthTMP;
    public A_Hand oppHand;
    public List<GameObject> oppHandList;
    public GameObject oppManaWindow;
    public GameObject[] oppManaUnitsList = new GameObject[10];
    public TMP_Text oppManaTMP;
    public GameObject oppField;
    public A_BattleField oppBattleField;



    [Header("덱 부분")]
    public List<GameObject> myDeckList;
    public Transform myDeckPos;
    public List<GameObject> oppDeckList;
    public Transform oppDeckPos;


    [Header("화살 부분")]
    public GameObject arrow;
    public Transform arrowP1;
    public Transform arrowP2;
    public GameObject minionArrow;
    public Transform minionArrowP1;
    public Transform minionArrowP2;


    [Header("다른 부분")]
    public Transform currentSelection;
    public Color starColor;
    public Color starUsedColor;


    // 게임 시작
    public void InstantiateDecks()
    {
        // 내것도 하고 
        print("instantiateDecks()");

        for (int i = 0; i < Hub.ProgressManager.currentDeck.Count; i++)
        {
            print("instantiate card");
            GameObject myCard = Instantiate(Hub.EnemyInfoManager.CardsList[Hub.ProgressManager.currentDeck[i]].prefab);
            myCard.GetComponent<A_Card>().IsPlayer = IsPlayers.Player;
            myCard.GetComponent<A_Card>().CurrentPosition = CurrentPositions.BattleOther;
            myDeckList.Add(myCard);
            myCard.transform.SetParent(myDeckPos);
            myCard.transform.localPosition = Vector3.zero;
            myCard.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
        }
        myDeckList = myDeckList.OrderBy(x => Random.value).ToList();

        // 상대것도 해야 함
        for (int i = 0; i < Hub.EnemyInfoManager.EnemiesList[Hub.ProgressManager.currentEnemyIdx].cardLists.Length; i++)
        {
            GameObject oppCard = Instantiate(Hub.EnemyInfoManager.CardsList[Hub.EnemyInfoManager.EnemiesList[Hub.ProgressManager.currentEnemyIdx].cardLists[i]].prefab);
            oppCard.GetComponent<A_Card>().IsPlayer = IsPlayers.Opp;
            oppCard.GetComponent<A_Card>().CurrentPosition = CurrentPositions.BattleOther;
            oppCard.transform.Find("CardBack").gameObject.SetActive(true);
            oppDeckList.Add(oppCard);
            oppCard.transform.SetParent(oppDeckPos);
            oppCard.transform.localPosition = Vector3.zero;
        }
        oppDeckList = oppDeckList.OrderBy(x => Random.value).ToList();
    }



    #region Card 조작 파트

    public void DrawCard(bool isPlayer, int amount)
    {
        StartCoroutine(DrawCardCor(isPlayer, amount));
    }
    public IEnumerator DrawCardCor(bool isPlayer, int amount)
    {
        if (isPlayer)
        {
            // 내 꺼일 경우
            if (myDeckList.Count < amount) Fatigue(true, amount - myDeckList.Count);
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    Hub.SoundManager.SfxSelectPlay(4);
                    yield return StartCoroutine(TranslateToHand(true, myDeckList[myDeckList.Count - 1]));
                    myHandList.Add(myDeckList[myDeckList.Count - 1]);
                    myDeckList.RemoveAt(myDeckList.Count - 1);
                }
            }
        }
        else
        {
            // 상대 꺼일 경우
            if (oppDeckList.Count < amount) Fatigue(true, amount - oppDeckList.Count);
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    Hub.SoundManager.SfxSelectPlay(4);
                    yield return StartCoroutine(TranslateToHand(false, oppDeckList[oppDeckList.Count - 1]));
                    oppHandList.Add(oppDeckList[oppDeckList.Count - 1]);
                    oppDeckList.RemoveAt(oppDeckList.Count - 1);
                }
            }
        }
    }

    // 손으로 이동하게 해주는 메소드
    IEnumerator TranslateToHand(bool isPlayer, GameObject card)
    {
        if (isPlayer)
        {
            myHand.AddUnit();
            Transform currentHand = myHand.unitsList[myHand.unitsList.Count - 1].transform;
            Vector3 handPos = myHand.unitsList[myHand.unitsList.Count - 1].transform.position;

            Vector3 startPos = card.transform.position;
            float elapsedTime = 0f;
            while (elapsedTime < drawDuration)
            {
                card.transform.position = Vector3.Lerp(startPos, handPos, elapsedTime / drawDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            card.transform.position = handPos;
            myDeckList[myDeckList.Count - 1].transform.SetParent(currentHand);
            myDeckList[myDeckList.Count - 1].GetComponent<A_Card>().CurrentPosition = CurrentPositions.Battlehand;          // 순서 위랑 바뀌면 절대 안 됨.
            myDeckList[myDeckList.Count - 1].transform.localPosition = Vector3.zero;
            myDeckList[myDeckList.Count - 1].transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            oppHand.AddUnit();
            Transform currentHand = oppHand.unitsList[oppHand.unitsList.Count - 1].transform;
            Vector3 handPos = oppHand.unitsList[oppHand.unitsList.Count - 1].transform.position;

            Vector3 startPos = card.transform.position;
            float elapsedTime = 0f;
            while (elapsedTime < drawDuration)
            {
                card.transform.position = Vector3.Lerp(startPos, handPos, elapsedTime / drawDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            card.transform.position = handPos;
            oppDeckList[oppDeckList.Count - 1].transform.SetParent(currentHand);
            oppDeckList[myDeckList.Count - 1].GetComponent<A_Card>().CurrentPosition = CurrentPositions.BattleOther;         // 순서 위랑 바뀌면 절대 안 됨. 
            oppDeckList[oppDeckList.Count - 1].transform.localPosition = Vector3.zero;

        }




    }

    public void Fatigue(bool isPlayer, int amount)
    {
        print("모든 카드 소진!");
    }

    // 2차를 위해 남겨놓음
    public void ShuffleCards(bool isPlayer)
    {
        if (isPlayer)
        {

        }
        else
        {

        }
    }

    #endregion


    #region 게임 플로우 파트

    public void TurnEnd(bool isPlayer)
    {
        // 명령 내린 거 있으면 그거 다 하고 난 다음에 기다려야 되니까
        if (isPlayer)
        {
            if (BattleState == BattleStates.MyTurn)
            {
                BattleState = BattleStates.OppTurn;
            }
        }
        else
        {
            if (BattleState == BattleStates.OppTurn)
            {
                BattleState = BattleStates.MyTurn;
            }
        }
    }



    // 턴 시작 애니메이션
    private void ShowTurnBanner(string turnText, int turnNumber)
    {        
        // "전투 시작"일 경우 턴 번호를 표시하지 않음
        if (turnText == "전투 시작")
        {
            bannerText.text = turnText;
        }
        else
        {
            bannerText.text = $"{turnText}\n<size=24>{turnNumber}번째 턴</size>";
        }
        

        //이거 다시 번역으로 처리
        turnNum.Value = turnNumber.ToString();

        // 화면 클릭 방지 설정
        interactionBlocker.blocksRaycasts = true;
        interactionBlocker.DOFade(1, 0.2f);

        // 배너 등장 애니메이션
        battleStartBanner.transform.DOLocalMoveY(0, 0.5f).From(new Vector3(0, -200, 0))
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
            // 몇 초 후 배너가 사라지게 설정
            DOVirtual.DelayedCall(1f, () =>
                {
                    battleStartBanner.transform.DOLocalMoveY(-200, 0.5f).SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                        // 화면 클릭 가능하게 설정
                        interactionBlocker.blocksRaycasts = false;
                            interactionBlocker.DOFade(0, 0.2f);
                        });
                });
            });
    }

    // 버튼에서 사용하려고 함.
    public void AIUseHandForButton(int idx)
    {
        AIUseHand(idx);
    }

    public bool AIUseHand(int idx)
    {
        A_Hand oppHand = Hub.BattleSceneManager.oppHand.GetComponent<A_Hand>();
        A_MinionCard myMinionCard = oppHand.unitsList[idx].transform.GetChild(0).GetComponent<A_MinionCard>();
        /*
        if (oppHand.unitsList.Count <= idx)
        {
            return false;
        }

        if (myMinionCard.cost > Hub.BattleSceneManager.OppMana)
        {
            return false;
        }
        */


        //////////////////////////////////////////////////////////////////////////
        // 이 부분은 minionCard에서 발췌하는건데 이거 반드시 모듈화 해야 함.



        Hub.BattleSceneManager.OppMana -= myMinionCard.cost;

        // 아티펙트 6: 15개의 마나를 사용하면, 마나 +1
        if (Hub.ProgressManager.currentArtifact.Contains(6)) Hub.ArtifactManager.Artifact6(myMinionCard.cost);

        A_BattleField oppField = Hub.BattleSceneManager.oppField.GetComponent<A_BattleField>();
        oppField.AddUnit();      // 유닛 자리 추가하고 
        GameObject currentPrefab = Instantiate(myMinionCard.cardOnField, oppField.unitsList[oppField.unitsList.Count - 1].transform);     // 그 유닛에다가 집어넣고 
        A_MinionCardInField fieldCard = currentPrefab.GetComponent<A_MinionCardInField>();
        fieldCard.IsPlayer = IsPlayers.Opp;
        fieldCard.Health = myMinionCard.health;
        fieldCard.Attack = myMinionCard.attack;
        fieldCard.IsTaunt = myMinionCard.abilityButtons[0] ? true : false;
        fieldCard.IsWarCry = myMinionCard.abilityButtons[1] ? true : false;
        fieldCard.IsShield = myMinionCard.abilityButtons[2] ? true : false;
        fieldCard.isInSleep = true;
        fieldCard.IsUsed = false;
        fieldCard.Initialize();

        print("카드 사용됨");
        oppHand.UnitUsed();          // hand의 유닛은 없애기. OnPointEnter 하면 들어가니까 이 값은 있을 수 밖에 없음. 
        Destroy(oppHand.unitsList[idx].gameObject);
        oppHand.unitsList.RemoveAt(idx);


        /////////////////////////////////////////////////////////////////////





        return true;
    }

    public IEnumerator AIUseFieldCardCor(GameObject startCard, GameObject targetCard)
    {

        // 공격 하기 전에 
        Transform originalTransform = startCard.transform.parent;
        Vector3 startPos = startCard.transform.position;
        Vector3 targetPos = targetCard.transform.position;
        float attackDuration = startCard.GetComponent<A_MinionCardInField>().attackDuration;
        startCard.transform.SetParent(Hub.SingletonUIManager.windowDescription.transform);
        int attackerAmount = 0;             // 공격하는 하수인이 받는 피해
        int receiverAmount = 0;

        // 공격
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration / 2)
        {
            startCard.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / (attackDuration * 3));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Vector3 middlePos = startPos;
        yield return new WaitForSeconds(0.25f);

        elapsedTime = 0f;
        while (elapsedTime < (attackDuration / 2))
        {
            startCard.transform.position = Vector3.Lerp(middlePos, targetPos, elapsedTime / (attackDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startCard.transform.position = targetPos;



        // 타격
        Hub.SoundManager.SfxSelectPlay(6);
        if (targetCard.GetComponent<A_Hero>() != null) // 영웅을 공격한 경우
        {
            receiverAmount = startCard.GetComponent<A_MinionCardInField>().Attack;
            A_Hero heroHit = targetCard.GetComponent<A_Hero>();
            heroHit.hitTMP.text = receiverAmount == 0 ? "-" + receiverAmount.ToString() : "-" + receiverAmount.ToString();
            heroHit.hit.SetActive(false);
            heroHit.hit.SetActive(true);
        }
        else // 하수인을 공격한 경우
        {
            A_MinionCardInField attackerHit = startCard.GetComponent<A_MinionCardInField>();
            A_MinionCardInField receiverHit = targetCard.GetComponent<A_MinionCardInField>();
            
            // 공격하는 하수인의 피해 표시 
            if (attackerHit.IsShield) attackerHit.IsShield = false;
            else
            {
                attackerAmount = receiverHit.Attack;
            }
            attackerHit.hitTMP.text = attackerAmount == 0 ? "-" + attackerAmount.ToString() : "-" + attackerAmount.ToString();
            attackerHit.hit.SetActive(false);
            attackerHit.hit.SetActive(true);

            //
            if (receiverHit.IsShield) receiverHit.IsShield = false;
            else
            {
                receiverAmount = attackerHit.Attack;
            }
            receiverHit.hitTMP.text = receiverAmount == 0 ? "-" + receiverAmount.ToString() : "-" + receiverAmount.ToString();
            receiverHit.hit.SetActive(false);
            receiverHit.hit.SetActive(true);

        }
        

        // 다시 돌아오기
        elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            startCard.transform.position = Vector3.Lerp(targetPos, startPos, elapsedTime / attackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startCard.transform.position = startPos;
        startCard.transform.SetParent(originalTransform);


        // 아티펙트 4: 내 영웅이 10 피해를 입으면 카드를 한 장 뽑음.
        // if (targetCard.GetComponent<A_MinionCardInField>().IsPlayer == IsPlayers.Player & Hub.ProgressManager.currentArtifact.Contains(3)) Hub.ArtifactManager.Artifact3(receiverAmount);


        // 여기서 데미지를 처리      
        if (targetCard.GetComponent<A_Hero>() != null) Hub.ProgressManager.CurrentHealth -= receiverAmount;
        else
        {
            startCard.GetComponent<A_MinionCardInField>().Health -= attackerAmount;
            targetCard.GetComponent<A_MinionCardInField>().Health -= receiverAmount;
        }


    }


    // 동희야 눈감아
    public void AIButtonCard1To(int idx)
    {
        AIUseFieldCard(0, idx);
    }
    public void AIButtonCard2To(int idx)
    {
        AIUseFieldCard(1, idx);
    }
    public void AIButtonCard3To(int idx)
    {
        AIUseFieldCard(2, idx);
    }
    public void AIButtonCard4To(int idx)
    {
        AIUseFieldCard(3, idx);
    }
    public void AIButtonCard5To(int idx)
    {
        AIUseFieldCard(4, idx);
    }

    //public void AIUseFieldCard(int startIdx, int targetIdx)
    //{
    //    GameObject startCard = Hub.BattleSceneManager.oppBattleField.unitsList[startIdx].transform.GetChild(0).gameObject;
    //    GameObject targetCard = Hub.BattleSceneManager.myBattleField.unitsList[targetIdx].transform.GetChild(0).gameObject;
    //    StartCoroutine(AIUseFieldCardCor(startCard, targetCard));
    //}

    public void AIUseFieldCard(int startIdx, int targetIdx)
    {
        // 먼저 필드와 유닛리스트 null 체크
        if (oppBattleField == null || myBattleField == null ||
            oppBattleField.unitsList == null || myBattleField.unitsList == null)
            return;

        // 인덱스 범위 체크
        if (startIdx < 0 || targetIdx < 0 ||
            startIdx >= oppBattleField.unitsList.Count ||
            targetIdx >= myBattleField.unitsList.Count)
            return;

        var startUnit = oppBattleField.unitsList[startIdx];
        var targetUnit = myBattleField.unitsList[targetIdx];

        // 유닛 null 체크
        if (startUnit == null || targetUnit == null)
            return;

        // 자식 transform 체크
        if (startUnit.transform.childCount == 0 || targetUnit.transform.childCount == 0)
            return;

        GameObject startCard = startUnit.transform.GetChild(0).gameObject;
        GameObject targetCard;
        print("/////////////////////////" + targetIdx);
        if (targetIdx == -2)
        {
            targetCard = Hub.BattleSceneManager.playerHero.gameObject;
            print("영웅을 공격함");
        }
        else targetCard = targetUnit.transform.GetChild(0).gameObject;

        // 카드 오브젝트 null 체크
        if (startCard == null || targetCard == null)
            return;

        StartCoroutine(AIUseFieldCardCor(startCard, targetCard));
    }


    public void AIUseFieldCard(GameObject startIdx, GameObject targetIdx)
    {

    }

    public IEnumerator AITerm()
    {
        yield return new WaitForSeconds(2f);
    }

    public IEnumerator AITerm(float amount)
    {
        yield return new WaitForSeconds(amount);
    }

    private void SetStatistics()
    {
        clearedStageCount.text = $"{Hub.ProgressManager.CurrentStageIdx}";
        artifactsObtainedText.text = $"{Hub.ProgressManager.currentArtifact.Count}";
        // 처음에 기본적으로 카드 10장을 가지고 시작하므로 뺴줌
        cardsObtainedText.text = $"{Hub.ProgressManager.currentDeck.Count - 10}";
    }

    private void MoveToMain()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.TransitionManager.MoveTo(1, 1, 1);
    }











    #endregion

    #region 게임 결과 처리 파트

    // 필요 시 게임 결과 처리 로직 추가

    #endregion

    #region 보상 처리 파트
    
    // 이거 전부 따로 빼서 하는 듯 
    /*
    private void ShowCardSelection()
    {
        // "카드 획득" 버튼 비활성화
        cardRewardButton.interactable = false;

        // 카드 선택 화면 활성화
        cardSelectionScreen.SetActive(true);

        // 이전에 생성된 카드가 있다면 모두 제거
        foreach (Transform child in cardSelectionParent)
        {
            Destroy(child.gameObject);
        }

        // 랜덤한 카드 3개 선택
        cardOptions = GetRandomCardOptions(3);

        // 카드 프리팹 생성 및 표시
        foreach (int cardIndex in cardOptions)
        {
            // Hub.EnemyInfoManager.CardsList에서 보상 카드 프리팹을 가져와 생성
            GameObject card = Instantiate(Hub.EnemyInfoManager.CardsList[cardIndex].prefab, cardSelectionParent);
            card.GetComponent<A_Card>().CurrentPosition = CurrentPositions.Reward;  // 보상 카드 위치 설정
            card.transform.localScale = Vector3.one;

            // 카드 클릭 이벤트 설정
            Button cardButton = card.GetComponent<Button>();
            int selectedCardIndex = cardIndex; // 클로저 문제 해결을 위해 로컬 변수 사용
            cardButton.onClick.AddListener(() => OnCardSelected(selectedCardIndex));

            // DOTween을 사용하여 카드에 애니메이션 추가 (스케일 확대)
            card.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }

        // DOTween을 사용하여 카드 선택 화면 애니메이션 (위에서 내려오는 효과)
        cardSelectionScreen.transform.localPosition = new Vector3(0, Screen.height, 0);
        cardSelectionScreen.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBounce);
    }

    private List<int> GetRandomCardOptions(int count)
    {
        List<int> availableCards = new List<int>();

        // 해금된 카드 중에서 선택
        for (int i = 0; i < Hub.EnemyInfoManager.CardsList.Length; i++)
        {
            if (Hub.EnemyInfoManager.CardsList[i].prefab.GetComponent<A_Card>().unlockLevel <= Hub.ProgressManager.unlockLevel)
            {
                // 이미 2장 미만인 카드만 추가
                if (!Hub.ProgressManager.HasTwoCards(i))
                {
                    availableCards.Add(i);
                }
            }
        }

        // 선택할 카드 개수만큼 랜덤하게 선택
        List<int> selectedCards = new List<int>();
        for (int i = 0; i < count; i++)
        {
            if (availableCards.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableCards.Count);
            selectedCards.Add(availableCards[randomIndex]);
            availableCards.RemoveAt(randomIndex);
        }

        return selectedCards;
    }

    private void OnCardSelected(int cardIndex)
    {
        // 선택한 카드 획득
        Hub.ProgressManager.GetCard(cardIndex);

        // 카드 선택 화면 비활성화
        cardSelectionScreen.SetActive(false);

        // 승리 화면 페이드 아웃 후 메인으로 이동
        CanvasGroup victoryCanvasGroup = victoryScreen.GetComponent<CanvasGroup>();
        victoryCanvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            victoryScreen.SetActive(false);
            Hub.TransitionManager.MoveTo(1);
        });
    }

    public void Victory()
    {
        OppHP = 0;
    }

    */

    public void ButtonToMap()
    {
        if (Hub.ProgressManager.CurrentStageIdx == 12)
        {
            Hub.ProgressManager.InitializeProgress();
            Hub.TransitionManager.MoveTo(7, 1, 1);            
        }
        else
        {
            Hub.ProgressManager.CurrentGold += 15;
            Hub.SingletonUIManager.ButtonMap();
        }
    }

    #endregion

    #region 패배 처리 파트

    // NextButton을 누르면 After화면 출력되도록
    private void ShowStatistics()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        defeatBefore.SetActive(false);
        defeatMoveToMainButton.onClick.RemoveAllListeners();
        defeatMoveToMainButton.onClick.AddListener(MoveToMain);
        SetStatistics();

        defeatAfter.SetActive(true);
    }

    #endregion


}
