using UnityEngine;
using System;
using System.Collections;

public enum GameStateType {
	MAIN_MENU,
	SOCIAL,
	GAME,
	SHOP,
	GAME_CENTER,
	PAUSE,
	GAME_OVER,
	SAMPLE
}

public class Game : MonoSingleton<Game> {
	public enum LayerType {
		Player = 10,
		Enemy = 11,
		PlayerAttack = 12,
		EnemyAttack = 13,
		HurtDead = 14,
		Item = 16
	}

	public enum ItemType {
		Haste = 8,
		Heal = 24,
		Rage = 32,
		Silvercoin = 50,
		Goldcoin = 55
	}
	
	private GameStateManager _stateManager;
	
	public GameStateType defaultGameState;
	public GameObject gameUiRoot;
	public GameObject game2DRoot;
	public float defaultPixelToUnits;

	private OptionData _optionData;

	public Weapon.WeaponType weaponInitial;

	public override void Init () {
		DontDestroyOnLoad(gameObject);
		InitializeGUI();
		InitializeState();
		InitBasic2DViewStatic();
		Debug.Log("Game Initialized");
	}

	void InitBasic2DViewStatic () {
		Camera cam = Basic2DView.get2DCamera;
	}
	
	void InitializeGUI() {
		Assert.NotNull(gameUiRoot, "GameUI Root gameObject is missing or has not been set.");
		_optionData = new OptionData ();
	}
	
	void InitializeState() {
		_stateManager = new GameStateManager(gameObject);
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.MAIN_MENU), typeof(MainMenuState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.GAME), typeof(GameState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.SHOP), typeof(WeaponState));
		_stateManager.RegisterState(Cast.ToString<GameStateType>(GameStateType.PAUSE), typeof(PauseState));
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
