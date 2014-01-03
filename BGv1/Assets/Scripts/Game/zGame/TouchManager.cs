using UnityEngine;
using System.Collections;

public class TouchManager {

	private ArrayList trackers = new ArrayList();
	private Hashtable trackerLookup = new Hashtable();
	private ArrayList endedTrackers = new ArrayList();

	private static TouchManager instance = null;
	public static TouchManager getInstance{
		get{ 
			if (instance == null)
				instance = new TouchManager ();

			return instance;
		}
	}

	public ArrayList getTrackers{
		get{ return trackers;}
	}

	public void Update(){

		endedTrackers.Clear ();

		foreach (TouchTracker tracker in trackers)
			tracker.Clean ();

		for(int i = 0; i < Input.touchCount; i++){

			Touch touch = Input.GetTouch (i);
			TouchTracker tracker = (TouchTracker)trackerLookup [touch.fingerId];

			if (tracker != null)
				tracker.Update (touch);
			else
				tracker = BeginTracking (touch);
		}

		for (int x = 0; x < trackers.Count; x++){
			TouchTracker tracker = (TouchTracker)trackers[x];
			if (!tracker.isDirty){
				endedTrackers.Add(tracker);
			}
		}

		for (int x = 0; x < endedTrackers.Count; x++){
			EndTracking((TouchTracker)endedTrackers[x]);		
		}
	}

	public TouchTracker BeginTracking(Touch touch){
		TouchTracker tracker = new TouchTracker (touch);
		trackers.Add (tracker);
		trackerLookup [touch.fingerId] = tracker;

		return tracker;
	}

	public void EndTracking(TouchTracker tracker){
		tracker.End();

		trackers.Remove(tracker);
		trackerLookup[tracker.fingerId] = null;
	}
}
