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
    public delegate void TouchCardHandler(CardBase p_touchCard);
    protected event TouchCardHandler TouchCardEvent;
    protected UTweenScale destoryEff;
    private BoxCollider2D boxCollider2D;
    private float touchTime = 0;
    private enum TouchType { Idle, Down, Up};
    private TouchType touchType = TouchType.Idle;

    void Awake() {
        thisTransform = this.transform;
        destoryEff = this.GetComponent<UTweenScale>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    void Start() {

    }

    void Update() {
        if(touchType == TouchType.Down) {
            touchTime += Time.deltaTime;
            if(touchTime > 2) {
                touchType = TouchType.Idle;
                touchTime = 0;
                OnMouseLongDown();
            }
        }
        M_Update();
    }

    protected virtual void M_Update() {

    }

    protected virtual void M_Init() {

    }

    public string GetCardID() {
        return cardID;
    }

    private void OnMouseUp() {
        if(touchType != TouchType.Down) {
            return;
        }

        touchType = TouchType.Idle;
        TouchCardEvent.Invoke(this);
    }


    private void OnMouseDown() {
        touchTime = 0;
        touchType = TouchType.Down;
    }

    private void OnMouseLongDown() {
        print("【卡片】 長按 ");
    }

    public void Init(string p_cardID, int p_positionIndex, TouchCardHandler p_onTouchCardEvent) {
        cardID = p_cardID;
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