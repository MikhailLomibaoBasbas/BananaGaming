using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager {
	private int _stageNum;
	private Vector2 _minMaxEnemiesPerWave = new Vector2(0.2f, 0.3f);
	private Vector2 _minMaxDelayInSecPerWave = new Vector2(5, 10); 
	private int _maxWave = 4;
	private int _baseRequiredKills = 5;
	private int _requiredKillsIncrementPerStage = 2;

	
	private int _unlockedMonstersCount;
	private List<EnemyController.EnemyType> _enemyDistribution;
	private EnemyController.EnemyType[] _enemyArrangedDistribution;
	public EnemyController.EnemyType[] GetEnemyArrangedDistribution {get {return _enemyArrangedDistribution;}}

	private bool _needsRefresh {
		set {
			_randomEnemiesSummonCountNeedsRefresh = value;
			_randomDelayNeedsRefresh = value;
			_requiredKillsNeedsRefresh = value;
		}
	}

	private bool _requiredKillsNeedsRefresh = true;
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

	private bool _randomEnemiesSummonCountNeedsRefresh = true;
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
	private bool _randomDelayNeedsRefresh = true;
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
		//val = 4;
		RefreshValues();
		_stageNum = val;
		_enemyArrangedDistribution = null;
		SetEnemyDistribution (val);
		int tempNormCount = 0;
	}
	private void SetEnemyDistribution (int stage) {
		_unlockedMonstersCount = 1 + (stage / 2);
		if(_unlockedMonstersCount > (int)EnemyController.EnemyType.Jumper + 1)
			_unlockedMonstersCount = (int)EnemyController.EnemyType.Jumper + 1;
		float[] ratio = new float[_unlockedMonstersCount];
		float ocRatio = 1;
		float cRatio = 1;
		int i = 1;
		while (i < _unlockedMonstersCount) {
			cRatio *= 0.8f;
			ratio[i] = ocRatio - cRatio;
			ocRatio = cRatio;
			//Debug.LogError(i + " " + ratio[i]);
			i++;
		}
		ratio[0] = cRatio;
		_enemyDistribution = new List<EnemyController.EnemyType>();
		_enemyArrangedDistribution = new EnemyController.EnemyType[GetRequiredKills];
		//Debug.LogError("REQ KILLS: " + GetRequiredKills);
		int indexStored = 0;
		for(int x = 0; x < _unlockedMonstersCount; x++) {
			int eTypeCount = (int)(ratio[x] * GetRequiredKills);
			for(int y = indexStored; y < indexStored + eTypeCount; y++) {
				_enemyDistribution.Add((EnemyController.EnemyType)x);
			}
			indexStored += eTypeCount;
		}
		//Debug.LogError(_enemyDistribution.Count + " " + GetRequiredKills);
		int deficit = GetRequiredKills - _enemyDistribution.Count;
		for(int x = 0; x < deficit; x++) {
			_enemyDistribution.Add(EnemyController.EnemyType.Normal);
		}
		//Debug.LogError(_enemyDistribution.Count + " " + GetRequiredKills);
		int edCount = _enemyDistribution.Count;
		for(int x = 0; x < edCount; x++) {
			int randIndex = Random.Range(0, _enemyDistribution.Count);
			_enemyArrangedDistribution[x] = _enemyDistribution[randIndex];
			_enemyDistribution.RemoveAt(randIndex);
		}

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
