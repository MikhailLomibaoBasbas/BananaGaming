﻿using UnityEngine;
using System.Collections;

public class BasicCharacterController : MonoBehaviour, ICharacterController {

	public enum CharacterType {
		Enemy = 9,
		Player = 8,
		HurtDead = 12
	}
	
	public enum CharacterState {
		Idle,
		Move,
		Jump,
		Attack,
		Hurt,
		Dead,
		None
	}
	
	public enum GameType {
		Runner,
		ROF
	}

	public delegate void OnCharacterStateFinished (CharacterState state, BasicCharacterController instance);
	private event OnCharacterStateFinished onCharacterStateFinished;

	public delegate void OnCharacterStateStart (CharacterState state, BasicCharacterController instance);
	private event OnCharacterStateStart onCharacterStateStart;

	public float fpa;
	public float fps;
	private float defaultAnimTime;
	public int health;
	public float attackSpeed;
	public float moveSpeed;
	public bool attackEnabled;
	public float rcDistanceToAttack; //For Raycast
	public Vector2 flinchForce;
	public AudioClip[] audioClips;
	protected bool isIdle;
	protected bool isMoving;
	protected bool isAttacking;
	protected bool isDead;
	protected bool isHurt;
	Animator animator;
	public Transform cachedTransform{get;set;}
	public Rigidbody2D cachedRigidBody2D{get;set;}
	protected CharacterStats characterStats;

	private CharacterState lastState;
	public CharacterState currentState;
	protected Renderer mRenderer;
	protected Vector2 originalPos;

	bool isOccupied;

	bool isInitalized = false;
	bool isActiveInGame = false;
	protected float cachedDeltaTime;

	//For Raycast and Collider
	protected int originalLayer;
	protected float rcDirectionMult; //-1 or 1 only, -1 = left, 1 = right
	protected int rcLayerMaskTarget;
	protected int colHurtLayer;

	private float movementCap;

	#region Computed Values Getters
	public float getTranslateUnitsPerSecond {
		get {return characterStats.getTranslateUnitsPerSecond;}
	}
	public float getMoveAnimSpeed {
		get { return getTranslateUnitsPerSecond / 7.33f;}
	}
	public float getAttackPerSecond {
		get { return characterStats.getAttackPerSecond;}
	}
	public float getAttackTime {
		get { return characterStats.getAttackTime;}
	}
	#endregion

	#region UpdateCharacterStats
	public void UpdateCharacterStats () {
		characterStats = new CharacterStats(health, defaultAnimTime, attackSpeed, moveSpeed);
	}
	#endregion


	#region Init
	void Awake() {
		DoInit();
	}

	private void DoInit () {
		if(!isInitalized) {
			Init();
			isInitalized = true;
		}
	}
	public virtual void Init () {
		cachedTransform = transform;
		cachedRigidBody2D = rigidbody2D;
		animator = GetComponent<Animator>();
		//Debug.LogError(animator);
		animator.speed = 1f;
		mRenderer = GetComponentInChildren<Renderer>();
		isActiveInGame = true;
		defaultAnimTime = fpa / fps;
		currentState = CharacterState.None;
		originalPos = cachedTransform.position;
		originalLayer = gameObject.layer;
		InitRaycast();
		InitCharacterStats();
	}
	protected virtual void InitRaycast () {
		rcDirectionMult = 1f;
	}
	protected virtual void InitCharacterStats () {
		characterStats = new CharacterStats(health, defaultAnimTime, attackSpeed, moveSpeed);
	}
	#endregion

	#region Update Methods

	void Update () {
		if(isActiveInGame) {
			if(!mRenderer.isVisible && currentState != CharacterState.Move)
				return;
			OnUpdate();
		}
	}
	public virtual void OnUpdate () {	
		cachedDeltaTime = Time.deltaTime;
		if(attackEnabled) {
			if(isAttacking) {
				UpdateAttackAction();
				return;
			}
			if(AreTargetsNear()) {
				UpdateTargetsNearAction();
				return;
			}
		}
		if(isHurt) {
			UpdateHurtAction();
			return;
		}
		if(isDead) {
			UpdateDeadAction();
			return;
		}
		if(isIdle) {
			UpdateIdleAction();
			return;
		}

		UpdateMoveAction();

	}


