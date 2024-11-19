using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    Login,
    Main,
    Box,
    Battle,
    Event,
    Rest,
    Shop,
    Ending
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameStates _GameState;
    public GameStates Gamestate
    {
        get { return _GameState; }
        set
        {
            _GameState = value;
            switch (value)
            {
                case GameStates.Login: //이거 불러 올 경우가 없을 것 같은데
                    Hub.SingletonUIManager.windowHeader.SetActive(false);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Main: //
                    Hub.SingletonUIManager.windowHeader.SetActive(false);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Box:
                    Hub.SingletonUIManager.windowHeader.SetActive(true);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Battle:
                    Hub.SingletonUIManager.windowHeader.SetActive(true);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Event:
                    Hub.SingletonUIManager.windowHeader.SetActive(true);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Rest:
                    Hub.SingletonUIManager.windowHeader.SetActive(true);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Shop:
                    Hub.SingletonUIManager.windowHeader.SetActive(true);
                    Hub.SingletonUIManager.TurnOffFours();
                    break;
                case GameStates.Ending:
                    Hub.SingletonUIManager.windowHeader.SetActive(false);
                    Hub.SingletonUIManager.TurnOffFours();                    
                    break;
                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
