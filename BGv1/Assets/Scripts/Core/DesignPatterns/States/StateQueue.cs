using System;
using System.Collections.Generic;
using UnityEngine;

public class StateQueue
{
	private List<IState> _states = new List<IState>();
	
	public StateQueue () {
	}
	
	public void Push(IState state) {
		_states.Add(state);
	}
	
	public IState Pop() {
		if(_states.Count > 0) {
			IState state = _states[_states.Count-1];
			_states.RemoveAt(_states.Count-1);
			return state;
		}
		return null;
	}
	
	public IState Peek() {
		if(_states.Count > 0) return _states[_states.Count-1];
		return null;
	}
	
	public IState Previous() {
		if(_states.Count > 1) return _states[_states.Count-2];
		return null;
	}
	
	public bool NotEmpty() {
		return _states.Count > 0;
	}
	
	public IState GetStateAt(int index) {
		if(_states.Count > index) {
			return _states[index];
		}
		return null;
	}
	
	public int Count() {
		
		return _states.Count;
	}
}

