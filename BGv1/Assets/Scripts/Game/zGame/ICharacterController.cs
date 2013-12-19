using UnityEngine;
using System.Collections;

public interface ICharacterController  {
	void Init();

	void OnUpdate();

	void UpdateTargetsNearAction();

	void UpdateAttackAction();

	void UpdateMoveAction();

	void UpdateHurtAction();

	void UpdateDeadAction();
}
