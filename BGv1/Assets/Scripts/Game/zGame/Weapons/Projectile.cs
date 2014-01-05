using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	protected Transform _onActiveParent;
	protected Transform _weaponContainer;
	protected float _distance;
	protected float _time;
	protected float _currTime;
	protected Transform _cachedTransform;
	protected Collider2D _collider;
	protected SpriteRenderer _spriteRenderer;
	protected int _damage;

	protected bool _isActive = false;
	public bool isActive {get {return _isActive;}}
	protected Animator _animator;
	public int targetTriggerLayer;

	protected bool _updateEnabled = false;

	void Awake () {
		Invoke("Init", 0.01f);
	}
	protected virtual void Init () {
		_cachedTransform = transform;
		_onActiveParent = Game.instance.game2DRoot.transform;
		_animator = GetComponent<Animator>();
		_collider = collider2D;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_collider.enabled = false;
		_spriteRenderer.enabled = false;
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
		_cachedTransform.parent = _onActiveParent;
		_updateEnabled = true;
	}
	public virtual void Hide(){
		SetActiveInScene(false);
		_cachedTransform.parent = _weaponContainer;
		_cachedTransform.localPosition = Vector3.zero;
	}
	public virtual void OnUpdate () {}
	void Update () {
		OnUpdate();
	}
	public void SetActiveInScene (bool flag) {
		_isActive = flag;
		_collider.enabled = flag;
		_spriteRenderer.enabled = flag;
	}
	public virtual int GetDamage () {
		CancelInvoke("Hide");
		_updateEnabled = false;
		//_animator.SetTrigger("BloodSplat", true);
		Invoke("Hide", _time);
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
