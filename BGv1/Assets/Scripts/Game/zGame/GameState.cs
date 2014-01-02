using UnityEngine;
using System.Collections;

public class GameState : BasicGameState {

	Game2DView m_game2DView;
	private int _waveNo;

	public override void OnStart () {
		view2D = Game2DView.Create();
		m_game2DView = (Game2DView) view2D;
		_waveNo = 1;
		base.OnStart ();
	}

	public override void OnUpdate () {
	}
}
