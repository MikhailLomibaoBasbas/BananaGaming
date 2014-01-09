using UnityEngine;
using System.Collections;

public class Heal : Item {
	public enum HealType {
		Mend = 5,
		Soothe = 12,
		Rejuvinate = 20
	}
	public HealType healType;
	public override void Init () {
		itemType = Game.ItemType.Heal;
		base.Init ();
		value = (int)healType;
	}	
}
