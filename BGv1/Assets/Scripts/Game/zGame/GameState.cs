﻿﻿using UnityEngine;
using System.Collections;

public class GameState : BasicGameState {

	private Game2DView m_game2DView;
	private GameUIView m_gameUIView;
	private StageManager _stageManager;
	private int _stage;

	private int _score;


	private int _enemiesKilled;
	private int _enemiesSummonedThisStage;
	private int _enemiesKilledThisStage;
	

	public override void OnStart () {
		view2D = Game2DView.Create();
		viewUI = GameUIView.Create ();
		m_game2DView = (Game2DView) view2D;
		m_gameUIView = (GameUIView) viewUI;
		m_game2DView.getPlayerController.SetCharacterStateFinishedEventListener (PlayerStateFinished);
		m_game2DView.GetTower.AddTowerDeadListener (OnTowerHit);
		_stageManager = new StageManager();
		static_audiomanager.getInstance.play_bgm ("Audio/Bgm/InGame");
		//AudioManager.GetInstance.PlayRandomBGMCombination();
		//StartStage(_stage = 10);
		Invoke("Test", 0.1f);
		AddListener();
		base.OnStart ();
	}
	private void Test () {
		//m_game2DView.SummonEnemyAtContainer1(EnemyController.EnemyType.Jumper, 1);
		//m_game2DView.SummonEnemyAtContainer2(EnemyController.EnemyType.Aggressive, 1);
		m_game2DView.SummonEnemyAtContainer1(EnemyController.EnemyType.Normal, 20);
	}

	public override void OnUpdate () {
	}

	public override void OnPause () {
		m_game2DView.OnPauseCalled ();
				CancelInvoke ("SummonEnemies");
		base.OnPause ();
	}

	public override void OnResume () {
				m_game2DView.OnResumeCalled ();
				Invoke ("SummonEnemies", _stageManager.GetRandomDelayEnemyWave);
		base.OnResume ();
	}

	public override void OnEnd () {
		m_game2DView.getPlayerController.RemoveCharacterStateListeners ();
		RemoveListener ();
		base.OnEnd ();
	}

	private void AddListener () {
		m_game2DView.AddEnemiesDeadListener(OnEnemyDead);
		m_gameUIView.onClickPause += OnClickPause;
		m_gameUIView.onPressShoot += OnPressShoot;
		m_gameUIView.onClickSwitch += OnClickSwitch;
	}

	private void RemoveListener () {
		m_game2DView.RemoveEnemiesDeadListener(OnEnemyDead);
		m_gameUIView.onClickPause -= OnClickPause;
		m_gameUIView.onPressShoot -= OnPressShoot;
		m_gameUIView.onClickSwitch -= OnClickSwitch;

		//m_game2DView.getPlayerController.SetCharacterStateFinishedEventListener (PlayerStateFinished);
		m_game2DView.GetTower.RemoveListeners ();
	}


	void OnClickPause(GameObject go){
		Debug.Log ("Pause");
		static_audiomanager.getInstance.play_sfx ("Audio/Sfx/Switch1", m_gameUIView.transform.position);
		Game.instance.PushState (GameStateType.PAUSE);
	}

	void OnClickSwitch(GameObject go){
		m_game2DView.getPlayerController.ChangeWeapon ();
	}

	void OnPressShoot(GameObject go, bool press){
		Debug.LogWarning (press);
		m_game2DView.getPlayerController.Attack (press);
	}

	private void StartStage (int stg) {
		m_game2DView.getPlayerController.minClamp = (stg > 5) ? new Vector2(-1500, -650): new Vector2(-100, -650);
		_stageManager.SetStage(stg);
		m_gameUIView.GetStageLabel.text = stg.ToString();
		_enemiesKilledThisStage = 0;
		_enemiesSummonedThisStage = 0;
		Invoke("SummonEnemies", 3f);
	}

	private void StartStageUIAnimation () {
	}

	public void SummonEnemies () {
		int enemiesToSummon = _stageManager.GetRandomEnemiesSummonCount;
		//if(_enemiesSummonedThisStage + enemiesToSummon > _stageManager.GetRequiredKills)
			//enemiesToSummon = (_stageManager.GetRequiredKills - _enemiesSummonedThisStage);
		Debug.LogWarning("HAYOP HAYOP HAYOP!!!");
		Debug.LogWarning("--Enemies to Summon: " + enemiesToSummon);
		for(int i = _enemiesSummonedThisStage; i < _enemiesSummonedThisStage + enemiesToSummon; i++) {
			Debug.Log(i + " " +  _stageManager.GetRequiredKills) ;
			//Debug.LogWarning(_stageManager.GetEnemyArrangedDistribution.Length);
			if(i >= _stageManager.GetRequiredKills) {
				break;
			}
			if(_stage > 5) {
				if(i %2 == 0)
					m_game2DView.SummonEnemyAtContainer1(_stageManager.GetEnemyArrangedDistribution[i], 1);
				else
					m_game2DView.SummonEnemyAtContainer2(_stageManager.GetEnemyArrangedDistribution[i], 1);
			} else {
				m_game2DView.SummonEnemyAtContainer1(_stageManager.GetEnemyArrangedDistribution[i], 1);
			}
		}
		_enemiesSummonedThisStage += enemiesToSummon;
		Debug.Log("----Enemies Summoned This Stage: " + _enemiesSummonedThisStage);



		///m_game2DView.SummonEnemyAtContainer1(EnemyController.EnemyType.Normal, enemiesToSummon);
		if(_enemiesSummonedThisStage < _stageManager.GetRequiredKills)
			Invoke("SummonEnemies", _stageManager.GetRandomDelayEnemyWave);
		_stageManager.RefreshValuesForNextWave();
	}

	private void OnEnemyDead(int score) {
		_score += score;
		_enemiesKilled++;
		_enemiesKilledThisStage++;
		Debug.LogWarning("Enemies Killed: " + _enemiesKilledThisStage);
		if(_enemiesKilledThisStage >= _stageManager.GetRequiredKills) {
			CancelInvoke("SummonEnemies");
			m_game2DView.RemoveAllEnemiesInScene();
			StartStage(++_stage);
		}
	}

	public void PlayerStateFinished(BasicCharacterController.CharacterState state, BasicCharacterController ins) {
		switch (state) {
		case BasicCharacterController.CharacterState.Hurt:
			//ins.health
			break;
		case BasicCharacterController.CharacterState.Dead:
			break;

		}
	}


	public void OnTowerHit (int health) {
		m_gameUIView.setPlayerHealth (health);
	}

}
