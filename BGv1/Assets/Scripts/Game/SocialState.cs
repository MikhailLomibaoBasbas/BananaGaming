using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocialState : BasicGameState {
	
	public override void OnStart() {
		viewUI = SocialView.Create();
		AddGUIListeners();
		InitFacebookInfo();
		base.OnStart();
	}
	
	public override void OnUpdate() {
		base.OnUpdate();
	}
	
	public override void OnEnd() {
		RemoveGUIListeners();
		base.OnEnd();
	}
	
	void AddGUIListeners() {
		(viewUI as SocialView).navigationView.onClickBack += OnClickBack;
	}
	
	void RemoveGUIListeners() {
		(viewUI as SocialView).navigationView.onClickBack -= OnClickBack;
	}
	
	void OnClickBack(GameObject go) {
		stateManager.PopState();
	}
	
	void InitFacebookInfo() {
		SocialView socialView = viewUI as SocialView;
		socialView.SetName("Loading");
		
		if(Facebook.HasOpenSession()) {
			StartCoroutine(LoadFacebookUserInfo());
		}else{
			StartCoroutine(LoginToFacebook());
		}
	}
	
	IEnumerator LoginToFacebook() {
		Debug.Log("LoginToFacebook");
		yield return StartCoroutine(Facebook.OpenSession("590648990953596"));//!TODO: Save fbAppId somewhere later
		
		SocialView socialView = viewUI as SocialView;
		if(Facebook.lastResult.success) {
			socialView.SetName("Logged In Successfully");
		}else{
			socialView.SetName("Logged In Failed!");
		}
	}
	
	IEnumerator LoadFacebookUserInfo() {
		Debug.Log("LoadFacebookUserInfo");
		Facebook.CloseSession();
		
		SocialView socialView = viewUI as SocialView;
		socialView.SetName("Logged Out");
		yield break;
	}
}