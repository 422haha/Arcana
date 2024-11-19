using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    [Header("로그인 창")]
    public GameObject login;
    [Header("회원가입 창")]
    public GameObject signup;
    [Header("비밀번호 찾기 창")]
    public GameObject pwFind;

    private void Awake()
    {
        // 브금틀기
        Hub.SoundManager.BgmSelectPlay(0);
    }

    private void Update()
    {
        if (Hub.LoginManager.IsRegistered())
        {
            ToLogin();
        }
    }

    // 회원가입창으로 이동하는 함수
    public void ToSignup()
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        login.SetActive(false);
        signup.SetActive(true);
        pwFind.SetActive(false);
    }

    // 로그인창으로 이동하는 함수
    public void ToLogin()
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        login.SetActive(true);
        signup.SetActive(false);
        pwFind.SetActive(false);
    }

    // 비밀번호창으로 이동하는 함수
    public void ToPwFind()
    {
        // 소리
        Hub.SoundManager.SfxSelectPlay(0);
        login.SetActive(false);
        signup.SetActive(false);
        pwFind.SetActive(true);
    }
}
