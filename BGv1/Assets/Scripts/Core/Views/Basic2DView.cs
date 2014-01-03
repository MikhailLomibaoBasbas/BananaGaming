using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Basic2DView : BasicView {

	public static T Create<T>(string resourcePath) where T: Component {
		return Create<T>(resourcePath, getGame2DRoot);
	}
	public static GameObject getGame2DRoot {get{return Game.instance.game2DRoot;}}

	private static Camera _2Dcamera = null;
	public static Camera get2DCamera {
		get {
			if(_2Dcamera == null) {
				_2Dcamera = Game.instance.game2DRoot.transform.FindChild("Camera").camera;
				if(Application.platform == RuntimePlatform.IPhonePlayer) {
					_2Dcamera.orthographicSize = (Screen.height / 2 )/ Game.instance.defaultPixelToUnits;
				} else {
					_2Dcamera.orthographicSize = (640 / 2 )/ Game.instance.defaultPixelToUnits;
				}
			}
			return _2Dcamera;
		}
	}

	string GetCannotUseMessage(string functionName) {
		return functionName + " Method in Basic2DView does nothing, " +
			"this class's sole purpose are for Non-UI 2D.";
	}

	protected override void AddButtonClickListener (string childName, UIEventListener.VoidDelegate listener) {
		Debug.LogWarning(GetCannotUseMessage("AddButtonClickListener"));
		return;
	}
	public override void RemoveButtonClickHandlers () {
		Debug.LogWarning(GetCannotUseMessage("RemoveButtonClickHandlers"));
		return;
	}

	public override void Show (bool animated) {
		base.Show (animated);
	}
	public override void Hide (bool animated) {
		base.Hide (animated);
	}
	public override void Close (bool animated) {
		base.Close (animated);
	}

	void FadeAnimation (bool transitionIn) {

	}
}
