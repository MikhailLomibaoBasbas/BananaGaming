using UnityEngine;
using System;

public class Sprite : MonoBehaviour {
	
	public int numRow;
	public int numCol;
	
	private Renderer _renderer;
	protected Renderer localRenderer {
		get {
			if(_renderer == null)
				_renderer = renderer;
			return _renderer;
		}
	}
	
	private int _row = 1;
	public int row {
		get { return _row; }
	}
	
	private int _column = 1;
	public int column {
		get { return _column; }
	}
	
	private Vector2 _size = Vector2.one;
	private Vector2 _offset = Vector2.zero;
	
	void Start() {
	}
	
	public void SetSpriteIndex(int row, int column) {
		float uIndex = column % numCol;
		float vIndex = row / numRow;
		// use negative sizes so we can invert the image
		_size.x = -1.0f/numCol;
		_size.y = -1.0f/numRow;
		
		_offset.x = Mathf.Abs(_size.x + uIndex * _size.x);
		_offset.y = 1.0f - vIndex * _size.y;
		localRenderer.material.SetTextureScale("_MainTex", _size);
		localRenderer.material.SetTextureOffset("_MainTex", _offset);
	}
	
}

