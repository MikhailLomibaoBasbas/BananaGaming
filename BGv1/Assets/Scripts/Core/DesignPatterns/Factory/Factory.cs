using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class Factory<T, U> where T : struct where U : new() {
	
	private Dictionary<T, Type> map;
	
	public Factory() {
		map = new Dictionary<T, Type>();
	}
	
	public void RegisterType(T key, Type type) {
		if(type.IsSubclassOf(typeof(U))) {
			map.Add(key, type);
		}else{
			Debug.LogError("[Factory.RegisterType] Trying to register an item of type " + type.Name + 
				" which does not derive from " + typeof(U).Name);
		}
	}
	
	public U CreateType(T key) {
		if(map.ContainsKey(key)) {
			Type type = map[key];
			return (U)Activator.CreateInstance(type);
		}else{
			Debug.LogWarning("[Factory] There is no type registered to " + key.ToString());
			return default(U);
		}
	}
	
	public U CreateType(T key, params object[] args) {
		if(map.ContainsKey(key)) {
			Type type = map[key];
			return (U)Activator.CreateInstance(type, args);
		}else{
			Debug.LogWarning("[Factory] There is no type registered to " + key.ToString());
			return default(U);
		}
	}
	
	public Type GetType(T key) {
		return map[key];
	}
}
