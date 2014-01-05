using UnityEngine;
using System.Collections;

public class OptionData {

	private static OptionData instance = null;
	public static OptionData getInstance{
		get{ 
			if (instance == null)
				instance = new OptionData ();

				return instance;
		}
	}

	private int _bgmOn = 1;
	private int _sfxOn = 1;

	public int BGMON{
		get{ _bgmOn = PlayerPrefs.GetInt ("bgmon"); 
			return _bgmOn; }
		set{ _bgmOn = value; 
			PlayerPrefs.SetInt ("bgmon", _bgmOn);}
	}

	public int SFXON{
		get{ _sfxOn = PlayerPrefs.GetInt ("sfxon"); 
			return _sfxOn; }
		set{ _sfxOn = value; 
			PlayerPrefs.SetInt ("sfxon", _sfxOn);}
	}

	public OptionData(){
		if (!PlayerPrefs.HasKey ("bgmon"))
			PlayerPrefs.SetInt ("bgmon", _bgmOn);

		if (!PlayerPrefs.HasKey ("sfxon"))
			PlayerPrefs.SetInt ("sfxon", _sfxOn);
	}
}
