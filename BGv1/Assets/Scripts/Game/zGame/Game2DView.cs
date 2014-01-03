using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Game2DView : Basic2DView {

	private const string PREFAB_PATH = "Prefabs/2D/Game2DPanel";
	public static Game2DView Create () {
		return Create<Game2DView>(PREFAB_PATH);
	}
	
	private PlayerController _playerController;
	public PlayerController getPlayerController {get{return _playerController;}}

	private List<EnemyController> _enemyControllers1;
	private List<EnemyController> _enemyControllers2;

	private CameraFollow _cameraFollow;
	public CameraFollow getCameraFollow {get {return _cameraFollow;}}

	private Transform _towerGO;
	public Transform getTowerGO{get{return _towerGO;}}

	
	void Awake () {
		_playerController = GetComponentInChildren<PlayerController>();
		_enemyControllers1 = new List<EnemyController>(GetChild("EnemyContainer1").GetComponentsInChildren<EnemyController>());
		_enemyControllers2 = new List<EnemyController>(GetChild("EnemyContainer2").GetComponentsInChildren<EnemyController>());
		foreach(EnemyController ec in _enemyControllers1)
			ec.setActiveInScene(false);
		foreach(EnemyController ec in _enemyControllers2)
			ec.setActiveInScene(false);

		_cameraFollow = get2DCamera.GetComponent<CameraFollow>();
		_cameraFollow.target = _playerController.transform;
		_cameraFollow.isActive = true;

		_towerGO = GetChild("Tower").transform;
	}

	public void SummonEnemyAtContainer1(EnemyController.EnemyType type, int count) {
		int i = 0;
		foreach(EnemyController ec in _enemyControllers1) {
			if(ec.enemyType == type) {
				ec.setActiveInScene(true, Vector3.up * Random.Range(-Screen.height / 2f, Screen.height / 2f), false);
				i++;
			}
			if(i == count)
				break;
		}
	}

}
