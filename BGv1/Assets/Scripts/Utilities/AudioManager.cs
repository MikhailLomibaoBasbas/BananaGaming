using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager {
	private static AudioManager _instance = null;
	public static AudioManager GetInstance {
		get {
			if(_instance == null)
				_instance = new AudioManager();
			return _instance;
		}
	}
	
	public enum BGMType {
		Bass1, Bass2, Bass3, Bass4,
		Drums1, Drums2, Drums3, Drums4,
		Line1, Line2, Line3, Line4,
		InGame, MainMenu
	}

	/*public enum SFXType {
	}*/
	
	private GameObject _container = null;
	public GameObject getContainer {
		get {
			if(_container == null)
				_container = new GameObject("zAudioManager");
			return _container;
		}
	}
	
	private const string DEFAULT_FILE_FORMAT = ".wav";
	
	private const string BGM_PATH = "Audio/Bgm/";
	private Dictionary<BGMType, AudioClip> _bgmMap = null;
	public Dictionary<BGMType, AudioClip> getBgmMap {
		get {
			if(_bgmMap == null) {
				_bgmMap = new Dictionary<BGMType, AudioClip>();
				int bgmCount = (int)BGMType.MainMenu;
				for(int i = 0; i < bgmCount; i++) {
					BGMType t = (BGMType)i;
					AudioClip ac = Resources.Load(BGM_PATH + t.ToString()) as AudioClip;
					_bgmMap.Add(t, ac);
				}
			}
			return _bgmMap;
		}
	}
	
	private List<AudioSource> _bgmAudioSources = null;
	private Dictionary<BGMType, AudioSource> _usedBGMAudioSource = new Dictionary<BGMType, AudioSource>();
	private int _bgmAudioSourcesInitCount = 3;
	public AudioSource getBgmAudioSource {
		get {
			if (_bgmAudioSources == null) {
				_bgmAudioSources = new List<AudioSource>();
				for(int i = 0; i < _bgmAudioSourcesInitCount; i++) {
					AudioSource auS = new GameObject("bgm_audiosource_" + (i+1).ToString()).AddComponent<AudioSource>();
					auS.transform.parent = getContainer.transform;
					auS.transform.localPosition = Vector3.zero;
					_bgmAudioSources.Add(auS);
				}
			}
			AudioSource ausource = null;
			for(int i = 0; i < _bgmAudioSources.Count; i++) {
				if(!_bgmAudioSources[i].isPlaying) {
					ausource = _bgmAudioSources[i];
					break;
				}
			}
			if(ausource == null) {
				ausource = new GameObject("bgm_audiosource_" + (_bgmAudioSources.Count + 1).ToString()).AddComponent<AudioSource>();
				ausource.transform.parent = getContainer.transform;
				ausource.transform.localPosition = Vector3.zero;
				_bgmAudioSources.Add(ausource);
				_bgmAudioSourcesInitCount++;
			}
			return ausource;
		}
	}
	
	private AudioManager() {
		Dictionary<BGMType, AudioClip> pooledBgmMap = getBgmMap;
		AudioSource pooledAC = getBgmAudioSource;
		getContainer.AddComponent<AudioListener>();
	}
	
	public void PlayBGM(BGMType type, bool loop = true, float volume = 1f) {
		//Debug.LogError("fdsfs");
		if(_usedBGMAudioSource.ContainsKey(type)) {
			Debug.LogWarning("BGM " + type + "is already playing");
			return;
		}
		AudioSource auSource = getBgmAudioSource;
		_usedBGMAudioSource.Add(type, auSource);
		auSource.loop = loop;
		//Debug.LogWarning(getBgmMap[type]);
		auSource.clip = getBgmMap[type];
		auSource.volume = volume;
		auSource.Play();
	}
	
	public void StopBGM(BGMType type) {
		if(!_usedBGMAudioSource.ContainsKey(type)) {
			Debug.LogWarning("BGM " + type + "is already playing");
			return;
		}
		_usedBGMAudioSource[type].Stop();
	}
	
	public void StopBGMAll () {
		foreach(AudioSource auS in _usedBGMAudioSource.Values) {
			auS.Stop();
		}
		_usedBGMAudioSource.Clear();
	}
	
	public string PlayRandomBGMCombination () {
		StopBGMAll();
		BGMType bass = (BGMType)(Random.Range((int)BGMType.Bass1, (int)BGMType.Bass4 + 1));
		BGMType drums = (BGMType)(Random.Range((int)BGMType.Drums1, (int)BGMType.Drums4 + 1));
		BGMType line = (BGMType)(Random.Range((int)BGMType.Line1, (int)BGMType.Line4 + 1));
		PlayBGM(bass, true, 1);
		PlayBGM(drums, true, 0.5f);
		PlayBGM(line, true, 0.7f);
		Debug.Log("Random BG Playing: " + bass.ToString() + " " + drums.ToString() + " " + line.ToString());
		return bass.ToString() + " " + drums.ToString() + " " + line.ToString();
	}
	
	#region Specific BG Combinations
	public void PlayKhailSpecial () {
		StopBGMAll();
		PlayBGM(BGMType.Drums4);
		PlayBGM(BGMType.Line4);
		PlayBGM(BGMType.Bass4);
	}
	
	//YOU CAN ADD YOUR OWN COMBINATION
	#endregion
	
}
