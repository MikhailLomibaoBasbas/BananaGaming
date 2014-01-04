﻿using UnityEngine;
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

		_stageManager = new StageManager();
		StartStage(_stage = 1);
		AddListener();
		base.OnStart ();
	}

	public override void OnUpdate () {
	}

	public override void OnPause () {
		base.OnPause ();
	}

	public override void OnResume () {
		base.OnResume ();
	}

	public override void OnEnd () {
		base.OnEnd ();
	}

	private void AddListener () {
		m_game2DView.AddEnemiesDeadListener(OnEnemyDead);
	}

	private void RemoveListener () {
		m_game2DView.RemoveEnemiesDeadListener(OnEnemyDead);
	}


	private void StartStage (int stg) {
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
		_enemiesSummonedThisStage += enemiesToSummon;
		if(_enemiesSummonedThisStage > _stageManager.GetRequiredKills)
			enemiesToSummon -= (_enemiesSummonedThisStage - _stageManager.GetRequiredKills);
		Debug.LogError(enemiesToSummon);
		m_game2DView.SummonEnemyAtContainer1(EnemyController.EnemyType.Normal, enemiesToSummon);
		Invoke("SummonEnemies", _stageManager.GetRandomDelayEnemyWave);
		_stageManager.RefreshValuesForNextWave();
	}
	private void SummonBoss () {
	}

	private void OnEnemyDead(int score) {
		_score += score;
		_enemiesKilled++;
		_enemiesKilledThisStage++;
		//Debug.LogError(_enemiesSummonedThisStage + " " + _stageManager.GetRequiredKills);
		if(_enemiesKilledThisStage >= _stageManager.GetRequiredKills) {
			CancelInvoke("SummonEnemies");
			StartStage(++_stage);
		}
	}


}
