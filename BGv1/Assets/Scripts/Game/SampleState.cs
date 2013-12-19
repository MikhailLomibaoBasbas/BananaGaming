using UnityEngine;
using System.Collections;

public class SampleState : BasicGameState {

	SampleView m_sampleView;

	public override void OnStart () {
		//Note: Initialize this State's View and All the necessary Initialization 
		//Methods here.
		// e.g. view = myView.Create();
		viewUI = SampleView.Create ();
		m_sampleView = (SampleView)viewUI;
		AddGUIListeners();
		base.OnStart ();
	}
	
	public override void OnUpdate () {
		base.OnUpdate ();
	}
	
	public override void OnEnd () {
		// Note: Method is Called when you Pop or Change this State.
		base.OnEnd ();
	}

	public override void OnPause () {
		// Note: Method is Called when you Push this State.
		base.OnPause ();
	}

	public override void OnResume () {
		// Note: Method is Called when you Pop another State in front
		// of this state.
		base.OnResume ();
	}
	
	void AddGUIListeners() {
		// Note: Add the GUIListener on OnResume() and OnStart() to ensure
		// there wouldn't be an overlapping Event Listener.
		//e.g (view as InGameView).navigationView.onClickBack += OnClickBack;
		m_sampleView.navigationView.onClickBack += OnClickBack;
	}
	
	void RemoveGUIListeners() {
		// Note: Remove the GUIListener on OnPause() and OnEnd() to ensure
		// there wouldn't be an overlapping Event Listener.
		//e.g. (view as InGameView).navigationView.onClickBack -= OnClickBack;
		m_sampleView.navigationView.onClickBack -= OnClickBack;
	}
	
	void OnClickBack(GameObject go) {
		// Note: Example of a UIEventListener method.
		//stateManager.PopState();
		stateManager.PopState ();
		//GameObject.Destroy (m_sampleView.gameObject);
	}


}
