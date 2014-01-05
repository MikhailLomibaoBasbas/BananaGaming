using UnityEngine;
using System;

public class MainMenuView : BasicView
{
	private const string MAIN_MENU_PANEL = "Prefabs/GUI/MainMenuPanel";
	public static MainMenuView Create() {
		return Create<MainMenuView>(MAIN_MENU_PANEL);
	}
	
	public UIEventListener.VoidDelegate onClickPlay = null;
	public UIEventListener.VoidDelegate onClickMusic = null;
	public UIEventListener.VoidDelegate onClickSound = null;

	private const string BTN_PLAY 		= "Anchor/BtnPlay";
	private const string BTN_MUSIC 	= "Anchor/BtnMusic";
	private const string BTN_SOUND = "Anchor/BtnSound";

	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_PLAY)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_MUSIC)).onClick += OnClickMusic;
		UIEventListener.Get(GetChild(BTN_SOUND)).onClick += OnClickSound;
	}
	
	void OnClickPlay(GameObject go) {
		if(onClickPlay != null)
			onClickPlay(go);
	}
	
	void OnClickMusic(GameObject go) {
		if(onClickMusic != null)
			onClickMusic(go);
	}

	void OnClickSound(GameObject go) {
		if(onClickSound != null)
			onClickSound(go);
	}

	public void SetMusicOn(bool isOn){
		if (isOn)
			GetChild (BTN_MUSIC).transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "on";
		else
			GetChild (BTN_MUSIC).transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "off";
	}

	public void SetSoundOn(bool isOn){
		if (isOn)
			GetChild (BTN_SOUND).transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "on";
		else
			GetChild (BTN_SOUND).transform.FindChild ("Background").GetComponent<UISprite> ().spriteName = "off";
	}

}