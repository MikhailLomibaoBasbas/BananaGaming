using UnityEngine;
using System.Collections;

public class Coin : Item {
	public enum CoinType {
		Silver = 1,
		Gold = 5
	}
	public CoinType coinType;
	public override void Init () {
		itemType = Game.ItemType.Coin;
		base.Init ();
		value = (int)coinType;
	}	
}
