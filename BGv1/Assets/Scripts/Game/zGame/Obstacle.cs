using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col) {
		GameObject colGo = col.gameObject;
		switch(colGo.layer) {
			case (int)Game.LayerType.Player:
			case (int)Game.LayerType.Enemy:
			colGo.GetComponent<BasicCharacterController>().moveSpeed /= 2f;
			colGo.GetComponent<BasicCharacterController>().UpdateCharacterStats();
				break;
		}

	}
	
	void OnCollisionExit2D(Collision2D col) {
		GameObject colGo = col.gameObject;
		switch(colGo.layer) {
		case (int)Game.LayerType.Player:
		case (int)Game.LayerType.Enemy:
			colGo.GetComponent<BasicCharacterController>().moveSpeed *= 2f;
			colGo.GetComponent<BasicCharacterController>().UpdateCharacterStats();
			break;
		}
	}
}
