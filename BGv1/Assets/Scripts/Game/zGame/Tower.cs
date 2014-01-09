using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	private static Tower _instance = null;
	public static Tower GetInstance {
		get {
			if(_instance == null)
				_instance = GameObject.FindObjectOfType(typeof(Tower)) as Tower;
			return _instance;
		}
	}

	public delegate void OnTowerHit (int health);
	public event OnTowerHit onTowerHit;

	public enum Features {
		Fortify,
		ReplenishHealth,
		Attack
	}

	public int health;
	public Game.LayerType hurtTriggerType;
	private int _hurtTrigger;
	private SpriteRenderer _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_hurtTrigger = (int)hurtTriggerType;
	}

	private void Init () {
	}

	public void AddTowerDeadListener (OnTowerHit listener) {
		if(onTowerHit != null)
			onTowerHit = null;
		onTowerHit += listener;
	}

	public void RemoveListeners () {
		onTowerHit = null;
	}
	
	void OnTriggerEnter2D(Collider2D col) {
		Debug.LogError (col.gameObject.layer + " " + hurtTriggerType);
		Damage (col.gameObject);
	}

	private void Damage (GameObject go) {
		if(go.layer == _hurtTrigger) {
			health -= go.GetComponent<EnemyProjectile>().GetDamage(null);
			if (health > 0) {
				StopAllCoroutines ();
				StartCoroutine (DamageAnimCoroutine ());
			}
			Debug.LogError("Col " + go + "  health " + health);
			if(onTowerHit != null)
				onTowerHit (health);
		}
	}

	private IEnumerator DamageAnimCoroutine () {
		_spriteRenderer.color = Color.red;
		yield return new WaitForSeconds(0.1f);
		_spriteRenderer.color = Color.white;
	}

}
