using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class A_Hero : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hit;
    public TMP_Text hitTMP;
    public IsPlayers IsPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myHeroCardBorder.pointerOn.SetActive(true);
        else Hub.BattleSceneManager.oppHeroCardBorder.pointerOn.SetActive(true);


        if (Hub.BattleSceneManager.isDraggingOn)
        {
            Hub.BattleSceneManager.draggingTo = this.gameObject;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsPlayer == IsPlayers.Player) Hub.BattleSceneManager.myHeroCardBorder.pointerOn.SetActive(false);
        else Hub.BattleSceneManager.oppHeroCardBorder.pointerOn.SetActive(false);

        if (Hub.BattleSceneManager.isDraggingOn)
        {
            Hub.BattleSceneManager.draggingTo = null;
        }
    }


}
