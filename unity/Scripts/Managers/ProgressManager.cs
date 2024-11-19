using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;

public enum CurrentStages
{
    Box,
    Battle,
    Event,
    Rest,
    Shop,
    Ending
}

public class ProgressManager : MonoBehaviour
{
    [Header("계속 가져가는 진행사항들")]
    public int unlockLevel = 0;
    public int formalStageIdx = 0;

    [SerializeField] private int _CurrentStageIdx = 0;
    public int CurrentStageIdx
    {
        get { return _CurrentStageIdx; }
        set
        {
            _CurrentStageIdx = value;
            UpdateUI();
            SaveGameState();
        }
    }

    public int battleCount = 0;
    [SerializeField] private int _CurrentHealth = 30;
    public int CurrentHealth
    {
        get { return _CurrentHealth; }
        set
        {
            _CurrentHealth = value;
            if (_CurrentHealth > MaxHealth) _CurrentHealth = MaxHealth;
            Hub.SingletonUIManager.intCurrentHP = _CurrentHealth;
            Hub.SingletonUIManager.tmpHP.text = Hub.SingletonUIManager.intCurrentHP + "/" + Hub.SingletonUIManager.intMaxHP;
            if (Hub.BattleSceneManager != null) Hub.BattleSceneManager.playerHealthTMP.text = _CurrentHealth.ToString();
            // SaveGameState();
        }
    }

    [SerializeField] private int _MaxHealth = 30;
    public int MaxHealth
    {
        get { return _MaxHealth; }
        set
        {
            _MaxHealth = value;
            Hub.SingletonUIManager.intMaxHP = _MaxHealth;
            Hub.SingletonUIManager.tmpHP.text = Hub.SingletonUIManager.intCurrentHP + "/" + Hub.SingletonUIManager.intMaxHP;
            // SaveGameState();
        }
    }

    [SerializeField] private int _CurrentGold;
    public int CurrentGold
    {
        get { return _CurrentGold; }
        set
        {
            _CurrentGold = value;
            Hub.SingletonUIManager.tmpGold.text = _CurrentGold.ToString();
            //SaveGameState();
        }
    }

    public List<int> currentDeck;
    public int[] currentPotions;
    public List<int> currentArtifact;

    [Header("현재 스테이지 정보")]
    public CurrentStages CurrentStage;
    [SerializeField]private bool _IsThisStageCleared = false;
    public bool IsThisStageCleared
    {
        get { return _IsThisStageCleared; }
        set
        {
            _IsThisStageCleared = value;
            SaveGameState();
        }
    }

    [Header("현재 전투일 경우")]
    public int currentEnemyIdx;
    public int shuffleNumber;

    [System.Serializable]
    public class Rewards
    {
        public string rewardType;
        public int rewardIdx;
    }
    public List<Rewards> rewardsList;

    [Header("현재 이벤트일 경우")]
    public int currentEventIdx;

    [Header("이벤트 출현 체크")]
    public bool[] isEventShown;

    [Header("현재 상점일 경우")]
    public int[] currentShopCardsList;
    public int[] currentShopArtifactsList;
    public int[] currentShopPotionsList;

    #region Initialization
    public void InitializeProgress()
    {
        _CurrentStageIdx = 0;
        battleCount = 0;
        CurrentHealth = 30;
        MaxHealth = 30;
        CurrentGold = 0;
        currentDeck = new List<int> { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4 };
        currentPotions = new int[3] { 0, 0, 0 };            // 이거 다시 기본값 0으로 바꿈. 기본값 0: 빈 포션
        currentArtifact = new List<int> { 0 };
        IsThisStageCleared = false;
        currentEnemyIdx = -1;
        shuffleNumber = 0;
        rewardsList = new List<Rewards>();
        currentEventIdx = -1;
        isEventShown = new bool[11];
        currentShopCardsList = new int[5];
        currentShopArtifactsList = new int[3];
        currentShopPotionsList = new int[3];

        SaveGameState();
    }
    #endregion

    #region Resource Management
    public int CheckAmountOfPotions()
    {
        int amount = 0;
        for (int i = 0; i < currentPotions.Length; i++)
        {
            if (currentPotions[i] != 0)
            {
                amount += 1;
            }
        }
        return amount;
    }

