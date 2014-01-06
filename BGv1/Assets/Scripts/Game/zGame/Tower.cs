using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	public enum Features {
		Fortify,
		ReplenishHealth,
		Attack
	}

	public int health;
	public Game.LayerType hurtTriggerType;
	private int _hurtTrigger;

	void Awake () {
	}

	private void Init () {
	}


	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.layer == _hurtTrigger) {
			health--;
			GetComponent<SpriteRenderer>().color = Color.red;
		}
	}

}
