using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [Header("덱 창")]
    public GameObject deckWindow;

    [Header("정렬 버튼들")]
    // 획득순
    public Button orderByTImeButton;
    public Button orderByTypeButton;
    public Button orderByCostButton;
    public Button orderByNameButton;

    [Header("Grid 부모")]
    public Transform cardParent;

    [Header("카드들")]
    [SerializeField]
    private List<Transform> cards;

    // 획득순 정렬 카드들
    private Transform[] originalCards;

    // 0 : 획득순, 1 : 타입순, 2 : 비용순, 3 : 이름순
    private int orderType;

    [Header("버튼 텍스트들")]
    [SerializeField]
    private TMP_Text[] texts = new TMP_Text[4];

    private void Start()
    {
        // 기본적으로 획득순 정렬
        orderType = 0;

        // 텍스트 색칠
        if (ExcludeExeption()) ChangeColor(orderType);

        // 버튼 텍스트들 할당해주기
        texts[0] = orderByTImeButton.GetComponentInChildren<TMP_Text>();
        texts[1] = orderByTypeButton.GetComponentInChildren<TMP_Text>();
        texts[2] = orderByCostButton.GetComponentInChildren<TMP_Text>();
        texts[3] = orderByNameButton.GetComponentInChildren<TMP_Text>();

        // 이벤트 붙이기
        orderByTImeButton.onClick.AddListener(() => Order(orderByTImeButton.gameObject.transform));
        orderByTypeButton.onClick.AddListener(() => Order(orderByTypeButton.gameObject.transform));
        orderByCostButton.onClick.AddListener(() => Order(orderByCostButton.gameObject.transform));
        orderByNameButton.onClick.AddListener(() => Order(orderByNameButton.gameObject.transform));
    }

    // 덱 생성 로직
    public void SetCards()
    {
        // 카드 가져오기
        for (int i = 0; i < cardParent.childCount; i++)
        {
            cards.Add(cardParent.GetChild(i));
        }

        originalCards = cards.ToArray();
    }

    // 정렬 초기화 로직
    public void SetCardsDefault()
    {
        cards = originalCards.ToList();
        orderType = 0;

        if (ExcludeExeption()) ChangeColor(orderType);

        for (int i = 0; i < cards.Count(); i++)
        {
            cards[i].SetSiblingIndex(i);
        }
    }

    // 텍스트 색 변경 로직
    void ChangeColor(int orderType)
    {
        for (int i = 0; i < texts.Length; i++)
            if (i == orderType)
                texts[i].color = Color.yellow;
            else
                texts[i].color = Color.black;
    }

    bool ExcludeExeption()
    {
        foreach (TMP_Text text in texts)
        {
            if (text == null)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 카드를 누른 버튼 기준으로 정렬하는 로직
    /// 모든 버튼에게 똑같이 들어간다.
    /// </summary>
    /// <param name="clickedButton">클릭된 버튼 오브젝트의 transform 정보</param>
    void Order(Transform clickedButton)
    {
        Debug.LogError("텍스트 오브젝트를 찾지 못함");

        // 이벤트 분기 처리
        switch (clickedButton.tag)
        {
            // 획득 순
            case "ByTime":
                orderType = 0;

                // 오리지널 배열 (처음 받아왔을때의 배열)
                cards = originalCards.ToList();
                break;
            // 타입 순
            // 하수인카드 > 마법카드 순
            // 하수인카드는 게임오브젝트에 A_MinionCard가 붙어있고
            // 마법카드는 게임오브젝트에 A_SpellCard가 붙어있음
            case "ByType":
                orderType = 1;

                cards.Clear();
                // 마법카드 저장할 임시 배열
                List<Transform> tempSpellCards = new List<Transform>();

                foreach (Transform card in originalCards)
                {
                    if (card == null)
                        continue;

                    // 하수인 카드라면
                    // 일단 추가
                    if (card.GetComponent<A_MinionCard>() != null)
                        cards.Add(card);
                    // 마법 카드라면
                    // 임시 배열에 추가
                    else if (card.GetComponent<A_SpellCard>() != null)
                        tempSpellCards.Add(card);
                }

                // 둘이 합쳐주기
                cards.AddRange(tempSpellCards);
                break;
            // 가격 순
            case "ByCost":
                orderType = 2;

                // 가격 오름차순 정렬
                cards = cards
                    .OrderBy(card =>
                    {
                        int cost = card.GetComponent<A_MinionCard>().defaultCost;
                        return cost;
                    })
                    .ToList();
                break;
            // 이름 순
            case "ByName":
                orderType = 3;

                // 이름 별 오름차순 정렬
                cards = cards
                    .OrderBy(card =>
                    {
                        TMP_Text nameText = card.GetComponentsInChildren<TMP_Text>()
                            .FirstOrDefault(text => text.CompareTag("CardName"));
                        // nameText가 null이 아닐 경우 텍스트 반환, 없으면 빈 문자열 반환
                        return nameText != null ? nameText.text : string.Empty;
                    })
                    .ToList();

                break;
        }

        // 텍스트 색칠
        if (ExcludeExeption()) ChangeColor(orderType);

        // 카드의 하이라키 상 위치도 cards 리스트의 순서와 똑같이 정렬 (눈에 보이는 처리)
        for (int i = 0; i < cards.Count(); i++)
        {
            cards[i].SetSiblingIndex(i);
        }
    }
}
