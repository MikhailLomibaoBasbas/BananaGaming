using UnityEngine;
using System;
using System.Collections;

public static class Functions {
	
	public static float ParabolicValue(float min, float max, float time) {
		time = Mathf.Clamp01(time);
		float a = -(max-min)/(0.5f*0.5f);
		return min+a*(time-0.5f)*(time-0.5f)+(max-min);
	}
	
	public static float SpringValue(float start, float end, float time){
		time = Mathf.Clamp01(time);
		time = (Mathf.Sin(time * Mathf.PI * (0.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
		return start + (end - start) * time;
	}
	
}
