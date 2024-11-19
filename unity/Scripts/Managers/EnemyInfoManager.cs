using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 각 카드나 아티펙트 같은 곳에 두기 좀 그래서 여기에다 둠
// 지금 이 프리팹이, 게임 속에 있는지 상점에 있는지 등을 보는 곳 
public enum CurrentPositions
{
    Idle, // 아무것도 아닌 상태
    Header,
    Shop, // 상점에서
    Battlehand, // 전투 중에 손에 있는 경우
    BattleOther, // 전투 중에 손 외에 있는 경우(Idle로 대체해도 되고 이걸로 해도 되고)
    Description, // 필드에서 포인터를 대면 나오는
    Reward,
    Deck // 덱에서
    // 이벤트, 휴식에서는 필요 없을 듯. 이벤트는 Idle로 해도 될 거 같고
}

public enum CurrentOwner
{
    // 이건 할까 말까 고민, 어차피 카드가 상대방 거라면
    // 뒤집어서 나오도록 하는 거 외에는 쓸모가 없어보이는데,
    // 그냥 CurrentPositions에 하나 넣는 것도 좋아보이고 
}

public class EnemyInfoManager : MonoBehaviour
{
    private void OnValidate()
    {
        for (int i = 0; i < CardsList.Length; i++)
        {
            if (CardsList[i].prefab != null)
            {
                CardsList[i].name = "[" + i + "] " + CardsList[i].prefab.name;
                CardsList[i].idx = i;
            }
        }

        for (int i = 0; i < ArtifactsList.Length; i++)
        {
            if (ArtifactsList[i].prefab != null)
            {
                ArtifactsList[i].name = "[" + i + "] " + ArtifactsList[i].prefab.name;
                ArtifactsList[i].idx = i;
            }
        }

        for (int i = 0; i < EnemiesList.Length; i++)
        {
            EnemiesList[i].name = "[" + i + "] " + EnemiesList[i].portrait.name;
            //EnemiesList[i].idx = i;
        }

        for (int i = 0; i < PotionsList.Length; i++)
        {
            PotionsList[i].name = "[" + i + "] " + PotionsList[i].prefab.name;
            PotionsList[i].idx = i;
        }

    }


    [System.Serializable]
    public class Cards
    {
        // Array index가 아니라 id로 처리해야 나중에 업데이트를 해도 문제가 없는데
        // 이름 바꾸니까 어차피 array 번호 확인용으로라도 id 놔둠.
        [HideInInspector] public string name;
        [HideInInspector] public int idx;        
        public GameObject prefab;        
    }
    public Cards[] CardsList;
    

    [System.Serializable]
    public class Enemies
    {
        [HideInInspector] public string name = "enemy";
        public Sprite portrait;
        public int groupNum;
        public int givenHearts;
        public int[] cardLists;        
    }
    public Enemies[] EnemiesList;

    [System.Serializable]
    public class Artifacts
    {
        // Array index가 아니라 id로 처리해야 나중에 업데이트를 해도 문제가 없는데
        // 이름 바꾸니까 어차피 array 번호 확인용으로라도 id 놔둠.
        [HideInInspector] public string name = "artifact";
        [HideInInspector] public int idx;        
        public GameObject prefab;
    }
    public Artifacts[] ArtifactsList;

    [System.Serializable]
    public class Potions
    {
        // Array index가 아니라 id로 처리해야 나중에 업데이트를 해도 문제가 없는데
        // 이름 바꾸니까 어차피 array 번호 확인용으로라도 id 놔둠.
        [HideInInspector] public string name = "potion";
        [HideInInspector] public int idx;        
        /// <summary>
        /// 희귀도 부분
        /// 0: 언커먼
        /// 1: 커먼
        /// 2: 레어
        /// </summary>
        public int rarity;
        public int price;
        public GameObject prefab;
    }
    public Potions[] PotionsList;



}
