using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class A_FindPw : MonoBehaviour
{
    [Header("비밀번호 찾기 창")]
    private GameObject pwFind;
    [Header("이메일 인풋")]
    public TMP_InputField emailInput;
    [Header("로그인으로 링크")]
    public Button toLoginBtn;
    [Header("제출 버튼")]
    public Button submitButton;
    [Header("비밀번호 찾기 상태 텍스트")]
    public TMP_Text status;

    private void Start()
    {
        // onValueChanged 이벤트 연결
        emailInput.onValueChanged.AddListener((_) => status.text = string.Empty);
    }

    void Update()
    {
        IsBtnInteractive();
        // 엔터 눌렀을 때 제출
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (submitButton.interactable)
            {
                PwFindButtonClick();
            }
        }
    }

    // 다른 창으로 넘어갈때 초기화
    void OnDisable()
    {
        emailInput.text = "";
    }

    // 버튼 활성화 여부
    private void IsBtnInteractive()
    {
        if (emailInput.text.Length == 0)
            submitButton.interactable = false;
        else
            submitButton.interactable = true;
    }

    public void PwFindButtonClick()
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.LoginManager.RequestPasswordReset(emailInput.text, status);
    }
}
