using UnityEngine;
using System.Collections;

public class GameOverView : BasicView {

	private const string PAUSE_PANEL = "Prefabs/GUI/GameOverPanel";
	public static GameOverView Create() {
		return Create<GameOverView>(PAUSE_PANEL);
	}

	public UIEventListener.VoidDelegate onClickResume = null;
	public UIEventListener.VoidDelegate onClickShop = null;	
	public UIEventListener.VoidDelegate onClickMain = null;

	private const string BTN_RESUME 		= "Anchor/BtnResume";
	private const string BTN_SHOP 	= "Anchor/BtnShop";
	private const string BTN_MAIN 	= "Anchor/BtnMain";

	void Awake() {
		UIEventListener.Get(GetChild(BTN_RESUME)).onClick += OnClickResume;
		UIEventListener.Get(GetChild(BTN_SHOP)).onClick += OnClickShop;
		UIEventListener.Get(GetChild(BTN_MAIN)).onClick += OnClickMain;

	}

	void OnClickResume(GameObject go) {
		if(onClickResume != null)
			onClickResume(go);
	}

	void OnClickShop(GameObject go) {
		if(onClickShop != null)
			onClickShop(go);
	}

	void OnClickMain(GameObject go) {
		if(onClickMain != null)
			onClickMain(go);
	}
}
