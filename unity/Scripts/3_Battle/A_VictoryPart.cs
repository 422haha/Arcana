using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class A_VictoryPart : MonoBehaviour
{
    public GameObject rewardList;

    public GameObject goldPrefab;
    public GameObject cardPrefab;
    public GameObject artifactPrefab;
    public GameObject potionPrefab;

    public int goldAmount;  // 골드 보상 수량
    public Transform inventoryPosition;  // 수령 시 이동할 인벤토리 위치
    public Button nextButton;

    // 반짝이는 테두리 이펙트 프리팹 (UI 요소여야 함)
    public GameObject sparklingBorderPrefab;

    // 반짝이는 이펙트 프리팹
    public GameObject sparkleEffectPrefab;

    // 아이들 상태에서 반짝이는 이펙트 프리팹
    public GameObject idleSparklePrefab;

    private List<GameObject> rewards = new List<GameObject>(); // 보상 목록

    [Header("카드 선택 관련 요소들")]
    public GameObject cardSelectionScreen;
    public Transform cardSelectionParent;
    public Button receiveButton;
    public TMP_Text countText;

    private List<int> cardOptions = new List<int>();
    private List<GameObject> selectedCards = new List<GameObject>();
    private int maxSelectedCards = 2;
    private Dictionary<GameObject, int> cardIndexMap = new Dictionary<GameObject, int>();

    private bool isAnimating = false; // 애니메이션 중인지 여부

    private void Awake()
    {
        // 다음으로 버튼 초기화 (처음부터 활성화)
        nextButton.gameObject.SetActive(true);
        nextButton.onClick.AddListener(OnNextButtonClicked);

        // 소리 실행 이벤트 붙이기
        nextButton.onClick.AddListener(() => Hub.SoundManager.SfxSelectPlay(0));
        receiveButton.onClick.AddListener(() => Hub.SoundManager.SfxSelectPlay(0));

        // 보상 버튼 생성 및 초기화
        CreateRewardButton(goldPrefab, "gold");
        CreateRewardButton(cardPrefab, "card");

        // 아티팩트 10% 확률
        if (Random.Range(0, 10) == 1)
        {
            CreateRewardButton(artifactPrefab, "artifact");
        }

        // 포션 10% 확률
        if (Random.Range(0, 10) == 1)
        {
            CreateRewardButton(potionPrefab, "potion");
        }
    }

    // 보상 버튼 생성 메서드
    private void CreateRewardButton(GameObject prefab, string rewardType)
    {
        GameObject reward = Instantiate(prefab, rewardList.transform);
        Button rewardButton = reward.GetComponent<Button>();
        if (rewardButton == null)
        {
            rewardButton = reward.AddComponent<Button>();
        }
        rewardButton.onClick.AddListener(() => {
            if (!isAnimating) // 애니메이션 중이 아닐 때만 수령 가능
            {
                CollectReward(reward, rewardType);
            }
        });

        // 버튼 효과 스크립트 추가
        reward.AddComponent<ButtonEffects>();

        // 아이들 상태에서 반짝이는 이펙트 추가
        if (idleSparklePrefab != null)
        {
            GameObject idleSparkle = Instantiate(idleSparklePrefab, reward.transform);
            idleSparkle.transform.SetAsFirstSibling(); // 버튼 배경 뒤에 위치하도록 설정
            // Ensure the sparkle effect loops continuously
            ParticleSystem ps = idleSparkle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.loop = true;
                ps.Play();
            }
        }

        rewards.Add(reward);
    }

    // "다음으로" 버튼 클릭 시 호출되는 메서드
    private void OnNextButtonClicked()
    {
        if (!isAnimating) // 애니메이션 중이 아닐 때만 동작
        {
            // 보상을 수령하지 않고 다음 단계로만 넘어가도록 설정
            Hub.SingletonUIManager.ButtonMap();
        }
    }

    // 보상의 타입을 가져오는 헬퍼 메서드
    private string GetRewardType(GameObject reward)
    {
        if (reward == null) return "";
        if (reward.GetComponent<Button>() == null) return "";

        // rewardType을 결정하는 로직
        if (reward.name.Contains("gold")) return "gold";
        if (reward.name.Contains("card")) return "card";
        if (reward.name.Contains("artifact")) return "artifact";
        if (reward.name.Contains("potion")) return "potion";
        return "";
    }

    // 전리품 획득 로직
    private void CollectReward(GameObject reward, string rewardType, bool skipAnimation = false)
    {
        if (isAnimating && !skipAnimation)
            return; // 애니메이션 중이라면 중단

        switch (rewardType)
        {
            case "gold":
                Hub.ProgressManager.IncreaseGold(goldAmount);
                break;
            case "card":
                ShowCardSelection(); // 카드 선택 화면 띄우기
                break;
            case "artifact":
                Hub.ProgressManager.GetRandomArtifacts(1);
                // 아티팩트 보상 로직
                break;
            case "potion":
                Hub.ProgressManager.GetRandomPotions(1);
                // 포션 보상 로직
                break;
        }

        if (skipAnimation)
        {
            reward.SetActive(false);
            return;
        }

        // 애니메이션 시작
        isAnimating = true;

        // 반짝이는 이펙트를 생성하고 페이드 아웃 애니메이션을 적용
        if (sparkleEffectPrefab != null)
        {
            GameObject sparkle = Instantiate(sparkleEffectPrefab, reward.transform.position, Quaternion.identity, reward.transform);
            Destroy(sparkle, 1f); // 1초 후 이펙트 제거
        }

        // 버튼이 살짝 확대되었다가 서서히 사라지는 애니메이션 적용
        Sequence rewardSequence = DOTween.Sequence();
        rewardSequence.Append(reward.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad)) // 살짝 확대
                      .Append(reward.transform.DOScale(0f, 0.6f).SetEase(Ease.InQuad))     // 크기 줄이기
                      .Join(reward.GetComponent<CanvasGroup>().DOFade(0, 0.6f))           // 페이드 아웃
                      .OnComplete(() =>
                      {
                          reward.SetActive(false);
                          isAnimating = false;
                      });
    }

    // 카드 선택 보상 로직
    private IEnumerator ShowCardSelectionCoroutine()
    {
        isAnimating = true; // 애니메이션 시작

        // 카드 선택 화면 활성화 및 초기화
        cardSelectionScreen.SetActive(true);
        foreach (Transform child in cardSelectionParent)
        {
            Destroy(child.gameObject);
        }

        // 랜덤한 카드 5개 선택
        cardOptions = GetRandomCardOptions(5);

        // 화면 비율에 따른 카드 크기 및 간격 조정
        float cardScale = 0.4f; // 기존 0.5에서 약간 축소
        float cardSpacing = 150f; // 간격을 약간 좁힘
        float startX = -((cardOptions.Count - 1) * cardSpacing) / 2;

        for (int i = 0; i < cardOptions.Count; i++)
        {
            int cardIndex = cardOptions[i];
            GameObject card = Instantiate(Hub.EnemyInfoManager.CardsList[cardIndex].prefab, cardSelectionParent);
            card.GetComponent<A_Card>().CurrentPosition = CurrentPositions.Reward;

            // 카드의 스케일과 위치를 조정
            card.transform.localScale = Vector3.one * cardScale; // 스케일 감소
            card.transform.localPosition = new Vector3(startX + (i * cardSpacing), 0, 0);

            // 카드에 버튼 컴포넌트 추가 및 클릭 이벤트 할당
            Button cardButton = card.GetComponent<Button>() ?? card.AddComponent<Button>();
            cardButton.onClick.AddListener(() => {
                if (!isAnimating) // 애니메이션 중이 아닐 때만 클릭 가능
                {
                    OnCardClicked(card, cardIndex);
                }
            });

            // 버튼 효과 스크립트 추가
            card.AddComponent<ButtonEffects>();

            cardIndexMap[card] = cardIndex;

            // 카드 애니메이션 적용
            CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = card.AddComponent<CanvasGroup>();
            }
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // 카드가 일정 크기로 확대되도록 초기화
            card.transform.localScale = Vector3.zero;
            card.transform.DOScale(cardScale, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
        }

        // 수령하기 버튼 초기화
        receiveButton.interactable = false;
        receiveButton.onClick.RemoveAllListeners();
        receiveButton.onClick.AddListener(CollectSelectedCards);

        // 초기 애니메이션이 끝날 때까지 대기 (전체 카드가 애니메이션을 마칠 때까지 대기)
        yield return new WaitForSeconds(0.6f);

        isAnimating = false; // 애니메이션 종료
    }

    private void ShowCardSelection()
    {
        StartCoroutine(ShowCardSelectionCoroutine());
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

    private void OnCardClicked(GameObject card, int cardIndex)
    {
        if (isAnimating) return; // 애니메이션 중이면 클릭 무시

        // 카드 선택 관리
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            RemoveHighlight(card);
        }
        else if (selectedCards.Count < maxSelectedCards)
        {
            selectedCards.Add(card);
            AddHighlight(card);
        }

        // 수량 텍스트 변경
        countText.text = $"{selectedCards.Count}/2";
        // 수령 버튼 활성화 여부 결정
        receiveButton.interactable = selectedCards.Count == maxSelectedCards;
    }

    private void AddHighlight(GameObject card)
    {
        GameObject highlightEffect = new GameObject("HighlightEffect");
        highlightEffect.transform.SetParent(card.transform, false);

        RectTransform effectRect = highlightEffect.AddComponent<RectTransform>();
        effectRect.anchorMin = new Vector2(-0.1f, -0.1f);
        effectRect.anchorMax = new Vector2(1.1f, 1.1f);
        effectRect.offsetMin = Vector2.zero;
        effectRect.offsetMax = Vector2.zero;

        // 하이라이트 효과 컴포넌트 추가
        highlightEffect.AddComponent<CardHighlightEffect>();

        // 가장 뒤쪽 레이어에 배치
        highlightEffect.transform.SetSiblingIndex(0);
    }


    private void RemoveHighlight(GameObject card)
    {
        Transform highlight = card.transform.Find("HighlightEffect");
        if (highlight != null)
        {
            // HighlightEffect 제거
            Destroy(highlight.gameObject);
        }
    }

    private void CollectSelectedCards()
    {
        if (isAnimating) return; // 애니메이션 중이면 실행 무시

        isAnimating = true;

        Sequence collectSequence = DOTween.Sequence();

        foreach (GameObject card in selectedCards)
        {
            if (cardIndexMap.TryGetValue(card, out int cardIndex))
            {
                // 카드 애니메이션 시퀀스 추가
                collectSequence.Append(
                    card.transform.DOMove(inventoryPosition.position, 0.8f).SetEase(Ease.InQuad)
                );
                collectSequence.Join(
                    card.transform.DOScale(0.1f, 0.8f).SetEase(Ease.InQuad)
                );
                collectSequence.Join(
                    card.GetComponent<CanvasGroup>().DOFade(0, 0.8f)
                );
                collectSequence.Join(
                    card.transform.DORotate(new Vector3(0, 720, 0), 0.8f, RotateMode.FastBeyond360)
                );

                // 애니메이션 완료 후 카드 인벤토리에 추가 및 제거
                collectSequence.OnComplete(() =>
                {
                    print("이거 안 불러와짐ㅋㅋ");                    
                    Destroy(card);
                });

                Hub.ProgressManager.GetCard(cardIndex);
            }
        }

        // 모든 애니메이션 완료 후 창 닫기
        collectSequence.OnComplete(() =>
        {
            selectedCards.Clear();
            receiveButton.interactable = false;
            cardSelectionScreen.SetActive(false);
            isAnimating = false;
        });
    }
}
