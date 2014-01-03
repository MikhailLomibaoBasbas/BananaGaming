using UnityEngine;
using System.Collections;

public class StageManager {
	private int _stageNum;
	private Vector2 _minMaxZombiePerWave = new Vector2(0.1f, 0.2f);
	private Vector2 _minMaxDelayInSecPerWave = new Vector2(5, 10); 
	private int _maxWave = 4;
	private int _baseRequiredKills = 5;
	private int _requiredKillsIncrementPerStage = 5;

	private bool _needsRefresh;

	private int _requiredKills = 0;
	public int GetRequiredKills {
		get {
			if(_needsRefresh)
				_requiredKills = _baseRequiredKills + (_requiredKillsIncrementPerStage * _stageNum);
			return _requiredKills;
		}
	}


	private int _zombiesInCurrentWave = default(int);
	public int GetZombiesInCurrentWave {
		get {
			if(_zombiesInCurrentWave == default(int))
				_zombiesInCurrentWave = GetRandomZombiesWave;
			return _zombiesInCurrentWave;
		}
	}
	public int GetRandomZombiesWave {
		get {
			if(_needsRefresh) {
				_zombiesInCurrentWave = Random.Range((int)_minMaxZombiePerWave.x, (int)_minMaxZombiePerWave.y);
				return _zombiesInCurrentWave;
			}
			return default(int);
		}
	}


	private float _delayForNextWave = default(float);
	public float GetDelayForNextWave {
		get {
			if(_delayForNextWave == default(float))
				_delayForNextWave = GetRandomDelayZombieWave;
			return _delayForNextWave;
		}
	}
	public float GetRandomDelayZombieWave {
		get {
			if(_needsRefresh) {
				_delayForNextWave = Random.Range(_minMaxDelayInSecPerWave.x, _minMaxDelayInSecPerWave.y);
				return _delayForNextWave;
			}
			return default(float);
		}
	}


	public void SetStage(int val) {
		_stageNum = val;
		_needsRefresh = true;
		_zombiesInCurrentWave = default(int);
		_delayForNextWave = default(float);
	}

	public StageManager (int stage) {
		SetStage(stage);
	}
}
