﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Game2DView : Basic2DView {

	private const string PREFAB_PATH = "Prefabs/2D/Game2DPanel";
	public static Game2DView Create () {
		return Create<Game2DView>(PREFAB_PATH);
	}

	private List<EnemyController> _enemyControllers1;
	private List<EnemyController> _enemyControllers2;

	private CameraFollow _cameraFollow;
	public CameraFollow getCameraFollow {get {return _cameraFollow;}}
	
	private int enemyCountPerType = 20;

	private Tower _tower;
	public Tower GetTower {
		get {
			if (_tower == null)
				_tower = GetComponentInChildren<Tower> ();
			return _tower;
		}
	}
	
	void Awake () {
		_playerController = GetComponentInChildren<PlayerController>();
		InitEnemies();
		_cameraFollow = get2DCamera.GetComponent<CameraFollow>();
		_cameraFollow.target = _playerController.transform;
		_cameraFollow.isActive = true;
	}

	
	private void InitEnemies () {
		_enemyControllers1 = new List<EnemyController>();
		_enemyControllers2 = new List<EnemyController>();
		                                           
		for(int j = 0; j < (int)EnemyController.EnemyType.Jumper + 1; j++) {
			EnemyController ec = EnemyController.Create((EnemyController.EnemyType)j, Game.instance.game2DRoot);
			//Debug.LogError(((EnemyController.EnemyType)j).ToString());
			ec.name = "Enemy" + ec.enemyType.ToString();
			for(int i = 0; i < enemyCountPerType; i++){
				EnemyController ecCopy =  EnemyController.Instantiate(ec) as EnemyController;
				ecCopy.name += (" " + (i+1));
				ecCopy.cachedTransform.parent = GetEnemyContainer1GO.transform;
				EnemyController ecCopy2 =  EnemyController.Instantiate(ec) as EnemyController;
				ecCopy2.name += (" " + (i+1));
				ecCopy2.cachedTransform.parent = GetEnemyContainer2GO.transform;
				ecCopy.setActiveInScene(false, Vector3.zero, true, false);
				ecCopy2.setActiveInScene(false, Vector3.zero, true, false);
				ecCopy.cachedTransform.localPosition = ecCopy2.cachedTransform.localPosition = Vector3.zero;
				_enemyControllers1.Add(ecCopy);
				_enemyControllers2.Add(ecCopy2);
				//ecCopy.originalTarget = ecCopy2.originalTarget = getTowerTrans;
				//ecCopy.setCurrentTarget = ecCopy2.setCurrentTarget = getTowerTrans;
			}
			GameObject.Destroy(ec.gameObject);
		}
		/*_enemyControllers1 = new List<EnemyController>(GetEnemyContainer1GO.GetComponentsInChildren<EnemyController>());
		_enemyControllers2 = new List<EnemyController>(GetEnemyContainer2GO.GetComponentsInChildren<EnemyController>());
		foreach(EnemyController ec in _enemyControllers1)
			ec.setActiveInScene(false);
		foreach(EnemyController ec in _enemyControllers2)
			ec.setActiveInScene(false);*/
	}

	private const string PLAYER_GO = "PlayerV2";
	private PlayerController _playerController;
	public PlayerController getPlayerController {
		get{
			if(_playerController == null)
				_playerController = GetChild(PLAYER_GO).GetComponent<PlayerController>();
			return _playerController;
		}
	}

	private Transform _towerTrans;
	public Transform getTowerTrans{
		get{
			if(_towerTrans == null)
				_towerTrans = GetChild("Tower").transform;
			return _towerTrans;
		}
	}


	private const string ENEMY_CONTAINER1 = "EnemyContainer1";
	private GameObject _enemyContainer1GO;
	public GameObject GetEnemyContainer1GO {
		get {
			if(_enemyContainer1GO == null)
				_enemyContainer1GO = GetChild(ENEMY_CONTAINER1);
			return _enemyContainer1GO;
		}
	}
	
	private const string ENEMY_CONTAINER2 = "EnemyContainer2";
	private GameObject _enemyContainer2GO;
	public GameObject GetEnemyContainer2GO {
		get {
			if(_enemyContainer2GO == null)
				_enemyContainer2GO = GetChild(ENEMY_CONTAINER2);
			return _enemyContainer2GO;
		}
	}


	public void AddEnemiesStateStartListener (BasicCharacterController.OnCharacterStateStart listener) {
		foreach(EnemyController ec in _enemyControllers1) {
			ec.SetCharacterStateStartEventListener(listener);
		}
		foreach(EnemyController ec in _enemyControllers2) {
			ec.SetCharacterStateStartEventListener(listener);
		}
	}
	public void RemoveEnemiesStateStartListener (BasicCharacterController.OnCharacterStateStart listener) {
		foreach(EnemyController ec in _enemyControllers1) {
			ec.SetCharacterStateStartEventListener(listener);
		}
		foreach(EnemyController ec in _enemyControllers2) {
			ec.SetCharacterStateStartEventListener(listener);
		}
	}

	public void SummonEnemyAtContainer1(EnemyController.EnemyType type, int count) {
		int i = 0;
		int[] randIndexPos = StaticManager_Helper.GetRandomNonRepeatingNumbers(count, -count/ 2, (count / 2) + (count % 2 == 1? 1:0));
		int step = 7;
		bool foundOne = false;
		foreach(EnemyController ec in _enemyControllers1) {
			if(i >= count) {
				foundOne = true;
				break;
			}
			if(ec.enemyType == type) {
				if(!ec.enabled) {
					ec.setActiveInScene(true, Vector3.up * (3 * Screen.height / 2) / step * randIndexPos[i], false);
					i++;
				}
			}
		}
		if(!foundOne)
			Debug.LogError("WHY");
	}
	public void SummonEnemyAtContainer2(EnemyController.EnemyType type, int count) {
		int i = 0;
		int[] randIndexPos = StaticManager_Helper.GetRandomNonRepeatingNumbers(count, -count/ 2, (count / 2) + (count % 2 == 1? 1:0));
		int step = 9;
		bool foundOne = false;
		foreach(EnemyController ec in _enemyControllers2) {
			if(i >= count) {
				foundOne = true;
				break;
			}
			if(ec.enemyType == type) {
				if(!ec.enabled) {
					ec.setActiveInScene(true, Vector3.up * (3 * Screen.height / 2) / step * randIndexPos[i], false);
					i++;
				}
			}

		}
		if(!foundOne)
			Debug.LogError("WHY");
	}

	public void RemoveAllEnemiesInScene() {
		for(int i = 0; i < _enemyControllers1.Count; i++) {
			_enemyControllers1[i].setActiveInScene(false);
			_enemyControllers2[i].setActiveInScene(false);
		}
	}
		List<EnemyController> _enemiesDisabledOnPause = new List<EnemyController>();
		public void OnPauseCalled () {
				getPlayerController.enabled = false;
				for (int i = 0; i < _enemyControllers1.Count; i++) {
						EnemyController ec = _enemyControllers1 [i];
						if (ec.enabled) {
								ec.setActiveInScene (false);
								_enemiesDisabledOnPause.Add (ec);
						}
						ec = _enemyControllers2 [i];
						if (ec.enabled) {
								ec.setActiveInScene (false);
								_enemiesDisabledOnPause.Add (ec);
						}
				}
		}

		public void OnResumeCalled () {
				getPlayerController.enabled = true;
				for (int i = 0; i < _enemiesDisabledOnPause.Count; i++) {
						_enemiesDisabledOnPause [i].setActiveInScene(true);

				}
				_enemiesDisabledOnPause.Clear ();
		}


}
