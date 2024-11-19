using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Lean.Localization;
using System.Linq;
using static EventSceneManager;

public class EventSceneManager : MonoBehaviour
{
    int eventNum;           // 이번의 이벤트 번호 
    public GameObject beforeEvent;
    public GameObject afterEvent;


    public LeanLocalizedTextMeshProUGUI title;
    public LeanLocalizedTextMeshProUGUI discription;    
    public LeanLocalizedTextMeshProUGUI[] buttonsAnswer;
    public LeanLocalizedTextMeshProUGUI resultDiscription;
    public LeanLocalizedTextMeshProUGUI resultNotification;
    public LeanLocalizedTextMeshProUGUI buttonToMap;


    [System.Serializable]
    public class Result
    {
        public float probability; // 해당 결과가 나올 확률 (0~1)
        public Reward[] rewards; // 보상 배열
        public LeanPhrase resultDiscription; // 결과 설명
        public LeanPhrase resultNotification; // 결과 알림
    }

    [System.Serializable]
    public class Reward
    {
        public RewardType rewardType;
        public int value;
    }


    [System.Serializable]
    public class Conditions
    {
        public ConditionType conditionType;
        public int minGold;
        public int minHealth;
        // 아래의 아티팩트와 카드는 전부 pk값
        public int requiredArtifact;
        public int requiredCard;
        public int requiredMinionsAmount;
        public int requiredMinEmptyPotionSlot;
    }

    [System.Serializable]
    public class Answers
    {
        public Result[] possibleResults; // 선택지의 결과가 여러개 중 랜덤으로 나올 경우 처리
        public LeanPhrase buttonAnswer;
        public Conditions[] conditions;
    }

    [System.Serializable]
    public class EventConditions
    {
        public EventConditionType eventCondition;
        public int minGold;
        public int minHealth;
        public int requiredArtifact;
    }
    

    [System.Serializable]
    public class Events
    {
        public string name;
        public LeanPhrase title;
        public LeanPhrase discription;
        public EventConditions[] eventCondition;
        public Answers[] answers;
    }
    public Events[] events;


    public void Awake()
    {
        // 브금틀기
        Hub.SoundManager.BgmSelectPlay(5);

        // 씬 초기화 예외처리
        beforeEvent.SetActive(true);
        afterEvent.SetActive(false);

        // 아직 출력되지 않은 이벤트들 중에서 랜덤으로 번호 가져오기
        if (!Hub.ProgressManager.IsThisStageCleared)
        {
            // 아직 클리어 되어있지 않는다면 랜덤으로 이벤트 맵 랜덤으로 가져오기
            eventNum = GetRandomUnshownEvent();
        }
        else
        {
            // 클리어 처리 되어있다면 저장된 이벤트 인덱스 가져오기
            eventNum = Hub.ProgressManager.currentEventIdx;
        }
        //eventNum = 10;

        // BeforeEvent
        title.TranslationName = events[eventNum].title.name;
        discription.TranslationName = events[eventNum].discription.name;

        //안 쓰는 버튼은 끄기
        for (int i = 0; i < 4 - events[eventNum].answers.Length; i++)
        {
            buttonsAnswer[3 - i].GetComponentInParent<Button>().gameObject.SetActive(false);
        }
            
        //각 버튼 별로 표시하기
        for (int i = 0; i < events[eventNum].answers.Length; i++)
        {
            // 조건에 맞지 않으면 안 뜨게 하는 기능 여기에 넣으면 될 듯
            if (CheckConditions(events[eventNum].answers[i].conditions))
            {
                buttonsAnswer[i].GetComponentInParent<Button>().gameObject.SetActive(true);
                buttonsAnswer[i].TranslationName = events[eventNum].answers[i].buttonAnswer.name;
            }
            else
            {
                buttonsAnswer[i].GetComponentInParent<Button>().interactable = false;
                buttonsAnswer[i].GetComponent<TextMeshProUGUI>().color = Color.red;
                buttonsAnswer[i].TranslationName = events[eventNum].answers[i].buttonAnswer.name;
            }
        }
    }

    public void Start()
    {
        if (!Hub.ProgressManager.IsThisStageCleared)
        {
            Hub.ProgressManager.IsThisStageCleared = true;
        }
    }

