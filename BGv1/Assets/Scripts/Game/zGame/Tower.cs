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
	private

	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.gameObject.layer == _hurtTrigger) {
			health--;
			GetComponent<SpriteRenderer>().color = Color.red;
		}
	}

}
