using UnityEngine;
using System;
using System.Collections;

public enum GameStateType {
	MAIN_MENU,
	IN_GAME,
	SOCIAL,
	SHOP,
	GAME_CENTER,
	GAME_OVER,
	SAMPLE
}

public class Game : MonoSingleton<Game> {
	
	private GameStateManager _stateManager;
	
	public GameStateType defaultGameState;
	public GameObject gameUiRoot;
	public GameObject game2DRoot;
	public float defaultPixelToUnits;

	public override void Init () {
		DontDestroyOnLoad(gameObject);
		InitializeGUI();
		InitializeState();
		Debug.Log("Game Initialized");
	}
	
	void InitializeGUI() {
		Assert.NotNull(gameUiRoot, "GameUI Root gameObject is missing or has not been set.");
	}
	
	void InitializeState() {
		_stateManager = new GameStateManager(gameObject);
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.MAIN_MENU), typeof(MainMenuState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.IN_GAME), typeof(InGameState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.GAME_OVER), typeof(GameOverState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.SOCIAL), typeof(SocialState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.SAMPLE), typeof(SampleState));
		ChangeState(defaultGameState);
 	}
	
	public void ChangeState(GameStateType state) {
		_stateManager.ChangeState(Cast.ToString<GameStateType>(state));
	}
	
	public void PushState(GameStateType state) {
		_stateManager.PushState(Cast.ToString<GameStateType>(state));
	}

	#region MonoBehaviour callbacks
	void Update() {
		_stateManager.UpdateCurrentState();
	}
	#endregion
	
}
