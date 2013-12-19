using System;
using System.Collections.Generic;
using UnityEngine;

public class StateManager
{	
	private StateQueue _stateQueue;
	protected StateQueue stateQueue {
		get {
			if(_stateQueue == null) _stateQueue = new StateQueue();
			return _stateQueue;
		}
	}
	
	public StateManager() {
	}
	
	public void UpdateCurrentState() {
		int stateIndex = stateQueue.Count();
		while(stateIndex > 0) {
			IState state = stateQueue.GetStateAt(--stateIndex);
			state.OnUpdate();
			if(stateIndex > 0 && !state.isTransparent) break;
		}
	}
	
	public void ChangeState(IState state) {
		ClearStates();
		stateQueue.Push(state);
		state.OnStart();
	}
	
	public void PushState(IState state) {
		if(stateQueue.NotEmpty())
			stateQueue.Peek().OnPause();
		
		stateQueue.Push(state);
		state.OnStart();
	}
	
	public void PopState() {
		IState currentState = stateQueue.Pop();
		if(currentState != null) currentState.OnEnd();
		if(stateQueue.NotEmpty()) stateQueue.Peek().OnResume();
	}
	
	public void PopState(object data) {
		IState currentState = stateQueue.Pop();
		if(currentState != null) currentState.OnEnd();
		if(stateQueue.NotEmpty()) stateQueue.Peek().OnResume(data);
	}
	
	public IState GetCurrentState() {
		return stateQueue.Peek();
	}
	
	public IState GetPreviousState() {
		return stateQueue.Previous();
	}
	
	void ClearStates() {
		while(stateQueue.NotEmpty()) stateQueue.Pop().OnEnd();
	}
}

