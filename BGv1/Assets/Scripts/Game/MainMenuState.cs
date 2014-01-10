using UnityEngine;
using System.Collections;

public class MainMenuState : BasicGameState {

	private MainMenuView mMenuView;
	public override void OnStart() {
		viewUI = MainMenuView.Create();
		mMenuView = (MainMenuView)viewUI;
		fade_animation.begin (mMenuView.gameObject, FadeType.FadeIn, Color.clear, 0.2f, null, null, null, true);
		static_audiomanager.getInstance.play_bgm ("Audio/Bgm/MainMenu");
		AddGUIListeners();
		base.OnStart();
	}
	
	public override void OnUpdate() {
		base.OnUpdate();
	}
	
	public override void OnEnd() {
		fade_animation.begin (mMenuView.gameObject, FadeType.FadeOut, Color.clear, 0.2f, null, null, null, true);
		RemoveGUIListeners();
		base.OnEnd();
	}

	public override void OnPause () {
		base.OnPause ();
	}

	public override void OnResume () {
		base.OnResume ();

	}
	
	void AddGUIListeners() {
		(viewUI as MainMenuView).onClickPlay += OnClickPlay;
		(viewUI as MainMenuView).onClickMusic += OnClickMusic;
		(viewUI as MainMenuView).onClickSound += OnClickSound;
	}
	
	void RemoveGUIListeners() {
		(viewUI as MainMenuView).RemoveButtonClickHandlers ();
		(viewUI as MainMenuView).onClickPlay -= OnClickPlay;
		(viewUI as MainMenuView).onClickMusic -= OnClickMusic;
		(viewUI as MainMenuView).onClickSound -= OnClickSound;
	}
	
	void OnClickPlay(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", mMenuView.transform.position);
		Game.instance.PushState(GameStateType.SHOP);
	}
	
	void OnClickMusic(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", mMenuView.transform.position);
		if (OptionData.getInstance.BGMON != 1)
			mMenuView.SetMusicOn (true);
		else
			mMenuView.SetMusicOn (false);
		OptionData.getInstance.BGMON = (OptionData.getInstance.BGMON != 1) ? 1 : 0;
		//Game.instance.PushState(GameStateType.SOCIAL);
	}

	void OnClickSound(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", mMenuView.transform.position);
		if (OptionData.getInstance.SFXON != 1)
			mMenuView.SetSoundOn (true);
		else
			mMenuView.SetSoundOn (false);
		OptionData.getInstance.SFXON = (OptionData.getInstance.SFXON != 1) ? 1 : 0;
		//Game.instance.PushState(GameStateType.SOCIAL);
	}
}
