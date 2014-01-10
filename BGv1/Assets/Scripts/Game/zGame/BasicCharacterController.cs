using UnityEngine;
using System.Collections;

public class BasicCharacterController : MonoBehaviour, ICharacterController {

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
	public float baseAttackTime;
	private float _defaultAnimTime = default(float);
	protected float defaultAnimTime{
		get{
			if(_defaultAnimTime == default(float))
				_defaultAnimTime = fpa / fps;
			return _defaultAnimTime;
		}

	}
	public int health;
	public float attackSpeed;
	public float moveSpeed;
	public bool attackEnabled;
	public bool bloodEnabled;
	private Transform _bloodContainers;
	private Animator[] _bloodSplatterAnimators;
	public float rcDistanceToAttack; //For Raycast
	public float flinchForce;
	public AudioClip[] audioClips;
	protected bool isIdle;
	protected bool isMoving;
	protected bool isAttacking;
	protected bool isDead;
	protected bool isHurt;
	protected Animator animator;
	public Transform cachedTransform{get;set;}
	public Rigidbody2D cachedRigidBody2D{get;set;}
	public Collider2D cachedCollider2D{get;set;}
	protected CharacterStats characterStats;

	private CharacterState lastState;
	public CharacterState currentState;
	//protected Renderer mRenderer;
	protected Vector2 originalPos;

	bool isOccupied;

	bool isInitalized = false;
	protected bool isActiveInGame = false;
	protected float cachedDeltaTime;

	//For Raycast and Collider
	protected int originalLayer;
	protected float rcDirectionMult; //-1 or 1 only, -1 = left, 1 = right
	protected int rcLayerMaskTarget;
	public Game.LayerType hurtTriggerType;
	//public Game.layer hurtTriggerType;
	public bool hasHurtInvulnerability;
	private int hurtTrigger;

	private float movementCap;

	private float _attackDelayMult = 0.63f;

	public Vector2 minClamp;
	public Vector2 maxClamp;

	#region Computed Values Getters
	public float getTranslateUnitsPerSecond {
		get {return characterStats.getTranslateUnitsPerSecond;}
	}
	public float getMoveAnimSpeed {
		get { return getTranslateUnitsPerSecond / 300f;/*7.33f;*/}
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
		characterStats = new CharacterStats(health, defaultAnimTime * baseAttackTime, attackSpeed, moveSpeed);
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
		isIdle = true;
		currentState = lastState = CharacterState.Idle;
		cachedTransform = transform;
		cachedRigidBody2D = rigidbody2D;
		cachedCollider2D = collider2D;
		animator = GetComponent<Animator>();
		animator.speed = 1f;
		//mRenderer = GetComponentInChildren<Renderer>();
		isActiveInGame = true;
		currentState = CharacterState.None;
		originalPos = cachedTransform.position;
		originalLayer = gameObject.layer;
		hurtTrigger = (int)hurtTriggerType;
		//Debug.LogError(collider2D.GetType() == typeof(BoxCollider2D));
		//Physics2D.IgnoreLayerCollision(hurtTrigger, gameObject.layer, true);
		InitRaycast();
		InitCharacterStats();
		InitBloodSplatter();
	}
	protected virtual void InitRaycast () {
		rcDirectionMult = 1f;
	}
	protected virtual void InitCharacterStats () {
		UpdateCharacterStats();
	}
	private void InitBloodSplatter () {
		if(bloodEnabled) {
			if((_bloodContainers = cachedTransform.FindChild("_BloodSplatterContainer")) != null) {
				_bloodSplatterAnimators = _bloodContainers.GetComponentsInChildren<Animator>();
				for(int i = 0; i < _bloodSplatterAnimators.Length; i++) {
					_bloodSplatterAnimators[i].SetGameObjectActive(false);
				}
			}
		}
	}
	#endregion

	#region Update Methods

