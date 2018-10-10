using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public enum SuitType{RoyalFlush, FiveKind, STRFlush, FourKind, FullHourse, Flush, Straight, ThreeKind, TwoPair, Pair, HightCard};
public enum ColorType{C,D,H,S};

public class JudgingCardType : MonoBehaviour {
	[Header("Test")]
	public string[] testCards = new string[5];
	[Header("-----")]
	[SerializeField]private List<string> hasPokerCards = new List<string>(); //擁有的牌
	[SerializeField]private int[] suit_Colors = new int[4];
	[SerializeField]private int[] suit_Numbers = new int[13];
	[SerializeField]private int[,] suit_cards = new int[4, 13];
	[SerializeField]private List<string> suitPokerCards = new List<string>(); //最後牌型的幾張牌

	//SetPokerCards 將牌放入判斷
	//GetSuitType 取得牌型
	//GetSuitPokerCards 取得是哪幾張牌

	void Start () {

	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.A)) {
			SetPokerCards (testCards);
		}
		if (Input.GetKeyUp (KeyCode.S)) {
			print(GetSuitType());
		}
		if (Input.GetKeyUp (KeyCode.D)) {
			print (GetSuitPokerCards());
		}
	}

	public void SetPokerCards(string[] p_cards){
		while (hasPokerCards.Count > 0) {
			hasPokerCards.RemoveAt (0);
		}

		//這邊要把判斷的卡片
		for (int i = 0; i < p_cards.Length; i++) {
			hasPokerCards.Add (p_cards [i]);
		}
	}

	private void SetJudgmentSuitData(List<string> p_pokerSuit){
		for (int i = 0; i < hasPokerCards.Count; i++) {
			string[] _value = Regex.Split (hasPokerCards[i], "_");

			SetSuitColorsData (_value[0]);
			SetSuitNumbersData (_value [1]);
			SetSuitCardsData (_value [0], _value [1]);
		}
	}

	private void SetSuitColorsData(string p_color){
		if (p_color == "C") {
			suit_Colors [0]++;
		}
		else if (p_color == "D") {
			suit_Colors [1]++;
		}
		else if (p_color == "H") {
			suit_Colors [2]++;
		}
		else if (p_color == "S") {
			suit_Colors [3]++;
		}
	}

	private void ClearJudgmentSuitData(){
		for (int i = 0; i < suit_Colors.Length; i++) {
			suit_Colors [i] = 0;
		}

		for (int i = 0; i < suit_Numbers.Length; i++) {
			suit_Numbers [i] = 0;
		}

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 13; j++) {
				suit_cards [i, j] = 0;
			}
		}
	}

	private void SetSuitNumbersData(string p_number){
		suit_Numbers[int.Parse(p_number)-1]++;
	}

	private void SetSuitCardsData(string p_color, string p_number){
		int _colorNumber = 0;

		if (p_color == "C") {
			_colorNumber = 0;
		}
		else if (p_color == "D") {
			_colorNumber = 1;
		}
		else if (p_color == "H") {
			_colorNumber = 2;
		}
		else if (p_color == "S") {
			_colorNumber = 3;
		}

		suit_cards [_colorNumber, int.Parse(p_number)-1] ++;
	}

	public SuitType GetSuitType(){
		ClearJudgmentSuitData ();
		SuitType _suitType = SuitType.HightCard;
		SetJudgmentSuitData (hasPokerCards);

		if (IsRoyalFlush ()) {
			_suitType = SuitType.RoyalFlush;
		}
		else if (IsSTRFlush ()) {
			_suitType = SuitType.STRFlush;
		} 
		else if (IsFourKind ()) {
			_suitType = SuitType.FourKind;
		} 
		else if (IsFullHourse ()) {
			_suitType = SuitType.FullHourse;
		} 
		else if (IsFlush ()) {
			_suitType = SuitType.Flush;
		} 
		else if (IsStraight ()) {
			_suitType = SuitType.Straight;
		} 
		else if (IsThreeKind ()) {
			_suitType = SuitType.ThreeKind;
		} 
		else if (IsTwoPair ()) {
			_suitType = SuitType.TwoPair;
		} 
		else if (IsPair ()) {
			_suitType = SuitType.Pair;
		}
		return _suitType;
	}

	private bool IsRoyalFlush(){
		ClearSuitPokerCards ();
		for (int i = 0; i < 4; i++) {
			int j = 9;
			int _comboCount = 0;

			for (int k = 0; k < 5; k++) {
				int _number = (j + k == 13) ? 0 : j + k;
				if (suit_cards[i,_number] > 0) {
					_comboCount++;
				}

				if (k == 4 && _comboCount >= 5) {
					//儲存卡片 用來知道是哪幾張牌是最後牌型
					string[] _cards = SearchSTRFlushCards ((ColorType)i,j + 1);
					SaveSuitPokerCards (_cards);
					return true;
				}
			}
		}
		return false;
	}

	private bool IsSTRFlush(){
		ClearSuitPokerCards ();
		for (int i = 0; i < 4; i++) {
			for (int j = 9; j >= 0; j--) {
				int _comboCount = 0;

				for (int k = 0; k < 5; k++) {
					int _number = (j + k == 13) ? 0 : j + k;
					if (suit_cards[i,_number] > 0) {
						_comboCount++;
					}

					if (k == 4 && _comboCount >= 5) {
						//儲存卡片 用來知道是哪幾張牌是最後牌型
						string[] _cards = SearchSTRFlushCards ((ColorType)i,j + 1);
						SaveSuitPokerCards (_cards);
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool IsFourKind(){
		ClearSuitPokerCards ();

		for (int i = 13; i >0; i--) {
			int _number = i == 13 ? 0 : i;
			if (suit_Numbers [_number] >= 4) {
				//儲存卡片 用來知道是哪幾張牌是最後牌型
				string[] _cards = SearchNumberOfCards (_number+1);
				SaveSuitPokerCards (_cards);
				return true;
			}
		}
		return false;
	}

	private bool IsFullHourse(){
		ClearSuitPokerCards ();
		for (int i = 13; i >0; i--) {
			int _number = i == 13 ? 0 : i;

			if (suit_Numbers [_number] >= 3) {
				for (int k = 13; k > 0; k--) {
					int _number_II = k == 13 ? 0 : k;
					if (suit_Numbers [_number_II] >= 2 && i != k) {
						//儲存卡片 用來知道是哪幾張牌是最後牌型
						string[] _cards = SearchNumberOfCards (_number+1);
						SaveSuitPokerCards (_cards);
						string[] _cards_II = SearchNumberOfCards (_number_II+1);
						SaveSuitPokerCards (_cards_II);
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool IsFlush(){
		ClearSuitPokerCards ();

		for (int i = 0; i < suit_Colors.Length; i++) {
			if (suit_Colors [i] >= 5) {
				//儲存卡片 用來知道是哪幾張牌是最後牌型
				string[] _cards = SearchColorCards((ColorType)i);
				SaveSuitPokerCards (_cards);

				return true;
			}
		}
		return false;
	}

	private bool IsStraight(){
		ClearSuitPokerCards ();

		for (int i = 9; i >=0; i--) {
			int _comboCount = 0;

			for (int j=0 ; j < 5; j++) {
				int _number = (i+j == 13) ? 0 : i+j;
				if (suit_Numbers [_number] > 0) {
					_comboCount++;
				}

				if (j==4 && _comboCount >= 5) {
					//儲存卡片 用來知道是哪幾張牌是最後牌型
					string[] _cards = SearchStraightCards (i+1);
					SaveSuitPokerCards (_cards);
					return true;
				}
			}
		}
		return false;
	}

	private bool IsThreeKind(){
		ClearSuitPokerCards ();

		for (int i = 13; i >0; i--) {
			int _number = i == 13 ? 0 : i;
			if (suit_Numbers [_number] >= 3) {
				//儲存卡片 用來知道是哪幾張牌是最後牌型
				string[] _cards = SearchNumberOfCards (_number+1);
				SaveSuitPokerCards (_cards);
				return true;
			}
		}

		return false;;
	}

	private bool IsTwoPair(){
		ClearSuitPokerCards ();
		int _pairCoung = 0;

		//從A -> K -> Q ...
		for (int i = 13; i >0; i--) {
			int _number = i == 13 ? 0 : i;
			if (suit_Numbers [_number] >= 2) {
				_pairCoung++;

				//儲存卡片 用來知道是哪幾張牌是最後牌型
				string[] _cards = SearchNumberOfCards (_number+1);
				SaveSuitPokerCards (_cards);
			}
		}

		return (_pairCoung>=2);
	}

	private bool IsPair(){
		ClearSuitPokerCards ();

		//從A -> K -> Q ...
		for (int i = 13; i >0; i--) {
			int _number = i == 13 ? 0 : i;
			if (suit_Numbers [_number] >= 2) {
				//儲存卡片 用來知道是哪幾張牌是最後牌型
				string[] _cards = SearchNumberOfCards (_number+1);
				SaveSuitPokerCards (_cards);
				return true;
			}
		}

		return false;
	}

	private void ClearSuitPokerCards(){
		for(int i=0; i<suitPokerCards.Count;i++){
			suitPokerCards.Clear();
		}
	}

	private void SaveSuitPokerCards(string[] p_string){
		for(int i=0; i<p_string.Length;i++){
			suitPokerCards.Add(p_string[i]);
		}
	}

	public string GetSuitPokerCards(){
		string _cards = "";
		for (int i = 0; i < suitPokerCards.Count; i++) {
			if (i != 0) {
				_cards += ",";
			}
			_cards += suitPokerCards [i];
		}

		return _cards;
	}

	private string[] SearchNumberOfCards(int p_number){
		List<string> _cards = new List<string> ();

		for (int i = 0; i < hasPokerCards.Count; i++) {
			string[] _value = Regex.Split (hasPokerCards [i], "_");
			if (int.Parse(_value [1]) == p_number) {
				_cards.Add (hasPokerCards [i]);
			}
		}

		return _cards.ToArray ();
	}

	private string[] SearchStraightCards(int p_startCardNumber){
		List<string> _cards = new List<string> ();
		int[] _fiveCardNumber = new int[5];

		if (p_startCardNumber == 1) {
			_fiveCardNumber [0] = 1;
			for (int i = 4; i > 0; i--) {
				_fiveCardNumber [4 - i + 1] = (i + p_startCardNumber) == 14 ? 1 : (i + p_startCardNumber);
			}
		} 
		else {
			for (int i = 4; i >= 0; i--) {
				_fiveCardNumber [4 - i] = (i + p_startCardNumber) == 14 ? 1 : (i + p_startCardNumber);
			}
		}

		for (int i = 0; i < _fiveCardNumber.Length; i++) {
			for (int j = 0; j < hasPokerCards.Count; j++) {
				string[] _value = Regex.Split (hasPokerCards [j], "_");
				if (int.Parse (_value [1]) == _fiveCardNumber [i]) {
					_cards.Add (hasPokerCards [j]);
					break;
				}
			}
		}

		return _cards.ToArray ();
	}

	private string[] SearchColorCards(ColorType p_colorType){
		List<string> _cards = new List<string> ();
		for (int i = 0; i < hasPokerCards.Count; i++) {
			if (_cards.Count >= 5) {
				break;
			}

			string[] _cardValue = Regex.Split (hasPokerCards [i], "_");
			if (_cardValue [0] == p_colorType.ToString ()) {
				_cards.Add (hasPokerCards [i]);
			}
		}
		return _cards.ToArray();
	}

	private string[] SearchSTRFlushCards(ColorType p_color, int p_startCardNumber){
		List<string> _cards = new List<string> ();
		int[] _fiveCardNumber = new int[5];

		if (p_startCardNumber == 1) {
			_fiveCardNumber [0] = 1;
			for (int i = 4; i > 0; i--) {
				_fiveCardNumber [4 - i + 1] = (i + p_startCardNumber) == 14 ? 1 : (i + p_startCardNumber);
			}
		} else {
			for (int i = 4; i >= 0; i--) {
				_fiveCardNumber [4 - i] = (i + p_startCardNumber) == 14 ? 1 : (i + p_startCardNumber);
			}
		}

		for (int i = 0; i < _fiveCardNumber.Length; i++) {
			for (int j = 0; j < hasPokerCards.Count; j++) {
				string _cardvalue = ((ColorType)p_color).ToString() + "_" + _fiveCardNumber[i]; 
				if (hasPokerCards[j] == _cardvalue) {
					_cards.Add (hasPokerCards [j]);
					break;
				}
			}
		}

		return _cards.ToArray ();
	}
}
