using UnityEngine;
using System;

public class InGameView : BasicView
{
	private const string IN_GAME_PANEL = "Prefabs/GUI/GamePanel";

	private const string BTN_SHOOT = "AnchorBottomRight/btnShoot";
	private const string BTN_ATTACK = "AnchorBottomRight/btnAttack";
	private const string BTN_NAV= "AnchorBottomleft/btnNav";
	private const string BTN_SWITCH = "AnchorBottomRight/btnSwitch";
	private const string SLIDER_PLAYERHEALTH = "AnchorTopLeft/PlayerStatus/PlayerHealth";
	private const string SLIDER_TOWERHEALTH = "AnchorTopRight/TowerStatus/TowerHealth";

	private const string LBL_COIN = "AnchorTop/lblCoin";

	public UIEventListener.VoidDelegate onClickShoot = null;
	public UIEventListener.VoidDelegate onClickAttack = null;
	public UIEventListener.VoidDelegate onClickSwitch = null;


	protected UISlider healthSlider;
	protected UISlider towerSlider;
	protected UILabel lblCoin;

	public static InGameView Create() {
		return Create<InGameView>(IN_GAME_PANEL);
	}
	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_SHOOT)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_ATTACK)).onClick += OnClickAttack;
		UIEventListener.Get(GetChild(BTN_SWITCH)).onClick += OnClickSwitch;

		healthSlider = GetChild (SLIDER_PLAYERHEALTH).GetComponent<UISlider> ();
		towerSlider = GetChild (SLIDER_TOWERHEALTH).GetComponent<UISlider> ();
		lblCoin = GetChild (LBL_COIN).GetComponent<UILabel> ();
	}

	void OnClickPlay(GameObject go) {
		if(onClickShoot != null)
			onClickShoot(go);
	}

	void OnClickAttack(GameObject go) {
		if(onClickAttack != null)
			onClickAttack(go);
	}

	void OnClickSwitch(GameObject go) {
		if(onClickSwitch != null)
			onClickSwitch(go);
	}

	public void setPlayerHealth(int damage){
		int totalHealth = 100;
		float sliderVal =  totalHealth - damage / totalHealth; 

		healthSlider.sliderValue = sliderVal;
	}

	public void setTowerHealth(int damage){
		int totalTowerHealth = 100;
		float sliderVal =  totalTowerHealth - damage / totalTowerHealth; 

		healthSlider.sliderValue = sliderVal;
	}

	public void setCoinCount(int coin){
		lblCoin.text = coin.ToString ();
	}
}

