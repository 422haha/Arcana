using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class A_Login : MonoBehaviour
{
    EventSystem system;

    // 로그인창
    private GameObject login;
    // 아이디 인풋
    public TMP_InputField idInput;
    // 비밀번호 인풋
    public TMP_InputField pwInput;
    // 회원가입창으로 이동 버튼
    public Button toSignupBtn;
    // 제출 버튼
    public Button submitButton;
    // 로그인 상태 Txt
    public TMP_Text status;

    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;

        // 로그인창 할당
        login = this.gameObject;

        // 로그인창 첫 인풋을 선택
        idInput.Select();
        // 클릭 이벤트 연결
        submitButton.onClick.AddListener(OnLoginClick);
        // onValueChange 이벤트 연결
        idInput.onValueChanged.AddListener((_) => status.text = string.Empty);
        pwInput.onValueChanged.AddListener((_) => status.text = string.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        // IME모드 끄기
        Input.imeCompositionMode = IMECompositionMode.Off;
        IsBtnInteractive();
        // 탭키를 눌렀을 때 다음 버튼 or 인풋으로 넘어가도록
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 아래의 Selectable 객체를 선택
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                next.Select();
            }
        }
        // 엔터 눌렀을 때 로그인 버튼 눌리도록
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (submitButton.interactable)
            {
                Debug.Log("로그인 버튼 클릭");
                OnLoginClick();
            }
        }
    }

    // 다른 창으로 넘어갈때 초기화
    private void OnDisable()
    {
        idInput.text = "";
        pwInput.text = "";
    }

    // 버튼 활성화 여부
    private void IsBtnInteractive()
    {
        if (idInput.text.Length == 0 || pwInput.text.Length == 0)
            submitButton.interactable = false;
        else
            submitButton.interactable = true;
    }

    public void OnLoginClick()
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.LoginManager.ProcessLogin(idInput.text, pwInput.text, status);
    }
}
