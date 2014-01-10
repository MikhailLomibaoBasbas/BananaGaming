using UnityEngine;
using System.Collections;

public class Rage : Item {
	public enum RageType {
		Maddened = 4,
		Enraged = 9,
		Wrathful = 14
	}

	public RageType rageType;
	public override void Init () {

		itemType = Game.ItemType.Rage;
		base.Init ();
		value = (int)rageType;
	}

	public override void DropItem (){
		base.DropItem ();
		if (randNumber > 79)
			rageType = RageType.Maddened;
		else if (randNumber > 49)
			rageType = RageType.Enraged;
		else 
			rageType = RageType.Wrathful;
		value = (int)rageType;
	}
}
