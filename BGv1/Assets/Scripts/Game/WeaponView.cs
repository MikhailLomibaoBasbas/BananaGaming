using UnityEngine;
using System.Collections;

public class WeaponView : BasicView {

	private const string WEAPON_PANEL = "Prefabs/GUI/WeaponPanel";
	public static WeaponView Create() {
		return Create<WeaponView>(WEAPON_PANEL);
	}

	public UIEventListener.VoidDelegate onClickPlay = null;
	public UIEventListener.VoidDelegate onClickBack = null;
	public UIEventListener.VoidDelegate onClickHand = null;
	public UIEventListener.VoidDelegate onClickShot = null;
	public UIEventListener.VoidDelegate onClickAk = null;

	private const string BTN_PLAY 		= "AnchorBottomRight/btnPlay";
	private const string BTN_BACK 	= "AnchorBottomLeft/btnBack";
	private const string BTN_HANDGUN = "Anchor/w1";
	private const string BTN_SHOTGUN = "Anchor/w2";
	private const string BTN_AK = "Anchor/w3";

	private GameObject[] buttons;
	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_PLAY)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_BACK)).onClick += OnClickBack;
		UIEventListener.Get(GetChild(BTN_HANDGUN)).onClick += OnClickHand;
		UIEventListener.Get(GetChild(BTN_SHOTGUN)).onClick += OnClickShot;
		UIEventListener.Get(GetChild(BTN_AK)).onClick += OnClickAk;

	}
	void Start(){
		buttons = new GameObject[3];
		buttons [0] = GetChild (BTN_HANDGUN);
		buttons [1] = GetChild (BTN_SHOTGUN);
		buttons [2] = GetChild (BTN_AK);

		EquipButton (buttons [0]);
		Game.instance.weaponInitial = Weapon.WeaponType.Pistol;
	}
	void OnClickPlay(GameObject go) {
		if(onClickPlay != null)
			onClickPlay(go);
	}

	void OnClickBack(GameObject go) {
		if(onClickBack != null)
			onClickBack(go);
	}

	void OnClickHand(GameObject go) {
		if(onClickHand != null)
			onClickHand(go);
	}

	void OnClickShot(GameObject go) {
		if(onClickShot != null)
			onClickShot(go);
	}

	void OnClickAk(GameObject go) {
		if(onClickAk != null)
			onClickAk(go);
	}

	public void EquipButton(GameObject go){
		for (int i = 0; i < buttons.Length; i++) {
			if (go.name.Contains (buttons [i].name))
				buttons[i].transform.FindChild("Label").GetComponent<UILabel>().text = "Equipped";
			else
				buttons[i].transform.FindChild("Label").GetComponent<UILabel>().text = "Not Equipped";
		}
	}
	
}
