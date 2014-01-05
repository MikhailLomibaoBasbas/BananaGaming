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
		return "Prefabs/2D/Enemy/Enemy" + type.ToString();
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
	
	public Transform originalTarget;

	protected Transform _currentTarget;
	public Transform setCurrentTarget {
		set {
			_currentTarget = value;
		}
	}

	private int _originalHealth;
	public int getOriginalHealth {get{return _originalHealth;}}
	private float _originalMoveSpeed;
	public float getOriginalMoveSpeed {get{return _originalMoveSpeed;}}


	public EnemyType enemyType;
	protected SpriteRenderer _spriteRenderer;

	private CircleCollider2D _attackCircleCollider;


	private float getTargetDistance {get {return Vector3.Distance(_currentTarget.position, cachedTransform.position);}}
	private float _distanceThreshold = 80f;

	public delegate void OnEnemyDeadFinished (int score);
	public event OnEnemyDeadFinished onEnemyDeadFinished = null;

	private bool _canMove = false;

	public override void Init () {
		//Before Base Init Statements
		_currentTarget = originalTarget;
		_originalHealth = health;
		_originalMoveSpeed = moveSpeed;
		base.Init ();

		//After Base Init Statements
		_attackCircleCollider = transform.FindChild("attackCollider").GetComponent<CircleCollider2D>();
		_attackCircleCollider.enabled = false;
		//SetCharacterStateStartEventListener(OnEnemyStateStarted);
		//SetCharacterStateFinishedEventListener(OnEnemyStateFinished);
		_spriteRenderer = GetComponent<SpriteRenderer>();
		//move();
	}

	public void setActiveInScene (bool flag, Vector3 pos = default(Vector3), bool isGlobal = true, bool withDelay = true) {
		float delay = (withDelay) ? Random.Range(0.5f, 1.0f): 0.1f;
		//Debug.LogError(Time.time + " " + delay);
		isActiveInGame = flag;
		enabled = flag;
		if(flag) {
			health = _originalHealth;
			gameObject.SetActive(flag);
			if(pos != default(Vector3))
				cachedTransform.SetPosition(pos, isGlobal);
			iTween.ColorTo(gameObject, Color.white, delay);
			Invoke("PostSetActiveScene_True", delay);
		} else {
			_canMove = false;
			cachedCollider2D.enabled = false;
			iTween.ColorTo(gameObject, Color.clear, delay);
			Invoke("PostSetActiveInScene_False", delay);
		}
	}
	private void PostSetActiveScene_True() {
		//Debug.LogError("fds");
		_canMove = true;
		cachedCollider2D.enabled  = true;
		move();
	}
	private void PostSetActiveInScene_False() {
		gameObject.SetActive(false);
		cachedTransform.localPosition = Vector3.zero;
	}


	private void move () {
		DoCharacterState(CharacterState.Move);
		animator.SetFloat("Acceleration", 1f);
	}  

	private void idle () {
		animator.SetFloat("Acceleration", 0);
		DoCharacterState(CharacterState.Idle);
		_currentMoveDurTime = 0;
	}

	private void attack () {
		cachedTransform.localScale = new Vector3(_currentTarget.position.x < cachedTransform.position.x ? -1f: 1f, 1f ,1f);
		DoCharacterState(CharacterState.Attack);
		_attackCircleCollider.enabled = true;
		Invoke("DisableAttackCollider", getAttackTime * 0.20f);
	}
	private void DisableAttackCollider() { _attackCircleCollider.enabled = false;}

	public override void OnUpdate () {
		base.OnUpdate ();
		if(!isHurt && !isDead)
			UpdateMoveCooldownAction();
		UpdateAttackCooldownAction();
		UpdateEnemyBehaviour();

		if((getTargetDistance < _distanceThreshold)) {
			if(!_isAttackOnCooldown) {
				attack();
			}
		}

	}

	private void UpdateMoveCooldownAction () {
		if(getTargetDistance < _distanceThreshold )
			return;
		_currentMoveCoolTime += cachedDeltaTime;
		if(_currentMoveCoolTime < moveCooldown) {
		} else {
			move();
			_currentMoveCoolTime = 0;
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
		if(!_canMove)
			return;
		_currentMoveDurTime += cachedDeltaTime;
		if(_currentMoveDurTime < moveDuration) {
			Vector3 normDir = (_currentTarget.position - cachedTransform.position).normalized;
			cachedTransform.position += normDir * moveSpeed * cachedDeltaTime;
			cachedTransform.localScale = new Vector3( ((normDir.x > 0) ? 1: -1), 1, 1);
		} else {
			_currentMoveDurTime = 0;
			idle();
		}
	}
	
	protected override void CharacterStateStarted (CharacterState state) {
		switch(state) {
		case CharacterState.Hurt:
			break;
		case CharacterState.Attack:
			_isAttackOnCooldown = true;
			break;
		}
	}
	protected override void CharacterStateFinished (CharacterState state){
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

	private void UpdateAggressiveEnemy () {
		if((Vector3.Distance(cachedTransform.position, PlayerController.GetInstance.cachedTransform.position)) < 320f) {
			if(_currentTarget != PlayerController.GetInstance.cachedTransform) {
				_currentTarget = PlayerController.GetInstance.cachedTransform;
				moveSpeed *= 2f;
				UpdateCharacterStats();
			}

		} else {
			if(_currentTarget != originalTarget) {
				_currentTarget = originalTarget;
				moveSpeed /= 2f;
				UpdateCharacterStats();
			}
		}
	}

	private void UpdateStealthEnemy () {
		float stealthThreshold = 250f;
		float distance = (Vector3.Distance(cachedTransform.position, PlayerController.GetInstance.cachedTransform.position));
		float result =  distance / stealthThreshold > 1 ? 1: (distance / stealthThreshold) - 0.5f;
		if(result < 0)
			result = 0;
		_spriteRenderer.color = Color.Lerp(Color.white, Color.clear, 
		                                  result);
	}

	bool didJump = false;
	private void UpdateJumper () {
		float jumpThreshold = 800;
		float distance = (Vector3.Distance(cachedTransform.position, PlayerController.GetInstance.cachedTransform.position));
		if(distance < jumpThreshold) {
			if(didJump)
				return;
			DoCharacterState(CharacterState.Attack);
			didJump = true;
			(collider2D as BoxCollider2D).enabled = false;
			AnimationsMashUp animMashup = StaticAnimationsManager.getInstance.getAvailableAnimMashUp;
			animMashup.target = cachedTransform;
			animMashup.animationTime = 0.8f;
			animMashup.setBezierAnim (true, 
			                          new List<Vector2>(){
				new Vector2(cachedTransform.position.x, cachedTransform.position.y),
				new Vector2(Random.Range(PlayerController.GetInstance.cachedTransform.position.x, cachedTransform.position.x),
				           cachedTransform.position.y + 200f),
				new Vector2(PlayerController.GetInstance.cachedTransform.position.x, PlayerController.GetInstance.cachedTransform.position.y)
					
			});
			animMashup.start ();
			Invoke("JumpFinished", animMashup.animationTime);
		}
	}
	private void JumpFinished(){(collider2D as BoxCollider2D).enabled = true;}
	#endregion
}