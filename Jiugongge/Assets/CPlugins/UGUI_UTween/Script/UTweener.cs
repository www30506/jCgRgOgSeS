using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UTweener : MonoBehaviour {
	protected enum UseType{Sprite2D, UGUI, Object_3D};
	protected UseType type;
	public enum LoopType {Once,Loop,PingPong};
	public LoopType loopType = LoopType.Once;
	public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public float Duration = 1.0f;
	public bool ignoreTimeScale =false;
	public UnityEvent OnFinishedEvent;

	protected RectTransform myRectTransfrom;
	protected Transform myTransform;
	protected float time, percent;
	protected bool start = true, pingpong = false;

	void Awake(){
		myRectTransfrom = this.GetComponent<RectTransform> ();
		if(myRectTransfrom ==null){
			type = UseType.Sprite2D;
			myTransform = this.GetComponent<Transform>();
		}
		else{
			type = UseType.UGUI;
		}
		percent = 1.0f / Duration;
		if (Duration <= 0)	start = false;
	}


	public void OnFinished(){
		OnFinishedEvent.Invoke ();
	}

	void OnDisable(){
		ReSetToStart();
	}

	public void ReSetToStart(){
		start = true;
		time = 0.0f;
		pingpong = false;
		if (Duration <= 0)	start = false;
	}
}
