using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectEuler : MonoBehaviour {

	// Use this for initialization

	public int latticeRouteSize = 20;
	void Start () {
		StartLatticePath (latticeRouteSize);
	}


	void StartLatticePath (int size) {
		int[,] route = InitializeRoute (size);
	}

	int[,] InitializeRoute (int size) {
		int[,] route = new int[size,size];
		for(int j = 0; j < size; j++) {
			for(int i = 0; i < size; i++) {
				route[j,i] = 0;
			}
		}
		return route;
	}
}
