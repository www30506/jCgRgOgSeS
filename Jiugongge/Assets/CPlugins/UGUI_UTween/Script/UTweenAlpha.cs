using UnityEngine;
using System.Collections;


[AddComponentMenu("UI/Tween/TweenAlpha")]
public class UTweenAlpha : UTweener {
	[Range(0f,1f)]
	public float Form;
	[Range(0f,1f)]
	public float To;
	protected translateDelegate[] translateType = new translateDelegate[3];
	private float distanceVector, tempAlpha;
	private CanvasRenderer crd;
	private SpriteRenderer spriteRenderer;

	void Start(){
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
		switch(type){
		case UseType.UGUI:
			crd.SetAlpha (p_Alpha);
			break;
		case UseType.Sprite2D:
			Color _color = spriteRenderer.color;
			_color.a = p_Alpha;
			spriteRenderer.color = _color;
			break;
		}
	}
}
