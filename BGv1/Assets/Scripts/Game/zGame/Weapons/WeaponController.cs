using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {
	public Weapon.WeaponType type;
	private List<SingleTargetProjectile> s_projectiles = null;
	private List<ShotgunProjectile> a_projectiles = null;
	private Weapon _weapon;

	private bool _isAttacking;
	public bool isAttacking{get{return _isAttacking;}}
	private float _batRecoveryTime;

	private const string AMMO_CONTAINER_PATH = "ammoContainer";
	private Transform _ammoContainerTrans;

	public bool isUnlocked {get{return _weapon.isUnlocked;}}

	private bool _isAOEAttacking;

	void Awake () {
		_weapon = WeaponDataController.getInstance.weaponsMap[type.ToString()];
		_ammoContainerTrans = transform.FindChild(AMMO_CONTAINER_PATH);
		InitWeapons();
		_isAttacking = false;
		if(!isUnlocked)
			gameObject.SetActive(false);
	}

	private void InitWeapons () {
		if(_weapon.isAOE) 
			InitAOEProjectile();
		else 
			InitSingleProjectile();
	}


	private void InitSingleProjectile () {
		s_projectiles = new List<SingleTargetProjectile>(
			_ammoContainerTrans.GetComponentsInChildren<SingleTargetProjectile>(true)
			);
		foreach(SingleTargetProjectile pr in s_projectiles) {
			pr.SetValues(_ammoContainerTrans, _weapon.projectileTime, _weapon.projectileDistance, _weapon.damage);
		}
	}

	private void InitAOEProjectile () {
		a_projectiles = new List<ShotgunProjectile>(
			_ammoContainerTrans.GetComponentsInChildren<ShotgunProjectile>(true)
			);
		foreach(ShotgunProjectile pr in a_projectiles) {
			pr.SetValues(_ammoContainerTrans, _weapon.projectileTime, _weapon.projectileDistance,  _weapon.damage);
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
		if(!_weapon.isAOE) { 
			foreach(SingleTargetProjectile pr in s_projectiles) {
				if(pr.isActive)
					pr.Hide();
			}
		} else {
			foreach(ShotgunProjectile pr in a_projectiles) {
				if(pr.isActive)
					pr.Hide();
			}
		}
	}

	public void Attack () {
		if(!_isAttacking /*&& _weapon.currentAmmo > 0*/) {
			if(_weapon.isAOE)
				StartAOETargetAttack();
			else
				StartSingleTargetAttack();
		}
	}

	private void StartSingleTargetAttack () {
		foreach(SingleTargetProjectile pr in s_projectiles) {
			if(!pr.isActive) {
				pr.Show(transform.position.x > transform.parent.position.x);
				_isAttacking = true;
				_batRecoveryTime = 0;
				//_weapon.currentAmmo--;
				break;
			}
		}
	}

	private void StartAOETargetAttack () {
		foreach(ShotgunProjectile pr in a_projectiles) {
			if(!pr.isActive) {
				pr.Show(true);
				_isAttacking = true;
				_batRecoveryTime = 0;
				//_weapon.currentAmmo--;
				break;
			}
		}
	}



}
