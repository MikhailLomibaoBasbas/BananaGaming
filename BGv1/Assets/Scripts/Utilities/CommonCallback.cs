using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class CommonCallback : MonoBehaviour {
	#region callback feature

	public delegate void OnCommonAnimationFinished ();
	public event OnCommonAnimationFinished onCommonAnimationFinished = null;

	public void addCallbackv2 (OnCommonAnimationFinished callback) {
		if (onCommonAnimationFinished != null)
			onCommonAnimationFinished = null;
		onCommonAnimationFinished += callback;
	}
	
	object _classPtr;
	string _function;
	object[] _parameters;
	bool _isOneStrike;
	float _delay;
	
	public void doCallback() {
		if (onCommonAnimationFinished != null)
			onCommonAnimationFinished ();
		onCommonAnimationFinished = null;

		//if (_classPtr == null)
			//return;
		if(_classPtr != null)
			Invoke("startCallback", _delay);
	}
	
	private void startCallback () {
		System.Type myTypeObj = _classPtr.GetType ();	    
		MethodInfo myMethodInfo = myTypeObj.GetMethod (_function);	  
		myMethodInfo.Invoke (_classPtr, _parameters);
			
		if (_isOneStrike) {
			_classPtr = null;
			_function = null;
			_parameters = null;
			_isOneStrike = false;
			_delay = 0;
		}
	}

	public void addCallback (object p_class_ptr, string p_function, object[] p_parameters, bool oneStrike = false, float delay = 0){
		_classPtr = p_class_ptr;
		_function = p_function;
		_parameters = p_parameters;
		_isOneStrike = oneStrike;
		_delay = delay;
	}
	
	public void removeCallbacks () {
		_classPtr = null;
		_function = null;
		_parameters = null;
		_isOneStrike = false;
		_delay = 0;
		onCommonAnimationFinished = null;
	}
	
	#endregion
	
}
