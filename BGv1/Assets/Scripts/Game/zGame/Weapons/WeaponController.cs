using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {
	public Weapon.WeaponType type;
	private List<Projectile> _projectiles;
	private Weapon _weapon;

	private bool _isAttacking;
	public bool isAttacking{get{return _isAttacking;}}
	private float _batRecoveryTime;

	private const string AMMO_CONTAINER_PATH = "ammoContainer";

	public bool isUnlocked {get{return _weapon.isUnlocked;}}

	void Awake () {
		_weapon = WeaponDataController.getInstance.weaponsMap[type.ToString()];
		if(type != Weapon.WeaponType.Shotgun) 
			InitProjectile();
		_isAttacking = false;
		if(!isUnlocked)
			gameObject.SetActive(false);
	}


	private void InitProjectile () {
		Transform ammoContainerTrans = transform.FindChild(AMMO_CONTAINER_PATH);
		_projectiles = new List<Projectile>(
			ammoContainerTrans.GetComponentsInChildren<Projectile>(true)
			);
		foreach(Projectile pr in _projectiles) {
			pr.SetValues(ammoContainerTrans, _weapon.projectileTime, _weapon.projectileDistance);
		}

	}


	void Update () {
		if(_isAttacking) {
			if(_batRecoveryTime < _weapon.baseAttackTime) {
				_batRecoveryTime += Time.deltaTime;
			} else {
				_batRecoveryTime = 0;
				_isAttacking = false;
			}
		}
	}

	public void Show () {
		gameObject.SetActive(true);
	}

	public void Hide () {
		gameObject.SetActive(false);
		foreach(Projectile pr in _projectiles) {
			if(pr.isActive)
				pr.Hide();
		}
	}

	public void Attack () {
		if(!_isAttacking /*&& _weapon.currentAmmo > 0*/) {
			switch(type) {
			case Weapon.WeaponType.Shotgun:
				DoAOETargetAttack();
				break;
			case Weapon.WeaponType.AK47:
			case Weapon.WeaponType.Pistol:
				break;
			default:
				break;
			}
		}
	}

	void DoSingleTargetAttack () {
		foreach(Projectile pr in _projectiles) {
			if(!pr.isActive) {
				pr.Show(transform.position.x > transform.parent.position.x);
				_isAttacking = true;
				_batRecoveryTime = 0;
				//Debug.LogError(_weapon.currentAmmo);
				//_weapon.currentAmmo--;
				break;
			}
		}
	}

	void DoAOETargetAttack () {

	}
}
