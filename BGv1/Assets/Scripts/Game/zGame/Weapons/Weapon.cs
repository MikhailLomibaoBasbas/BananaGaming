using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon {
	public enum WeaponType {
		Pistol, Shotgun, AK47
	}
	public string id;
	public string name;
	public float projectileTime;
	public float projectileDistance;
	public int damage;
	public bool isAOE;
	public float baseAttackTime;
	public int currentAmmo;
	public int maxAmmo;
	public WeaponType type;
	public bool isUnlocked;

	public Weapon (string mId, string mName, float time, float distance, int dmg, bool aoe, float bat, int cAmmo, int mAmmo, WeaponType mType, bool misUnlocked){
		id = mId;
		name = mName;
		projectileTime = time;
		projectileDistance = distance;
		damage = dmg;
		baseAttackTime = bat;
		isAOE = aoe;
		currentAmmo = cAmmo;
		maxAmmo = mAmmo;
		type = mType;
		isUnlocked = misUnlocked;
	}
}
