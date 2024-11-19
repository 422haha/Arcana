using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Lean.Localization;


public class ShopSceneManager : MonoBehaviour
{
    // 구매까지는 다 됬는데
    // 희귀도랑 해금 레벨에 따라 나오는거 하면 될 듯 
    // 해금 레벨도 일단 어느정도 되 있긴 함.

    [Header("내부 부분")]
    public Transform currentSelection;


    [System.Serializable]
    public class Slot
    {
        public Transform prefabPos;
        public int elementIdx;
    }
    [Header("상점 슬롯 부분")]
    public Slot[] cardsListInShop;
    public Slot[] potionsListInShop;
    public Slot[] artifactsListInShop;


    // 해금에 따라 또 나뉘어야 되잖아
    // 나중에는 포션이나 아티펙트도 해금 하게 하면 좋을 거 같긴 한데 
    // 일단 카드에 대해서 해금 그룹을 묶어야겠지.
    [Header("랜덤으로 나온 오브젝트들")]
    public List<int> pickedCards;
    public List<int> pickedPotions;
    public List<int> pickedArtifacts;



    public int ReRoll(int num)
    {
        return Random.Range(0, num);
    }

    public IEnumerator PickCards(int count)
    {
        pickedCards.Clear();
        for (int i = 0; i < count; i++)
        {
            // do while로 
            // 해금 레벨이 아니거나 중복이면 다시 리롤
            int cardNum = ReRoll(Hub.EnemyInfoManager.CardsList.Length);
            int rerollCount = 0;
            while (Hub.EnemyInfoManager.CardsList[cardNum].prefab.GetComponent<A_Card>().unlockLevel > Hub.ProgressManager.unlockLevel || pickedCards.Contains(cardNum) || Hub.ProgressManager.currentDeck.Contains(cardNum))
            {
                cardNum = ReRoll(Hub.EnemyInfoManager.CardsList.Length);
                rerollCount += 1;
                if (rerollCount == Hub.EnemyInfoManager.CardsList.Length) break;
            }

            if (pickedCards.Count != count)
            {
                pickedCards.Add(cardNum);
                Hub.ProgressManager.currentShopCardsList[i] = cardNum;
            }
        }
        yield return null;
    }

    public IEnumerator PickPotions(int count)
    {
        pickedPotions.Clear();
        for (int i = 0; i < count; i++)
        {
            // do while로 
            // 중복이면 다시 리롤 (이미 뽑은 거거나, 이미 구매한 거라면)
            int potionNum = ReRoll(Hub.EnemyInfoManager.PotionsList.Length);
            int rerollCount = 0;
            while (pickedPotions.Contains(potionNum) | Hub.ProgressManager.currentPotions.Contains(potionNum) | potionNum == 0)
            {
                potionNum = ReRoll(Hub.EnemyInfoManager.PotionsList.Length);
                rerollCount += 1;
                if (rerollCount == Hub.EnemyInfoManager.PotionsList.Length) break;
            }

            if (pickedPotions.Count != count) pickedPotions.Add(potionNum);
            Hub.ProgressManager.currentShopPotionsList[i] = potionNum;
        }
        yield return null;
    }

    public IEnumerator PickArtifacts(int count)
    {
        pickedArtifacts.Clear();
        for (int i = 0; i < count; i++)
        {
            // do while로 
            // 중복이면 다시 리롤 (이미 뽑은 거거나, 이미 구매한 거라면)
            int artifactNum = ReRoll(Hub.EnemyInfoManager.ArtifactsList.Length);
            int rerollCount = 0;
            while (pickedArtifacts.Contains(artifactNum) | Hub.ProgressManager.currentArtifact.Contains(artifactNum))
            {
                artifactNum = ReRoll(Hub.EnemyInfoManager.ArtifactsList.Length);
                rerollCount += 1;
                if (rerollCount == Hub.EnemyInfoManager.ArtifactsList.Length) break;
            }
                

            if (pickedArtifacts.Count != count) pickedArtifacts.Add(artifactNum);
            Hub.ProgressManager.currentShopArtifactsList[i] = artifactNum;
        }
        yield return null;
    }


    public void LoadCards(int count)
    {
        pickedCards.Clear();
        for (int i = 0; i < count; i++) pickedCards.Add(Hub.ProgressManager.currentShopCardsList[i]);
    }
    public void LoadPotions(int count)
    {
        pickedPotions.Clear();
        for (int i = 0; i < count; i++) pickedPotions.Add(Hub.ProgressManager.currentShopPotionsList[i]);
    }
    public void LoadArtifacts(int count)
    {
        pickedArtifacts.Clear();
        for (int i = 0; i < count; i++) pickedArtifacts.Add(Hub.ProgressManager.currentShopArtifactsList[i]);
    }












    [Header("상인 파트")]
    public LeanLocalizedTextMeshProUGUI tmpDialog;
    public LeanPhrase[] dialogGreeting;
    public LeanPhrase[] dialogPotionsAreFull;
    public LeanPhrase[] dialogThankYou;
    public LeanPhrase[] dialogNotEngouhMoney;