	// Hurt Checker
	void OnTriggerEnter2D (Collider2D collider) { // If this does not work. Try OnCollisionEnter2D(Collision2D collision)
		GameObject colGO = collider.gameObject;
		if(colGO.layer == colHurtLayer) {
			DoCharacterState(CharacterState.Hurt);

			Physics2D.IgnoreLayerCollision(colHurtLayer, gameObject.layer, true);
		} 
	}
	void OnCollisionEnter2D(Collision2D collision) {
		GameObject colGO = collision.gameObject;
		if(colGO.layer == colHurtLayer) {
			DoCharacterState(CharacterState.Hurt);
			Physics2D.IgnoreLayerCollision(colHurtLayer, gameObject.layer, true);
		}
	}
	// End Hurt Check


	//Check for targets nearby. 
	bool AreTargetsNear () {
		Debug.DrawRay(cachedTransform.position, Vector2.right * rcDirectionMult * rcDistanceToAttack, Color.white);
		return (Physics2D.Raycast(cachedTransform.position, Vector2.right * rcDirectionMult, rcDistanceToAttack, rcLayerMaskTarget));
	}
	//End TargetCheck


	public virtual void UpdateDeadAction () {}
	public virtual void UpdateMoveAction () {/*UpdateTranslate();*/ UpdateTranslate2();}
	public virtual void UpdateIdleAction () {}
	public virtual void UpdateAttackAction () {}
	public virtual void UpdateHurtAction () {}
	public virtual void UpdateTargetsNearAction () {} //Mostlikely you will attack
	#endregion

	public void DoCharacterState (CharacterState state) {
		if(!isOccupied) {
			if(state != currentState) {
				if(state != CharacterState.None) {
					/*AudioClip clip = audioClips[(int)state];
					if(clip != null)
						static_audiomanager.getInstance.play_sfx(clip);*/
				}
				lastState = currentState;
				switch(state) {
					case CharacterState.Idle:
						Idle();
						break;
					case CharacterState.Attack:
						Attack();
						break;
					case CharacterState.Move:
						Move();
						break;
					case CharacterState.Hurt:
						Hurt();
						break;
					case CharacterState.Dead:
						Dead();
						break;
					case CharacterState.None:
						None();
						break;
					default:
						break;
				}
			}
		}
	}
	void Idle () {
		SetCurrentState(CharacterState.Idle);
		animator.speed = 1.0f;
		DoCharacterStateStartEvent();

	}
	void Attack () {
		SetCurrentState(CharacterState.Attack);
		animator.speed = characterStats.getAttackPerSecond;
		DoCharacterStateStartEvent();
		StopCharacterStateWithDelay(characterStats.getAttackTime);
	}
	void Move(bool mWillTranslate = false) {
		SetCurrentState(CharacterState.Move);
		animator.speed = getMoveAnimSpeed;
		DoCharacterStateStartEvent();
	}
	 void Hurt () {
		SetCurrentState(CharacterState.Hurt);
		animator.speed = 1f;
		--health;
		
		cachedRigidBody2D.velocity = (flinchForce);
		DoCharacterStateStartEvent();
		StopCharacterStateWithDefaultDelay();
	}
	void Dead () {
		SetCurrentState(CharacterState.Dead);
		DoCharacterStateStartEvent();
		StopCharacterStateWithDefaultDelay();
	}
	void None () {
		SetCurrentState(CharacterState.None);
	}

	void SetCurrentState (CharacterState state) {
		//Debug.LogWarning(cachedTransform + " " + state);
		if(state != CharacterState.None && state != CharacterState.Move && state != CharacterState.Idle)
			isOccupied = true;
		gameObject.layer = (state == CharacterState.Hurt || state == CharacterState.Dead) ?
			(int)CharacterType.HurtDead: originalLayer;
		CancelInvoke("StopCharacterState");
		//if(gameObject.name == "PlayAround-Hero2")
			//Debug.LogError(gameObject + " " + state);
		animator.SetBool("isHurt", isHurt = state == CharacterState.Hurt);
		animator.SetBool("isMoving", isMoving = state == CharacterState.Move);
		animator.SetBool("isIdle", isIdle = state == CharacterState.Idle);
		animator.SetBool("isAttacking", isAttacking = state == CharacterState.Attack);

		isDead = state == CharacterState.Dead;
		if(isDead)
			animator.SetTrigger("isDead");
		
			currentState = state;

	}
	#region Callback
	public void SetCharacterStateFinishedEventListener (OnCharacterStateFinished eventListener) {
		if(onCharacterStateFinished != null)
			onCharacterStateFinished = null;
		onCharacterStateFinished += eventListener;
	}
	public void SetCharacterStateStartEventListener (OnCharacterStateStart eventListener) {
		if(onCharacterStateStart != null)
			onCharacterStateStart = null;
		onCharacterStateStart += eventListener;
	}

