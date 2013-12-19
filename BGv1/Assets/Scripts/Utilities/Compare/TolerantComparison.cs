using UnityEngine;
using System.Collections;

public static class TolerantComparison
{
	public static float accuracy = 0.001f;
	
	public static bool Equals(float a, float b) {
		return Mathf.Abs(a-b) <= accuracy;
	}
	
	public static bool Equals(float a, float b, float delta) {
		return Mathf.Abs(a-b) <= delta;
	}
	
	public static bool Equals(Vector2 a, Vector2 b) {
		return Vector2.Distance(a, b) <= accuracy;
	}
	
	public static bool Equals(Vector2 a, Vector2 b, float delta) {
		return Vector2.Distance(a, b) <= delta;
	}
	
	public static bool Equals(Vector3 a, Vector3 b) {
		return Vector3.Distance(a, b) <= accuracy;
	}
	
	public static bool Equals(Vector3 a, Vector3 b, float delta) {
		return Vector3.Distance(a, b) <= delta;
	}
	
	public static bool Equals(Vector4 a, Vector4 b) {
		return Vector4.Distance(a, b) <= accuracy;
	}
	
	public static bool Equals(Vector4 a, Vector4 b, float delta) {
		return Vector4.Distance(a, b) <= delta;
	}
}

