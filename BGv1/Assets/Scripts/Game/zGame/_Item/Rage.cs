using UnityEngine;
using System.Collections;

public class Rage : Item {
	public enum RageType {
		Upset = 2,
		Tantrum = 5,
		Wrath = 8
	}

	public RageType rageType;
	public override void Init () {
		itemType = Game.ItemType.Rage;
		base.Init ();
		value = (int)rageType;
	}
}
