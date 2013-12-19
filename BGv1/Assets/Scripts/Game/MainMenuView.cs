using UnityEngine;
using System;

public class MainMenuView : BasicView
{
	private const string MAIN_MENU_PANEL = "Prefabs/GUI/MainMenuPanel";
	public static MainMenuView Create() {
		return Create<MainMenuView>(MAIN_MENU_PANEL);
	}
	
	public UIEventListener.VoidDelegate onClickPlay = null;
	public UIEventListener.VoidDelegate onClickSocialPage = null;
	public UIEventListener.VoidDelegate onClickShopPage = null;
	public UIEventListener.VoidDelegate onClickGameCenter = null;
	
	private const string BTN_PLAY 		= "Anchor/BtnPlay";
	private const string BTN_SOCIAL 	= "Anchor/BtnSocial";
	private const string BTN_SHOP 		= "Anchor/BtnShop";
	private const string BTN_GAMECENTER = "Anchor/BtnGameCenter";
	private const string BTN_SAMPLE = "Anchor/BtnSample";
	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_PLAY)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_SOCIAL)).onClick += OnClickSocial;
		UIEventListener.Get(GetChild(BTN_SHOP)).onClick += OnClickShop;
		UIEventListener.Get(GetChild(BTN_GAMECENTER)).onClick += OnClickGameCenter;
	}

	public void AddSampleButtonClickListener(UIEventListener.VoidDelegate listener) {
		AddButtonClickListener (BTN_SAMPLE, listener);
	}
	
	void OnClickPlay(GameObject go) {
		if(onClickPlay != null)
			onClickPlay(go);
	}
	
	void OnClickSocial(GameObject go) {
		if(onClickSocialPage != null)
			onClickSocialPage(go);
	}
	
	void OnClickShop(GameObject go) {
		if(onClickShopPage != null)
			onClickShopPage(go);
	}
	
	void OnClickGameCenter(GameObject go) {
		if(onClickGameCenter != null)
			onClickGameCenter(go);
	}
}