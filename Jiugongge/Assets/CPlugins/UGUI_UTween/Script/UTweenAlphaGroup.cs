using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("UI/Tween/TweenAlphaGroup")]
public class UTweenAlphaGroup : UTweener {
	[Range(0f,1f)]
	public float Form;
	[Range(0f,1f)]
	public float To;
	protected translateDelegate[] translateType = new translateDelegate[3];
	private float distanceVector, tempAlpha;
	private CanvasRenderer crd;
	private SpriteRenderer spriteRenderer;
	private CanvasGroup canvasGroup;

	void Start(){
		canvasGroup = this.GetComponent<CanvasGroup>();
		translateType [0] = new translateDelegate(Once);
		translateType [1] = new translateDelegate(Loop);
		translateType [2] = new translateDelegate(PingPong);
		distanceVector = To - Form;

		if(type == UseType.Sprite2D){
			spriteRenderer = this.GetComponent<SpriteRenderer>();
		}
		else if (type == UseType.UGUI){
			crd = this.GetComponent<CanvasRenderer> ();
		}
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
		tempAlpha = distanceVector * Curve.Evaluate(time * percent);
		tempAlpha = Form + tempAlpha;

		SetAlpha(tempAlpha) ;

		if (time > Duration) {
			start = false;
			tempAlpha = distanceVector * Curve.Evaluate(1);
			tempAlpha = Form + tempAlpha;
			SetAlpha(tempAlpha);
			OnFinished();
			this.enabled = false;
		}
	}

	public void Loop(){
		tempAlpha = distanceVector * Curve.Evaluate (time * percent);
		tempAlpha = Form + tempAlpha;

		SetAlpha(tempAlpha) ;

		if (time > Duration) {
			time = 0;
		}
	}

	public void PingPong(){
		if (pingpong) 
			tempAlpha = distanceVector * Curve.Evaluate ((Duration - time) * percent);
		else 
			tempAlpha = distanceVector * Curve.Evaluate (time * percent);

		tempAlpha = Form + tempAlpha;

		SetAlpha(tempAlpha);

		if (time > Duration) {
			time = 0;
			pingpong = !pingpong;
		}
	}

	private void SetAlpha(float p_Alpha){
		canvasGroup.alpha = p_Alpha;
	}
}
