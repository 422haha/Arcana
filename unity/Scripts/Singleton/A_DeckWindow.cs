using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_DeckWindow : MonoBehaviour
{
    [Header("덱 매니저")]
    public DeckManager deckManager;

    // 켜질 때 카드 가져오도록
    private void OnEnable()
    {
        deckManager.SetCards();
    }

    // 꺼질때 정렬 초기화하도록
    private void OnDisable()
    {
        deckManager.SetCardsDefault();
    }
}
