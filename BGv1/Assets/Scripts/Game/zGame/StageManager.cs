using UnityEngine;
using System.Collections;

public class StageManager {
	private int _stageNum;
	private Vector2 _minMaxEnemiesPerWave = new Vector2(0.2f, 0.3f);
	private Vector2 _minMaxDelayInSecPerWave = new Vector2(5, 10); 
	private int _maxWave = 4;
	private int _baseRequiredKills = 5;
	private int _requiredKillsIncrementPerStage = 5;

	
	private int _unlockedMonstersCount;
	private EnemyController.EnemyType[] _enemyDistribution;
	private EnemyController.EnemyType[] _enemyArrangedDistribution;

	private bool _needsRefresh {
		set {
			_randomEnemiesSummonCountNeedsRefresh = value;
			_randomDelayNeedsRefresh = value;
			_requiredKillsNeedsRefresh = value;
		}
	}

	private bool _requiredKillsNeedsRefresh;
	private int _requiredKills = 0;
	public int GetRequiredKills {
		get {
			if(_requiredKillsNeedsRefresh) {
				_requiredKills = _baseRequiredKills + (_requiredKillsIncrementPerStage * _stageNum);
				_requiredKillsNeedsRefresh = false;
			}
			return _requiredKills;
		}
	}

	private bool _randomEnemiesSummonCountNeedsRefresh;
	private int _randomEnemiesSummonCount;
	public int GetRandomEnemiesSummonCount {
		get {
			if(_randomEnemiesSummonCountNeedsRefresh) {
				_randomEnemiesSummonCount = System.Convert.ToInt32(Random.Range(_minMaxEnemiesPerWave.x * GetRequiredKills, _minMaxEnemiesPerWave.y * GetRequiredKills));
				_randomEnemiesSummonCountNeedsRefresh = false;
			}
			return _randomEnemiesSummonCount;
		}
	}
	private bool _randomDelayNeedsRefresh;
	private float _randomDelayEnemy;
	public float GetRandomDelayEnemyWave {
		get {
			if(_randomDelayNeedsRefresh) {
				_randomDelayEnemy =  Random.Range(_minMaxDelayInSecPerWave.x, _minMaxDelayInSecPerWave.y);
				_randomDelayNeedsRefresh = false;
			}
			return _randomDelayEnemy;
		}
	}


	public void SetStage(int val) {
		_stageNum = val;
		_unlockedMonstersCount = 1 + (val / 2);
		if(_unlockedMonstersCount > (int)EnemyController.EnemyType.Jumper + 1)
			_unlockedMonstersCount = (int)EnemyController.EnemyType.Jumper + 1;
		RefreshValues();
	}
	private void SetEnemyDistribution () {

	}

	public void RefreshValuesForNextWave () {
		_randomDelayNeedsRefresh = 
			_randomEnemiesSummonCountNeedsRefresh = true;
	}

	public void RefreshValues () {
		_needsRefresh = true;
	}


	public StageManager (int stage) {

		SetStage(stage);
	}
	public StageManager(){
		_unlockedMonstersCount = 1;
	}
	

}
