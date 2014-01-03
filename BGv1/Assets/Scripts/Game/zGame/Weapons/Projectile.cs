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

	protected bool _isActive = false;
	public bool isActive {get {return _isActive;}}
	protected Animator _animator;
	public int targetTriggerLayer;

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
	public void SetValues (Transform wc, float time, float dist) {
		_weaponContainer = wc;
		_time = time;
		_distance = dist;
	}
	public virtual void Show(bool isRight){
		_isActive = true;
		_collider.enabled = true;
		_spriteRenderer.enabled = true;

		//float multiplier = isRight ? 1f: -1f;
		//Vector3 tempScale = _cachedTransform.localScale;
		//tempScale.x *= multiplier;
		//_cachedTransform.localScale = tempScale;
		_cachedTransform.parent = _onActiveParent;
	}
	public virtual void Hide(){
		_isActive = false;
		_collider.enabled = false;
		_spriteRenderer.enabled = false;
		_cachedTransform.parent = _weaponContainer;
		_cachedTransform.localPosition = Vector3.zero;
	}
}
