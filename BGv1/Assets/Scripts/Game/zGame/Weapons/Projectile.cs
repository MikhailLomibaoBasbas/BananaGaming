using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	protected Transform _onActiveParent;
	protected Transform _weaponContainer;
	protected float _distance;
	protected float _time;
	protected float _currTime;
	protected Transform _cachedTransform;
	protected Collider2D _collider2D;
	protected SpriteRenderer _spriteRenderer;
	protected int _damage;

	protected bool _isActive = false;
	public bool isActive {get {return _isActive;}}
	protected Animator _animator;
	//public int targetTriggerLayer;

	protected bool _updateEnabled = false;

	protected bool _didInit = false;

	void Awake () {
		Invoke("Init", 0.01f);
	}
	protected virtual void Init () {
		if(!_didInit) {
			_cachedTransform = transform;
			_onActiveParent = Game.instance.game2DRoot.transform;
			_animator = GetComponent<Animator>();
			_collider2D = collider2D;
			_spriteRenderer = GetComponent<SpriteRenderer>();
			//_collider2D.enabled = false;
			//_spriteRenderer.enabled = false;
			SetActiveInScene(false);
			_didInit = true;
		}
	}
	public void SetValues (Transform wc, float time, float dist, int dmg) {
		_weaponContainer = wc;
		_time = time;
		_distance = dist;
		_damage = dmg;
	}
	public virtual void Show(bool isRight){
		SetActiveInScene(true);
		//float multiplier = isRight ? 1f: -1f;
		//Vector3 tempScale = _cachedTransform.localScale;
		//tempScale.x *= multiplier;
		//_cachedTransform.localScale = tempScale;
		_currTime = 0;
		_cachedTransform.parent = _onActiveParent;
		_updateEnabled = true;
	}
	public virtual void Hide(){
		SetActiveInScene(false);
		//Debug.LogError(Time.time + "fds");
		_cachedTransform.parent = _weaponContainer;
		_cachedTransform.localPosition = Vector3.zero;
		_cachedTransform.localRotation = Quaternion.identity;
		_cachedTransform.localScale = Vector3.one;
		_updateEnabled = false;
		_currTime = 0;
	}
	public virtual void OnUpdate () {}
	void Update () {
		if(_updateEnabled)
			OnUpdate();
	}
	public void SetActiveInScene (bool flag) {
		/*if(!_didInit)
			Init();*/
		_isActive = flag;
		gameObject.SetActive(flag);
		//_collider2D.enabled = flag;
		//_spriteRenderer.enabled = flag;

	}
	public virtual int GetDamage () {
		CancelInvoke("Hide");
		//_animator.SetTrigger("BloodSplat", true);
		//Invoke("Hide", _time);
		Hide();
		return _damage;
	}

	/*void OntriggerEnter2D (Collider2D collider) {
		GameObject colGO = collider.gameObject;
		if(colGO.layer == targetTriggerLayer) {
			_animator.SetBool("", true);
			Hide();
		}
	}*/
}
