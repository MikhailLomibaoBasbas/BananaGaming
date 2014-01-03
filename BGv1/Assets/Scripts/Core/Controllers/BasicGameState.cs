using UnityEngine;
using System;

public class BasicGameState : MonoBehaviour, IState
{
	public bool isTransparent {get;set;}
	public GameObject gameUiRoot {get;set;}
	public GameStateManager stateManager {get;set;}
	public IView viewUI {get;set;}
	public IView view2D {get; set;}
	
	public virtual void OnStart() {
		if(viewUI != null) viewUI.Show(false);
		if(view2D != null) view2D.Show(false);
	}
	
	public virtual void OnUpdate() {
	}
	
	public virtual void OnPause() {
		if(viewUI != null) viewUI.Hide(false);
		if(view2D != null) view2D.Hide(false);
		enabled = false;
	}
	
	public virtual void OnResume() {
		if(viewUI != null) viewUI.Show(false);
		if(view2D != null) view2D.Show(false);
		enabled = true;
	}
	
	public virtual void OnResume(object data) {
		OnResume();
	}
	
	public virtual void OnEnd() {
		if(viewUI != null) viewUI.Close(false);
		if(view2D != null) view2D.Close(false);
		Destroy(this);
	}
}

