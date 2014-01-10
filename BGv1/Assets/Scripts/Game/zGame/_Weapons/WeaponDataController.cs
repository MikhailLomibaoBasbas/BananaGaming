using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WeaponDataController {
	private static WeaponDataController _instance = null;
	public static WeaponDataController getInstance {
		get {
			if(_instance == null)
				_instance = new WeaponDataController();
			return _instance;
		}
	}

	public Dictionary<string, Weapon> weaponsMap;
	public WeaponDataController(){
		weaponsMap = new Dictionary<string, Weapon>();
		Weapon pistol = new Weapon("5nrjh6oi5i", "Pistol", 0.4f, 1000, 1, false, 0.3f, 0, 0, Weapon.WeaponType.Pistol, true);
		Weapon shotGun = new Weapon("ok0ck8z9sn", "Shotgun", 0.2f, 500, 5, true, 1.5f, 30, 30, Weapon.WeaponType.Shotgun, true); // Instant
		Weapon ak47 = new Weapon("5fogt8di5i", "AK47", 0.3f, 1500, 2, false, 0.1f,  200, 200, Weapon.WeaponType.AK47, true);

		weaponsMap.Add(pistol.name, pistol);
		weaponsMap.Add(shotGun.name, shotGun);
		weaponsMap.Add(ak47.name, ak47);
	}
}
