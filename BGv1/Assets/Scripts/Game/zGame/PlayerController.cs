using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : BasicCharacterController {

	private static PlayerController instance;
	public static PlayerController GetInstance {
		get {
			if(instance == null)
				instance = FindObjectOfType(typeof(PlayerController)) as PlayerController;
			return instance;
		}
	}

	//Base Attack Time Variables
	private float _bAT_cd_time;
	private bool _onKnifeBATCooldown;
	private bool _isAutomatic;

	private float _currentAcceleration;
	private float _accelerationRate = 5f;
	private bool _isMoveKeysPressed;
	private CameraFollow _cameraFollow;


	public Vector2 minClamp;
	public Vector2 maxClamp;

	private float _baseAttackTimeErrorMult = 1.5f;

	public Weapon.WeaponType currentWeaponType;
	private Dictionary<string, WeaponController> _weaponControllerMap = new Dictionary<string, WeaponController>();
	private WeaponController _currentWeapon;
	private float _knifeBAT = 1f;
	private CircleCollider2D _knifeCircleCollider;



	
	public override void Init (){
		base.Init ();
		Invoke("DoInit", 0.01f);
	}
	private void DoInit () {
		_knifeCircleCollider = transform.FindChild("Knife").GetComponentInChildren<CircleCollider2D>();
		_knifeCircleCollider.enabled = false;
		
		_weaponControllerMap = new Dictionary<string, WeaponController>();
		WeaponController[] weapons = GetComponentsInChildren<WeaponController>(true);
		foreach(WeaponController wp in weapons) {
			if(wp.isUnlocked) {
				_weaponControllerMap.Add(wp.name, wp);
				//Debug.LogError(wp.name);
			}
		}
		_currentWeapon = _weaponControllerMap[currentWeaponType.ToString()];
	}

	public override void OnUpdate () {
		base.OnUpdate ();
		if(Input.GetMouseButton(0)) {
			if(!_onKnifeBATCooldown) {
				_currentWeapon.Attack();
			}
		}
		if(Input.GetMouseButtonDown(1)) {
			if(!_onKnifeBATCooldown && _currentWeapon.isAttacking){ 
				DoCharacterState(CharacterState.Attack);
				_knifeCircleCollider.enabled = true;
				_onKnifeBATCooldown = true;
				Invoke("DisableAttackCollider", _knifeBAT);
			}
		}
		if(Input.GetKeyDown(KeyCode.Q)) {
			ChangeWeapon();
		}
		OnKnifeBATCooldownUpdate();
		UpdateClamp();


		if(!isHurt) {
			_isMoveKeysPressed = false;
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
				transform.position += -Vector3.right * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				if(!_onKnifeBATCooldown)
					transform.localScale = new Vector3(-1f, 1f, 1f);
				_isMoveKeysPressed = true;
			}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
				transform.position += Vector3.right * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				if(!_onKnifeBATCooldown)
					transform.localScale = new Vector3(1f, 1f, 1f);
				_isMoveKeysPressed = true;
			}
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
				transform.position += Vector3.up * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				_isMoveKeysPressed = true;
			}

			if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
				transform.position += -Vector3.up * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				_isMoveKeysPressed = true;
			}
			OnCurrentAccelerationUpdate();
		}
	}

	void UpdateClamp () {
		Vector3 tempPos = transform.position;
		if(tempPos.x < minClamp.x)
			tempPos.x = minClamp.x;
		if(tempPos.x > maxClamp.x)
			tempPos.x = maxClamp.x;
		if(tempPos.y < minClamp.y)
			tempPos.y = minClamp.y;
		if(tempPos.y > maxClamp.y)
			tempPos.y = maxClamp.y;
		transform.position = tempPos;
	}

	private void OnCurrentAccelerationUpdate () {
		float accelerationCopy = _currentAcceleration;
		if(_isMoveKeysPressed) {
			if(_currentAcceleration < 1f) {
				_currentAcceleration += cachedDeltaTime * _accelerationRate;
				if(_currentAcceleration >= 1)
					_currentAcceleration = 1;
			}
		} else {
			if(_currentAcceleration > 0) {
				_currentAcceleration -= cachedDeltaTime * _accelerationRate;
				if(_currentAcceleration <= 0) {
					_currentAcceleration = 0;
					DoCharacterState(CharacterState.Idle);
				}
			}
		}
		if(accelerationCopy != _currentAcceleration)
			animator.SetFloat("Acceleration", _currentAcceleration);
	}
	
	private void DisableAttackCollider() { _knifeCircleCollider.enabled = false;}

	private void OnKnifeBATCooldownUpdate () {
		if(_bAT_cd_time < getAttackTime) {
			_bAT_cd_time += cachedDeltaTime;
		} else {
			_bAT_cd_time = 0;
			_onKnifeBATCooldown = false;
			_knifeCircleCollider.enabled = false;
		}
	}

	public override void UpdateHurtAction () {

	}

	private void ChangeWeapon () {
		foreach(WeaponController wp in _weaponControllerMap.Values) {
			wp.SetGameObjectActive(false);
		}
		int weaponInt = (int)currentWeaponType;
		do {
			weaponInt++;
			if(weaponInt > (int)Weapon.WeaponType.AK47)
				weaponInt = 0;
			currentWeaponType = (Weapon.WeaponType)weaponInt;
		} while(!_weaponControllerMap.ContainsKey(currentWeaponType.ToString()));

		_currentWeapon = _weaponControllerMap[currentWeaponType.ToString()];
		_currentWeapon.SetGameObjectActive(true);
		//Animator.SetBool("changeWeaponName", true);
		//Invoke("SopChangeWeaponNameAnimator", time); 

		Debug.Log("Current Weapon: " + _currentWeapon.name);
	}



}
