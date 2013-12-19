using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Basic2DView : BasicView {

	public static T Create<T>(string resourceName) where T: Component {
		return Create<T>(resourceName, Game.instance.game2DRoot);
	}

	public static T Create<T>(string resourceName, GameObject parent) where T: Component {
		return Create<T>(resourceName, parent);
	}

	private static Camera _camera = null;
	public static Camera getCamera {
		get {
			if(_camera == null) {
				_camera = Game.instance.game2DRoot.transform.FindChild("Camera").camera;
				_camera.orthographicSize = (Screen.height / 2 )/ Game.instance.defaultPixelToUnits;
			}
			return _camera;
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
