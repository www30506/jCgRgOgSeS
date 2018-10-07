﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class GameSystem_Poker : MonoBehaviour {
	private enum OperationType{Addition, Subtraction, Multiplication, Division};
	private enum SystemStatue{Idle, Working, Win , Loss};
	private enum CardMoveType{Left, Right, Up, Down};
	[SerializeField]private CardMoveType cardMoveState = CardMoveType.Left;

	[Header("Data")]
	[SerializeField]private int[] completeTargets;
	[SerializeField]private bool[] IscompleteTargets;
	[SerializeField]private OperationType operationState= OperationType.Addition;
	[SerializeField]private string[] initCardsID;
	[SerializeField]private Queue<int> drawCardsQueue;
	[SerializeField]private float timeLeft;
	[SerializeField]private float useTime;
	[SerializeField]private int changeOperationCount;
	[Header("====")]

	[SerializeField]private int drawCardIndex;

	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<Card> cards;
	private Queue<Card> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;
	[SerializeField]private GameView_Poker gameView;
	private SystemStatue systemStatue = SystemStatue.Idle;
	[SerializeField]private Card playerCard;
	private int prePlayerPositionIndex;
	private int endlessModeDrawCardCount = 0;
	private float getStarTime = 0;
	private int endlessMode_GetScore = 0;
	public int endlessMode_CompleteCount = 0;

	void Start () {
		InitDrawCardList ();
//		CreateCardPool ();
//		CreateCards ();
//		CreateCompleteTarget ();
//		InitActionValue ();
//		gameView.HideChangeOperationCountText ();
//		getStarTime = GlobalData.MAIN_MODE_GETSTARTIME;
//		gameView.SetOperationBtnActive ("Addition");
	}

	private void InitDrawCardList(){
		drawCardsQueue = new Queue<int>();
		List<int> _tempList = new List<int>();
		for (int i = 0; i < 52; i++) {
			_tempList.Add (i + 17);
		}

		while(_tempList.Count >0){
			int _random = UnityEngine.Random.Range (0, _tempList.Count);
			drawCardsQueue.Enqueue (_tempList [_random]);
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

	private string GetTargetRange(int p_level){
		int _targetCount = GetTargetCount(p_level);
		int _startNumber = 0;
		string _targetRange = "";

		if (p_level % 5 == 0) {
			_startNumber = 1 + (((p_level-1) / 5)*10);
		}
		else if (p_level % 5 == 1) {
			_startNumber = 1 + ((p_level / 5)*10);
		}
		else if (p_level % 5 == 2) {
			_startNumber = 2 + ((p_level / 5)*10);
		}
		else if (p_level % 5 == 3) {
			_startNumber = 4 + ((p_level / 5)*10);
		}
		else if (p_level % 5 == 4) {
			_startNumber = 7 + ((p_level / 5)*10);
		}

		if(p_level % 5 == 0){
			_targetRange = "1~" + ((p_level / 5) * 10);
		}
		else{
			for (int i = 0; i < _targetCount; i++) {
				if (string.IsNullOrEmpty (_targetRange)) {
					_targetRange += "" + (i + _startNumber);
				} 
				else {
					_targetRange += "," + (i + _startNumber);
				}
			}
		}

		return _targetRange;
	}

	private void InitActionValue(){
		if (Game.endlessMode) {
			timeLeft = GlobalData.ENDLESS_MODE_START_TIME;
		} 
		else {
//			int _tagetCount = GetTargetCount (Game.NOWLEVEL + 1);
//			timeLeft = _tagetCount * GlobalData.TARGET_TIME;
			if ((Game.NOWLEVEL + 1) % 5 == 0) {
				timeLeft = GlobalData.MAIN_MODE_TIMELEFT_Random;
			} 
			else {
				timeLeft = GlobalData.MAIN_MODE_TIMELEFT;
			}
		}
		gameView.SetActionValue (timeLeft);
	}

	private void SetActionValue(int p_value){
		timeLeft += p_value;
		gameView.SetActionValue (timeLeft);
	}

	private void CreateCompleteTarget(){
		int _tagetCount = GetTargetCount (Game.NOWLEVEL +1);
		string _targetRange = GetTargetRange(Game.NOWLEVEL+1);
		List<int> _rangeGroup = new List<int> ();
		print ("_targetRange : " + _targetRange);
		if (_targetRange.Contains ("~")) {
			string[] _aaa = Regex.Split (_targetRange, "~");
			int _startNumber = int.Parse(_aaa[0]);
			int _endNumber = int.Parse (_aaa [1]);

			int _tempNumber = _startNumber;
			while(_tempNumber <= _endNumber){
				_rangeGroup.Add (_tempNumber++);
			}
		}
		else{
			string[] _targetGroup;
			_targetGroup = Regex.Split (_targetRange, ",");

			for (int i = 0; i < _targetGroup.Length; i++) {
				_rangeGroup.Add (int.Parse(_targetGroup[i]));
			}
		}


		completeTargets = new int[_tagetCount];
		IscompleteTargets = new bool[_tagetCount];
		for (int i = 0; i < completeTargets.Length; i++) {

			int _index = UnityEngine.Random.Range (0, _rangeGroup.Count);
			completeTargets [i] = _rangeGroup[_index];
			_rangeGroup.RemoveAt (_index);

		}

		//重小排到大
		for (int i = 0; i < completeTargets.Length-1; i++) {
			for (int j = i + 1; j < completeTargets.Length; j++) {
				if (completeTargets [i] > completeTargets [j]) {
					int _temp = completeTargets [i];
					completeTargets [i] = completeTargets [j];
					completeTargets [j] = _temp;
				}
			}
		}

		gameView.InitCompleteTarget (completeTargets);
	}

	private void CreateCardPool(){;
		cardsPool = new Queue<Card>();

		for(int i=0; i< maxCards; i++){
			GameObject _obj = Instantiate(Resources.Load("Prefabs/Game/Card"))  as GameObject;
			Card _card = _obj.GetComponent<Card>();
			cardsPool.Enqueue(_card);
			_obj.transform.SetParent(cardPoolTransform, false);
		}
	}

	private Card GetCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				return cards[i];
			}
		}

		return null;
	}

	IEnumerator CreateCard(string p_CardID, int p_positionIndex){
		Card _card = cardsPool.Dequeue();
		cards.Add (_card);
		_card.Init(p_CardID, p_positionIndex, OnTouchCardEvent);
		yield return StartCoroutine(_card.CreateEff());
		_card.transform.SetParent(cardListTransform, false);

		if (_card.GetCardType() == "Player") {
			playerCard = _card;
			InitPlayerStartValue ();
		}
	}

	private void CreateCards(){
		for(int i=0; i< 9; i++){
			StartCoroutine(CreateCard(initCardsID[i], i));
		}
	}

	private void InitPlayerStartValue (){
		if (Game.NOWLEVEL > 4 || Game.endlessMode) {
			string[] _StartValueRange = Regex.Split (GlobalData.PLAYER_START_VALUE, "~");
			int _additionValue = UnityEngine.Random.Range (int.Parse(_StartValueRange[0]), int.Parse(_StartValueRange[1])+1);
			playerCard.AdditionValue (_additionValue);
		}
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.A)) {
			for (int i = 0; i < drawCardsQueue.Count; i++) {
				int _aa =drawCardsQueue.Dequeue();
				drawCardsQueue.Enqueue (_aa);
				print (_aa);
			}
		}
