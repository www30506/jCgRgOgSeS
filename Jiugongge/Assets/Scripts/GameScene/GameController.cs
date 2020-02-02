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
    [SerializeField] private Card_Player player;


    void Start() {
        InitGame();
    }

    private void InitGame(){
        for(int i=0; i< GameData.column; i++) {
            for(int j=0; j< GameData.Row; j++) {
                int _index = i * GameData.Row + j;
                //玩家位置
                if (_index == 8) {
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
        player = _card;
    }

    private void CreateMonster(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Monster);
        _obj.transform.SetParent(cardParnet, false);
        Card_Monster _card = _obj.GetComponent<Card_Monster>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    private void CreateItem(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Monster);
        _obj.transform.SetParent(cardParnet, false);
        Card_Item _card = _obj.GetComponent<Card_Item>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    private void CreateTrap(int p_CardPosition) {
        GameObject _obj = Instantiate(cardPrefab_Monster);
        _obj.transform.SetParent(cardParnet, false);
        Card_Trap _card = _obj.GetComponent<Card_Trap>();
        _card.Init(0.ToString(), p_CardPosition, OnCardTouch);
    }

    public void OnCardTouch(CardBase p_touchCard) {
        print("【Controller】Touch");
        if (IsValidClick(p_touchCard.GetPositionIndex())) {
            print("【有效】");
        }
        else {
            ShowValidTip();
        }

    }

    public bool IsValidClick(int p_touchCardPositionIndex) {
        int _playerPositionIndex = player.GetPositionIndex();
        //不在最右邊並且點擊的位置在玩家右邊
        if (_playerPositionIndex % GameData.Row != GameData.Row-1 && p_touchCardPositionIndex == _playerPositionIndex + 1) {
            return true;
        }
        //不在最左邊並且點擊的位置在玩家左邊
        else if (_playerPositionIndex % GameData.Row != 0 && p_touchCardPositionIndex == _playerPositionIndex - 1) {
            return true;
        }
        //不在最上面並且點擊的位置在玩家上面
        else if (_playerPositionIndex / GameData.Row != 0 && p_touchCardPositionIndex == _playerPositionIndex - GameData.Row) {
            return true;
        }
        //不在最下面並且點擊的位置在玩家下面
        else if (_playerPositionIndex / GameData.Row != GameData.column-1 && p_touchCardPositionIndex == _playerPositionIndex + GameData.Row) {
            return true;
        }

        return false;
    }

    private void ShowValidTip() {
        print("顯示可以移動的提示");
    }

    void Update() {

    }
}