    public void AddPotion(int elementIdx)
    {
        Debug.Log("포션 추가 호출");
        for (int i = 0; i < currentPotions.Length; i++)
        {
            if (currentPotions[i] == 0)
            {
                currentPotions[i] = elementIdx;
                Destroy(Hub.SingletonUIManager.potionsTransform[i].GetChild(0).gameObject);
                GameObject potion = Instantiate(Hub.EnemyInfoManager.PotionsList[elementIdx].prefab, Hub.SingletonUIManager.potionsTransform[i]);
                potion.GetComponent<A_PotionIcon>().CurrentPosition = CurrentPositions.Header;
                potion.GetComponent<A_PotionIcon>().elementIdx = elementIdx;
                potion.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                // SaveGameState(); 이거 여기 있으면, 상점 등에서 얻은 다음에 다시 로드하면 이거만 얻은 채로 로드 될텐데
                break;
            }
        }
    }

    public void DiscardPotion(int idx)
    {
        currentPotions[idx] = 0;
        Destroy(Hub.SingletonUIManager.potionsTransform[idx].GetChild(0).gameObject);
        GameObject potion = Instantiate(Hub.EnemyInfoManager.PotionsList[0].prefab, Hub.SingletonUIManager.potionsTransform[idx]);
        potion.GetComponent<A_PotionIcon>().CurrentPosition = CurrentPositions.Header;
        potion.GetComponent<A_PotionIcon>().elementIdx = idx;
        potion.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    }

    public void UsePotion(int elementIdx)
    {
        // 제일 좋은건 각 포션 스크립트에서
        // inspector에서 처리하도록 하는게 좋겠지만
        // 일단은 여기다 처리함

        switch (elementIdx)
        {
            // 0은 빈 포션이라서 없음. 
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            default:
                break;
        }
    }

    public void IncreaseGold(int amount)
    {
        CurrentGold += amount;
    }

    public void DecreaseGold(int amount)
    {
        if (CurrentGold - amount < 0)
        {
            CurrentGold = 0;
        }
        else
        {
            CurrentGold -= amount;
        }
    }

    public void IncreaseHealth(int amount)
    {
        if (CurrentHealth + amount > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth += amount;
        }
    }

