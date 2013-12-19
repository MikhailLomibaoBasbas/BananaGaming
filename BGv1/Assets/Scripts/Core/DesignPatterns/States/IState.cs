using System;

public interface IState
{
	bool isTransparent {get;set;}
	
	void OnStart();
	
	void OnUpdate();
	
	void OnPause();
	
	void OnResume();
	
	void OnResume(object data);
	
	void OnEnd();
}

