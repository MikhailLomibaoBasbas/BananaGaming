using UnityEngine;
using System.Collections;

public class WeaponView : BasicView {

	private const string WEAPON_PANEL = "Prefabs/GUI/WeaponPanel";
	public static WeaponView Create() {
		return Create<WeaponView>(WEAPON_PANEL);
	}

	public UIEventListener.VoidDelegate onClickPlay = null;
	public UIEventListener.VoidDelegate onClickBack = null;

	private const string BTN_PLAY 		= "AnchorBottomRight/btnPlay";
	private const string BTN_BACK 	= "AnchorBottomLeft/btnBack";
	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_PLAY)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_BACK)).onClick += OnClickBack;

	}

	void OnClickPlay(GameObject go) {
		if(onClickPlay != null)
			onClickPlay(go);
	}

	void OnClickBack(GameObject go) {
		if(onClickBack != null)
			onClickBack(go);
	}
	
}
