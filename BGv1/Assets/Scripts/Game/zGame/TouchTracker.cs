using UnityEngine;
using System.Collections;

public class TouchTracker {

	public int fingerId;
	public Touch selfTouch;
	public bool isDirty = false;
	private float totalTime = 0.0f;
	public Touch firstTouch;

	public TouchTracker(Touch touch){
		this.fingerId = firstTouch.fingerId;
		this.firstTouch = firstTouch;

		Begin ();
		Update (firstTouch);
	}


	public void Update(Touch touch){
		this.selfTouch = touch;
		isDirty = true;

		totalTime += Time.deltaTime;
	}

	public void Clean(){
		isDirty = false;
	}

	public void Begin(){
		Debug.Log ("Begin");
	}

	public void End(){
		Debug.Log ("End");
	}
}
