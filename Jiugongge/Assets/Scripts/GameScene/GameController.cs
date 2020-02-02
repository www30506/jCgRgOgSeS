using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {
    [SerializeField] private GameObject cardPrefab_Player;
    [SerializeField] private GameObject cardPrefab_Monster;
    [SerializeField] private GameObject cardPrefab_Item;
    [SerializeField] private GameObject cardPrefab_Trap;
    [SerializeField] private Transform cardParnet;

    void Start() {
        InitGame();
    }

    private void InitGame(){
        for(int i=0; i< GameData.column; i++) {
            for(int j=0; j< GameData.Row; j++) {
                int _index = i * GameData.Row + j;
                //玩家位置
                if (_index == 4) {
                    CreatePlayer(_index);
                }
                else {
                    CreateMonster(_index);
                }
            }
        }
    }

    private void CreatePlayer(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Player);
        _obj.transform.SetParent(cardParnet, false);
        Card_Player _card = _obj.GetComponent<Card_Player>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    private void CreateMonster(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Monster);
        _obj.transform.SetParent(cardParnet, false);
        CardBase _card = _obj.GetComponent<CardBase>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    private void CreateItem(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Monster);
        _obj.transform.SetParent(cardParnet, false);
        CardBase _card = _obj.GetComponent<CardBase>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    private void CreateTrap(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Monster);
        _obj.transform.SetParent(cardParnet, false);
        CardBase _card = _obj.GetComponent<CardBase>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    public void OnCardTouch(int p_touchCardPositionIndex) {
        
    }

    private void ShowCanMoveTip() {

    }

    void Update() {

    }
}