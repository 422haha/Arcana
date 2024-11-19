using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class A_Signup : MonoBehaviour
{
    EventSystem system;


    #region public 선언부
    // 회원가입창
    private GameObject signup;
    [Header("최원가입 창 인스펙터들")]
    // 이메일 인풋
    public TMP_InputField emailInput;
    // 이메일 인증 인풋
    public TMP_InputField emailConfirmInput;
    // 회원가입창 닉네임 인풋
    public TMP_InputField nicknameInput;
    // 회원가입창 비밀번호 인풋
    public TMP_InputField pwInput;
    // 회원가입창 비밀번호 확인 인풋
    public TMP_InputField pwConfirmInput;
    // 이메일 인증 보내기 버튼
    public Button emailSendBtn;
    // 이메일 인증하기 버튼
    public Button emailConfirmBtn;
    // 로그인창으로 이동 버튼
    public Button toLoginBtn;
    // 회원가입창 제출 버튼
    public Button submitButton;
    // 이메일 상태창
    public TMP_Text emailStatus;
    // 이메일 인증 상태창
    public TMP_Text emailConfirmStatus;
    // 닉네임 상태창
    public TMP_Text nicknameStatus;
    // 비밀번호 확인 상태창
    public TMP_Text pwConfirmStatus;
    // 비밀번호 점수 슬라이더
    public Slider pwSlider;
    [Header("슬라이더 (위험)")]
    public Sprite dangerFill;
    [Header("슬라이더 (안전)")]
    public Sprite safeFill;

    [Header("비밀번호 조건창 인스펙터들")]
    // 비밀번호 조건창 게임오브젝트
    public GameObject pwOption;
    // 비밀번호 조건창 위치
    public Transform pwOptionPos;
    // 비밀번호 길이 체크
    public Image lenCheckImg;
    // 비밀번호 대문자 체크
    public Image upperCheckImg;
    // 비밀번호 소문자 체크
    public Image lowerCheckImg;
    // 비밀번호 숫자 체크
    public Image numCheckImg;
    // 체크 이미지
    public Sprite checkSprite;
    // X 이미지
    public Sprite closeSprite;
    #endregion
    // 비밀번호 유효한지 여부
    private bool isPwValidate = false;
    // 비밀번호 확인
    private bool isPwConfirmed = false;
    // 슬라이더 목표값
    private float targetScore = 0;
    // 코루틴 실행여부
    private bool isChanging = false;

    void Start()
    {
        system = EventSystem.current;

        // 회원가입 창 할당
        signup = this.gameObject;

        // 회원가입창 첫 인풋을 선택
        emailInput.Select();

        //버튼 이벤트 연결
        SetupButtonEvent();

        // 인풋 이벤트 연결
        SetupInputEvent();
    }

    // Update is called once per frame
    void Update()
    {
        OnTabOrReturnClick();
        IMEDisable();
        IsBtnInteractable();
    }

    // 회원가입 창 벗어나면 초기화
    private void OnDisable()
    {
        emailInput.text = string.Empty;
        emailConfirmInput.text = string.Empty;
        nicknameInput.text = string.Empty;
        pwInput.text = string.Empty;
        pwConfirmInput.text = string.Empty;
        pwSlider.value = 0;
        pwSlider.fillRect.GetComponent<Image>().color = Color.white;
    }

    private void SetupButtonEvent()
    {
        emailSendBtn.onClick.AddListener(() => Hub.LoginManager.RequestEmailVerification(emailInput.text, emailStatus));
        emailConfirmBtn.onClick.AddListener(() => Hub.LoginManager.VerifyEmailCode(emailInput.text, emailConfirmInput.text, emailConfirmStatus));
        submitButton.onClick.AddListener(() => OnRegisterClick());
        pwInput.onValueChanged.AddListener(VerifyPassWord);
        pwConfirmInput.onValueChanged.AddListener((_) => PasswordConfirm());
    }

    private void SetupInputEvent()
    {
        emailInput.onValueChanged.AddListener((_) => emailStatus.text = string.Empty);
        emailConfirmInput.onValueChanged.AddListener((_) => emailConfirmStatus.text = string.Empty);
    }

    // 버튼, 인풋 활성화여부
    private void IsBtnInteractable()
    {
        // 이메일 전송 버튼
        // 이메일 인풋필드 비어있으면 비활성화
        if (emailInput.text.Length == 0)
        {
            emailSendBtn.interactable = false;
        }
        else
        {
            emailSendBtn.interactable = true;
        }

        // 이메일 인증 인풋
        if (!Hub.NetworkManager.IsEmailSended())
        {
            emailConfirmInput.interactable = false;
        }
        else
        {
            emailConfirmInput.interactable = true;
        }

        // 이메일 인증 버튼
        // 이메일 인증 인풋필드 비어있으면 비활성화
        if (emailConfirmInput.text.Length == 0)
        {
            emailConfirmBtn.interactable = false;
        }
        else
        {
            emailConfirmBtn.interactable = true;
        }

        // 제출버튼
        // 인풋필드 비어있으면 제출버튼 비활성화
        // 이메일 인증 안했을 시 제출버튼 비활성화
        // 패스워드 유효하지 않거나, 확인하지 않았을 때 비활성화
        if (emailInput.text.Length == 0 || emailConfirmInput.text.Length == 0 ||
            nicknameInput.text.Length == 0 || pwInput.text.Length == 0 || pwConfirmInput.text.Length == 0
            || !Hub.NetworkManager.IsEmailSended() || !isPwValidate 
            || !isPwConfirmed || !Hub.NetworkManager.IsRegistrationValid())
        {
            submitButton.interactable = false;
        }
        else
        {
            submitButton.interactable = true;
        }
    }

    // Tab, Return버튼 체크
    private void OnTabOrReturnClick()
    {
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
        // 엔터키를 눌렀을 때 다음차례인 버튼이 눌리도록
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // 이메일 전송 버튼
            if (emailInput == EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>())
            {
                emailSendBtn.onClick.Invoke();
                Debug.Log("이메일 전송버튼 클릭");
            }
            else if (emailConfirmInput == EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>())
            {
                emailConfirmBtn.onClick.Invoke();
                Debug.Log("이메일 인증버튼 클릭");
            }
            // 그 외의 상황
            else
            {
                if (submitButton.interactable)
                {
                    // 회원가입 버튼 클릭
                    submitButton.onClick.Invoke();
                    Debug.Log("회원가입 버튼 클릭");
                }
            }
        }
    }

    // IME처리 (Password 한글 입력시 오류나는것 방지)
    private void IMEDisable()
    {
        if (pwInput.isFocused || pwConfirmInput.isFocused)
        {
            Input.imeCompositionMode = IMECompositionMode.Off;
        }
        else if (Input.imeCompositionMode != IMECompositionMode.On)
        {
            Input.imeCompositionMode = IMECompositionMode.On;
        }
    }

    private void OnRegisterClick()
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        Hub.LoginManager.ProcessRegister(
            emailInput.text,
            pwInput.text,
            nicknameInput.text
        );
    }

    // 정규표현식, 비밀번호 점수 부여
    private void VerifyPassWord(string input)
    {
        // 비밀번호 점수
        int score = 0;
        // 필수 비밀번호 조건
        bool lenCheck = false;
        bool upperCheck = false;
        bool lowerCheck = false;
        bool numCheck = false;
        // 슬라이더 fill 이미지
        Image fillImage = pwSlider.fillRect.GetComponent<Image>();

        // 공백 제거
        pwInput.text = input.Replace(" ", "");
        
        // 1. 길이 검사 (8자 이상)
        if (pwInput.text.Length < 8)
        {
            lenCheckImg.sprite = closeSprite;
            lenCheck = false;
        }
        if (pwInput.text.Length >= 8)
        {
            score++;
            lenCheckImg.sprite = checkSprite;
            lenCheck = true;
        }
        if (pwInput.text.Length >= 12) // 추가 점수
            score++;

        // 2. 대문자 포함 여부
        if (Regex.IsMatch(pwInput.text, "[A-Z]"))
        {
            score++;
            upperCheckImg.sprite = checkSprite;
            upperCheck = true;
        }
        else
        {
            upperCheckImg.sprite = closeSprite;
            upperCheck = false;
        }

        // 3. 소문자 포함 여부
        if (Regex.IsMatch(pwInput.text, "[a-z]"))
        {
            score++;
            lowerCheckImg.sprite = checkSprite;
            lowerCheck = true;
        }
        else
        {
            lowerCheckImg.sprite = closeSprite;
            lowerCheck = false;
        }

        // 4. 숫자 포함 여부
        if (Regex.IsMatch(pwInput.text, "[0-9]"))
        {
            score++;
            numCheckImg.sprite = checkSprite;
            numCheck = true;
        }
        else
        {
            numCheckImg.sprite = closeSprite;
            numCheck = false;
        }

        // 5. 특수문자 포함 여부
        if (Regex.IsMatch(pwInput.text, @"[\W_]"))
            score++;

        // 6. 연속된 문자나 숫자 검사
        if (Regex.IsMatch(pwInput.text, @"(.)\1\1"))
            score--;

        // 비밀번호 유효성검사
        if (lenCheck && upperCheck && lowerCheck && numCheck) isPwValidate = true;
        else isPwValidate = false;

        targetScore = score;
        if (targetScore < 4 || !(lenCheck && upperCheck && lowerCheck && numCheck))
            fillImage.sprite = dangerFill;
        else
            fillImage.sprite = safeFill;
        
        if (!isChanging && signup.activeSelf) StartCoroutine(SmoothSliderChange());
    }

    // 슬라이더 값 부드럽게 바꾸는 코루틴
    private IEnumerator SmoothSliderChange()
    {
        isChanging = true;
        float currentScore = pwSlider.value;

        // 슬라이더 값을 목표값으로 부드럽게 이동
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // lerp 사용 값 보간
            pwSlider.value = Mathf.Lerp(currentScore, targetScore, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막에 정확히 목표값으로 설정
        pwSlider.value = targetScore;
        isChanging = false;
    }

    // 비밀번호 확인
    private void PasswordConfirm()
    {
        if (pwConfirmInput.text.Length == 0)
            pwConfirmStatus.text = string.Empty;

        if (!isPwValidate) 
        {
            pwConfirmStatus.text = "비밀번호를 확인해주세요.";
            pwConfirmStatus.color = Color.red;
            isPwConfirmed = false;
        }
        else
        {
            if (pwInput.text == pwConfirmInput.text)
            {
                Hub.TranslationManager.AddTranslation(Hub.NetworkManager.pwConfirmStatus, pwConfirmStatus,
                    "비밀번호가 일치합니다.", "Confirmed!");
                pwConfirmStatus.color = Color.green;
                isPwConfirmed = true;
            }
            else
            {
                Hub.TranslationManager.AddTranslation(Hub.NetworkManager.pwConfirmStatus, pwConfirmStatus,
                    "비밀번호가 일치하지 않습니다.", "Not match");
                pwConfirmStatus.color = Color.red;
                isPwConfirmed = false;
            }
        }
    }
}