    public void ButtonEventPressed(int num)
    {
        Hub.SoundManager.SfxSelectPlay(0);

        // 결과에 따른 선택지 변화
        beforeEvent.SetActive(false);
        afterEvent.SetActive(true);

        // 확률에 따른 결과 선택
        Result selectedResult = SelectResult(events[eventNum].answers[num].possibleResults);

        // 선택된 결과의 텍스트 표시
        resultDiscription.TranslationName = selectedResult.resultDiscription.name;
        resultNotification.TranslationName = selectedResult.resultNotification.name;

        // 조건에 따른 결과를 넣어주어야 할 듯 
        // 골드, 포션, 아티펙트, 체력, 카드 추가/ 삭제 기능
        // OOP로 해주기
        if (CheckReward(selectedResult.rewards))
        {
            buttonToMap.GetComponentInParent<Button>().onClick.AddListener(ButtonToBattle);
        }
        else
        {
            buttonToMap.GetComponentInParent<Button>().onClick.AddListener(ButtonToMap);
        }
    }

    // 선택지의 결과가 여러개일 경우 랜덤으로 출력되도록 함
    private Result SelectResult(Result[] possibleResults)
    {
        // 확률 값이 유효한지 먼저 체크
        float totalProbability = possibleResults.Sum(r => r.probability);
        if (Mathf.Abs(totalProbability - 1f) > 0.0001f)
        {
            Debug.Log("선택지의 결과값의 확률 총합이 1이 아닙니다.");
        }

        float randomValue = Random.value;

        float accumulatedProbability = 0f;
        for (int i = 0; i < possibleResults.Length; i++)
        {
            accumulatedProbability += possibleResults[i].probability;

            if (randomValue <= accumulatedProbability)
            {
                return possibleResults[i];
            }
        }

        // 부동소수점 오차로 인해 마지막 결과가 선택되지 않을 수 있으므로
        // 마지막 결과 반환
        return possibleResults[possibleResults.Length - 1];
    }

    // 이벤트의 조건을 체크
    private bool CheckEventCondition(EventConditions[] eventConditions)
    {
        // 조건이 없다면 이벤트 출력
        if (eventConditions == null || eventConditions.Length == 0) return true;

        // 모든 조건을 확인
        foreach (var condition in eventConditions)
        {
            if (condition.eventCondition == EventConditionType.MinGold)
            {
                // 골드 조건 체크
                if (condition.minGold == 0 || Hub.ProgressManager.CurrentGold < condition.minGold)
                {
                    return false;
                }
            }
            else if (condition.eventCondition == EventConditionType.MinHealth)
            {
                // 최소 체력 조건 체크
                if (condition.minHealth == 0 || Hub.ProgressManager.CurrentHealth < condition.minHealth)
                {
                    return false;
                }
            }
            else if (condition.eventCondition == EventConditionType.HasEmptyPotionSlot)
            {
                // 빈 포션 슬롯이 있는지 체크
                if (Hub.ProgressManager.CheckAmountOfPotions() != 0)
                {
                    return false;
                }
            }
            else if (condition.eventCondition == EventConditionType.RequiredArtifact)
            {
                // 아티팩트 소지 조건 체크
                if (condition.requiredArtifact == 0 || !Hub.ProgressManager.currentArtifact.Contains(condition.requiredArtifact))
                {
                    return false;
                }
            }
            else if (condition.eventCondition == EventConditionType.HasDwarfCards)
            {
                // 드워프 카드(45~52) 소지 여부 체크
                bool hasDwarfCard = Hub.ProgressManager.currentDeck.Any(card => card >= 45 && card <= 52);
                if (!hasDwarfCard)
                {
                    return false;
                }
            }
            else if (condition.eventCondition == EventConditionType.Dragon)
            {
                // 용과의 거래 이벤트는 금화 25이상 소지하거나 체력이 2이상 있을 때만
                if (Hub.ProgressManager.CurrentGold < 25 || Hub.ProgressManager.CurrentHealth <= 1)
                {
                    return false;
                }
            }
            else if (condition.eventCondition == EventConditionType.VoicesCallingForPeace)
            {
                // 평화를 외치는 목소리 이벤트는 체력이 8이상이거나 하수인 카드가 2장 이상 있을때만
                if (Hub.ProgressManager.CurrentHealth < 8 || Hub.ProgressManager.GetMinionCardCount() < 2)
                {
                    return false;
                }
            }
        }

        // 모든 조건을 만족하면 true 반환
        return true;
    }

