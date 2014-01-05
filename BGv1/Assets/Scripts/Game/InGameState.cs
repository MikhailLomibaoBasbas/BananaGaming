using UnityEngine;
using System.Collections;

public class InGameState : BasicGameState {
	
	public override void OnStart () {
		viewUI = GameUIView.Create();
		AddGUIListeners();
				Debug.Log ("Ingame State");
		base.OnStart ();
	}
	
	public override void OnUpdate () {
		base.OnUpdate ();
	}
	
	public override void OnEnd () {
		RemoveGUIListeners();
		base.OnEnd ();
	}
	
	void AddGUIListeners() {
		//(viewUI as InGameView).navigationView.onClickBack += OnClickBack;
	}
	
	void RemoveGUIListeners() {
		//(viewUI as InGameView).navigationView.onClickBack -= OnClickBack;
	}
	
	void OnClickBack(GameObject go) {
		stateManager.PopState();
	}
}
