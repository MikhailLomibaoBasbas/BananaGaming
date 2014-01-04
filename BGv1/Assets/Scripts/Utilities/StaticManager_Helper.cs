using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class StaticManager_Helper {

	public static T CreateComponent<T>(string resourcePath, GameObject parentObj) where T : Component {
		GameObject panelPrefab = Resources.Load(resourcePath) as GameObject;
		T component = default(T);
		if(panelPrefab != null) {
			GameObject panel = GameObject.Instantiate(panelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			if(parentObj != null)
				panel.transform.parent = parentObj.transform;
			panel.transform.localScale = Vector3.one;
			component = panel.GetComponent<T>();
			if(component == null) component = panel.AddComponent<T>();
		}else{
			Debug.LogWarning("Can't find prefab (" + resourcePath + ") when creating " + typeof(T).Name);
		}
		return component;
	}

	public static T CreateComponent<T>(GameObject bGO, GameObject parentObj, bool willRetainTransform = true) where T : Component {
		GameObject tempGO = GameObject.Instantiate(bGO) as GameObject;
		T component = default(T);
		if(parentObj != null)
			tempGO.transform.parent = parentObj.transform;
		if (!willRetainTransform) {
			tempGO.transform.position = Vector3.zero;
			tempGO.transform.rotation = Quaternion.identity;
			tempGO.transform.localScale = Vector3.one;
		}
		if((component = tempGO.GetComponent<T> ()) == null)
			component = tempGO.AddComponent<T>();
		return component;
	}

	public static GameObject CreatePrefab(string resourceName, GameObject parentGO = null, bool willRetainTransform = false, int layer = -1, bool dontDestroyOnLoad = false) {
	GameObject Prefab = Resources.Load(resourceName) as GameObject;
	GameObject panel = null;
		if(Prefab != null) {
			panel = GameObject.Instantiate (Prefab) as GameObject;
			if (parentGO != null)
				panel.transform.parent = parentGO.transform;
			if (!willRetainTransform) {
				panel.transform.position = Vector3.zero;
				panel.transform.rotation = Quaternion.identity;
				panel.transform.localScale = Vector3.one;
			}
			if (layer > 0) {
				Transform[] childrenTrans = panel.GetComponentsInChildren<Transform> ();
				for(int i = 0; i < childrenTrans.Length; i++)
					childrenTrans[i].gameObject.layer = layer;
			}
			if(dontDestroyOnLoad)
				GameObject.DontDestroyOnLoad (panel);
		}else{
			Debug.LogWarning("Can't find prefab \"" + resourceName);
		}
		return panel;
	}


	public static GameObject GetChild (this GameObject parent, string name) {
		Transform child = parent.transform.FindChild(name);
		if(child != null) {
			return child.gameObject;
		}else{
			Debug.LogWarning(parent.name + " has no child \"" + name + "\"");
			return null;
		}
	}

	public static GameObject CreateChild (this GameObject parent, string name, int layer = -1) {
		GameObject child = new GameObject (name);
		if (layer > -1)
			child.layer = layer;
		if (parent != null)
			child.transform.parent = parent.transform;
		else
			Debug.LogWarning ("GameObject is Created but has no Parent Game Object");
		return child;
	}

	
	public static void SetAlpha (this Material material, float value) {	
		Color color = material.color;
   	 	color.a = value;
    	material.color = color;
	}

	public static void SetGameObjectActive(this Component component, bool flag) {
		component.gameObject.SetActive (flag);
	}

	public static void SetPosition(this Transform mTransform, Vector3 mPosition, bool isGlobal = true) {
		//Debug.LogError (isGlobal + " " + mPosition);
		if (isGlobal)
			mTransform.position = mPosition;
		else
			mTransform.localPosition = mPosition;
	}

	public static Vector3 GetPosition(this Transform mTransform, bool isGlobal = true) {
		Vector3 tempPos = Vector3.zero;
		if (isGlobal)
			tempPos = mTransform.position;
		else
			tempPos = mTransform.localPosition;
		return tempPos;
	}

	public static void SetRotation(this Transform mTransform, Vector3 mRotEuler, bool isGlobal = true) {
		if (isGlobal)
			mTransform.rotation = Quaternion.Euler(mRotEuler);
		else
			mTransform.localRotation = Quaternion.Euler(mRotEuler);
	}

	public static void SetRotation(this Transform mTransform, Quaternion mRotation, bool isGlobal = true) {
		if (isGlobal)
			mTransform.rotation = mRotation;
		else
			mTransform.localRotation = mRotation;
	}


	public static Vector3 getSineWavePoints (Vector3 moveFr, Vector3 moveT, float ratio, 
	                           float ampli = 50f, float centerAmpli = 0f, float frequency = 1f, float phase = 0f) {
		Vector3 tempPos = Vector3.zero;
		if (Mathf.Abs (moveFr.x - moveT.x) > Mathf.Abs (moveFr.y - moveT.y)) {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio) + (ampli * Mathf.Sin ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
		} else {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio) + (ampli * Mathf.Sin ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio);
		}
		return tempPos;
	}

	public static Vector3 getCosineWavePoints (Vector3 moveFr, Vector3 moveT, float ratio, 
	                             float ampli = 50f, float centerAmpli = 0f, float frequency = 1f, float phase = 0f) {
		Vector3 tempPos = Vector3.zero;
		if (Mathf.Abs (moveFr.x - moveT.x) > Mathf.Abs (moveFr.y - moveT.y)) {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio) + (ampli * Mathf.Cos ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
		} else {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio) + (ampli * Mathf.Cos ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio);
		}
		return tempPos;
	}

	public static Vector2 GetRandomPointFromPerpendicularLine (Vector2 p1, Vector2 p2, float thresholdX, float commonPointRatio = 0.5f) {
		Vector2 midpoint = (p2 - p1) * commonPointRatio;
		float origLineSlope = (p2.y - p1.y) / (p2.x - p1.x);
		float mSlope = -(1 / origLineSlope); 

		Vector2 tempVector = Vector2.zero;
		float x = Random.Range (-thresholdX, thresholdX);
		float y = mSlope * (x - midpoint.x) + midpoint.y;
		return new Vector2 (x, y);
	}

	public static Vector3 GetRandomInBetweenPoint(Vector3 p1, Vector3 p2, Vector3 offset = default(Vector3)) {
		Vector3 resultVector = Vector3.zero;

		if (p1.x > p2.x)
			resultVector.x = Random.Range (p2.x - offset.x, p1.x + offset.x);
		else
			resultVector.x = Random.Range (p1.x - offset.x , p2.x + offset.x);

		if (p1.y > p2.y)
			resultVector.y = Random.Range (p2.y - offset.y, p1.y + offset.y);
		else
			resultVector.y = Random.Range (p1.y - offset.y, p2.y + offset.y);

		if (p1.z > p2.z)
			resultVector.z = Random.Range (p2.z - offset.z, p1.z + offset.z);
		else
			resultVector.z = Random.Range (p1.z - offset.z, p2.z + offset.z);

		return resultVector;
	}

	public static Vector2 GetRandomInBetweenPoint2D(Vector2 p1, Vector2 p2, Vector2 offset = default(Vector2)) {
		Vector2 resultVector = Vector3.zero;

		if (p1.x > p2.x)
			resultVector.x = Random.Range (p2.x - offset.x, p1.x + offset.x);
		else
			resultVector.x = Random.Range (p1.x - offset.x , p2.x + offset.x);

		if (p1.y > p2.y)
			resultVector.y = Random.Range (p2.y - offset.y, p1.y + offset.y);
		else
			resultVector.y = Random.Range (p1.y - offset.y, p2.y + offset.y);
		return resultVector;
	}

	public static string CompareMethods(params System.Action[] actions) {
		string result = default(string);
		int actionsLength = actions.Length;
		System.Diagnostics.Stopwatch[] stopWatches = new System.Diagnostics.Stopwatch[actionsLength];
		for(int i = 0; i < actionsLength; i++) {
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
			System.Action action = actions[i];
			stopWatch.Start();
			action();
			stopWatch.Stop();

			stopWatches[i] = stopWatch;
			result += action.Method.Name + " Ticks: " + stopWatch.ElapsedTicks + "\r\n";
		}
		Debug.Log(result);
		//Debug.LogError(result);
		return result;
	}


	public static void ChangeHierarchyLayer(Transform root, int layer) {
		root.gameObject.layer = layer;
		foreach(Transform child in root)
			ChangeHierarchyLayer(child, layer);
	}

	public static int[] GetRandomNonRepeatingNumbers(int count, int fromNum, int toNum) {
		List<int> numbers = new List<int>();
		for(int i = 0; i < (toNum - fromNum) + 1; i++) {
			numbers.Add(fromNum + i);
		}
		int[] resultNumbers = new int[count];
		for(int i = 0; i < count; i++) {
			int randIndex = Random.Range(0, numbers.Count);
			//Debug.LogError(randIndex);
			resultNumbers[i] = numbers[randIndex];
			numbers.RemoveAt(randIndex);
		}
		return resultNumbers;
	}
}