    private void Awake()
    {
        // 브금 틀기
        Hub.SoundManager.BgmSelectPlay(7);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShopSetting());
    }

    public IEnumerator ShopSetting()
    {
        // isThisStageCleared false: 로딩하기 전 
        // isThisStageCleared true: 불러오기 한 거 

        #region 카드/포션/아티펙트 리롤

        if (!Hub.ProgressManager.IsThisStageCleared)
        {
            yield return PickCards(cardsListInShop.Length);
            yield return PickArtifacts(potionsListInShop.Length);
            yield return PickPotions(artifactsListInShop.Length);

            print("///////////////////////////");
            print(Hub.ProgressManager.currentShopCardsList[0]);
            print(Hub.ProgressManager.currentShopArtifactsList[0]);
            print(Hub.ProgressManager.currentShopPotionsList[0]);
            print("///////////////////////////");
            Hub.ProgressManager.IsThisStageCleared = true;
        }
        else
        {
            LoadCards(cardsListInShop.Length);
            LoadArtifacts(potionsListInShop.Length);
            LoadPotions(artifactsListInShop.Length);
        }

        #endregion

        #region 카드/포션/아티펙트 Instantiate

        // 상점에서 몇 번이 눌렀는지 확인하고 그러기보다는
        // 각 프리팹에 이미 onClick이 있으니까 이거 이용
        // 각 프리팹에 번호를 여기서 입력해주는 것으로 처리
        // 각 프리팹이 상점일 때만 버튼을 눌렀을 때 event를 추가해서 처리하는 것도 좋겠지만 
        // 일단은 이렇게 함. 


        for (int i = 0; i < cardsListInShop.Length; i++)
        {
            GameObject card = Instantiate(Hub.EnemyInfoManager.CardsList[pickedCards[i]].prefab);
            card.transform.SetParent(cardsListInShop[i].prefabPos);
            card.transform.localScale = new Vector3(0.33f, 0.33f, 1);
            card.transform.localPosition = new Vector3(0, 5f, 1);
            card.GetComponent<A_MinionCard>().CurrentPosition = CurrentPositions.Shop;
            card.GetComponent<A_MinionCard>().elementIdx = pickedCards[i];
        }
        for (int i = 0; i < potionsListInShop.Length; i++)
        {
            GameObject potion = Instantiate(Hub.EnemyInfoManager.PotionsList[pickedPotions[i]].prefab);
            potion.transform.SetParent(potionsListInShop[i].prefabPos);
            potion.GetComponent<A_PotionIcon>().SetOriginalTransform();
            potion.transform.localScale = new Vector3(0.33f, 0.33f, 1);
            potion.transform.localPosition = new Vector3(0, 5f, 1);
            potion.GetComponent<A_PotionIcon>().CurrentPosition = CurrentPositions.Shop;
            potion.GetComponent<A_PotionIcon>().elementIdx = pickedPotions[i];
        }
        for (int i = 0; i < artifactsListInShop.Length; i++)
        {
            GameObject artifact = Instantiate(Hub.EnemyInfoManager.ArtifactsList[pickedArtifacts[i]].prefab);
            artifact.transform.SetParent(artifactsListInShop[i].prefabPos);
            artifact.GetComponent<A_ArtifactIcon>().SetOriginalTransform();
            artifact.transform.localPosition = new Vector3(0, 5f, 1);
            artifact.transform.localScale = new Vector3(0.33f, 0.33f, 1);
            artifact.GetComponent<A_ArtifactIcon>().CurrentPosition = CurrentPositions.Shop;
            artifact.GetComponent<A_ArtifactIcon>().elementIdx = pickedArtifacts[i];
        }

        #endregion

        // Dialog
        int num = Random.Range(0, dialogGreeting.Length);
        tmpDialog.TranslationName = dialogGreeting[num].name;
    }

    public bool TryBuy(int price)
    {
        try
        {
            if (price <= Hub.ProgressManager.CurrentGold)
            {
                Hub.SoundManager.SfxSelectPlay(2);
                Hub.ProgressManager.CurrentGold -= price;
                tmpDialog.TranslationName = dialogThankYou[Random.Range(0, dialogThankYou.Length)].name;
                Debug.Log("구매 성공");
                return true;
            }
            else
            {
                Hub.SoundManager.SfxSelectPlay(10);
                tmpDialog.TranslationName = dialogNotEngouhMoney[Random.Range(0, dialogNotEngouhMoney.Length)].name;
                Debug.Log("골드 부족");
                return false;
            }
        }
        catch { Debug.Log("구매 에러"); }
        return false;
    }
    public bool TryBuy(int price, int carryingAmount)
    {
        if (carryingAmount >= Hub.ProgressManager.currentPotions.Length)
        {
            Hub.SoundManager.SfxSelectPlay(10);
            tmpDialog.TranslationName = dialogPotionsAreFull[Random.Range(0, dialogPotionsAreFull.Length)].name;
            Debug.Log("가득 참");
            return false;
        }

        try
        {
            if (price <= Hub.ProgressManager.CurrentGold)
            {
                Hub.SoundManager.SfxSelectPlay(2);
                Hub.ProgressManager.CurrentGold -= price;
                tmpDialog.TranslationName = dialogThankYou[Random.Range(0, dialogThankYou.Length)].name;
                Debug.Log("구매 성공");
                return true;
            }
            else
            {
                Hub.SoundManager.SfxSelectPlay(10);
                tmpDialog.TranslationName = dialogNotEngouhMoney[Random.Range(0, dialogNotEngouhMoney.Length)].name;
                Debug.Log("골드 부족");
                return false;
            }
        }
        catch { Debug.Log("구매 에러"); }
        return false;
    }


    public void ButtonToMap()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.SingletonUIManager.ButtonMap();
    }

}
