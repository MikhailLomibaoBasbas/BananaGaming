using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class static_audiomanager {
	
	private static static_audiomanager instance = null;
	private GameObject instance_gameObject = null; // we also use this as our bgm gameobject
	private float bgm_volume = Application.isEditor ? 1 : 1f;
	private float sfx_volume = Application.isEditor ? 1 : 1f;
	//private float amb_volume = Application.isEditor ? 0 : 0.4f;
	
	private List<AudioSource> sfx_audiosouces = new List<AudioSource>();
	private int sfx_base_count = 10;
	//private bool need_resync = true;	
	private string bgm_playing = string.Empty;
	private Dictionary<string,AudioClip> clip_bank = new Dictionary<string, AudioClip>();
	private Dictionary<AudioSource,Transform> asrouce_transform = new Dictionary<AudioSource, Transform>();

	private bool bgmOn = true;
	private bool sfxOn = true;
	//private GameObject playerReference = null;
	
	public static static_audiomanager getInstance
	{
		get {
			if (instance == null)
			{
				instance = new static_audiomanager();
			}
			return instance;
		}
	}
	
	public GameObject g_instance_parent
	{
		get{
		return instance_gameObject;
		}
	}
	
	private static_audiomanager()
	{
		instance_gameObject = new GameObject("z_audiomanager");
		GameObject.DontDestroyOnLoad(instance_gameObject);	
		if(GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener == null)
			instance_gameObject.AddComponent<AudioListener>();
			
		
		for (int x = 0 ; x < sfx_base_count; x++)
		{
			GameObject g = new GameObject("sfx_instance_"+(x+1));
			AudioSource asrc = g.AddComponent<AudioSource>();
			GameObject.DontDestroyOnLoad(g);
			g.transform.parent = instance_gameObject.transform;
			g.transform.localPosition = Vector3.zero;
			sfx_audiosouces.Add(asrc);
			asrouce_transform.Add(asrc,g.transform);
		}
	}
	
	
	public Transform get_transform_from_asrouce(AudioSource asrc )
	{
		return asrouce_transform[asrc];
	}
	
	public AudioSource get_source_inbank()
	{
		AudioSource ret = null;
		foreach (AudioSource g in sfx_audiosouces)
		{
			if (!g.isPlaying || !g.gameObject.activeSelf)
				ret = g;
		}
		if (ret == null)
		{
			GameObject g = new GameObject("sfx_instance_"+(sfx_audiosouces.Count));
			AudioSource asrc = g.AddComponent<AudioSource>();
			GameObject.DontDestroyOnLoad(g);
			g.transform.parent = instance_gameObject.transform;
			g.transform.localPosition = Vector3.zero;
			sfx_audiosouces.Add(asrc);
			asrouce_transform.Add(asrc,g.transform);
			ret = asrc;
		}
		return ret;
			
	}

	public void play_sfx(AudioClip aclp) {
		if (aclp == null)
		{
			//Debug.LogError(" NOT EXISITING SFX ");
			//Debug.LogError (path);
			return;
		}
		
		AudioSource asrc = get_source_inbank();
		Transform tt = get_transform_from_asrouce(asrc);
		//if (connected_listner)
		//{
			tt.parent = instance_gameObject.transform;
			tt.localPosition = Vector3.zero;
		//}
		// else
		// {
		// 	if (tt.parent != null)
		// 		tt.parent = null;
		// 	tt.position = p;
			
		// }
		//asrc.pitch = pitch;
		asrc.clip = aclp;
		asrc.volume = sfx_volume;
		asrc.Play();
	}

	public void play_sfx(string path, Vector3 p) { play_sfx (path, p, 1, true, 1); }
	public void play_sfx(string path, Vector3 p, float rvol) { play_sfx (path, p, rvol, true, 1 ); }
	public void play_sfx(string path, Vector3 p, float rvol, bool connected_listner) { play_sfx (path, p, rvol, connected_listner, 1); }
	public void play_sfx(string path, Vector3 p , float rvol, bool connected_listner, float pitch)
	{
		AudioClip aclp = null;
		if (clip_bank.ContainsKey(path))
			aclp = clip_bank[path];
		else
		{
		 	aclp = Resources.Load(path) as AudioClip;
			if (aclp != null)
				clip_bank.Add(path,aclp);
		}
		
		if (aclp == null)
		{
			Debug.LogError(" NOT EXISITING SFX ");
			Debug.LogError (path);
			return;
		}
		
		AudioSource asrc = get_source_inbank();
		Transform tt = get_transform_from_asrouce(asrc);
		if (connected_listner)
		{
			tt.parent = instance_gameObject.transform;
			tt.localPosition = Vector3.zero;
		}
		else
		{
			if (tt.parent != null)
				tt.parent = null;
			tt.position = p;
			
		}
		asrc.pitch = pitch;
		asrc.clip = aclp;
		asrc.volume = sfx_volume * rvol;
		asrc.Play();
		
	}
	
	
	public void play_bgm(string path)
	{
		if (path == bgm_playing) // no need to replay again
			return;
		
		AudioClip aclp = Resources.Load(path) as AudioClip;
		if (aclp == null)
		{
			Debug.LogError(" NOT EXISITING BGM ");
			Debug.LogError (path);			
			return;
		}
		
		if (!instance_gameObject.GetComponent<AudioSource>())
		{
			instance_gameObject.AddComponent<AudioSource>();
		}
		
		AudioSource bgm_ausource = instance_gameObject.GetComponent<AudioSource>();
		bgm_ausource.clip = aclp;
		bgm_ausource.loop = true;
		bgm_ausource.volume = bgm_volume;
		bgm_ausource.Play();
		bgm_playing = path;

	}

	public bool isBGMOn() {
		return bgmOn;
	}

	public bool isSFXOn() {
		return sfxOn;
	}

	public bool toggleBGM() {
		bgmOn = !bgmOn;
		return bgmOn;
	}

	public bool toggleSFX() {
		sfxOn = !sfxOn;
		return sfxOn;
	}

}
