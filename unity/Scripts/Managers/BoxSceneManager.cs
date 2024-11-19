using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class BoxSceneManager : MonoBehaviour
{
    [Header("보상 패널")]
    public TMP_Text rewardNotification;
    public LeanToken reward;
    public GameObject buttonOpen;
    public GameObject buttonToMap;

    [Header("배경 설명 부분")]
    public GameObject window;                                  // 스토리 윈도우
    public GameObject transitionWindow;                        // 변환창 윈도우
    public GameObject[] lines;                                 // 스토리 라인들
    public TMP_Text[] texts;                                   // 스토리 텍스트 오브젝트들

    [Header("스킵 버튼 부분")]
    // 스킵 버튼
    public Button skipButton;

    // 변환창 컬러
    private Color tmpColor;

    // 현재 라운드
    private int round = 0;

    // 타이핑 실행여부
    private bool isTyping;

    // 타이핑 종료여부
    private bool isTypingEnd;

    // 첫 티이핑 플래그
    private bool isFromStartTyping;

    // Box로 이동 중인지 여부
    private bool isMoving = false;

    [Header("파티클 시스템 부분")]
    public GameObject particle;

    public void Awake()
    {
        // 처음에 스토리 설명 창은 무조건 띄우도록 해서 실수 방지
        window.SetActive(true);
        Hub.SoundManager.BgmSelectPlay(2);   
        Hub.ProgressManager.IsThisStageCleared = true;
    }

    private void Start()
    {
        // 스킵 버튼 이벤트 붙이기
        skipButton.onClick.AddListener(() => StartCoroutine(Move()));
        StartCoroutine(InitializeMap());
        tmpColor = transitionWindow.GetComponent<Image>().color;

        if (Hub.ProgressManager.CurrentStageIdx <= 0 && Hub.ProgressManager.IsThisStageCleared)
        {
            Hub.SingletonUIManager.isLanguageDropdownDisable = true;
            lines[round].SetActive(true);

            isFromStartTyping = true;
            StartCoroutine(DelayedTyping());
        }
        else
            window.SetActive(false);
    }

    private void Update()
    {
        // 스토리 창이 활성화 된 상태이며, 마우스 왼쪽 버튼을 눌렀을 때
        if (window.activeSelf && Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 타이핑 중이라면 스킵
            if (isTyping && !isTypingEnd)
            {
                isTyping = false;
            }
            // 아니라면 (끝났다면) 다음 코루틴 실행
            else if (!isTyping && isTypingEnd && round <= lines.Length)
            {
                Debug.Log(round);
                if (round == lines.Length && !isMoving)
                    StartCoroutine(Move());

                if (round == 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        lines[i].SetActive(false);
                    }
                }

                if (round < lines.Length)
                {
                    lines[round].SetActive(true);

                    isFromStartTyping = false;
                    StartCoroutine(DelayedTyping());
                }
            }
        }
    }

    #region 스토리 영역 구현
    private Coroutine typingCoroutine;

    // 트랜지션 효과
    // 나중에 TransitionManager로 옮길 것
    IEnumerator Move()
    {
        Hub.SingletonUIManager.isLanguageDropdownDisable = false;
        isMoving = true;
        yield return StartCoroutine(TransitionOn());
        window.SetActive(false);

        // Particle 효과도 같이 비활성화
        particle.SetActive(false);

        yield return StartCoroutine(TransitionOff());
    }

    IEnumerator TransitionOn()
    {
        transitionWindow.SetActive(true);
        for (float i = 0f; i < 1f; i += 0.05f)
        {
            tmpColor.a = i;
            transitionWindow.GetComponent<Image>().color = tmpColor;
            yield return new WaitForSeconds(0.015f);
        }
        tmpColor.a = 1f;
        transitionWindow.GetComponent<Image>().color = tmpColor;
        yield return new WaitForSeconds(1.7f);
    }

    IEnumerator TransitionOff()
    {
        transitionWindow.SetActive(true);
        for (float i = 1f; i > 0f; i -= 0.05f)
        {
            tmpColor.a = i;
            transitionWindow.GetComponent<Image>().color = tmpColor;
            yield return new WaitForSeconds(0.015f);
        }
        tmpColor.a = 0f;
        transitionWindow.GetComponent<Image>().color = tmpColor;
        transitionWindow.SetActive(false);
    }

    private IEnumerator DelayedTyping()
    {
        string textToType;
        if (isFromStartTyping)
        {
            // 한 프레임 대기하여 컴포넌트 완전히 활성화하도록 처리
            yield return null;

            textToType = texts[round].text;
            texts[round].text = string.Empty;
        }
        else
        {
            // 타이핑 시작 전 텍스트를 미리 지정
            textToType = texts[round].text;
            // 텍스트 초기화
            texts[round].text = string.Empty;

            // 한 프레임 대기하여 컴포넌트 완전히 활성화하도록 처리
            yield return null;
        }

        StartTyping(texts[round], textToType);
        round++;
    }

    // 타이핑 효과주는 코루틴
    private IEnumerator Typing(TMP_Text target, string text)
    {
        isTyping = true;
        isTypingEnd = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (!isTyping)
            {
                target.text = text;
                isTypingEnd = true;
                yield break;
            }

            target.text += text[i];
            // 속도
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
        isTypingEnd = true;
    }

    // 다음 타이핑 시작하는 코루틴
    private void StartTyping(TMP_Text target, string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(Typing(target, text));
    }
    #endregion

    // 이 스크립트에서 이전 ProgressManager 초기화
    // 초기화 하기 전 보상부터 주기

    #region 맵맵맵

    private IEnumerator InitializeMap()
    {
        // 씬 로드 완료 대기
        yield return new WaitForSeconds(0.5f);

        // 맵 데이터만 업데이트하고 표시는 하지 않음
        if (Hub.MapManager?.CurrentMap != null && Hub.MapManager.view != null)
        {
            Hub.MapManager.view.ShowMap(Hub.MapManager.CurrentMap);
        }
    }
    #endregion

    public void ButtonOpen()
    {
        Hub.SoundManager.SfxSelectPlay(1);

        buttonOpen.SetActive(false);
        int rewardAmount = (int)((Hub.ProgressManager.formalStageIdx * 10) * Random.Range(0.9f, 1.2f) + Random.Range(1, 21));
        //Hub.ProgressManager.InitializeProgress();
        // 이전 기록 초기화
        Hub.ProgressManager.formalStageIdx = 0;
        Hub.ProgressManager.CurrentGold += rewardAmount;
        rewardNotification.enabled = true;
        reward.Value = rewardAmount.ToString();
    }

    public void ButtonToMap()
    {
        Hub.SoundManager.SfxSelectPlay(0);

        Hub.SingletonUIManager.ButtonMap();
    }
}