	void Update () {
		if(isActiveInGame) {
			//if(!mRenderer.isVisible && currentState != CharacterState.Move)
				//return;
			OnUpdate();
		}
	}
	public virtual void OnUpdate () {	
		cachedDeltaTime = Time.deltaTime;
		UpdateClamp();
		if(attackEnabled) {
			if(isAttacking) {
				UpdateAttackAction();
				return;
			}
			/*if(AreTargetsNear()) {
				UpdateTargetsNearAction();
				return;
			}*/
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

	void UpdateClamp () {
		Vector3 tempPos = transform.position;
		transform.position = new Vector3(Mathf.Clamp(tempPos.x, minClamp.x, maxClamp.x),
		                                 Mathf.Clamp(tempPos.y, minClamp.y, maxClamp.y));
	}

	// Hurt Checker
	void OnTriggerEnter2D (Collider2D col2d) { // If this does not work. Try OnCollisionEnter2D(Collision2D collision)
		CheckCollision(col2d.gameObject);
	}
	void OnCollisionEnter2D(Collision2D collision) {
		CheckCollision(collision.gameObject);
	}
	private void CheckCollision(GameObject colGO) {
		if(colGO.layer == hurtTrigger) {
			StartCollideHurtAction(colGO);
		}
		switch(colGO.layer) {
		case (int)Game.LayerType.Item:
			//Debug.LogError ("ss");
				StartCollidedItemAction(colGO);
				break;
		}
	}
	protected virtual void StartCollideHurtAction (GameObject go) {
		Projectile tProj = go.GetComponent<Projectile>();
		if (tProj == null)
			tProj = go.transform.parent.GetComponent<Projectile> ();
		int tempDmg = tProj.GetDamage(go.transform);
		health -= tempDmg;
		
		DoCharacterState( (health > 0) ? CharacterState.Hurt: CharacterState.Dead);
		cachedTransform.position -= Vector3.right * (go.transform.position - cachedTransform.position).normalized.x * flinchForce;
		if(hasHurtInvulnerability)
			Physics2D.IgnoreLayerCollision(hurtTrigger, gameObject.layer, true);
	}
	protected virtual void StartCollidedItemAction (GameObject go) {
	}
	// End Hurt Check


	//Check for targets nearby. 
	bool AreTargetsNear () {
		Debug.DrawRay(cachedTransform.position, Vector2.right * rcDirectionMult * rcDistanceToAttack, Color.white);
		return (Physics2D.Raycast(cachedTransform.position, Vector2.right * rcDirectionMult, rcDistanceToAttack, rcLayerMaskTarget));
	}	
	//End TargetCheck


	public virtual void UpdateDeadAction () {}
	public virtual void UpdateMoveAction () {/*UpdateTranslate(); UpdateTranslate2();*/}
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
		//Debug.LogError(getAttackTime + " " + getAttackPerSecond);
		animator.speed = getAttackPerSecond;
		DoCharacterStateStartEvent();
		StopCharacterStateWithDelay(characterStats.getAttackTime * _attackDelayMult);
	}
	void Move(bool mWillTranslate = false) {
		SetCurrentState(CharacterState.Move);
		//Debug.Log(getMoveAnimSpeed);
		animator.speed = getMoveAnimSpeed;
		DoCharacterStateStartEvent();
	}
	 void Hurt () {
		SetCurrentState(CharacterState.Hurt);
		animator.speed = 1f;
		//cachedRigidBody2D.velocity = (flinchForce);
		//if(hasHurtInvulnerability)
			//gameObject.layer = (int)Game.LayerType.HurtDead;
		StartBloodAnim();
		DoCharacterStateStartEvent();
		StopCharacterStateWithDefaultDelay();
	}
	void Dead () {
		SetCurrentState(CharacterState.Dead);
		gameObject.layer = (int)Game.LayerType.HurtDead;
		DoCharacterStateStartEvent();
		StopCharacterStateWithDefaultDelay();
	}
	void None () {
		SetCurrentState(CharacterState.None);
	}

	void SetCurrentState (CharacterState state) {
		if(state != CharacterState.None && state != CharacterState.Move && state != CharacterState.Idle)
			isOccupied = true;
		/*gameObject.layer = (state == CharacterState.Hurt || state == CharacterState.Dead) ?
			(int)Game.LayerType.HurtDead: originalLayer;*/
		CancelInvoke("StopCharacterState");
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
	public void RemoveCharacterStateListeners () {
		onCharacterStateStart = null;
		onCharacterStateFinished = null;
	}

	void StopCharacterStateWithDelay(float delay) {
		Invoke("StopCharacterState", delay * 0.9f);
	}
	void StopCharacterStateWithDefaultDelay() {
		StopCharacterStateWithDelay(defaultAnimTime);
	}
	void StopCharacterState() {
		DoCharacterStateFinishedEvent();
		isOccupied = false;
		//if(onCharacterStateFinished == null) PARANG NEED TO NA EWAN
			DoCharacterState(CharacterState.Idle);

	}
	protected void DoCharacterStateFinishedEvent () {
		if(onCharacterStateFinished != null)
			onCharacterStateFinished(currentState, this);
		CharacterStateFinished(currentState);
	}
	protected void DoCharacterStateStartEvent () {
		if(onCharacterStateStart != null)
			onCharacterStateStart(currentState, this);
		CharacterStateStarted(currentState);
	}
	protected virtual void CharacterStateStarted (CharacterState state) {
	}
	protected virtual void CharacterStateFinished(CharacterState state){
		switch(state) {
		case CharacterState.Hurt:
			if(hasHurtInvulnerability) {
				gameObject.layer = originalLayer;
				Physics2D.IgnoreLayerCollision(hurtTrigger, gameObject.layer, false);
			}
			break;
		case CharacterState.Dead:
			gameObject.layer = originalLayer;
			break;
		}
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

	#region Blood Anim
	private void StartBloodAnim () {
		if(bloodEnabled) 
			StartCoroutine(StartBloodAnimCoroutine());
	}
	private IEnumerator StartBloodAnimCoroutine () {
		Animator bsAnimator = null;
		for(int i = 0; i < _bloodSplatterAnimators.Length; i++) {
			bsAnimator = _bloodSplatterAnimators[i];
			if(!bsAnimator.gameObject.activeSelf) {
				//Debug.LogError("hahaha");
				bsAnimator.SetGameObjectActive(true);
				string triggerStr = Random.Range(0, 100) < 70 ? "Splat1": "Splat2";
				//Debug.LogError(triggerStr);
				bsAnimator.SetTrigger(triggerStr);
				bsAnimator.transform.parent = Game.instance.game2DRoot.transform;
				break;
			}
		}
		if(bsAnimator == null)
			yield break;
		yield return new WaitForSeconds(44f / 60f);
		bsAnimator.transform.parent = _bloodContainers;
		bsAnimator.transform.localPosition = Vector3.zero;
		bsAnimator.SetGameObjectActive(false);
	}
	#endregion
}