using UnityEngine;
using System.Collections;
using System.Reflection;

public class coroutine_manager : MonoBehaviour {
	
	public void SetReflection(object class_ptr, string function, object[] parameters ,float delay)
	{
		StartCoroutine(_DoReflection(class_ptr,function,parameters,delay));
	}
	
	private IEnumerator _DoReflection(object class_ptr, string function, object[] parameters ,float delay)
	{
		
		yield return new WaitForSeconds(delay);
		   	
	    System.Type myTypeObj = class_ptr.GetType();	    
	    MethodInfo myMethodInfo = myTypeObj.GetMethod(function);	  
	    myMethodInfo.Invoke(class_ptr, parameters);
		
	}
}
