using UnityEngine;
using System.Collections;

public class MainMenuState : BasicGameState {
	
	public override void OnStart() {
		view = MainMenuView.Create();
		AddGUIListeners();
		base.OnStart();
	}
	
	public override void OnUpdate() {
		base.OnUpdate();
	}
	
	public override void OnEnd() {
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
		(view as MainMenuView).AddSampleButtonClickListener (OnClickSample);
		(view as MainMenuView).onClickPlay += OnClickPlay;
		(view as MainMenuView).onClickSocialPage += OnClickSocial;
	}
	
	void RemoveGUIListeners() {
		(view as MainMenuView).RemoveButtonClickHandlers ();
		(view as MainMenuView).onClickPlay -= OnClickPlay;
		(view as MainMenuView).onClickSocialPage -= OnClickSocial;
	}
	
	void OnClickPlay(GameObject go) {
		Game.instance.PushState(GameStateType.IN_GAME);
	}
	
	void OnClickSocial(GameObject go) {
		Game.instance.PushState(GameStateType.SOCIAL);
	}

	void OnClickSample(GameObject go) {
		Game.instance.PushState(GameStateType.SAMPLE);
	}
}
