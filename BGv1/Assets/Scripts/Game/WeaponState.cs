using UnityEngine;
using System.Collections;

public class WeaponState : BasicGameState {

	private WeaponView weaponView;
	public override void OnStart() {
		viewUI = WeaponView.Create();
		weaponView = (WeaponView)viewUI;
		fade_animation.begin (weaponView.gameObject, FadeType.FadeIn, Color.clear, 0.2f, null, null, null, true);
		//static_audiomanager.getInstance.play_bgm ("Audio/Bgm/MainMenu");
		AddGUIListeners();
		base.OnStart();
	}

	public override void OnUpdate() {
		base.OnUpdate();
	}

	public override void OnEnd() {
		fade_animation.begin (weaponView.gameObject, FadeType.FadeOut, Color.clear, 0.2f, null, null, null, true);
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
		weaponView.onClickPlay += OnClickPlay;
		weaponView.onClickBack += OnClickBack;
		weaponView.onClickHand += OnClickHand;
		weaponView.onClickShot += OnClickShot;
		weaponView.onClickAk += OnClickAk;
	}

	void RemoveGUIListeners() {
		weaponView.RemoveButtonClickHandlers ();
		weaponView.onClickPlay -= OnClickPlay;
		weaponView.onClickBack -= OnClickBack;
		weaponView.onClickHand -= OnClickHand;
		weaponView.onClickShot -= OnClickShot;
		weaponView.onClickAk -= OnClickAk;
	}

	void OnClickPlay(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		Game.instance.PushState(GameStateType.GAME);
	}

	void OnClickBack(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		Game.instance.PushState(GameStateType.MAIN_MENU);
	}

	void OnClickHand(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		Game.instance.weaponInitial = Weapon.WeaponType.Pistol;
		weaponView.EquipButton (go);
	}

	void OnClickShot(GameObject go) {
		Game.instance.weaponInitial = Weapon.WeaponType.Shotgun;
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		weaponView.EquipButton (go);
	}

	void OnClickAk(GameObject go) {
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", weaponView.transform.position);
		Game.instance.weaponInitial = Weapon.WeaponType.AK47;
		weaponView.EquipButton (go);
	}
}
