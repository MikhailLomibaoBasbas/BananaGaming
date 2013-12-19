using UnityEngine;
using System.Collections;

public class ShareSocialListener : MonoBehaviour {
	
	public delegate void ShareSocialDidFinishSharing();
	public static event ShareSocialDidFinishSharing finishedSharing;
	
	public void OnSocialDidFinishSharing (string message) {
		if(finishedSharing != null) {
			finishedSharing();	
		} else {
			Debug.LogWarning("Finished Browsing Delegate is null");
		}
	}
	
}
