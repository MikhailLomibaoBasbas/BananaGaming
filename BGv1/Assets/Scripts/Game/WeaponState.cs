using UnityEngine;
using System.Collections;

public class WeaponState : BasicGameState {

	private WeaponView weaponView;
	public override void OnStart() {
		viewUI = WeaponView.Create();
		weaponView = (WeaponView)viewUI;

		//static_audiomanager.getInstance.play_bgm ("Audio/Bgm/MainMenu");
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
		(viewUI as WeaponView).onClickPlay += OnClickPlay;
		(viewUI as WeaponView).onClickBack += OnClickBack;
	}

	void RemoveGUIListeners() {
		(viewUI as WeaponView).RemoveButtonClickHandlers ();
		(viewUI as WeaponView).onClickPlay -= OnClickPlay;
		(viewUI as WeaponView).onClickBack -= OnClickBack;
	}

	void OnClickPlay(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		Game.instance.PushState(GameStateType.GAME);
	}

	void OnClickBack(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		Game.instance.PushState(GameStateType.MAIN_MENU);
	}
}
