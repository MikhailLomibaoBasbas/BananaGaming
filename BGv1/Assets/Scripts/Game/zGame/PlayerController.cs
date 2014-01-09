﻿using UnityEngine;
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




	private float _baseAttackTimeErrorMult = 1.5f;

	private bool _isAttackPressed;

	public Weapon.WeaponType currentWeaponType;
	private Dictionary<string, WeaponController> _weaponControllerMap = new Dictionary<string, WeaponController>();
	private WeaponController _currentWeapon;
	private float _knifeBAT = 1f;
	private CircleCollider2D _knifeCircleCollider;


	private Vector3 _originalScale;
	
	public override void Init (){
		instance = this;
		base.Init ();
		Invoke("DoInit", 0.01f);
	}
	private void DoInit () {
		_originalScale = cachedTransform.localScale;
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
		//_currentWeapon = _weaponControllerMap[currentWeaponType.ToString()];
		InputController.instance.onNavMove += OnNavigationMove;
		currentWeaponType = Game.instance.weaponInitial;
		//Debug.LogError ("Current weapon type : " + currentWeaponType);
		ChangeWeapon(currentWeaponType);
	}

	private void OnNavigationMove(Vector3 normalizedDisplacement, bool pressed) {
			Debug.Log (normalizedDisplacement);
		if (pressed) {
			if (!_isMoveKeysPressed)
				_isMoveKeysPressed = true;
			//FUCKING BUGGG HERE!!!

			cachedTransform.position += normalizedDisplacement * getTranslateUnitsPerSecond * cachedDeltaTime;// * _currentAcceleration;
			if (normalizedDisplacement.x < 0f) {
				if (!_onKnifeBATCooldown && !_isAttackPressed) {
					Vector3 tempScale = _originalScale;
					tempScale.x *= 1f;
					cachedTransform.localScale = tempScale;
				}
			} else {
				if (!_onKnifeBATCooldown && !_isAttackPressed) {
					Vector3 tempScale = _originalScale;
					tempScale.x *= -1f;
					cachedTransform.localScale = tempScale;
				}
			}

			if (!isMoving)
				DoCharacterState (CharacterState.Move);
			OnCurrentAccelerationUpdate ();
		} else {
			_isMoveKeysPressed = false;
		}
	}

	public override void OnUpdate () {
		base.OnUpdate ();
		#if FALSE
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

		if(Input.GetMouseButtonDown(0)) {
			_isAttackPressed = true;
		}
		if(Input.GetMouseButtonUp(0)) {
			_isAttackPressed = false;
		}
		if(Input.GetKeyDown(KeyCode.Q)) {
			ChangeWeapon();
		}
#endif
		if (_isAttackPressed) {
			_currentWeapon.Attack();
		}


		OnKnifeBATCooldownUpdate();
	
		UpdateMoveControls();
	}

	public void Attack (bool isPressed) {
		if(isPressed) {
			if(!_isAttackPressed)
					_isAttackPressed = true;
			if(!_onKnifeBATCooldown) {
				_currentWeapon.Attack();
			}
		} else {
			if(_isAttackPressed)
				_isAttackPressed = false;
		}
	}

	private void UpdateMoveControls () {
		if(!isHurt) {
			_isMoveKeysPressed = false;
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
				transform.position += -Vector3.right * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				if(!_onKnifeBATCooldown && !_isAttackPressed) {
					Vector3 tempScale = _originalScale;
					tempScale.x *= 1f;
					cachedTransform.localScale = tempScale;
				}
				_isMoveKeysPressed = true;
			}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
				cachedRigidBody2D.
					transform.position += Vector3.right * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				if(!_onKnifeBATCooldown && !_isAttackPressed) {
					Vector3 tempScale = _originalScale;
					tempScale.x *= -1f;
					cachedTransform.localScale = tempScale;
				}
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

	public void ChangeWeapon () {
		foreach(WeaponController wp in _weaponControllerMap.Values) {
			wp.Hide();
		}
		int weaponInt = (int)currentWeaponType;
		do {
			weaponInt++;
			if(weaponInt > (int)Weapon.WeaponType.AK47)
				weaponInt = 0;
			currentWeaponType = (Weapon.WeaponType)weaponInt;
		} while(!_weaponControllerMap.ContainsKey(currentWeaponType.ToString()));

		_currentWeapon = _weaponControllerMap[currentWeaponType.ToString()];
		_currentWeapon.Show();
		//Animator.SetBool("changeWeaponName", true);
		//Invoke("SopChangeWeaponNameAnimator", time); 

		Debug.Log("Current Weapon: " + _currentWeapon.name);
	}
	private void ChangeWeapon(Weapon.WeaponType type) {
		foreach(WeaponController wp in _weaponControllerMap.Values) {
			wp.Hide();
		}
		_currentWeapon = _weaponControllerMap[currentWeaponType.ToString()];
		_currentWeapon.Show();
	}

	private GameObject _exclamationPointGO = null;
	public void NotifyDanger () {
		StartCoroutine(StartNotifyDangerCoroutine());
	}
	private IEnumerator StartNotifyDangerCoroutine() {
		if(_exclamationPointGO == null)
			_exclamationPointGO = cachedTransform.FindChild("exclamationPoint").gameObject;
		_exclamationPointGO.SetActive(true);
		iTween.PunchScale(_exclamationPointGO, new Vector3(1.3f, 1.3f, 0), 0.5f);
		yield return new WaitForSeconds(1.0f);
		_exclamationPointGO.SetActive(false);
	}

	protected override void StartCollidedItemAction (GameObject go) {
		Item item = go.GetComponent<Item>();
		int val = item.GetValue(); // This is already with anim
		float dur = item.duration;
		switch(item.itemType) {
		case Game.ItemType.Coin:

			break;
		case Game.ItemType.Haste:
			Haste(dur, val);
			break;
		case Game.ItemType.Heal:
			break;
		case Game.ItemType.Rage:
			break;
		}
	}

	private void Haste (float dur, int val) {
		StartCoroutine(HasteCoroutine(dur, val));
	}
	private IEnumerator HasteCoroutine (float dur, int val) {
		moveSpeed += val;
		yield return new WaitForSeconds(val);
		moveSpeed += val;
	}

	private void Rage (float dur, int val) {
		StartCoroutine(RageCoroutine(dur, val));
	}
	private IEnumerator RageCoroutine (float dur, int val) {
		foreach(WeaponController wc in _weaponControllerMap.Values) {
			wc.AddDamage(val);
		}
		yield return new WaitForSeconds(dur);
		foreach(WeaponController wc in _weaponControllerMap.Values) {
			wc.AddDamage(-val);
		}
	}



}
