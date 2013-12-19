using UnityEngine;
using System;

public class BasicGameState : MonoBehaviour, IState
{
	public bool isTransparent {get;set;}
	public GameObject gameUiRoot {get;set;}
	public GameStateManager stateManager {get;set;}
	public IView view {get;set;}
	
	public virtual void OnStart() {
		if(view != null) view.Show(true);
	}
	
	public virtual void OnUpdate() {
	}
	
	public virtual void OnPause() {
		if(view != null) view.Hide(true);
		enabled = false;
	}
	
	public virtual void OnResume() {
		if(view != null) view.Show(true);
		enabled = true;
	}
	
	public virtual void OnResume(object data) {
		OnResume();
	}
	
	public virtual void OnEnd() {
		if(view != null) view.Close(true);
		Destroy(this);
	}
}

