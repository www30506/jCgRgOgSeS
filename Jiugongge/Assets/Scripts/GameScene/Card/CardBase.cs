using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardBase : MonoBehaviour {
    [SerializeField] protected SpriteRenderer frame;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected int positionIndex;
    [SerializeField] protected string cardID;
    [SerializeField] protected CardType m_type;
    [SerializeField] protected TextMesh m_valueTextMesh;

    protected Transform thisTransform;
    public delegate void TouchCardHandler(int p_touchCardPositionIndex);
    protected event TouchCardHandler TouchCardEvent;
    protected UTweenScale destoryEff;
    private BoxCollider2D boxCollider2D;

    void Awake() {
        thisTransform = this.transform;
        destoryEff = this.GetComponent<UTweenScale>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    void Start() {

    }

    void Update() {
        
    }

    protected virtual void M_Update() {

    }

    protected virtual void M_Init() {

    }

    public string GetCardID() {
        return cardID;
    }

    public void OnMouseUp() {
        print("【點擊卡片】類型 ： " + m_type.ToString() + "  , cardID : " + cardID + "  , PositionIndex : " + positionIndex);
        TouchCardEvent.Invoke(positionIndex);
    }


    public void OnMouseDown() {
        //print("【卡片按下】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
    }

    public void Init(string p_cardID, int p_positionIndex, TouchCardHandler p_onTouchCardEvent) {
        cardID = p_cardID;

        //m_name = PD.DATA ["CardTable"] [p_cardID] ["Name"].ToString();

        //改變圖片現在沒用到
        //ChangeSprite(p_cardID);
        SetCardSize();
        SetPosition(p_positionIndex);

#if UNITY_EDITOR
        this.gameObject.name = p_cardID;
#endif

        TouchCardEvent += p_onTouchCardEvent;
        M_Init();
    }

    private void SetCardSize() {
        //標準是3 * 3 
        float _x = 3 / (float)GameData.Row;
        float _y = 3 / (float)GameData.column;

        this.transform.localScale = new Vector3(_x, _y, 1);
    }

    public void ResetCard(string p_cardID) {
        cardID = p_cardID;

        //改變圖片現在沒用到
        //ChangeSprite(p_cardID);

    }

    public void ResetPositionIndex(int p_positionIndex) {
        SetPosition(p_positionIndex);
    }

    protected void ChangeSprite(string p_cardID) {
        sprite.sprite = Resources.Load<Sprite>("Textures/Cards/" + p_cardID);
    }

    protected void SetPosition(int p_positionIndex) {
        positionIndex = p_positionIndex;

        float _x = -3 + this.transform.localScale.x + ((p_positionIndex % GameData.Row) * this.transform.localScale.x * 2);
        float _y = 4 - this.transform.localScale.y - ((p_positionIndex / GameData.Row) * this.transform.localScale.y * 3);

        thisTransform.localPosition = new Vector3(_x, _y, 1);
    }

    public IEnumerator IE_SetPosition(int p_positionIndex) {
        positionIndex = p_positionIndex;

        Vector3 _newPosition = Vector3.zero;
        switch (p_positionIndex) {
            case 0:
            _newPosition = new Vector3(-2, 2, 0);
            break;
            case 1:
            _newPosition = new Vector3(0, 2, 0);
            break;
            case 2:
            _newPosition = new Vector3(2, 2, 0);
            break;
            case 3:
            _newPosition = new Vector3(-2, 0, 0);
            break;
            case 4:
            _newPosition = new Vector3(0, 0, 0);
            break;
            case 5:
            _newPosition = new Vector3(2, 0, 0);
            break;
            case 6:
            _newPosition = new Vector3(-2, -2, 0);
            break;
            case 7:
            _newPosition = new Vector3(0, -2, 0);
            break;
            case 8:
            _newPosition = new Vector3(2, -2, 0);
            break;
        }

        Vector3 _prePosition = thisTransform.localPosition;
        Vector3 _distance = _newPosition - _prePosition;
        float _tempTime = 0;

        while (_tempTime < 0.3f) {
            thisTransform.localPosition = _prePosition + (_distance * _tempTime / 0.3f);
            _tempTime += Time.deltaTime;
            yield return null;
        }

        thisTransform.localPosition = _newPosition;
    }

    public int GetPositionIndex() {
        return positionIndex;
    }

    public CardType GetCardType() {
        return m_type;
    }

    public IEnumerator Destory() {
        print("【卡片消滅特效】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
        TouchCardEvent = null;
        yield return StartCoroutine(IE_DestoryEff());
    }

    IEnumerator IE_DestoryEff() {
        destoryEff.enabled = true;
        destoryEff.ReSetToStart();
        yield return new WaitForSeconds(destoryEff.Duration);
    }


    public IEnumerator CreateEff() {
        print("【卡片生成特效】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
        this.transform.localScale = Vector3.one;
        yield return null;
    }
}