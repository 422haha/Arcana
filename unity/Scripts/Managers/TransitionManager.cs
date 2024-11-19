using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// 여기서는 Transition만 담당함.
/// main으로 넘어갔을 때 상단 바가 켜진다거나 이런 부분은
/// GameManager에서 담당함
/// </summary>

public class TransitionManager : MonoBehaviour
{
    public GameObject transition1;
    public GameObject trnasition2;
    private Color tmpColor;


    public void Awake()
    {
        tmpColor = transition1.GetComponent<Image>().color;
    }

    /// <summary>
    /// 아무 Transition 없이 이동하는 거 
    /// </summary>
    /// <param 이동할 번호="num"></param>
    public void MoveTo(int num) { StartCoroutine(MoveToCor(num)); }   
    /// <summary>
    /// Transition Type을 두 개 골라서 이동하는 방식
    /// 1: 첫번쨰 transition type (모르겠으면 이걸로 하면 됨)
    /// 2: 두번쨰 transition type
    /// </summary>
    /// <param 이동할 번호="num"></param>
    /// <param On 타입="type1"></param>
    /// <param Off 타입="type2"></param>
    public void MoveTo(int num, int OnType, int OffType) { StartCoroutine(MoveToCor(num, OnType, OffType)); }

    IEnumerator MoveToCor(int num)
    {
        yield return StartCoroutine(TransitionOnType1Cor());
        SceneManager.LoadScene(num);
        // 게임 매니저 Enum 바꿔주기
        switch (num)
        {
            case 0: //Login
                Hub.GameManager.Gamestate = GameStates.Login;
                break;
            case 1: //Main
                Hub.GameManager.Gamestate = GameStates.Main;
                break;
            case 2: //Box
                Hub.GameManager.Gamestate = GameStates.Box;
                break;
            case 3: //Battle
                Hub.GameManager.Gamestate = GameStates.Battle;
                break;
            case 4: //Event
                Hub.GameManager.Gamestate = GameStates.Event;
                break;
            case 5: //Rest
                Hub.GameManager.Gamestate = GameStates.Rest;
                break;
            case 6: //Shop
                Hub.GameManager.Gamestate = GameStates.Shop;
                break;
            case 7: //Ending
                Hub.GameManager.Gamestate = GameStates.Ending;
                break;
            default:
                break;
        }
        yield return StartCoroutine(TransitionOffType1Cor());
    }
    IEnumerator MoveToCor(int num, int OnType, int OffType)
    {
        switch (OnType)
        {
            case 1:
                yield return StartCoroutine(TransitionOnType1Cor());
                break;
            case 2:
                yield return StartCoroutine(TransitionOnType2Cor());
                break;
            default:
                break;
        }
        SceneManager.LoadScene(num);
        Hub.SingletonUIManager.ButtonMap();
        // 게임 매니저 Enum 바꿔주기
        switch (num)
        {
            case 0: //Login
                Hub.GameManager.Gamestate = GameStates.Login;
                break;
            case 1: //Main
                Hub.GameManager.Gamestate = GameStates.Main;
                break;
            case 2: //Box
                Hub.GameManager.Gamestate = GameStates.Box;
                Hub.SingletonUIManager.ButtonHeader();
                break;
            case 3: //Battle
                Hub.GameManager.Gamestate = GameStates.Battle;
                break;
            case 4: //Event
                Hub.GameManager.Gamestate = GameStates.Event;
                break;
            case 5: //Rest
                Hub.GameManager.Gamestate = GameStates.Rest;
                break;
            case 6: //Shop
                Hub.GameManager.Gamestate = GameStates.Shop;
                break;
            case 7: //Ending
                Hub.GameManager.Gamestate = GameStates.Ending;
                break;
            default:
                break;
        }
        switch (OffType)
        {
            case 1:
                yield return StartCoroutine(TransitionOffType1Cor());
                break;
            case 2:
                yield return StartCoroutine(TransitionOffType2Cor());
                break;
            default:
                break;
        }
    }


    /// //////////
    /// Type1: 그냥 전환되는 거 
    /// Type2: 암막이 내리고 걷히고 하는 걸로 바꿀까 하는데 현재는 Type1이랑 동일함.
    /// //////////

    IEnumerator TransitionOnType1Cor()
    {
        transition1.SetActive(true);
        for (float i = 0f; i < 1f; i += 0.05f)
        {
            tmpColor.a = i;
            transition1.GetComponent<Image>().color = tmpColor;
            yield return new WaitForSeconds(0.015f);
        }
        tmpColor.a = 1f;
        transition1.GetComponent<Image>().color = tmpColor;
        yield return new WaitForSeconds(1.7f);
    }
    IEnumerator TransitionOnType2Cor()
    {
        transition1.SetActive(true);
        for (float i = 0f; i < 1f; i += 0.05f)
        {
            tmpColor.a = i;
            transition1.GetComponent<Image>().color = tmpColor;
            yield return new WaitForSeconds(0.015f);
        }
        tmpColor.a = 1f;
        transition1.GetComponent<Image>().color = tmpColor;
        yield return new WaitForSeconds(1.7f);
    }

    IEnumerator TransitionOffType1Cor()
    {
        for (float i = 1f; i > 0f; i -= 0.05f)
        {
            tmpColor.a = i;
            transition1.GetComponent<Image>().color = tmpColor;
            yield return new WaitForSeconds(0.015f);
        }
        tmpColor.a = 0f;
        transition1.GetComponent<Image>().color = tmpColor;
        transition1.SetActive(false);
        yield return new WaitForSeconds(1.7f);
    }
    IEnumerator TransitionOffType2Cor()
    {
        for (float i = 1f; i > 0f; i -= 0.05f)
        {
            tmpColor.a = i;
            transition1.GetComponent<Image>().color = tmpColor;
            yield return new WaitForSeconds(0.015f);
        }
        tmpColor.a = 0f;
        transition1.GetComponent<Image>().color = tmpColor;
        transition1.SetActive(false);
        yield return new WaitForSeconds(1.7f);
    }




}