    public void DecreaseHealth(int amount)
    {
        CurrentHealth -= amount;
    }

    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
    }

    public void DecreaseMaxHealth(int amount)
    {
        if (MaxHealth - amount < 0)
        {
            MaxHealth = 0;
        }
        else
        {
            MaxHealth -= amount;
        }

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
    #endregion

    public void ClearAPC()
    {
        // Artifact
        foreach (Transform artifact in Hub.SingletonUIManager.articfactTransform)
        {
            Destroy(artifact.gameObject);
        }

        // Potion
        for (int i = 0; i < currentPotions.Length; i++)
        {
            // 포션은 어차피 가지고 있는 걸 삭제하고 추가하기 때문에 여기서 뭐 안해도 됨. 
        }

        // Card
        foreach (Transform card in Hub.SingletonUIManager.cardListTransform)
        {
            Destroy(card.gameObject);
        }


    }

    public void LoadAPC()
    {
        // Artifact 
        for (int i =0; i < currentArtifact.Count; i++)
        {
            GameObject artifactUnit = Instantiate(Hub.SingletonUIManager.artifactUnit, Hub.SingletonUIManager.articfactTransform);
            GameObject artifact = Instantiate(Hub.EnemyInfoManager.ArtifactsList[currentArtifact[i]].prefab, artifactUnit.transform);
            artifact.GetComponent<A_ArtifactIcon>().CurrentPosition = CurrentPositions.Header;
            artifact.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }

        // Potion
        for (int i = 0; i < currentPotions.Length; i++)
        {
            
            Destroy(Hub.SingletonUIManager.potionsTransform[i].GetChild(0).gameObject);
            GameObject potion = Instantiate(Hub.EnemyInfoManager.PotionsList[currentPotions[i]].prefab, Hub.SingletonUIManager.potionsTransform[i]);
            potion.GetComponent<A_PotionIcon>().CurrentPosition = CurrentPositions.Header;
            potion.GetComponent<A_PotionIcon>().elementIdx = currentPotions[i];
            potion.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }

        // Card
        for (int i = 0; i < currentDeck.Count; i++)
        {            
            GameObject card = Instantiate(Hub.EnemyInfoManager.CardsList[currentDeck[i]].prefab, Hub.SingletonUIManager.cardListTransform);
            card.transform.localScale = Vector3.one;
            card.GetComponent<A_Card>().CurrentPosition = CurrentPositions.Deck;

            
        }
    }



    #region Artifact Management
    public void GetArtifact(int value)
    {
        if (currentArtifact.Contains(value))
        {
            Debug.Log("이미 해당 아티팩트를 소지하고 있습니다.");
            return;
        }

        currentArtifact.Add(value);
        GameObject artifactUnit = Instantiate(Hub.SingletonUIManager.artifactUnit, Hub.SingletonUIManager.articfactTransform);
        GameObject artifact = Instantiate(Hub.EnemyInfoManager.ArtifactsList[value].prefab, artifactUnit.transform);
        artifact.GetComponent<A_ArtifactIcon>().CurrentPosition = CurrentPositions.Header;
        artifact.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        // SaveGameState();
    }

    public void RemoveArtifact(int value)
    {
        if (!currentArtifact.Contains(value))
        {
            Debug.Log("해당 아티팩트를 소지하고 있지 않습니다.");
            return;
        }

        currentArtifact.Remove(value);
        // SaveGameState();
    }

    public void GetRandomArtifacts(int amount)
    {
        List<int> unownedArtifacts = new List<int>();
        for (int i = 1; i <= 10; i++)
        {
            if (!currentArtifact.Contains(i))
            {
                unownedArtifacts.Add(i);
            }
        }

        if (unownedArtifacts.Count == 0)
        {
            Debug.Log("더 이상 획득할 수 있는 아티팩트가 없습니다.");
            return;
        }

        int actualAmount = Mathf.Min(amount, unownedArtifacts.Count);
        for (int i = 0; i < actualAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unownedArtifacts.Count);
            int selectedArtifact = unownedArtifacts[randomIndex];
            GetArtifact(selectedArtifact);
            unownedArtifacts.RemoveAt(randomIndex);
        }
    }
    #endregion

    #region Card Management
    public void GetCard(int value)
    {
        if (HasTwoCards(value))
        {
            Debug.Log($"이미 {value}번 카드를 2장 소지하고 있습니다.");
            return;
        }

        currentDeck.Add(value);
        GameObject card = Instantiate(Hub.EnemyInfoManager.CardsList[value].prefab, Hub.SingletonUIManager.cardListTransform);
        card.transform.localScale = Vector3.one;
        card.GetComponent<A_Card>().CurrentPosition = CurrentPositions.Deck;

        // SaveGameState(); 여기에는 안 함
    }

    public void RemoveCard(int value)
    {
        if (!currentDeck.Contains(value))
        {
            Debug.Log("해당 카드를 소지하고 있지 않습니다.");
            return;
        }

        currentDeck.Remove(value);
        ClearAPC();
        LoadAPC();
        // SaveGameState();
    }

    public void GetRandomCards(int amount)
    {
        List<int> availableCards = new List<int>();
        for (int i = 1; i <= 112; i++)
        {
            if (!HasTwoCards(i))
            {
                availableCards.Add(i);
            }
        }

        if (availableCards.Count == 0)
        {
            Debug.Log("더 이상 추가할 수 있는 카드가 없습니다.");
            return;
        }

        int actualAmount = Mathf.Min(amount, availableCards.Count);

        for (int i = 0; i < actualAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
            int selectedCard = availableCards[randomIndex];

            GetCard(selectedCard);
            availableCards.RemoveAt(randomIndex);
        }
    }

    public void GetRandomSpellCards(int amount)
    {
        // 마법 카드의 인덱스 범위(113~134)에서 현재 소지하지 않은 카드만 필터링
        List<int> availableSpellCards = new List<int>();
        for (int i = 113; i <= 134; i++)
        {
            if (!HasTwoCards(i))
            {
                availableSpellCards.Add(i);
            }
        }

        if (availableSpellCards.Count == 0)
        {
            Debug.Log("더 이상 추가할 수 있는 마법 카드가 없습니다.");
            return;
        }

        // 요청된 수량과 사용 가능한 카드 수 중 더 작은 값을 선택
        int actualAmount = Mathf.Min(amount, availableSpellCards.Count);

        // 랜덤하게 카드를 선택하고 덱에 추가
        for (int i = 0; i < actualAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableSpellCards.Count);
            int selectedCard = availableSpellCards[randomIndex];

            GetCard(selectedCard);
            availableSpellCards.RemoveAt(randomIndex);
        }
    }

    public void GetRandomPotions(int amount)
    {
        int currentPotionCount = CheckAmountOfPotions();

        if (currentPotionCount == 3)
        {
            Debug.Log("더 이상 포션을 획득할 수 없습니다.");
            return;
        }

        int actualAmount = Mathf.Min(amount, 3 - currentPotionCount);

        for (int i = 0; i < actualAmount; i++)
        {
            // 1부터 10까지의 랜덤한 포션 인덱스 생성
            int randomPotionIndex = UnityEngine.Random.Range(1, 11);
            AddPotion(randomPotionIndex);
        }
    }

    public void RemoveRandomMinionCards(int amount)
    {
        List<int> minionCards = currentDeck.Where(card => card >= 0 && card <= 112).ToList();

        if (minionCards.Count == 0)
        {
            Debug.Log("제거할 수 있는 하수인 카드가 없습니다.");
            return;
        }

        int actualAmount = Mathf.Min(amount, minionCards.Count);

        for (int i = 0; i < actualAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, minionCards.Count);
            int selectedCard = minionCards[randomIndex];

            RemoveCard(selectedCard);
            minionCards.RemoveAt(randomIndex);
        }
    }

    public int GetMinionCardCount()
    {
        return currentDeck.Count(card => card >= 0 && card <= 112);
    }

    public bool HasTwoCards(int value)
    {
        int cardCount = currentDeck.Count(card => card == value);
        return cardCount >= 2;
    }
    #endregion

    #region UI and Save Management
    private void UpdateUI()
    {
        if (Hub.SingletonUIManager != null)
        {
            if (Hub.SingletonUIManager.tmpCurrentStage != null)
                Hub.SingletonUIManager.tmpCurrentStage.text = _CurrentStageIdx.ToString();

            if (Hub.SingletonUIManager.tmpHP != null)
                Hub.SingletonUIManager.tmpHP.text = $"{_CurrentHealth}/{_MaxHealth}";

            if (Hub.SingletonUIManager.tmpGold != null)
                Hub.SingletonUIManager.tmpGold.text = _CurrentGold.ToString();

            if(Hub.SingletonUIManager.userName != null)
                Hub.SingletonUIManager.userName.text = PlayerPrefs.GetString("nickName", "미로그인상태");
        }
    }

    public void SaveGameState()
    {
        Hub.NetworkManager.SaveGameData(Hub.MapManager?.CurrentMap);
    }

    public void LoadProgressData(APIManager.PlayerProgress progress)
    {
        if (progress == null)
        {
            Debug.LogError("Progress data is null");
            return;
        }

        try
        {
            Debug.Log($"Loading progress data - Stage before: {_CurrentStageIdx}, New stage: {progress.currentStageIdx}");

            _CurrentStageIdx = progress.currentStageIdx;  // 직접 필드에 할당
            battleCount = progress.battleCount;
            CurrentHealth = progress.currentHealth;
            MaxHealth = progress.maxHealth;
            CurrentGold = progress.currentGold;
            currentDeck = progress.currentDeck ?? new List<int>();
            currentPotions = progress.currentPotions ?? new int[3] { 0, 0, 0 };
            currentArtifact = progress.currentArtifact ?? new List<int>();
            IsThisStageCleared = progress.isThisStageCleared;
            currentEnemyIdx = progress.currentEnemyIdx;
            unlockLevel = progress.unlockLevel;
            formalStageIdx = progress.formalStageIdx;
            shuffleNumber = progress.shuffleNumber;
            rewardsList = progress.rewardsList ?? new List<Rewards>();
            currentEventIdx = progress.currentEventIdx;
            isEventShown = progress.isEventShown ?? new bool[11];
            currentShopCardsList = progress.currentShopCardsList ?? new int[5];
            currentShopArtifactsList = progress.currentShopArtifactsList ?? new int[3];
            currentShopPotionsList = progress.currentShopPotionsList ?? new int[3];
            // CurrentStage도 설정해야 할 수 있음

            UpdateUI();
            Debug.Log($"Progress data loaded - Current stage: {_CurrentStageIdx}");
            Debug.Log("야 개짓는 소리좀 안나겧 해라" + progress.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load progress data: {e.Message}");
            throw;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameState();
    }
    #endregion
}