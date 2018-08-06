using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {
	[Header("Data")]
	[SerializeField]private int[] completeTargets;
	[SerializeField]private string[] initCardsID;
	[Header("====")]
	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<Card> cards;
	private Queue<Card> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;
	[SerializeField]private GameView gameView;
	private enum SystemStatue{Idle, Working};
	private SystemStatue systemStatue = SystemStatue.Idle;


	void Start () {
		LoadLevelData();
		CreateCardPool();
		CreateCards();
		CreateCompleteTarget ();
	}

	private void CreateCompleteTarget(){
		completeTargets = new int[3];
		List<int> _range = new List<int> ();
		for (int i = 1; i < 100; i++) {
			_range.Add (i);
		}

		for (int i = 0; i < completeTargets.Length; i++) {

			int _index = Random.Range (0, _range.Count);
			completeTargets [i] = _range[_index];
			_range.RemoveAt (_index);

		}

		gameView.InitCompleteTarget (completeTargets);
	}

	private void LoadLevelData(){
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

	private void DestoryCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				cards[i].DestoryEff();
				cardsPool.Enqueue(cards[i]);
				cards[i].transform.SetParent(cardPoolTransform, false);
			}
		}
	}

	private Card GetCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				return cards[i];
			}
		}
		Debug.LogError("錯誤 沒有取得卡片 位置編號 : " + p_positionIndex);
		return null;
	}

	private void CreateCard(string p_CardID, int p_positionIndex){
		Card _card = cardsPool.Dequeue();
		_card.Init(p_CardID, p_positionIndex);
		_card.CreateEff();
		_card.transform.SetParent(cardListTransform, false);
	}

	private void CreateCards(){
		for(int i=0; i< 9; i++){
			CreateCard(initCardsID[i], i);
		}
	}

	void Update () {
		
	}

	public void BackToMenu(){
		Game.LoadScene ("MenuScene");
	}
}
