using UnityEngine;
using System.Collections;

public class PauseState : BasicGameState {

	private PauseView pauseView;
	public override void OnStart() {
		viewUI = PauseView.Create();
		pauseView = (PauseView)viewUI;
		fade_animation.begin (pauseView.gameObject, FadeType.FadeIn, Color.clear, 0.2f, null, null, null, true);
		//static_audiomanager.getInstance.play_bgm ("Audio/Bgm/MainMenu");
		AddGUIListeners();
		base.OnStart();
	}

	public override void OnUpdate() {
		base.OnUpdate();
	}

	public override void OnEnd() {
		fade_animation.begin (pauseView.gameObject, FadeType.FadeOut, Color.clear, 0.2f, null, null, null, true);
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
		pauseView.onClickResume += OnClickResume;
		pauseView.onClickShop += OnClickShop;
		pauseView.onClickMain += OnClickMain;
	}

	void RemoveGUIListeners() {
		pauseView.RemoveButtonClickHandlers ();
		pauseView.onClickResume -= OnClickResume;
		pauseView.onClickMain -= OnClickShop;
		pauseView.onClickMain -= OnClickMain;

	}

	void OnClickResume(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", pauseView.transform.position);
		stateManager.PopState ();
	}

	void OnClickShop(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", pauseView.transform.position);
		stateManager.PopState ();
		Game.instance.PushState(GameStateType.SHOP);
	}

	void OnClickMain(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", pauseView.transform.position);
		stateManager.PopState ();	
		Game.instance.PushState(GameStateType.MAIN_MENU);
	}
}
