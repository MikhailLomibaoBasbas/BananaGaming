using UnityEngine;
using System;

public class GameUIView : BasicView
{
	private const string IN_GAME_PANEL = "Prefabs/GUI/GamePanel2";//"Prefabs/GUI/GamePanel";

	private const string BTN_SHOOT = "AnchorBottomRight/btnShoot";
	private const string BTN_PAUSE = "AnchorTopRight/btnPause";
	private const string BTN_ATTACK = "AnchorBottomRight/btnAttack";
	private const string BTN_NAV= "AnchorBottomleft/btnNav";
	private const string BTN_SWITCH = "AnchorBottomRight/btnSwitch";
	private const string SLIDER_PLAYERHEALTH = "AnchorTopLeft/PlayerStatus/PlayerHealth";
	private const string SLIDER_TOWERHEALTH = "AnchorTopLeft/TowerStatus/TowerHealth";

	private const string LBL_COIN = "AnchorTopRight/lblCoin";
	private const string LBL_STAGE = "AnchorTop/lblStage";

	public UIEventListener.VoidDelegate onClickShoot = null;
	public UIEventListener.VoidDelegate onClickAttack = null;
	public UIEventListener.VoidDelegate onClickSwitch = null;
	public UIEventListener.VoidDelegate onClickPause = null;

	protected UISlider healthSlider;
	protected UISlider towerSlider;
	protected UILabel lblCoin;

	public static GameUIView Create() {
		return Create<GameUIView>(IN_GAME_PANEL);
	}
	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_SHOOT)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_ATTACK)).onClick += OnClickAttack;
		UIEventListener.Get(GetChild(BTN_SWITCH)).onClick += OnClickSwitch;
		UIEventListener.Get(GetChild(BTN_PAUSE)).onClick += OnClickPause;

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

	void OnClickPause(GameObject go) {
		if(onClickPause != null)
			onClickPause(go);
	}

	private UILabel _stageLabel;
	public UILabel GetStageLabel {
		get {
			if(_stageLabel == null)
				_stageLabel = GetChild(LBL_STAGE).GetComponent<UILabel>();
			return _stageLabel;
		}
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