//		timeLeft -= Time.deltaTime;
//		useTime += Time.deltaTime;
//
//		gameView.SetActionValue (timeLeft);
//		if (timeLeft < 10) {
//			gameView.ActionValueFlash (true);
//		} 
//		else {
//			gameView.ActionValueFlash (false);
//		}
//
//
//		if (timeLeft < 0 && systemStatue != SystemStatue.Loss) {
//			print ("【遊戲結束】 時間結束");
//			systemStatue = SystemStatue.Loss;
//
//			if (Game.endlessMode) {
//				endlessMode_GetScore = endlessMode_CompleteCount;
//				gameView.ShowendlessModeGameOverUI(endlessMode_GetScore , useTime);
//				SaveData ();
//			} 
//			else {
//				gameView.ShowLossUI ();
//			}
//		}
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
			Card _card = GetCard (p_touchCardPositionIndex);

			DoCardAction (_card);

			yield return StartCoroutine(IE_DestoryCard(p_touchCardPositionIndex));

			yield return StartCoroutine (IE_MoveCard (p_touchCardPositionIndex));

			endlessModeDrawCardCount++;
			if (endlessModeDrawCardCount % 4 == 0) {
				string[] _skillCards = new string[]{"14", "15", "16"};
				drawCardIndex = UnityEngine.Random.Range (0, _skillCards.Length);
				yield return StartCoroutine (CreateCard (_skillCards [drawCardIndex], GetCreateCardPositionIndex ()));
			} 
			else {
				string[] _skillCards = new string[]{"10","1","2","3","4","5","6","7","8","9"};
				drawCardIndex = UnityEngine.Random.Range (0, _skillCards.Length);
				yield return StartCoroutine (CreateCard (_skillCards [drawCardIndex], GetCreateCardPositionIndex ()));
			}

			CheckCompleteTarget ();

			if (Game.endlessMode) {
				if (IsAllTargetsComplete ()) {
					print ("創造新的目標");
					changeOperationCount += GlobalData.ENDLESS_MODE_COMPLETE_TARGET_ADD_changeOperationCount;
					gameView.SetChangeOperationCountText (changeOperationCount);
					CheckCompleteTarget ();
					CreateCompleteTarget_EndlessMode ();
					gameView.ResetCompleteTargetEff ();
				}
			}
			else {
				if (IsAllTargetsComplete () && systemStatue != SystemStatue.Win) {
					print ("勝利");
					systemStatue = SystemStatue.Win;
					bool _isGetStar = useTime < getStarTime ? true : false;
					gameView.ShowWinUI (useTime, _isGetStar);
					SaveData ();
				}
			}
		}

		systemStatue = SystemStatue.Idle;
		yield return null;
	}
		
	private void SaveData(){
		PlayerData _playerData = PlayerData.Create ();

		if (Game.endlessMode) {
			float _bestTime = _playerData.endlessModeData.bestTime;
			if (useTime > _bestTime) {
				_playerData.endlessModeData.bestTime = useTime;
			}

			float _maxScore = _playerData.endlessModeData.maxScore;
			if (endlessMode_GetScore > _maxScore) {
				_playerData.endlessModeData.maxScore = endlessMode_GetScore;
			}	
		}
		else{
			float _bestTime = _playerData.levelDatas [Game.NOWLEVEL].BestTime;
			if (_bestTime == 0)
				_bestTime = 9999;

			if (useTime < _bestTime) {
				_playerData.levelDatas [Game.NOWLEVEL].BestTime = useTime;
			}
			_playerData.levelDatas [Game.NOWLEVEL].isComplete = true;

			bool _isGetStar = useTime < getStarTime ? true : false;
			if(_isGetStar){
				_playerData.levelDatas [Game.NOWLEVEL].getStart = _isGetStar;
			}
		}

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

	private void DoCardAction(Card p_card){
		switch (p_card.GetCardType()) {
		case "Action":
			SetActionValue(p_card.GetCardValue ());
			break;
		case "Base":
			if (operationState == OperationType.Addition) {
				playerCard.AdditionValue (p_card.GetCardValue ());
			} 
			else if (operationState == OperationType.Subtraction) {
				playerCard.SubtractionValue (p_card.GetCardValue ());
			}
			else if (operationState == OperationType.Multiplication) {
				playerCard.MultiplicationValue (p_card.GetCardValue ());
			}
			else if (operationState == OperationType.Division) {
				if (playerCard.CanDivision (p_card.GetCardValue ())) {
					playerCard.DivisionValue (p_card.GetCardValue ());
				}
				else {
					print ("不能執行");
				}
			}
			break;
		case "Question":
			StartCoroutine (IE_Question ());
			break;
		case "Marvel":
			StartCoroutine(IE_Marvel(p_card));
			break;
		case "ChangeOperation":
			changeOperationCount += p_card.GetCardValue ();
			gameView.SetChangeOperationCountText (changeOperationCount);
			break;
		case "Null":
			break;
		}
	}

	private IEnumerator IE_Question(){
		systemStatue = SystemStatue.Working;

		for (int i = 0; i < cards.Count; i++) {
			if (cards [i].GetCardType () == "Base") {
				int _randomNumber = UnityEngine.Random.Range (0, 10);
				cards[i].ResetCard(_randomNumber.ToString ());
			}
		}
		yield return null;
		systemStatue = SystemStatue.Idle;
	}

	private IEnumerator IE_Marvel(Card p_card){
		List<string> _cardsID = new List<string> ();
		//先取所有卡牌的位置存放在容器內
		for (int i = 0; i < cards.Count; i++) {
			if (cards [i].GetCardType () != "Player" && cards[i] != p_card) {
				_cardsID.Add (cards [i].GetCardID ());
			}
		}

		//再將所有卡牌重容器內隨機取得一個新位置
		for (int i = 0; i < cards.Count; i++) {
			if (cards [i].GetCardType () != "Player" && cards[i] != p_card) {
				int _index = UnityEngine.Random.Range (0, _cardsID.Count);
				string _newCardID = _cardsID[_index];
				cards [i].ResetCard (_newCardID);
				_cardsID.RemoveAt (_index);
			}
		}

		yield return null;
	}

	private bool IsNearby(int p_playerPositionIndex, int p_touchCardPositionIndex){
		bool _isNearby = false;

		if (p_touchCardPositionIndex == p_playerPositionIndex - 1 || //左邊
			p_touchCardPositionIndex == p_playerPositionIndex + 1 || //右邊
			p_touchCardPositionIndex == p_playerPositionIndex - 3 || //上面
			p_touchCardPositionIndex == p_playerPositionIndex + 3 //下面
		) {
			_isNearby = true;
		}

		return _isNearby;
	}

	public void OnChangeOperation(string p_Operation){
		if (Game.endlessMode) {
			if (changeOperationCount > 0) {
				if (operationState != (OperationType)Enum.Parse (typeof(OperationType), p_Operation)) {
					changeOperationCount--;
					operationState = (OperationType)Enum.Parse (typeof(OperationType), p_Operation);
					gameView.SetChangeOperationCountText (changeOperationCount);
					gameView.SetOperationBtnActive (p_Operation);
				}
			}
		}
		else {
			operationState = (OperationType)Enum.Parse (typeof(OperationType), p_Operation);
			gameView.SetOperationBtnActive (p_Operation);
		}
	}

	private void CheckCompleteTarget(){
		for (int i = 0; i < IscompleteTargets.Length; i++) {
			if (playerCard.GetCardValue () == completeTargets [i] && IscompleteTargets [i] == false) {
				IscompleteTargets [i] = true;

				gameView.CompleteTargetEff (i);
				if (Game.endlessMode) {
					timeLeft += GlobalData.ENDLESS_MODE_COMPLETE_ADD_TIME;
					gameView.SetActionValue (timeLeft);
					endlessMode_CompleteCount++;
				}
			}
		}
	}

	private bool IsAllTargetsComplete(){
		for (int i = 0; i < IscompleteTargets.Length; i++) {
			if (IscompleteTargets [i] == false) {
				return false;
			}
		}

		return true;
	}

	private void CreateCompleteTarget_EndlessMode(){
		int _tagetCount = UnityEngine.Random.Range (1, 6);
		string _targetRange = GlobalData.ENDLESS_MODE_TARGET_NUMBER_START + "~" + GlobalData.ENDLESS_MODE_TARGET_NUMBER_END;

		List<int> _rangeGroup = new List<int> ();
		print ("_targetRange : " + _targetRange);
		if (_targetRange.Contains ("~")) {
			string[] _aaa = Regex.Split (_targetRange, "~");
			int _startNumber = int.Parse(_aaa[0]);
			int _endNumber = int.Parse (_aaa [1]);

			int _tempNumber = _startNumber;
			while(_tempNumber <= _endNumber){
				_rangeGroup.Add (_tempNumber++);
			}
		}
		else{
			string[] _targetGroup;
			_targetGroup = Regex.Split (_targetRange, ",");

			for (int i = 0; i < _targetGroup.Length; i++) {
				_rangeGroup.Add (int.Parse(_targetGroup[i]));
			}
		}


		completeTargets = new int[_tagetCount];
		IscompleteTargets = new bool[_tagetCount];
		for (int i = 0; i < completeTargets.Length; i++) {

			int _index = UnityEngine.Random.Range (0, _rangeGroup.Count);
			completeTargets [i] = _rangeGroup[_index];
			_rangeGroup.RemoveAt (_index);

		}

		//重小排到大
		for (int i = 0; i < completeTargets.Length-1; i++) {
			for (int j = i + 1; j < completeTargets.Length; j++) {
				if (completeTargets [i] > completeTargets [j]) {
					int _temp = completeTargets [i];
					completeTargets [i] = completeTargets [j];
					completeTargets [j] = _temp;
				}
			}
		}

		gameView.InitCompleteTarget (completeTargets);
	}

	public void OnNextLevel(){
		Game.NOWLEVEL++;
		Game.endlessMode = false;
		Game.LoadScene ("GameScene");
	}
}