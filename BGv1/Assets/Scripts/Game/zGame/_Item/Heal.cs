using UnityEngine;
using System.Collections;

public class Heal : Item {
	public enum HealType {
		Mended = 5,
		Soothed = 12,
		Rejuvinated = 20
	}
	public HealType healType;
	public override void Init () {
		itemType = Game.ItemType.Heal;
		base.Init ();
		value = (int)healType;
	}

	public override void DropItem () {
		base.DropItem ();
		if (randNumber > 79)
			healType = HealType.Mended;
		else if (randNumber > 49)
			healType = HealType.Soothed;
		else 
			healType = HealType.Rejuvinated;
		value = (int)healType;
	}
}
