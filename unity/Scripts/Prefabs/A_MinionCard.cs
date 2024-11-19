using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Lean.Localization;



public class A_MinionCard : A_Card
{
    [Header("디폴트 카드 상태")]
    public int defaultAttack;
    public int defaultHealth;
    public int defaultCost;

    [Header("카드 상태")]
    [HideInInspector] public int attack;
    [HideInInspector] public int health;
    [HideInInspector] public int cost;

    [Header("TMP_text 부분")]
    public TMP_Text tmpAttack;
    public TMP_Text tmpHealth;
    public TMP_Text tmpCost;

    [Header("내부 부분")]
    public GameObject[] explanationMinionCard = new GameObject[3];
    public GameObject cardShader;
    public GameObject cardOnField;

    [HideInInspector] public bool[] abilityButtons = new bool[3];
    [HideInInspector] public string[] abilitylabel = new string[3] { "도발", "전투의 함성", "보호막" };

    // 버튼 클릭 시 호출될 메서드
    public void OnAbilityButtonClicked(int index)
    {
        // 배열의 해당 인덱스의 bool 값 토글
        abilityButtons[index] = !abilityButtons[index];

        // 필요한 동작 수행
        if (abilityButtons[index])
        {
            Debug.Log("켜짐: " + abilitylabel[index]);
        }
        else
        {
            Debug.Log("꺼짐: " + abilitylabel[index]);
        }
    }


    public override void Awake()
    {

        base.Awake();

        attack = defaultAttack;
        health = defaultHealth;
        cost = defaultCost;
        tmpAttack.text = attack.ToString();
        tmpHealth.text = health.ToString();
        tmpCost.text = cost.ToString();

        // 여기서 abilityButtons bool 버튼에 따라
        // explanation을 삭제할 거임
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (!abilityButtons[i]) Destroy(explanationMinionCard[i]);
        }




    }


    public override void CheckCardBorder()
    {
        base.CheckCardBorder();
        if (cost <= Hub.BattleSceneManager.MyMana && Hub.BattleSceneManager.BattleState == BattleStates.MyTurn && Hub.BattleSceneManager.myBattleField.unitsList.Count < 5) cardShader.SetActive(true);
        else cardShader.SetActive(false);
        print("이거 불러와지긴 해?");
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        // 덱에서 클릭했을 경우
        if (CurrentPosition == CurrentPositions.Deck)
        {

        }



    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (CurrentPosition == CurrentPositions.Battlehand)
        {
            this.transform.position = Input.mousePosition;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        switch (CurrentPosition)
        {
            case CurrentPositions.Idle:
                break;
            case CurrentPositions.Header:
                break;
            case CurrentPositions.Shop:
                break;
            case CurrentPositions.Battlehand:

                
                // 게임 보드 위에 해당 카드의 중심점이 포함되어 있는지 확인
                // 게임 보드를 잡음
                RectTransform gameBoardTransform = Hub.BattleSceneManager.gameBoard.GetComponent<RectTransform>();

                RectTransform cardRectTransform = this.GetComponent<RectTransform>();
                // 카드의 중심점(로컬 좌표)을 월드 좌표로 변환
                Vector3 cardCenterWorldPosition = this.GetComponent<RectTransform>().TransformPoint(new Vector3(0f, 0f, 0f));                
                // 월드 좌표를 화면 좌표로 변환
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, cardCenterWorldPosition);
                // 화면 좌표가 타겟 RectTransform의 범위 안에 있는지 확인
                bool isInside = RectTransformUtility.RectangleContainsScreenPoint(gameBoardTransform, screenPoint, Camera.main);
                
                if (isInside) // 게임 보드일 경우
                {
                    if (Hub.BattleSceneManager.MyMana >= cost && Hub.BattleSceneManager.myBattleField.unitsList.Count < 5) // 마나가 충분하면, 필드에 5개까지만
                    {
                        Hub.SoundManager.SfxSelectPlay(5);

                        Hub.BattleSceneManager.MyMana -= cost;

                        // 아티펙트 6: 15개의 마나를 사용하면, 마나 +1
                        if (Hub.ProgressManager.currentArtifact.Contains(6)) Hub.ArtifactManager.Artifact6(cost);

                        A_BattleField myField = Hub.BattleSceneManager.myField.GetComponent<A_BattleField>();
                        myField.AddUnit();      // 유닛 자리 추가하고 
                        GameObject currentPrefab = Instantiate(cardOnField, myField.unitsList[myField.unitsList.Count - 1].transform);     // 그 유닛에다가 집어넣고 
                        A_MinionCardInField fieldCard = currentPrefab.GetComponent<A_MinionCardInField>();
                        ///////////////////////////// 이거 반드시 바꿔줘야 함!! 
                        ///////////////////////////// 그리고 메소드로 해야 사용 가능!! 
                        fieldCard.IsPlayer = IsPlayers.Player;
                        fieldCard.Health = health;
                        fieldCard.Attack = attack;
                        fieldCard.IsTaunt = abilityButtons[0] ? true : false;
                        fieldCard.IsWarCry = abilityButtons[1] ? true : false;
                        fieldCard.IsShield = abilityButtons[2] ? true : false;
                        fieldCard.isInSleep = true;
                        fieldCard.IsUsed = false;
                        fieldCard.Initialize();

                        print("카드 사용됨");
                        myHand.UnitUsed();          // hand의 유닛은 없애기. OnPointEnter 하면 들어가니까 이 값은 있을 수 밖에 없음. 
                        Destroy(this.gameObject);
                    }
                    else // 마나가 충분하지 않으면
                    {
                        this.transform.SetParent(originalTransform);
                        this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        //StopCoroutine("TranslateScaleCor");
                        StartCoroutine(TranslateScaleCor(new Vector3(0.25f, 0.25f, 1f), Vector3.zero, false));       // 이거 안 해도 어차피 SettingPositions에서 해 줌.
                        myHand.SettingPositions();
                    }                    
                }
                else // 보드가 아닐 경우
                {
                    this.transform.SetParent(originalTransform);
                    this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    //StopCoroutine("TranslateScaleCor");
                    StartCoroutine(TranslateScaleCor(new Vector3(0.25f, 0.25f, 1f), Vector3.zero, false));       // 이거 안 해도 어차피 SettingPositions에서 해 줌.
                    myHand.SettingPositions();
                }






                
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
