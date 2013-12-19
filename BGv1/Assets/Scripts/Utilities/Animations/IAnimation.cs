using UnityEngine;
using System.Collections;

public interface IAnimation {
	void Initialize ();
	void start ();
	void stop ();
	void setValues ();
}
