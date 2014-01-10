using UnityEngine;
using System.Collections;

public class GameOverState : BasicGameState {

	private GameOverView gameOverView;
	public override void OnStart() {
		viewUI = GameOverView.Create();
		gameOverView = (GameOverView)viewUI;
		fade_animation.begin (gameOverView.gameObject, FadeType.FadeIn, Color.clear, 0.2f, null, null, null, true);
		AudioManager.GetInstance.PlayKhailSpecial ();
		//static_audiomanager.getInstance.play_bgm ("Audio/Bgm/MainMenu");
		AddGUIListeners();
		base.OnStart();
	}

	public override void OnUpdate() {
		base.OnUpdate();
	}

	public override void OnEnd() {
		fade_animation.begin (gameOverView.gameObject, FadeType.FadeOut, Color.clear, 0.2f, null, null, null, true);
		AudioManager.GetInstance.StopBGMAll ();
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
		gameOverView.onClickResume += OnClickResume;
		gameOverView.onClickShop += OnClickShop;
		gameOverView.onClickMain += OnClickMain;
	}

	void RemoveGUIListeners() {
		gameOverView.RemoveButtonClickHandlers ();
		gameOverView.onClickResume -= OnClickResume;
		gameOverView.onClickMain -= OnClickShop;
		gameOverView.onClickMain -= OnClickMain;

	}

	void OnClickResume(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", gameOverView.transform.position);
		stateManager.PopState ();
		Game.instance.PushState(GameStateType.GAME);
	}

	void OnClickShop(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", gameOverView.transform.position);
		stateManager.PopState ();
		Game.instance.PushState(GameStateType.SHOP);
	}

	void OnClickMain(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", gameOverView.transform.position);
		stateManager.PopState ();	
		Game.instance.PushState(GameStateType.MAIN_MENU);
	}
}
