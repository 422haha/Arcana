using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
{
    [Header("엔딩 크레딧")]
    public Transform endingCredit;

    [Header("크레딧 끝나는 위치")]
    public Transform endPosition;

    [Header("메인으로 이동 버튼")]
    public Button toMainButton;

    private float scrollSpeed = 60f;
    private bool isScrolling = false;

    private void Awake()
    {
        // 브금틀기
        Hub.SoundManager.BgmSelectPlay(8);
    }

    private void Start()
    {
        isScrolling = true;

        toMainButton.onClick.AddListener(MoveToMain);
    }

    // Update is called once per frame
    void Update()
    {
        if (endingCredit != null && isScrolling)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                scrollSpeed = 300f;
            else
                scrollSpeed = 60f;

            endingCredit.position = new Vector2(
                endingCredit.position.x,
                endingCredit.position.y + scrollSpeed * Time.deltaTime
            );

            if (endingCredit.position.y >= endPosition.position.y)
                isScrolling = false;
        }
    }

    void MoveToMain()
    {
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.TransitionManager.MoveTo(1, 1, 1);
    }
}
