using UnityEngine;
using System.Collections;

public class PlayerController : BasicCharacterController {

	public enum WeaponType {
		Knife,
		Pistol,
		Shotgun,
		AK47
	}

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
	private bool _onBATCooldown;
	private bool _isAutomatic;

	private float _currentAcceleration;
	private float _accelerationRate = 5f;
	private bool _isMoveKeysPressed;
	private CameraFollow _cameraFollow;
	private CircleCollider2D _attackCircleCollider;

	public Vector2 minClamp;
	public Vector2 maxClamp;

	private float _baseAttackTimeErrorMult = 1.5f;

	public override void Init (){
		base.Init ();
		//Basic2DView.get2DCamera.transform.parent = cachedTransform;
		_attackCircleCollider = transform.FindChild("attackCollider").GetComponent<CircleCollider2D>();
		_attackCircleCollider.enabled = false;
		InputController.instance.onNavMove += OnNavigationMove;
	}
	private void OnNavigationMove(Vector3 normalizedDisplacement) {
				//Debug.Log (normalizedDisplacement);
				cachedTransform.position += normalizedDisplacement * getTranslateUnitsPerSecond * cachedDeltaTime;// * _currentAcceleration;
	}

	public override void OnUpdate () {
		base.OnUpdate ();
		if(Input.GetMouseButtonDown(0)) {
			if(!_onBATCooldown){ 
				DoCharacterState(CharacterState.Attack);
				_attackCircleCollider.enabled = true;
				_onBATCooldown = true;
				Invoke("DisableAttackCollider", getAttackTime * 0.2f);
			}
		}
		OnBATCooldownUpdate();
		UpdateClamp();

		if(!isHurt) {
			_isMoveKeysPressed = false;
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
				transform.position += -Vector3.right * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				if(!_onBATCooldown)
					transform.localScale = new Vector3(-1f, 1f, 1f);
				_isMoveKeysPressed = true;
			}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
				transform.position += Vector3.right * getTranslateUnitsPerSecond * cachedDeltaTime * _currentAcceleration;
				if(!isMoving)
					DoCharacterState(CharacterState.Move);
				if(!_onBATCooldown)
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
	
	private void DisableAttackCollider() { _attackCircleCollider.enabled = false;}

	private void OnBATCooldownUpdate () {
		if(_bAT_cd_time < getAttackTime) {
			_bAT_cd_time += cachedDeltaTime;
		} else {
			_bAT_cd_time = 0;
			_onBATCooldown = false;
			_attackCircleCollider.enabled = false;
		}
	}

	public override void UpdateHurtAction () {

	}


}
