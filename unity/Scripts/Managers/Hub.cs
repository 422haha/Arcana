using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub : MonoBehaviour
{

    [Header("공통 파트")]
    static GameManager _GameManager;
    static TransitionManager _TransitionManager;
    static SingletonUIManager _SingletonUIManager;
    static ProgressManager _ProgressManager;
    static EnemyInfoManager _EnemyInfoManager;
    static ShopSceneManager _ShopSceneManager;
    static BattleSceneManager _BattleSceneManager;
    static ArtifactManager _ArtifactManager;

    public static GameManager GameManager { get { if (_GameManager == null) _GameManager = FindObjectOfType<GameManager>(); return _GameManager; } }
    public static TransitionManager TransitionManager { get { if (_TransitionManager == null) _TransitionManager = FindObjectOfType<TransitionManager>(); return _TransitionManager; } }
    public static SingletonUIManager SingletonUIManager { get { if (_SingletonUIManager == null) _SingletonUIManager = FindObjectOfType<SingletonUIManager>(); return _SingletonUIManager; } }
    public static BattleSceneManager BattleSceneManager { get { if (_BattleSceneManager == null) _BattleSceneManager = FindObjectOfType<BattleSceneManager>(); return _BattleSceneManager; } }
    public static ArtifactManager ArtifactManager { get { if (_ArtifactManager == null) _ArtifactManager = FindObjectOfType<ArtifactManager>(); return _ArtifactManager; } }


    [Header("헛햇훗 파트")]

    [Header("김세진 파트")]

    [Header("황준 파트")]
    static TranslationManager _TranslationManager;
    static DeckManager _DeckManager;
    public static EnemyInfoManager EnemyInfoManager { get { if (_EnemyInfoManager == null) _EnemyInfoManager = FindObjectOfType<EnemyInfoManager>(); return _EnemyInfoManager; } }
    public static ShopSceneManager ShopSceneManager { get { if (_ShopSceneManager == null) _ShopSceneManager = FindObjectOfType<ShopSceneManager>(); return _ShopSceneManager; } }

    public static TranslationManager TranslationManager { get { if (_TranslationManager == null) _TranslationManager = FindObjectOfType<TranslationManager>(); return _TranslationManager; } }
    public static DeckManager DeckManager { get { if (_DeckManager == null) _DeckManager = FindObjectOfType<DeckManager>(); return _DeckManager; } }

    [Header("강미연 파트")]
    static SoundManager _SoundManager;

    public static SoundManager SoundManager { get { if (_SoundManager == null) _SoundManager = FindObjectOfType<SoundManager>(); return _SoundManager; } }

    [Header("손동희 파트")]
    static APIManager _ApiManager;
    static NetworkManager _NetworkManager;
    static LoginManager _LoginManager;
    static Map.MapManager _Mapmanager;
    static Map.MapPlayerTracker _MapPlayerTracker;
    static Map.MapViewUI _MapViewUI;
    static AIManager _AIManager;

    public static APIManager APIManager { get { if (_ApiManager == null) _ApiManager = FindObjectOfType<APIManager>(); return _ApiManager; } }
    public static NetworkManager NetworkManager { get { if (_NetworkManager == null) _NetworkManager = FindObjectOfType<NetworkManager>(); return _NetworkManager; } }
    public static LoginManager LoginManager { get { if (_LoginManager == null) _LoginManager = FindObjectOfType<LoginManager>(); return _LoginManager; } }
    public static ProgressManager ProgressManager { get { if (_ProgressManager == null) _ProgressManager = FindObjectOfType<ProgressManager>(); return _ProgressManager; } }
    public static Map.MapManager MapManager { get { if (_Mapmanager == null) _Mapmanager = FindObjectOfType<Map.MapManager>(); return _Mapmanager; } }
    public static Map.MapPlayerTracker MapPlayerTracker { get { if (_MapPlayerTracker == null) _MapPlayerTracker = FindObjectOfType<Map.MapPlayerTracker>(); return _MapPlayerTracker; } }
    public static Map.MapViewUI MapViewUI { get { if (_MapViewUI == null) _MapViewUI = FindObjectOfType<Map.MapViewUI>(); return _MapViewUI; } }
    public static AIManager AIManager { get { if (_AIManager == null) _AIManager = FindObjectOfType<AIManager>(); return _AIManager; } }

    [Header("신건우 파트")]
    //아무것도 없으면 에러나길래 해놓은 거 
    public bool deleteThis;

    /*
    static PlayerStatus _PlayerStatus;
    static UIManager _UIManager;
    static SoundManager _SoundManager;
    static BGMManager _BGMManager;
    static SFXManager _SFXManager;
    static CVManager _CVManager;
    
    static StageManager _StageManager;
    static InputManager _InputManager;
    static SkillManager _SkillManager;

    public static PlayerStatus PlayerStatus { get { if (_PlayerStatus == null) _PlayerStatus = FindObjectOfType<PlayerStatus>(); return _PlayerStatus; } }
    public static UIManager UIManager { get { if (_UIManager == null) _UIManager = FindObjectOfType<UIManager>(); return _UIManager; } }
    public static SoundManager SoundManager { get { if (_SoundManager == null) _SoundManager = FindObjectOfType<SoundManager>(); return _SoundManager; } }
    public static BGMManager BGMManager { get { if (_BGMManager == null) _BGMManager = FindObjectOfType<BGMManager>(); return _BGMManager; } }
    public static SFXManager SFXManager { get { if (_SFXManager == null) _SFXManager = FindObjectOfType<SFXManager>(); return _SFXManager; } }
    public static CVManager CVManager { get { if (_CVManager == null) _CVManager = FindObjectOfType<CVManager>(); return _CVManager; } }
    
    public static StageManager StageManager { get { if (_StageManager == null) _StageManager = FindObjectOfType<StageManager>(); return _StageManager; } }
    public static InputManager InputManager { get { if (_InputManager == null) _InputManager = FindObjectOfType<InputManager>(); return _InputManager; } }
    public static SkillManager SkillManager { get { if (_SkillManager == null) _SkillManager = FindObjectOfType<SkillManager>(); return _SkillManager; } }

    [Header("황준 파트")]

    static PlayerController _PlayerController;
    public static PlayerController PlayerController { get { if (_PlayerController == null) _PlayerController = FindObjectOfType<PlayerController>(); return _PlayerController; } }


    [Header("ETC")]
    static DamageReceiver _DamageReceiver;
    static a_PlayerUI _a_PlayerUI;
    public static DamageReceiver DamageReceiver { get { if (_DamageReceiver == null) _DamageReceiver = FindObjectOfType<DamageReceiver>(); return _DamageReceiver; } }
    public static a_PlayerUI a_PlayerUI { get { if (_a_PlayerUI == null) _a_PlayerUI = FindObjectOfType<a_PlayerUI>(); return _a_PlayerUI; } }
    */



}
