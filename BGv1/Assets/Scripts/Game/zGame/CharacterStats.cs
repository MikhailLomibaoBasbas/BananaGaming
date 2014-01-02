using UnityEngine;
using System.Collections;

/// <summary>
///  The Formula for character movespeed and attackspeed is based on Dota 2.
/// </summary>

public class CharacterStats {
	public CharacterStats(){}
	public CharacterStats(int h, float b, float a, float m){
		health = h;
		_BAT = b;
		_attackSpeed = a;
		_moveSpeed = m;
	}
	public int health;

	protected float _BAT = 1f; //Base Attack Time.  It is the interval in seconds between each attack.
	public float setBAT {set{_BAT = value;}}

	protected float _attackSpeed;
	public float setAttackSpeed {
		set {
			_attackSpeed = value;
			if(_attackSpeed > 500)
				_attackSpeed = 500;
			_attackTime = default(float); 
			_attackPerSecond = default(float);
		}
	}

	private float _attackTime = default(float);
	public float getAttackTime {
		get {
			if(_attackTime == default(float)) 
				_attackTime =  _BAT / (1 + (_attackSpeed / 100f));
			return _attackTime;
		}
	}
	private float _attackPerSecond = default(float);
	public float getAttackPerSecond{
		get {
			if(_attackPerSecond == default(float)) {
				_attackPerSecond =  (1 + (_attackSpeed / 100)) / _BAT; //1f; //*_BAT*/; // Special case where BAT is not used because Animator divides the _BAT automatically
			}
			return _attackPerSecond;
		}
	}

	private float _moveSpeedCap = 1044;
	protected float _moveSpeed;
	private float _flatBonusMoveSpeed;
	public float setFlatBonusMoveSpeed {set {_flatBonusMoveSpeed = value;}}
	private float _bonusPercentageMoveSpeed;
	public float setBonusPercentageMoveSpeed {set {_bonusPercentageMoveSpeed = value;}}
	public float moveSpeed {set{_moveSpeed = value; }}
	private float _translateUnitsPerSecond = default(float);
	public float getTranslateUnitsPerSecond {
		get { 
			if(_translateUnitsPerSecond == default(float)) {
				if(_moveSpeed > _moveSpeedCap)
					_moveSpeed = _moveSpeedCap;
				_translateUnitsPerSecond = ((_moveSpeed /*/ 100f*/) + _flatBonusMoveSpeed) * (1 + (_bonusPercentageMoveSpeed / 100));
			}
			return _translateUnitsPerSecond;
		}
	}

}

internal class EnemyStats: CharacterStats {
	/// Legend: 
	/// PS = Per Stage	
	/// Inc = Increase 	
	/// mvspd = moveSpeed	
	/// aspd = attackSpeed

	public EnemyStats(int h, float b, float a, float m, float aips, float mips) {
		health = h;
		_BAT = b;
		_attackSpeed = a;
		_moveSpeed = m;
		_aspdIncPerStage = aips;
		_mvspdIncPerStage = mips;
	}
	private float _aspdIncPerStage = 10f;
	private float _mvspdIncPerStage = 5f;

	private float _baseMvspd = 200;
	public float setBaseMvspd {set {_baseMvspd = value;}}
	private float _baseAspd = 100;
	public float setBaseAspd {set {_baseAspd = value;}}

	private float _stage = 1;
	public float setStage {
		set {
			_stage = value;
			moveSpeed = _baseMvspd + (_mvspdIncPerStage * (_stage - 1));
			setAttackSpeed = _baseAspd + (_aspdIncPerStage * (_stage - 1));
		}
	}
}