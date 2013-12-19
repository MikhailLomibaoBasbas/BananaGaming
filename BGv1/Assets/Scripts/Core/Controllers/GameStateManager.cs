using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : StateManager
{
	private Dictionary<string, Type> _stateMap;
	private GameObject _parent;
	
	public GameStateManager(GameObject parent) {
		_parent = parent;
		_stateMap = new Dictionary<string, Type>();
	}
	
	public void RegisterState(string name, Type type) {
		if(type.IsSubclassOf(typeof(Component)) && type.IsSubclassOf(typeof(BasicGameState)))
			_stateMap.Add(name, type);
		else
			Debug.LogWarning("Trying to register type " + type.Name + " which is either not a Component or not a BasicGameState");
	}
	
	public BasicGameState ChangeState(string name) {
		BasicGameState gameState = null;
		if(_stateMap.ContainsKey(name)) {
			Type type = _stateMap[name];
			gameState = (BasicGameState)_parent.AddComponent(type);
			gameState.stateManager = this;
			ChangeState(gameState);
		}else{
			Debug.LogWarning("No registered type named " + name);
		}
		return gameState;
	}
	
	public BasicGameState PushState(string name) {
		BasicGameState gameState = null;
		if(_stateMap.ContainsKey(name)) {
			Type type = _stateMap[name];
			gameState = (BasicGameState)_parent.AddComponent(type);
			gameState.stateManager = this;
			PushState(gameState);
		}else{
			Debug.LogWarning("No registered type named " + name);
		}
		return gameState;
	}
}