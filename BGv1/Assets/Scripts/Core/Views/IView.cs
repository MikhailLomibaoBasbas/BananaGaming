using UnityEngine;
using System;

public interface IView
{
	void AddChild(GameObject child);
	
	void RemoveChild(string name);
	
	GameObject GetChild(string name);
	
	T GetComponentFromChild<T>(string name) where T : Component;
	
	void Show(bool animated);
	
	void Hide(bool animated);
	
	void Close(bool animated);
}