    // 선택지의 조건을 체크
    private bool CheckConditions(Conditions[] conditions)
    {
        // 조건이 없다면 true 반환
        if (conditions == null || conditions.Length == 0) return true;

        // 모든 조건을 검사
        foreach (var condition in conditions)
        {
            if (condition.conditionType == ConditionType.MinGold)
            {
                // 골드 조건 체크
                if (Hub.ProgressManager.CurrentGold < condition.minGold)
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.MinHealth)
            {
                // 최소 체력 조건 체크
                if (Hub.ProgressManager.CurrentHealth < condition.minHealth)
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.RequiredArtifact)
            {
                // 아티팩트 소지 조건 체크
                if (!Hub.ProgressManager.currentArtifact.Contains(condition.requiredArtifact))
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.MissingArtifact)
            {
                // 아티팩트 미소지 조건 체크
                if (Hub.ProgressManager.currentArtifact.Contains(condition.requiredArtifact))
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.RequiredCard)
            {
                // 카드 소지 조건 체크
                if (!Hub.ProgressManager.currentDeck.Contains(condition.requiredCard))
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.MissingCard)
            {
                // 카드 미소지 조건 체크
                if (Hub.ProgressManager.currentDeck.Contains(condition.requiredCard))
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.HasEnoughMinions)
            {
                // 하수인 카드를 충분히 가지고 있는지
                if (Hub.ProgressManager.GetMinionCardCount() < condition.requiredMinionsAmount)
                {
                    return false;
                }
            }
            if (condition.conditionType == ConditionType.HasEmptyPotionSlot)
            {
                // 빈 포션 슬롯을 충분히 가지고 있는지
                if (3 - Hub.ProgressManager.CheckAmountOfPotions() < condition.requiredMinEmptyPotionSlot)
                {
                    return false;
                }
            }
        }

        // 모든 조건을 만족하면 true 반환
        return true;
    }

    public bool CheckReward(Reward[] rewards)
    {
        // RewardType이 StartBattle인 경우에만 true로 설정하여 전투 맵으로 이동
        bool shouldStartBattle = false;

        // rewards 배열이 비어있거나 null인 경우 처리
        if (rewards == null || rewards.Length == 0)
            return false;

        // 모든 보상을 순회하면서 처리
        foreach (Reward reward in rewards)
        {
            switch (reward.rewardType)
            {
                case RewardType.DecreaseGold:
                    Hub.ProgressManager.DecreaseGold(reward.value);
                    break;
                case RewardType.IncreaseGold:
                    Hub.ProgressManager.IncreaseGold(reward.value);
                    break;
                case RewardType.DecreaseHealth:
                    Hub.ProgressManager.DecreaseHealth(reward.value);
                    break;
                case RewardType.IncreaseHealth:
                    Hub.ProgressManager.IncreaseHealth(reward.value);
                    break;
                case RewardType.DecreaseMaxHealth:
                    Hub.ProgressManager.DecreaseMaxHealth(reward.value);
                    break;
                case RewardType.IncreaseMaxHealth:
                    Hub.ProgressManager.IncreaseMaxHealth(reward.value);
                    break;
                case RewardType.GetArtifact:
                    Hub.ProgressManager.GetArtifact(reward.value);
                    break;
                case RewardType.RemoveArtifact:
                    Hub.ProgressManager.RemoveArtifact(reward.value);
                    break;
                case RewardType.GetCard:
                    Hub.ProgressManager.GetCard(reward.value);
                    break;
                case RewardType.RemoveCard:
                    Hub.ProgressManager.RemoveCard(reward.value);
                    break;
                case RewardType.GetRandomCard:
                    Hub.ProgressManager.GetRandomCards(reward.value);
                    break;
                case RewardType.GetRandomSpellCard:
                    Hub.ProgressManager.GetRandomSpellCards(reward.value);
                    break;
                case RewardType.RemoveRandomMinionCards:
                    Hub.ProgressManager.RemoveRandomMinionCards(reward.value);
                    break;
                case RewardType.GetRandomArtifacts:
                    Hub.ProgressManager.GetRandomArtifacts(reward.value);
                    break;
                case RewardType.GetRandomPotions:
                    Hub.ProgressManager.GetRandomPotions(reward.value);
                    break;
                case RewardType.DragonEvent:
                    // 여기에 용과의 거래 진행
                    ProgressDragonEvent();
                    break;
                case RewardType.StartBattle:
                    shouldStartBattle = true;
                    break;
            }
        }

        return shouldStartBattle;
    }

