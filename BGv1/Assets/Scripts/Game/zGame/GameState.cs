using UnityEngine;
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

	private int _towerHealth = 100;

	AnimationsMashUp _comboAnimationsMashup;

	public override void OnStart () {
		view2D = Game2DView.Create();
		viewUI = GameUIView.Create ();
		m_game2DView = (Game2DView) view2D;
		m_gameUIView = (GameUIView) viewUI;
		m_game2DView.getPlayerController.SetCharacterStateFinishedEventListener (PlayerStateFinished);
		m_game2DView.GetTower.AddTowerDeadListener( OnTowerHit );
		_stageManager = new StageManager();
		static_audiomanager.getInstance.play_bgm ("Audio/Bgm/InGame");
		//AudioManager.GetInstance.PlayRandomBGMCombination();
		_comboAnimationsMashup = StaticAnimationsManager.getInstance.getAvailableAnimMashUp;
		_comboAnimationsMashup.animationTime = 0.3f;
		_comboAnimationsMashup.setScaleAnim (Vector3.one * 1.2f, Vector3.one);
		m_gameUIView.getGOCombo.SetActive (false);
		StartStage(_stage);
		AddListener();
		Invoke("SecondOnStart", 0.5f);
		base.OnStart ();
	}
	private void SecondOnStart () {
		ShowStage (_stage);
		//ShowCombo (1, Vector3.zero);
		m_gameUIView.setPlayerHealth (m_game2DView.getPlayerController.health);
		m_gameUIView.setTowerHealth (m_game2DView.GetTower.health);
		//m_game2DView.SummonEnemyAtContainer1(EnemyController.EnemyType.Jumper, 1);
		//m_game2DView.SummonEnemyAtContainer2(EnemyController.EnemyType.Aggressive, 1);
		//m_game2DView.SummonEnemyAtContainer1(EnemyController.EnemyType.Normal, 4);

		//m_game2DView.SummonEnemyAtContainer2(EnemyController.EnemyType.Stealth, 5);
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
		m_game2DView.OnPauseCalled ();
		m_game2DView.getCameraFollow.isActive = false;
		m_game2DView.getPlayerController.RemoveCharacterStateListeners ();
		StaticAnimationsManager.getInstance.removeFromStack (_comboAnimationsMashup);
		RemoveListener ();
		base.OnEnd ();
	}

	private void AddListener () {
		m_game2DView.AddEnemiesStateStartListener(OnEnemyStateStart);
		m_gameUIView.onClickPause += OnClickPause;
		m_gameUIView.onPressShoot += OnPressShoot;
		m_gameUIView.onClickSwitch += OnClickSwitch;
	}

	private void RemoveListener () {
		m_game2DView.RemoveEnemiesStateStartListener(OnEnemyStateStart);
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
		//Debug.LogWarning (press);
		m_game2DView.getPlayerController.Attack (press);
	}

	private void StartStage (int stg) {
		m_game2DView.getPlayerController.minClamp = (stg > 5) ? new Vector2(-1500, -650): new Vector2(-100, -650);
		ShowStage (stg);
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
			//Debug.Log(i + " " +  _stageManager.GetRequiredKills) ;
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

	private int combo;
	private void CancelComboCounter () {
		combo = 0;
		m_gameUIView.getGOCombo.SetActive (false);
	}

	private void OnEnemyStateStart (BasicCharacterController.CharacterState state, BasicCharacterController instance) {
		Debug.LogWarning (state);
		switch (state) {
		case BasicCharacterController.CharacterState.Hurt:
			CancelInvoke ("CancelComboCounter");
			combo++;
			m_gameUIView.setHit (combo);
			Invoke ("CancelComboCounter", 3f);
				break;
		case BasicCharacterController.CharacterState.Dead:
			OnEnemyDead ((instance as EnemyController).score);
				break;
		}
	}



	private void OnEnemyDead(int score) {
		_score += score;
		m_gameUIView.setPointCount (_score);
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
			m_gameUIView.setPlayerHealth (ins.health);
			//ins.health
			break;
		case BasicCharacterController.CharacterState.Dead:

				Game.instance.ChangeState (GameStateType.GAME_OVER);
			break;

		}
	}

	public void OnTowerHit (int health) {
		m_gameUIView.setTowerHealth (health);
		if (health <= 0f) {
			Game.instance.PushState (GameStateType.GAME_OVER);
		}
	}

	public void ShowStage(int stage){
		m_gameUIView.setStageNum (stage);
		iTween.MoveTo (m_gameUIView.getGOStage, iTween.Hash ("y", 0f, "time", 1f, "islocal", true,
			"easetype", iTween.EaseType.spring ,"oncomplete", "FinishedShowStage", "oncompletetarget", transform.gameObject));
	}

	private void FinishedShowStage(){
		iTween.MoveTo (m_gameUIView.getGOStage, iTween.Hash ("y", 600f, "time", 1f, "delay", 1f, "islocal", true,
			"easetype", iTween.EaseType.spring));//,"oncomplete", "FinishedShowStage", "oncompletetarget", transform.gameObject));
	}

	public void ShowCombo(int hit, Vector3 pos){
		m_gameUIView.setHit (hit, pos);
		iTween.ScaleTo (m_gameUIView.getGOCombo, iTween.Hash ("scale", Vector3.one * 0.5f, "time", 0.3f, "islocal", true,
			"easetype", iTween.EaseType.spring ,"oncomplete", "FinishedShowCombo", "oncompletetarget", transform.gameObject));
	}

	private void FinishedShowCombo(){
		m_gameUIView.setHit (0, Vector3.zero);
		iTween.ScaleTo (m_gameUIView.getGOCombo, iTween.Hash ("scale", Vector3.zero, "time", 0.3f, "islocal", true,
			"easetype", iTween.EaseType.spring));// ,"oncomplete", "FinishedShowCombo", "oncompletetarget", transform.gameObject));
	}
}
