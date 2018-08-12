using UnityEngine;
using System.Collections;

[AddComponentMenu("UI/Tween/TweenRotation")]
public class UTweenRotation : UTweener {
	protected translateDelegate[] RotateType = new translateDelegate[3];
	public Vector3 Form = new Vector3(1,1,1), To = new Vector3(1,1,1);
	protected Vector3 distanceVector, tempVector;
	public float startDelayTime = 0.0f;

	void Start(){
		RotateType [0] = new translateDelegate(Once);
		RotateType [1] = new translateDelegate(Loop);
		RotateType [2] = new translateDelegate(PingPong);
	}

	void OnEnable(){
		distanceVector = To - Form;
	}

	void LateUpdate () {
		if (startDelayTime > 0) {
			startDelayTime -= Time.deltaTime;
			return;
		}

		if (start) {
			Rotation ();
		}
	}
	
	private void Rotation(){
		time += ignoreTimeScale? Time.unscaledDeltaTime : Time.deltaTime;
		RotateType [(int)loopType]();
	}
	
	public delegate void translateDelegate();
	public void Once(){
		tempVector = distanceVector * Curve.Evaluate(time*percent);
		tempVector = Form + tempVector;
		SetRotation(tempVector);
		
		if (time > Duration) {
			start = false;
			tempVector = distanceVector * Curve.Evaluate(1);
			tempVector = Form + tempVector;
			SetRotation(tempVector);
			OnFinished();
			this.enabled = false;
		}
	}
	
	public void Loop(){
		tempVector = distanceVector * Curve.Evaluate (time * percent);
		tempVector = Form + tempVector;

		SetRotation(tempVector);
		
		if (time > Duration) {
			time = 0;
		}
	}
	
	public void PingPong(){
		if (pingpong) 
			tempVector = distanceVector * Curve.Evaluate ((Duration - time) * percent);
		else 
			tempVector = distanceVector * Curve.Evaluate (time * percent);
		
		tempVector = Form + tempVector;
		
		SetRotation(tempVector);

		if (time > Duration) {
			time = 0;
			pingpong = !pingpong;
		}
	}

	private void SetRotation(Vector3 p_vector3){
		switch(type){
		case UseType.UGUI:
			myRectTransfrom.localEulerAngles = p_vector3;
			break;
		case UseType.Sprite2D:
			myTransform.localEulerAngles = p_vector3;
			break;
		}
	}
}