    // 이벤트 랜덤으로 출력
    public int GetRandomUnshownEvent() // 나중에 현재 스테이지 등급에 맞게 이벤트 출력시키기
    {
        // 조건을 만족하는 미출현 이벤트 찾기
        List<int> unshownEvents = new List<int>();
        for (int i = 0; i < events.Length; i++)
        {
            if (!Hub.ProgressManager.isEventShown[i] && CheckEventCondition(events[i].eventCondition))
            {
                unshownEvents.Add(i);
            }
        }

        // 랜덤으로 하나 선택
        int selectedIndex = Random.Range(0, unshownEvents.Count);
        int selectedEventNum = unshownEvents[selectedIndex];

        // 선택된 이벤트를 출현한 것으로 표시
        Hub.ProgressManager.isEventShown[selectedEventNum] = true;

        // 방문한 이벤트 저장
        Hub.ProgressManager.currentEventIdx = selectedEventNum;

        Debug.Log($"다음 이벤트 랜덤으로 선택됨: {selectedEventNum}");
        return selectedEventNum;
    }


    public void ChangeItem(string itemType, int value)
    {

    }

    // 용과의 거래 이벤트 결과 진행
    private void ProgressDragonEvent()
    {
        int gold = Hub.ProgressManager.CurrentGold;
        int amount = gold / 25;
        if (amount >= 4)
        {
            // 금화 25당 랜덤 카드 제공
            // 100골드 까지만 랜덤 카드 제공
            amount = 4;
        }
        else
        {
            amount = gold / 25;
        }
        Hub.ProgressManager.GetRandomCards(amount);
        Hub.ProgressManager.CurrentGold -= 25 * amount;

        gold = Hub.ProgressManager.CurrentGold;
        amount = gold / 50;
        // 이후에 돈이 있다면 50당 아티팩트 랜덤 제공
        Hub.ProgressManager.GetRandomArtifacts(amount);
        Hub.ProgressManager.CurrentGold -= 50 * amount;
    }


    public void ButtonToMap()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.SingletonUIManager.ButtonMap();
    }

    // 전투화면으로 이동
    public void ButtonToBattle()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Debug.Log("전투 시작");
        Hub.ProgressManager.IsThisStageCleared = false;
        Hub.TransitionManager.MoveTo(3);
    }

    // 이벤트 조건 종류
    public enum ConditionType
    {
        MinGold,
        MinHealth,
        RequiredArtifact,
        MissingArtifact,
        RequiredCard,
        HasEnoughMinions,
        HasEmptyPotionSlot,
        MissingCard
    }

    // 이벤트 선택지의 결과 종류(해당 종류에 맞게 분기 나눠서 보상 달라짐)
    public enum RewardType
    {
        DecreaseGold,
        IncreaseGold,
        DecreaseHealth,
        IncreaseHealth,
        DecreaseMaxHealth,
        IncreaseMaxHealth,
        GetArtifact,
        RemoveArtifact,
        GetCard,
        RemoveCard,
        GetRandomCard,
        GetRandomSpellCard,
        GetRandomArtifacts,
        GetRandomPotions,
        RemoveRandomMinionCards,
        DragonEvent,
        StartBattle
    }

    // 이벤트의 조건 타입
    public enum EventConditionType
    {
        MinGold,
        MinHealth,
        HasEmptyPotionSlot,
        RequiredArtifact,
        HasDwarfCards,
        Dragon,
        VoicesCallingForPeace
    }
}
