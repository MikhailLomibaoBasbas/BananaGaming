using UnityEngine;
using System.Collections;

public class MainMenuState : BasicGameState {
	
	public override void OnStart() {
		viewUI = MainMenuView.Create();
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
		(viewUI as MainMenuView).onClickPlay += OnClickPlay;
				(viewUI as MainMenuView).onClickOption += OnClickOption;
	}
	
	void RemoveGUIListeners() {
		(viewUI as MainMenuView).RemoveButtonClickHandlers ();
		(viewUI as MainMenuView).onClickPlay -= OnClickPlay;
				(viewUI as MainMenuView).onClickOption -= OnClickOption;
	}
	
	void OnClickPlay(GameObject go) {
		Game.instance.PushState(GameStateType.GAME);
	}
	
	void OnClickOption(GameObject go) {
		//Game.instance.PushState(GameStateType.SOCIAL);
	}
}
