using UnityEngine;
using System.Collections;
using System;

public class Cast : MonoBehaviour {	
	public static string ToString<T>(T val) {
		return Enum.GetName(typeof(T), val);
	}
}