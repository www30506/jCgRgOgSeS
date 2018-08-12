using System.Collections;
using UnityEngine;

[AddComponentMenu("UI/Tween/TweenSize")]
public class UTweenSize : UTweener {
	public Vector2 Form;
	public Vector2 To;
	protected translateDelegate[] translateType = new translateDelegate[3];
	private Vector2 distanceSize, tempSize;
	private RectTransform rectTransform;
	private SpriteRenderer spriteRenderer;

	void Start(){
		translateType [0] = new translateDelegate(Once);
		translateType [1] = new translateDelegate(Loop);
		translateType [2] = new translateDelegate(PingPong);
		distanceSize = To - Form;
	}

	void LateUpdate () {
		if (start) {
			Translate ();
		}
	}

	private void Translate(){
		time += ignoreTimeScale? Time.unscaledDeltaTime : Time.deltaTime;
		translateType [(int)loopType]();
	}

	public delegate void translateDelegate();
	public void Once(){
		tempSize = distanceSize * Curve.Evaluate(time * percent);
		tempSize = Form + tempSize;

		SetSize(tempSize) ;

		if (time > Duration) {
			start = false;
			tempSize = distanceSize * Curve.Evaluate(1);
			tempSize = Form + tempSize;
			SetSize(tempSize);
			OnFinished();
			this.enabled = false;
		}
	}

	public void Loop(){
		tempSize = distanceSize * Curve.Evaluate (time * percent);
		tempSize = Form + tempSize;

		SetSize(tempSize) ;

		if (time > Duration) {
			time = 0;
		}
	}

	public void PingPong(){
		if (pingpong) 
			tempSize = distanceSize * Curve.Evaluate ((Duration - time) * percent);
		else 
			tempSize = distanceSize * Curve.Evaluate (time * percent);

		tempSize = Form + tempSize;

		SetSize(tempSize);

		if (time > Duration) {
			time = 0;
			pingpong = !pingpong;
		}
	}

	private void SetSize(Vector2 p_size){
		rectTransform.sizeDelta = p_size;
	}
}
