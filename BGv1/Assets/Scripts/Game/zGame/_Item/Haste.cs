using UnityEngine;
using System.Collections;

public class Haste : Item {

	public enum HasteType {
		Swift = 50,
		Rush = 110,
		Alacrity = 180,
	}

	public HasteType hasteType;
	public override void Init () {
		itemType = Game.ItemType.Haste;
		base.Init ();
		value = (int)hasteType;
	}

}
