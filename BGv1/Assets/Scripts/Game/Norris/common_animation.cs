using UnityEngine;
using System.Collections;
using System.Reflection;

public class common_animation : MonoBehaviour {

	public Transform target_transform;

	//NGUI plugin
	//public UIPanelAlpha ui_alpha;
	public UIPanelAlphaCustom ui_alpha;
	public UILabel ui_label;
	public UISprite ui_sprite;
	public UISlicedSprite ui_sliced_sprite;
	public MeshRenderer go_material;

	public float max_animation_time = 1f;
	protected float animation_time = 0.0f;
	public bool is_animating = false;
	protected bool is_initialized = false;

	public ComponentType component_type;

	public virtual void initialize(){
		if( !is_initialized ){
			target_transform = transform;
			if ( (go_material = target_transform.GetComponent<MeshRenderer>() )!= null)
				component_type = ComponentType.Material;
			else if ( (ui_alpha = target_transform.GetComponent<UIPanelAlphaCustom> ()) != null)
				component_type = ComponentType.Alpha;
			else if ( (ui_sliced_sprite = target_transform.GetComponent<UISlicedSprite> ()) != null)
				component_type = ComponentType.SliceSprite;
			else if ( (ui_sprite = target_transform.GetComponent<UISprite> ()) != null)
				component_type = ComponentType.Sprite;
			else if ( (ui_label = target_transform.GetComponent<UILabel> ()) != null)
				component_type = ComponentType.Label;
			else
				Debug.LogWarning ("There is no component type found!");
			is_initialized = true;
		}

		//Debug.Log ("component_type" + ui_alpha.alpha);
	}

	#region callback feature

	private object _classPtr = null;
	private string _function;
	private object[] _parameters;
	private bool _isOneStrike;
	
	public void doCallback() {

		if (_classPtr == null)
			return;	
		System.Type myTypeObj = _classPtr.GetType();	    		
		MethodInfo myMethodInfo = myTypeObj.GetMethod(_function);	  
		myMethodInfo.Invoke(_classPtr, _parameters);
		if(_isOneStrike)
			_classPtr = null;
	}


	public void addCallBack( object p_class_ptr, string p_function, object[] p_parameters, bool oneStrike = false) {
		_classPtr = p_class_ptr;
		_function = p_function;
		_parameters = p_parameters;
		_isOneStrike = oneStrike;
	}


	public void removeCallback () {
		_classPtr = null;
		_function = string.Empty;
		_parameters = null;
	}

	#endregion

	#region animation type

	#endregion
}

public enum ComponentType{
	Alpha,
	SliceSprite,
	Sprite,
	Label,
	Material
}

public enum AnimationMethod{
	Linear,
	EaseIn,
	EaseOut,
	EaseInOut,
	BounceIn,
	BounceOut,
}
