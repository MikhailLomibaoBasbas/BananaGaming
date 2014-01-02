using UnityEngine;
using System.Collections;

public class static_coroutine {

	private static static_coroutine instance = null;
	private GameObject instance_coroutine = null; // we also use this as our bgm gameobject
	private coroutine_manager c_coroutine_manager = null;
	public static static_coroutine getInstance
	{
		get {
			if (instance == null)
			{
				instance = new static_coroutine();
			}
			return instance;
		}
	}

	public MonoBehaviour getCoroutineMono()
	{
		return c_coroutine_manager; 
	}
	public void DoReflection(object class_ptr, string function, object[] parameters ,float delay)
	{
		c_coroutine_manager.SetReflection(class_ptr,function,parameters,delay);
	}
	
	private static_coroutine()
	{
		instance_coroutine = new GameObject("coroutine_object");
		c_coroutine_manager = instance_coroutine.AddComponent<coroutine_manager>();
		GameObject.DontDestroyOnLoad(instance_coroutine);		
	}
}
