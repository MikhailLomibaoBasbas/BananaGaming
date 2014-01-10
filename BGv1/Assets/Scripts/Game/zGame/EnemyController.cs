 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : BasicCharacterController {

	public enum EnemyType {
		Normal, 
		Stealth,
		Aggressive,
		Jumper
		//Tank,
		//Dog,
		//Summoner
	}

	private static string GetEnemyResourcePath(EnemyType type) {
		return "Prefabs/2D/_Enemy/Enemy" + type.ToString();
	}
	public static EnemyController Create (EnemyType type, GameObject parentObj) {
		GameObject enemyGO = StaticManager_Helper.CreatePrefab(GetEnemyResourcePath(type), parentObj);
		EnemyController enemy = enemyGO.GetComponent<EnemyController>();
		return enemy;
	}

	public int score;

	public float moveDuration;
	protected float _currentMoveDurTime;

	public float moveCooldown;
	protected float _currentMoveCoolTime;

	public float attackCooldown;
	protected bool _isAttackOnCooldown;
	protected float _currentAttackCooldown;

	public int damage;
	
	public Transform originalTarget;

	protected Transform _currentTarget;
	public Transform setCurrentTarget {
		set {
			_currentTarget = value;
		}
	}
	protected Vector3 _randomTargetDirectionNormalized;
	private Vector3 _randomMoveDirBeforeTheOrigTargetDirNormalized;

	private int _originalHealth;
	public int getOriginalHealth {get{return _originalHealth;}}
	private float _originalMoveSpeed;
	public float getOriginalMoveSpeed {get{return _originalMoveSpeed;}}

	
	public EnemyType enemyType;
	protected SpriteRenderer _spriteRenderer;

	private CircleCollider2D _attackCircleCollider;
	private EnemyProjectile _enemyProjectile;

	private BoxCollider2D _boxCollider2D;


	private float getTargetDistance {get {return Vector3.Distance(_currentTarget.position, cachedTransform.position);}}
	private float _distanceThreshold = 80f;

	public delegate void OnEnemyDeadFinished (int score);
	public event OnEnemyDeadFinished onEnemyDeadFinished = null;
	
	private PlayerController _playerControllerInstance;

	private GameObject _itemContainer;
	private Dictionary<Game.ItemType, Item> _itemsMap;

	private bool _canMove = false;
	public bool canMove {
		get {
			return _canMove;
		}
		set {
			_canMove = value;
			_currentMoveDurTime = 0;
			//_currentMoveCoolTime = 0;

		}
	}

	public override void Init () {
		//Before Base Init Statements
		//originalTarget = PlayerController.GetInstance.cachedTransform;
		_itemContainer = Item.CreateItemContainer (gameObject);
		_itemsMap = new Dictionary<Game.ItemType, Item> ();
		Item[] items = _itemContainer.GetComponentsInChildren<Item> ();
		for (int i = 0; i < items.Length; i++) {
			_itemsMap.Add (items [i].itemType, items [i]);
		}
		_itemContainer.SetActive (false);
		_boxCollider2D = collider2D as BoxCollider2D;
		_playerControllerInstance = PlayerController.GetInstance;
		InitEnemyTypes();
		_originalHealth = health;
		_originalMoveSpeed = moveSpeed;
		base.Init ();
		//After Base Init Statements
		//_attackCircleCollider = transform.FindChild("attackCollider").GetComponent<CircleCollider2D>();
		//_attackCircleCollider.enabled = false;
		_enemyProjectile = transform.FindChild("attackCollider").GetComponent<EnemyProjectile>();
		//Debug.Log(getAttackTime);
		_enemyProjectile.SetValues(null, getAttackTime, 0f, damage);
		//SetCharacterStateStartEventListener(OnEnemyStateStarted);
		//SetCharacterStateFinishedEventListener(OnEnemyStateFinished);
		_spriteRenderer = GetComponent<SpriteRenderer>();
		//move();
	}

	private void InitEnemyTypes () {
		//Debug.LogError("fds");
		switch(enemyType) {
		case EnemyType.Normal:
		case EnemyType.Stealth:
			_distanceThreshold = 250f;
			originalTarget = _currentTarget = Tower.GetInstance.transform;
			break;
		case EnemyType.Aggressive:
			originalTarget =  _currentTarget = _playerControllerInstance.cachedTransform;
			break;
		case EnemyType.Jumper:
			_jumperAnimMashup = StaticAnimationsManager.getInstance.getAvailableAnimMashUp;
			originalTarget =  _currentTarget = null;//PlayerController.GetInstance.cachedTransform;
			break;
		}
	}


	public void  setActiveInScene (bool flag, Vector3 pos = default(Vector3), bool isGlobal = true, bool withDelay = true) {
		float delay = (withDelay) ? Random.Range(0.5f, 1.0f): 0.1f;
		if(flag) {
			_boxCollider2D.enabled = true;
			enabled = true;
			health = _originalHealth;
			gameObject.SetActive(true);
			if(pos != default(Vector3))
				cachedTransform.SetPosition(pos, isGlobal);
			Invoke("PorstSetActiveInScene_True", delay);
		} else {
			idle();
			cachedCollider2D.enabled = false;
			Invoke("PorstSetActiveInScene_False", delay);
		}
		//static_coroutine.getInstance.DoReflection(this, "PostSetActiveScene", new object[1]{flag}, delay);
	}
	private void PorstSetActiveInScene_True () {
		cachedCollider2D.enabled = true;
		move();
	}
	private void PorstSetActiveInScene_False () {
		enabled = false;
		cachedTransform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
	}


	private void move () {
		canMove = true;
		DoCharacterState(CharacterState.Move);
		animator.SetFloat("Acceleration", 1f);
		if(_currentTarget != null) {
			_randomMoveDirBeforeTheOrigTargetDirNormalized = (_currentTarget.position - cachedTransform.position);
		} else {
			_randomMoveDirBeforeTheOrigTargetDirNormalized = _randomTargetDirectionNormalized;
		}
		float xRandPos = _randomMoveDirBeforeTheOrigTargetDirNormalized.x * 0.8f;
		float yRandPos = _randomMoveDirBeforeTheOrigTargetDirNormalized.y * 0.8f;
		//Debug.LogError(_randomMoveDirBeforeTheOrigTargetDirNormalized);
		_randomMoveDirBeforeTheOrigTargetDirNormalized.x += Random.Range(-xRandPos, xRandPos);
		_randomMoveDirBeforeTheOrigTargetDirNormalized.y += Random.Range(-yRandPos, yRandPos);
		_randomMoveDirBeforeTheOrigTargetDirNormalized.Normalize();
		//Debug.LogWarning(_randomMoveDirBeforeTheOrigTargetDirNormalized);
	}  

	private void idle () {
		animator.SetFloat("Acceleration", 0);
		DoCharacterState(CharacterState.Idle);
		canMove = false;
	}

	private void attack () {
		if(_currentTarget != null)
			cachedTransform.localScale =  new Vector3(_currentTarget.position.x < cachedTransform.position.x ? 1f: -1f, 1f ,1f);
		DoCharacterState(CharacterState.Attack);
		_enemyProjectile.Show(true);
		//_attackCircleCollider.enabled = true;
		//Invoke("DisableAttackCollider", getAttackTime * 0.20f);
	}
	private void DisableAttackCollider() { _attackCircleCollider.enabled = false;}

	public override void OnUpdate () {
		base.OnUpdate ();
		if(!isHurt && !isDead) {
			UpdateMoveCooldownAction();

		}
		UpdateAttackCooldownAction();
		//Debug.LogWarning(_currentMoveCoolTime);
		UpdateEnemyBehaviour();

		if(_currentTarget != null) {
			//Debug.LogWarning(getTargetDistance);
			if((getTargetDistance < _distanceThreshold)) {
				if(!_isAttackOnCooldown) {
					attack();
				}
			}
		}

	}

	private void UpdateMoveCooldownAction () {
		if(_currentTarget != null) {
			if(getTargetDistance < _distanceThreshold)
				return;
		}
		if(_currentMoveCoolTime < moveCooldown) {
			_currentMoveCoolTime += cachedDeltaTime;
		} else {
			if(_currentTarget == null) {
				_randomTargetDirectionNormalized = new Vector3(Random.Range(-400, 400), Random.Range(-400, 400));
				//Debug.LogError(_randomTargetDirection.x - cachedTransform.position.x);
				//Debug.LogError(_randomTargetDirection.y - cachedTransform.position.y);
			}
			_currentMoveCoolTime = 0;
			move();
		}
	}

	private void UpdateAttackCooldownAction () {
		if(_isAttackOnCooldown) {
			_currentAttackCooldown += cachedDeltaTime;
			if(_currentAttackCooldown < attackCooldown){
			} else {
				_currentAttackCooldown = 0;
				_isAttackOnCooldown = false;
			}
		}
	}

	public override void UpdateMoveAction () {
		if(!canMove)
			return;
		_currentMoveDurTime += cachedDeltaTime;
		if(_currentMoveDurTime < moveDuration) {
			Vector3 normDir = (_currentTarget != null) ? (_currentTarget.position - cachedTransform.position).normalized:
				_randomTargetDirectionNormalized.normalized;
			if(_currentMoveDurTime < (moveDuration / 2f)) {
				normDir = _randomMoveDirBeforeTheOrigTargetDirNormalized.normalized;
			}

			cachedTransform.position += normDir * moveSpeed * cachedDeltaTime;
			cachedTransform.localScale = new Vector3( ((normDir.x > 0) ? -1: 1), 1, 1);
		} else {
			_currentMoveDurTime = 0;
			idle();
		}
	}
	
	protected override void CharacterStateStarted (CharacterState state) {
		base.CharacterStateStarted(state);
		switch(state) {
		case CharacterState.Hurt:
			StaticAnimationsManager.getInstance.setBlinkingAnimation (false, cachedTransform, 0.2f, cWrapMode.Once);
			break;
		case CharacterState.Attack:
			//_boxCollider2D.enabled = false;
			_isAttackOnCooldown = true;
			break;
		case CharacterState.Dead:
			canMove = false;

			int dropCheckVal = Random.Range (0, 100);
			if (dropCheckVal < (int)Game.ItemType.Haste)
				_itemsMap [Game.ItemType.Haste].DropItem ();

			else if (dropCheckVal < (int)Game.ItemType.Heal)
				_itemsMap [Game.ItemType.Heal].DropItem ();
			
			else if (dropCheckVal < (int)Game.ItemType.Rage)
				_itemsMap [Game.ItemType.Rage].DropItem ();
			
			else if (dropCheckVal < (int)Game.ItemType.Silvercoin)
				_itemsMap [Game.ItemType.Silvercoin].DropItem ();

			else if (dropCheckVal < (int)Game.ItemType.Goldcoin)
				_itemsMap [Game.ItemType.Goldcoin].DropItem ();
			break;
		}
	}
	protected override void CharacterStateFinished (CharacterState state){
		base.CharacterStateFinished(state);
		switch(state) {
		case CharacterState.Hurt:
			if(_currentMoveDurTime != 0)
				move();
			else
				idle();
			break;
		case CharacterState.Attack:
			idle();
			break;
		case CharacterState.Dead:
			if(onEnemyDeadFinished != null)
				onEnemyDeadFinished(score);
			setActiveInScene(false);
			break;
		}
	}

	#region ZombieTypes Behaviours
	protected virtual void UpdateEnemyBehaviour () {
		switch(enemyType) {
		case EnemyType.Aggressive:
			UpdateAggressiveEnemy();
			break;
		case EnemyType.Stealth:
			UpdateStealthEnemy();
			break;
		case EnemyType.Jumper:
			UpdateJumper();
			break;
		}
	}

	private bool _aggressiveOnDistanceThreshold = false;
	private void UpdateAggressiveEnemy () {
		if(!_aggressiveOnDistanceThreshold) {
			if((Vector3.Distance(cachedTransform.position, _currentTarget.position/*_playerControllerInstance.cachedTransform.position*/)) < 320f) {
				StartCoroutine(StartAggressiveBuffUpAnimCoroutine());
				_aggressiveOnDistanceThreshold = true;
			}
		} else {
			if((Vector3.Distance(cachedTransform.position, _currentTarget.position/*_playerControllerInstance.cachedTransform.position*/)) > 640f) {
				StartCoroutine(StartAggressiveDeBuffUpAnimCoroutine());
				_aggressiveOnDistanceThreshold = false;
			}
		}
	}
	private IEnumerator StartAggressiveBuffUpAnimCoroutine () {
		idle();
		iTween.PunchScale(gameObject, new Vector3(1.5f, 1.5f, 1), 0.5f);
		yield return new WaitForSeconds(0.5f);
		cachedTransform.localScale = new Vector3(1.5f, 1.5f, 1);
		move();
		moveSpeed *= 1.5f;
		UpdateCharacterStats();
	}
	private IEnumerator StartAggressiveDeBuffUpAnimCoroutine () {
		iTween.PunchScale(gameObject, new Vector3(0.8f, 0.8f, 1), 0.5f);
		idle();
		yield return new WaitForSeconds(0.5f);
		move();
		cachedTransform.localScale = new Vector3(1f, 1f, 1);
		moveSpeed /= 1.5f;
		UpdateCharacterStats();
	}

	private void UpdateStealthEnemy () {
		float stealthThreshold = 500f;
		float distance = (Vector3.Distance(cachedTransform.position, _playerControllerInstance.cachedTransform.position));
		float result =  distance / stealthThreshold > 1 ? 1: (distance / stealthThreshold) - 0.5f;
		if(result < 0)
			result = 0;
		Debug.Log (distance + " " + result + " " + _spriteRenderer.gameObject);

		_spriteRenderer.color = Color.Lerp(Color.grey, Color.clear, 
		                                  result);
	}

	AnimationsMashUp _jumperAnimMashup; 
	private bool _canJump = false;
	private float _jumpCooldown = 7f;
	private float _jumpCurrTime;
	private void UpdateJumper () {
		float jumpThreshold = 1000;
		float distance = (Vector3.Distance(cachedTransform.position, _playerControllerInstance.cachedTransform.position));
		if(_canJump) {
			if(distance < jumpThreshold) {
				StartCoroutine(StartJumpCoroutine());
			}
		} else {
			_jumpCurrTime += cachedDeltaTime;
			if(_jumpCurrTime > _jumpCooldown) {
				_canJump = true;
				_jumpCurrTime = 0f;
			}
		}
	}
	private IEnumerator StartJumpCoroutine () {
		_canJump = false;
		idle();
		DoCharacterState(CharacterState.Idle);
		//canMove = false;
		_playerControllerInstance.NotifyDanger();
		yield return new WaitForSeconds(1.0f);
		_boxCollider2D.enabled = false;
		attack();

		_jumperAnimMashup.target = cachedTransform;
		_jumperAnimMashup.animationTime = 0.5f;
		_jumperAnimMashup.setBezierAnim (true, 
		                          new List<Vector2>(){
			new Vector2(cachedTransform.position.x, cachedTransform.position.y),
			new Vector2(Random.Range(_playerControllerInstance.cachedTransform.position.x, cachedTransform.position.x),
			            _playerControllerInstance.cachedTransform.position.y + 300f),
						_playerControllerInstance.cachedTransform.position
				
		});
		cachedTransform.localScale = new Vector3(_playerControllerInstance.cachedTransform.position.x < cachedTransform.position.x ? -1f: 1f, 
		                                         1f, 1f);
		_jumperAnimMashup.start ();
		yield return new WaitForSeconds(_jumperAnimMashup.animationTime);
		_boxCollider2D.enabled = true;
		move();

	}
	#endregion
}