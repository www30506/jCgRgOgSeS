using UnityEngine;
using System.Collections;

[AddComponentMenu("UI/Tween/TweenColor")]
public class UTweenColor : UTweener {
	public Color Form = new Color(1,1,1,1);
	public Color To = new Color(0,0,0,1);
	protected translateDelegate[] translateType = new translateDelegate[3];
	private Color distanceVector, tempColor;
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

	void OnEnable(){
		distanceVector = To - Form;
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
		tempColor = distanceVector * Curve.Evaluate(time * percent);
		tempColor = Form + tempColor;

		SetColor(tempColor) ;

		if (time > Duration) {
			start = false;
			tempColor = distanceVector * Curve.Evaluate(1);
			tempColor = Form + tempColor;
			SetColor(tempColor);
			OnFinished();
			this.enabled = false;
		}
	}
	
	public void Loop(){
		tempColor = distanceVector * Curve.Evaluate (time * percent);
		tempColor = Form + tempColor;

		SetColor(tempColor);
		
		if (time > Duration) {
			time = 0;
		}
	}
	
	public void PingPong(){
		if (pingpong) 
			tempColor = distanceVector * Curve.Evaluate ((Duration - time) * percent);
		else 
			tempColor = distanceVector * Curve.Evaluate (time * percent);
		
		tempColor = Form + tempColor;
		
		SetColor (tempColor);

		if (time > Duration) {
			time = 0;
			pingpong = !pingpong;
		}
	}

	private void SetColor(Color p_color){
		switch(type){
		case UseType.UGUI:
			crd.SetColor (tempColor);
			break;
		case UseType.Sprite2D:
			spriteRenderer.color = p_color;
			break;
		}
	}
}
