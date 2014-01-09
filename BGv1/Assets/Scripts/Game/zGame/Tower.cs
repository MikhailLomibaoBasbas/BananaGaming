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
	}

	private void Init () {
	}


	void OnTriggerEnter2D(Collider2D col) {
		Debug.LogError(col);
		if(col.gameObject.layer == _hurtTrigger) {
			health -= col.GetComponent<EnemyProjectile>().GetDamage(null);
			StopAllCoroutines();
			StartCoroutine(DamageAnimCoroutine());
		}
	}

	private IEnumerator DamageAnimCoroutine () {
		_spriteRenderer.color = Color.red;
		yield return new WaitForSeconds(1.0f);
		_spriteRenderer.color = Color.white;
	}

}
