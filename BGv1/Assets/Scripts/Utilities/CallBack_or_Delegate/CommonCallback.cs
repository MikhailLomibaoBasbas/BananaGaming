using UnityEngine;
using System.Collections;
using System.Reflection;

public class CommonCallBack : MonoBehaviour {
	#region callback feature
	
	private object _classPtr = null;
	private string _function;
	private object[] _parameters;
	private bool _isOneStrike;
	
	public void doCallback() {

		if (_classPtr == null)
			return;
		System.Type myTypeObj = _classPtr.GetType();	    
		MethodInfo myMethodInfo = myTypeObj.GetMethod(_function);	  
		myMethodInfo.Invoke(_classPtr, _parameters);
			
		if(_isOneStrike)
		_classPtr = null;
	}
	
	public void addCallBack (object p_class_ptr, string p_function, object[] p_parameters, bool oneStrike = false){
		_classPtr = p_class_ptr;
		_function = p_function;
		_parameters = p_parameters;
		_isOneStrike = oneStrike;
	}
	
	public void removeCallback () {
		_classPtr = null;
		_function = string.Empty;
		_parameters = null;
	}
	
	#endregion
	
}
