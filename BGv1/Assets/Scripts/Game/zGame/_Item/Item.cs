using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {
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
			InitAnimMashup();
			Invoke("DropItem", 0.5f);
			_isInitialized = true;
		}
	}


	protected virtual void InitAnimMashup () {
		_animMashup = StaticAnimationsManager.getInstance.getAvailableAnimMashUp;
		_animMashup.target = transform;
		_animMashup.animationTime = 0.55f;
	}

	public virtual void DropItem () {
		_cachedTransform.parent = dropParent;
		Vector2 pos = _cachedTransform.position;
		_animMashup.setBezierAnim(true, new List<Vector2>(){ pos, pos + (Vector2.up * 100) , 
			(pos - (new Vector2(Random.Range(-100f,100f), 50f)))
				});
		_animMashup.start(false);
	}

	public virtual int GetValue () {
		StartCoroutine(StartPickedUpAnimationCoroutine());
		return value;
	}

	private IEnumerator StartPickedUpAnimationCoroutine () {
		_boxCollider2D.enabled = false;
		_animMashup.setMoveAnim(transform.position, transform.position + Vector3.up * 120f, true);
		_animMashup.setFadeAnim(1.0f, 0f);
		_animMashup.start(false);
		yield return new WaitForSeconds(_animMashup.animationTime);
		_boxCollider2D.enabled = true;
		_cachedTransform.parent = originalParent;
		gameObject.SetActive(false);
		Color color = _spriteRenderer.color;
		color.a = 1.0f;
		_spriteRenderer.color = color;
		OnPickedUpAnimationFinished();
	}

	protected virtual void OnPickedUpAnimationFinished () {
	}
}


