using UnityEngine;
using System.Collections;

public class Haste : Item {

	public enum HasteType {
		Swift = 100,
		Rush = 210,
		Alacrity = 310,
	}

	public HasteType hasteType;
	public override void Init () {
		itemType = Game.ItemType.Haste;
		base.Init ();
		value = (int)hasteType;
	}

	public override void DropItem () {
		base.DropItem ();
		if (randNumber > 79)
			hasteType = HasteType.Alacrity;
		else if (randNumber > 49)
			hasteType = HasteType.Rush;
		else 
			hasteType = HasteType.Swift;
		value = (int)hasteType;
	}

}
