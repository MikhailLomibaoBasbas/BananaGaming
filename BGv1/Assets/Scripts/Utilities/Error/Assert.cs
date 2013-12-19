using UnityEngine;
using System.Collections;

public static class Assert
{
	public static void True(bool expression) {
		True(expression, "Expression is not true");
	}
	
	public static void True(bool expression, string message) {
		if(!expression) Debug.LogError("Assertions failed: " + message);
	}
	
	public static void False(bool expression) {
		False(expression, "Expression is not false");
	}
	
	public static void False(bool expression, string message) {
		True(!expression, message);
	}
	
	public static void NotNull(object obj) {
		NotNull(obj);
	}
	
	public static void NotNull(object obj, string message) {
		False(obj == null, message);
	}
}

