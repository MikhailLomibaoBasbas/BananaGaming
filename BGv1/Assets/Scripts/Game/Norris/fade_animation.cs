using UnityEngine;
using System.Collections;

public class fade_animation : common_animation {

	private float alpha_from;
	private float alpha_to;

	private Color color_from;
	private Color color_to;

	private FadeType fade_type;

	public static fade_animation begin(GameObject go, FadeType type, Color fadeto, float time,
					object classPtr = null, string callbackFunction = null, object[] parameters = null,
              		bool start = false, bool oneStrike = false){
		fade_animation temp_fade;
		if ((temp_fade = go.GetComponent<fade_animation> ()) == null) 
			temp_fade = go.AddComponent<fade_animation> ();

		temp_fade.initialize ();
		temp_fade.set_values (type, fadeto, time);

		if (classPtr != null && callbackFunction != null)
			temp_fade.addCallBack (classPtr, callbackFunction, parameters, oneStrike);

		if (start)
			temp_fade.start_animation (start);

		return temp_fade;
	}

	public static fade_animation begin(GameObject go, FadeType type, Color fadeto, float time){
		return begin (go, type, fadeto, time, null, null, null, true);
	}
	// Use this for initialization

	public override void initialize(){
		base.initialize();
		if (component_type != ComponentType.Alpha)
			color_to = GetComponent<UIWidget> ().color;
	}

	public void reset(){
		animation_time = 0.0f;
	}

	public void start_animation (bool start){
		is_animating = start;
		animation_time = 0;
		if (ComponentType.Alpha == component_type) {
			ui_alpha.alpha = (start) ? alpha_from: alpha_to;
		} else if (ComponentType.SliceSprite == component_type) {
			ui_sliced_sprite.color = (start) ? color_from: color_to;
		} else if (ComponentType.Sprite == component_type) {
			ui_sprite.color = (start) ? color_from: color_to;
		} else if (ComponentType.Label == component_type) {
			ui_label.color = (start) ? color_from : color_to;
		}
	}

	// Update is called once per frame
	void Update () {
		if (is_animating) 
		{
			animation_time += (Time.timeScale != 0) ? Time.deltaTime / Time.timeScale : Time.fixedDeltaTime;

			if (animation_time < max_animation_time) {
				update_animation ();
			} else {
				start_animation (false);
				doCallback ();
			}
		}
	}

	private void update_animation(){
		float lerp_time = animation_time / max_animation_time;
		//Debug.Log ("LERP TIME: " + lerp_time);
		switch(component_type){
		case ComponentType.Material:
			go_material.material.color = Color.Lerp (color_from, color_to, lerp_time);
			break;
		case ComponentType.Alpha:
			ui_alpha.alpha = Mathf.Lerp (alpha_from, alpha_to, lerp_time);
			break;
		case ComponentType.SliceSprite:
			ui_sliced_sprite.color = Color.Lerp (color_from, color_to, lerp_time);
			break;
		case ComponentType.Sprite:
			ui_sprite.color = Color.Lerp (color_from, color_to, lerp_time);
			break;
		case ComponentType.Label:
			ui_label.color = Color.Lerp (color_from, color_to, lerp_time);
			break;
		}
	}

	public void set_values(FadeType type, Color fadeto, float time){
		fade_type = type;
		max_animation_time = time;
		//color_to = fadeto;

		set_fade_values ();
	}

	private void set_fade_values(){
		if (fade_type == FadeType.FadeIn) 
		{
			switch(component_type){
				case ComponentType.Material:
					color_from = Color.clear;
					color_to = go_material.material.color;
					go_material.material.color = color_from;
				break;
			case ComponentType.Alpha:
					alpha_from = 0;
					ui_alpha.alpha = alpha_from;
					alpha_to = 1;
				break;
			case ComponentType.SliceSprite:
					color_from = Color.clear;
					color_to = ui_sliced_sprite.color;
					ui_sliced_sprite.color = color_from;
				break;
			case ComponentType.Sprite:
					color_from = Color.clear;
					color_to = ui_sprite.color;
					ui_sprite.color = color_from;
				break;
				case ComponentType.Label:
					color_from = Color.clear;
					color_to = ui_label.color;
					ui_label.color = color_from;
				break;
			}
		}else if(fade_type == FadeType.FadeOut)
		{
			color_to = Color.clear;
			switch(component_type){
				case ComponentType.Material:
					color_from = go_material.material.color;
					go_material.material.color = color_from;
				break;
				case ComponentType.Alpha:
					alpha_from = 1;
					ui_alpha.alpha = alpha_from;
					alpha_to = 0;
				break;
				case ComponentType.SliceSprite:
					color_from = ui_sliced_sprite.color;
					ui_sliced_sprite.color = color_from;
				break;
				case ComponentType.Sprite:
					color_from = ui_sprite.color;
					ui_sprite.color = color_from;
				break;
				case ComponentType.Label:
					color_from = ui_label.color;
					ui_label.color = color_from;
				break;
			}
		}
	}
}


public enum FadeType{
	FadeIn,
	FadeOut
}