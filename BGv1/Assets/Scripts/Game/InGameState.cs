using UnityEngine;
using System.Collections;

public class InGameState : BasicGameState {
	
	public override void OnStart () {
		view = InGameView.Create();
		AddGUIListeners();
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
		(view as InGameView).navigationView.onClickBack += OnClickBack;
	}
	
	void RemoveGUIListeners() {
		(view as InGameView).navigationView.onClickBack -= OnClickBack;
	}
	
	void OnClickBack(GameObject go) {
		stateManager.PopState();
	}
}
