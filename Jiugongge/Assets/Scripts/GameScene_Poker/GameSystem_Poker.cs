using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class GameSystem_Poker : MonoBehaviour {
	private enum SystemStatue{Idle, Working, Win , Loss};
	private enum CardMoveType{Left, Right, Up, Down};
	[SerializeField]private CardMoveType cardMoveState = CardMoveType.Left;

	[Header("Data")]
	[SerializeField]private List<string> drawCards_ID_List;
	[SerializeField]private int score;
	[Header("====")]

	[SerializeField]private JudgingCardType judgingCardType;

	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<CardPoker> cards;
	private Queue<CardPoker> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;
	[SerializeField]private GameView_Poker gameView;
	private SystemStatue systemStatue = SystemStatue.Idle;
	[SerializeField]private CardPoker playerCard;
	[SerializeField]private CardPoker[] buttomCards;
	[SerializeField]private CardPoker nextCard;
	private int prePlayerPositionIndex;
	[SerializeField]private int combo = 1;

	void Start () {
		InitDrawCardList ();
		InitButtomCards ();
		CreateCardPool ();
		CreateCards ();
		gameView.SetScore (0);
		ChangeNextCardTip ();
	}


	private void InitButtomCards(){
		for (int i = 0; i < buttomCards.Length; i++) {
			buttomCards [i].ResetCard ("12");
		}
	}

	private void ChangeNextCardTip(){
		if(drawCards_ID_List.Count > 0){
			nextCard.ResetCard (drawCards_ID_List [0]);	
		}
	}

	private void InitDrawCardList(){
		drawCards_ID_List = new List<string>();
		List<int> _tempList = new List<int>();
		for (int i = 0; i < 52; i++) {
			_tempList.Add (i + 17);
		}

		while(_tempList.Count >0){
			int _random = UnityEngine.Random.Range (0, _tempList.Count);
			drawCards_ID_List.Add (_tempList [_random].ToString());
			_tempList.RemoveAt (_random);
		}
	}

	private int GetTargetCount(int p_level){
		int _targetCount = p_level % 5;
		if (_targetCount == 0) {
			_targetCount = 5;
		}

		return _targetCount;
	}

	private void CreateCardPool(){;
		cardsPool = new Queue<CardPoker>();

		for(int i=0; i< maxCards; i++){
			GameObject _obj = Instantiate(Resources.Load("Prefabs/Game/CardPoker"))  as GameObject;
			CardPoker _card = _obj.GetComponent<CardPoker>();
			cardsPool.Enqueue(_card);
			_obj.transform.SetParent(cardPoolTransform, false);
		}
	}

	private CardPoker GetCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				return cards[i];
			}
		}

		return null;
	}

	IEnumerator CreateCard(string p_CardID, int p_positionIndex){
		CardPoker _card = cardsPool.Dequeue();
		cards.Add (_card);
		_card.Init(p_CardID, p_positionIndex, OnTouchCardEvent);
		if (_card.GetCardType () == "Player") {
			playerCard = _card;
		} 
		else {
			drawCards_ID_List.RemoveAt (0);
		}

		yield return StartCoroutine(_card.CreateEff());
		_card.transform.SetParent(cardListTransform, false);
	}

	private void CreateCards(){
		for(int i=0; i< 9; i++){
			if (i != 4) {
				StartCoroutine (CreateCard (drawCards_ID_List[0], i));
			} 
			else {
				StartCoroutine (CreateCard (13.ToString(), i));
			}
		}
	}

	void Update () {
		if(Input.GetKeyUp(KeyCode.Q)){
			OnHitOutPoker();
			if(HasAnySuitType() == false){
				Debug.LogError("GameOver");
				systemStatue = SystemStatue.Win;
			}
		}
	}

	public void BackToMenu(){
		Game.LoadScene ("MenuScene");
	}

	private void OnTouchCardEvent(int p_touchCardPositionIndex){
		if (systemStatue != SystemStatue.Idle){
			print ("工作中");
			return;
		}

		StartCoroutine (IE_OnTouchCardEvent (p_touchCardPositionIndex));
	}

	IEnumerator IE_OnTouchCardEvent(int p_touchCardPositionIndex){
		systemStatue = SystemStatue.Working;

		if (IsNearby (playerCard.GetPositionIndex (), p_touchCardPositionIndex)) {
			CardPoker _card = GetCard (p_touchCardPositionIndex);

			//			DoCardAction (_card);

			CopyCardToButtom (_card.GetCardID());

			yield return StartCoroutine(IE_DestoryCard(p_touchCardPositionIndex));

			yield return StartCoroutine (IE_MoveCard (p_touchCardPositionIndex));

			yield return StartCoroutine (CreateCard (drawCards_ID_List[0], GetCreateCardPositionIndex ()));
			ChangeNextCardTip ();
//			OnHitOutPoker();

//			if(HasAnySuitType() == false){
//				Debug.LogError("GameOver");
//				systemStatue = SystemStatue.Win;
//				yield break;
//			}
		}

		systemStatue = SystemStatue.Idle;
		yield return null;
	}

	public void OnHitOutPoker(){
		int _length=0;

		for(int i=0; i<buttomCards.Length; i++){
			if (buttomCards[i].GetCardID () != "12") {
				_length++;
			}
		}

		string[] _cardsID = new string[_length];
		for(int i=0; i<_cardsID.Length; i++){
			_cardsID[i] = buttomCards[i].GetCardValue();
		}

		judgingCardType.SetPokerCards(_cardsID);
		SuitType _suitType = judgingCardType.GetSuitType();
		string _suitCards = judgingCardType.GetSuitPokerCards();

		if(_suitType != SuitType.HightCard){

			score += GetScore(_suitType, _suitCards);
			gameView.SetScore(score);

			DestoryButtomSuitCards(_suitCards);
			SortButtomCards();
			combo++;
		}
		else{
			print("沒有形成牌型");
		}
	}

	private int GetScore(SuitType p_suitType, string p_suitCards){
		string[] _cards = Regex.Split(p_suitCards, ",");
		int _Score = 0;
		for(int i=0; i<_cards.Length; i++){
			string[] _number = Regex.Split(_cards[i], "_");
			_Score += _number[1]=="1"? 20: int.Parse(_number[1]);
		}

		_Score = _Score * GlobalData.ScoreMagnification[(int)p_suitType] * combo;
		return _Score;
	}

	private void DestoryButtomSuitCards(string p_suitCards){
		string[] _cards = Regex.Split(p_suitCards, ",");
		for(int i=0; i< _cards.Length; i++){
			for(int j=0; j< buttomCards.Length; j++){
				if(_cards[i] == buttomCards[j].GetCardValue()){
					buttomCards[j].ResetCard("12");
				}
			}
		}
	}


	private void SortButtomCards(){
		for(int i=0;i<buttomCards.Length;i++){
			if(buttomCards[i].GetCardID() == "12"){
				for(int j=i; j<buttomCards.Length-1; j++){
					buttomCards[j].ResetCard(buttomCards[j+1].GetCardID());
				}
			}
		}
	}

	private bool HasAnySuitType(){
		bool _HasAnySuitType = true;
		if(drawCards_ID_List.Count <2){
			_HasAnySuitType = false;
		}
		return _HasAnySuitType;
	}

	private void CopyCardToButtom(string p_cardID){
		int _copyIndex = -1;

		//尋找位子
		for (int i = 0; i < buttomCards.Length; i++) {
			if (buttomCards [i].GetCardID () == "12") {
				_copyIndex = i;
				break;
			}
		}

		//如果沒位子就剃除最左邊
		if (_copyIndex == -1) {
			drawCards_ID_List.Add (buttomCards [0].GetCardID ());

			for (int i = 0; i < buttomCards.Length - 1; i++) {
				buttomCards [i].ResetCard (buttomCards [i + 1].GetCardID ());
			}

			buttomCards [buttomCards.Length - 1].ResetCard (p_cardID);
			combo =1;
		} 
		else {
			buttomCards [_copyIndex].ResetCard (p_cardID);
		}
	}

	private void SaveData(){
		PlayerData _playerData = PlayerData.Create ();

		_playerData.Save();
	}

	private int GetCreateCardPositionIndex(){
		int _CreateCardPositionIndex = 0;

		switch (cardMoveState) {
		case CardMoveType.Down:
			if (prePlayerPositionIndex - 3 >= 0) {
				_CreateCardPositionIndex = prePlayerPositionIndex -3;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			}
			break;

		case CardMoveType.Up:
			if (prePlayerPositionIndex + 3 > 8) {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex + 3;
			}
			break;

		case CardMoveType.Left:
			if (prePlayerPositionIndex % 3 == 2) {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex + 1;
			}
			break;

		case CardMoveType.Right:
			if (prePlayerPositionIndex % 3 == 0) {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex - 1;
			}
			break;
		}

		return _CreateCardPositionIndex;
	}

	IEnumerator IE_MoveCard(int p_touchCardPositionIndex){
		prePlayerPositionIndex = playerCard.GetPositionIndex ();
		StartCoroutine (playerCard.IE_SetPosition (p_touchCardPositionIndex));

		if (prePlayerPositionIndex - p_touchCardPositionIndex == 1) {
			cardMoveState = CardMoveType.Left;
		}
		else if (prePlayerPositionIndex - p_touchCardPositionIndex == -1) {
			cardMoveState = CardMoveType.Right;
		}
		else if (prePlayerPositionIndex - p_touchCardPositionIndex == 3) {
			cardMoveState = CardMoveType.Up;
		}
		else if (prePlayerPositionIndex - p_touchCardPositionIndex == -3) {
			cardMoveState = CardMoveType.Down;
		}

		switch (cardMoveState) {
		case CardMoveType.Down:
			if(prePlayerPositionIndex - 3 >= 0)
				StartCoroutine(GetCard (prePlayerPositionIndex - 3).IE_SetPosition (prePlayerPositionIndex));
			break;

		case CardMoveType.Up:
			if (prePlayerPositionIndex + 3 <= 8)
				StartCoroutine(GetCard (prePlayerPositionIndex + 3).IE_SetPosition (prePlayerPositionIndex));
			break;

		case CardMoveType.Left:
			if (prePlayerPositionIndex % 3 != 2)
				StartCoroutine(GetCard (prePlayerPositionIndex + 1).IE_SetPosition (prePlayerPositionIndex));
			break;

		case CardMoveType.Right:
			if (prePlayerPositionIndex % 3 != 0)
				StartCoroutine(GetCard (prePlayerPositionIndex-1).IE_SetPosition (prePlayerPositionIndex));
			break;
		}
		yield return new WaitForSeconds(0.5f);
	}

	IEnumerator IE_DestoryCard(int p_positionIndex){
		for(int i=0; i< cards.Count; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				yield return StartCoroutine(cards[i].Destory());
				cardsPool.Enqueue(cards[i]);
				cards[i].transform.SetParent(cardPoolTransform, false);
				cards.RemoveAt (i);
			}
		}
	}

	private void DoCardAction(CardPoker p_card){
		switch (p_card.GetCardType()) {
		case "Poker":
			break;
		case "Action":
			break;
		case "Base":
			break;
		case "Question":
			break;
		case "Marvel":
			break;
		case "ChangeOperation":
			break;
		case "Null":
			break;
		}
	}

	private bool IsNearby(int p_playerPositionIndex, int p_touchCardPositionIndex){
		bool _isNearby = false;

		if ((p_touchCardPositionIndex == p_playerPositionIndex - 1 && p_playerPositionIndex%3 !=  0)|| //左邊
			(p_touchCardPositionIndex == p_playerPositionIndex + 1 && p_playerPositionIndex%3 != 2)|| //右邊
			p_touchCardPositionIndex == p_playerPositionIndex - 3 || //上面
			p_touchCardPositionIndex == p_playerPositionIndex + 3 //下面
		) {
			_isNearby = true;
		}

		return _isNearby;
	}
}