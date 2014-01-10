using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {

	private const string PREFAB_PATH = "Prefabs/2D/_Item/ItemContainer";
	public static GameObject CreateItemContainer (GameObject parent) {
		return StaticManager_Helper.CreatePrefab (PREFAB_PATH, parent);
	}

	[System.NonSerialized]public Game.ItemType itemType;
	public Transform originalParent;
	public Transform dropParent;
	public float duration;
	[System.NonSerialized]public int value;
	protected bool _isInitialized = false;
	protected AnimationsMashUp _animMashup;
	protected SpriteRenderer _spriteRenderer;
	protected Transform _cachedTransform;

	protected BoxCollider2D _boxCollider2D;

	void Awake () {
		Init ();
	}

	public virtual void Init () {
		if(!_isInitialized) {
			duration = 5f;
			if(transform.parent != null)
				originalParent = transform.parent;
			dropParent = Game.instance.game2DRoot.transform;
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_cachedTransform = transform;
			_boxCollider2D = collider2D as BoxCollider2D;
			//InitAnimMashup();
			//Invoke("DropItem", 0.5f);
			_isInitialized = true;
		}
	}


	public virtual void DropItem () {
		gameObject.SetActive(true);
		_cachedTransform.parent = dropParent;
		Vector2 pos = _cachedTransform.position;
		AnimationsMashUp tAnimMashup = StaticAnimationsManager.getInstance.getAvailableAnimMashUp;
		tAnimMashup.target = _cachedTransform;
		tAnimMashup.animationTime = 0.5f;
		tAnimMashup.setBezierAnim(true, new List<Vector2>(){ pos, pos + (Vector2.up * 100) , 
			(pos - (new Vector2(Random.Range(-100f,100f), 50f)))
				});
		tAnimMashup.start(false);
	}

	public virtual int GetValue () {
		StartCoroutine(StartPickedUpAnimationCoroutine());
		return value;
	}

	private IEnumerator StartPickedUpAnimationCoroutine () {
		_boxCollider2D.enabled = false;
		AnimationsMashUp tAnimMashup = StaticAnimationsManager.getInstance.getAvailableAnimMashUp;
		tAnimMashup.target = _cachedTransform;
		tAnimMashup.setMoveAnim(transform.position, transform.position + Vector3.up * 120f, true);
		tAnimMashup.setFadeAnim(1.0f, 0f);
		tAnimMashup.start (false);
		yield return new WaitForSeconds(tAnimMashup.animationTime);
		_boxCollider2D.enabled = true;
		_cachedTransform.parent = originalParent;
		_cachedTransform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
		Color color = _spriteRenderer.color;
		color.a = 1.0f;
		_spriteRenderer.color = color;
		OnPickedUpAnimationFinished();
	}

	protected virtual void OnPickedUpAnimationFinished () {
	}

	void OnCollisionEnter2D (Collision2D collision) {
	}

	void OnTriggerEnter2D (Collider2D collision) {
	}
}