	void StopCharacterStateWithDelay(float delay) {
		Invoke("StopCharacterState", delay * 0.9f);
	}
	void StopCharacterStateWithDefaultDelay() {
		StopCharacterStateWithDelay(defaultAnimTime);
	}
	void StopCharacterState() {
		isOccupied = false;
		if(currentState == CharacterState.Hurt) {
			Physics2D.IgnoreLayerCollision(colHurtLayer, gameObject.layer, false);
		}
		DoCharacterStateFinishedEvent();
		DoCharacterState(CharacterState.None);
	}
	protected void DoCharacterStateFinishedEvent () {
		if(onCharacterStateFinished != null)
			onCharacterStateFinished(currentState, this);
	}
	protected void DoCharacterStateStartEvent () {
		if(onCharacterStateStart != null)
			onCharacterStateStart(currentState, this);
	}
	#endregion

	#region Value Changer
	public void ChangeMoveSpeed (float speed, bool autoMove = false) {
		characterStats.moveSpeed = moveSpeed = speed;
		if(autoMove)
			Move();
	}
	public void ChangeAttackSpeed(float speed, bool autoAttack = false) {
		characterStats.moveSpeed = attackSpeed = speed;
		if(autoAttack)
			Attack ();
	}
	#endregion

	#region Tranlate Functions
	private Vector2 translateToPos;
	private Vector2 translateFrPos;
	private float translateTime;
	private float timeTranslating;
	protected bool isTranslating;
	public void TranslateTo (float posX, float playerTPS = default(float), float speed = 1.0f) { //playerTPS = player's translate per second.
		translateFrPos = cachedTransform.position;
		Vector2 tempPos = translateFrPos;
		tempPos.x = posX;
		translateToPos = tempPos;
		_translateMultiplier = translateToPos.x - translateFrPos.x > 0 ? 1f: -1f;
		_playerTPS = playerTPS;
		translateTime = (Vector2.Distance(translateToPos, translateFrPos) / 
			(characterStats.getTranslateUnitsPerSecond + playerTPS)) / speed;
		timeTranslating = 0f;
		isTranslating = true;
		DoCharacterState(CharacterState.Move);
	}

	public void TranslateTo(Vector3 pos, float playerTPS = default(float), float speed = 1.0f) {
		translateFrPos = cachedTransform.position;
		translateToPos = pos;
		_translateMultiplier = translateToPos.x - translateFrPos.x > 0 ? 1f: -1f;
		_playerTPS = playerTPS;
		translateTime = (Vector2.Distance(translateToPos, translateFrPos) / 
			(characterStats.getTranslateUnitsPerSecond + playerTPS)) / speed;
		timeTranslating = 0f;
		isTranslating = true;
		DoCharacterState(CharacterState.Move);
	}

	void UpdateTranslate () {
		if(isTranslating) {
			
			timeTranslating += cachedDeltaTime;
			if(timeTranslating < translateTime) {
				cachedTransform.position = Vector2.Lerp(translateFrPos, translateToPos, timeTranslating/translateTime);
			} else {
				cachedTransform.position = translateToPos;
				isTranslating = false;
				translateFrPos = default(Vector2);
				translateToPos = default(Vector2);
				timeTranslating = 0f;
				translateTime = default(float);
				DoCharacterStateFinishedEvent();
			}
		}
	}

	private float _translateMultiplier;
	private float _playerTPS;
	void UpdateTranslate2 () {
		if(isTranslating) {
			if(Vector3.Distance(translateToPos, cachedTransform.position) > 0.1f) {
				cachedTransform.position += Vector3.right * _translateMultiplier * cachedDeltaTime * (characterStats.getTranslateUnitsPerSecond + _playerTPS);
			} else {
				//Debug.LogError("sss");
				cachedTransform.position = translateToPos;
				isTranslating = false;
				translateFrPos = default(Vector2);
				translateToPos = default(Vector2);
				timeTranslating = 0f;
				translateTime = default(float);
				DoCharacterStateFinishedEvent();
			}
		}
	}

	public void GoToOriginalPosition () {
		cachedTransform.position = originalPos;
	}
	#endregion
}