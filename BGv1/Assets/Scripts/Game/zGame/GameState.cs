using UnityEngine;
using System.Collections;

public class GameState : BasicGameState {

	Game2DView m_game2DView;
	InGameView m_ingameView;

	private int _waveNo;

	public override void OnStart () {
		view2D = Game2DView.Create();
		viewUI = InGameView.Create ();
		m_game2DView = (Game2DView) view2D;
		m_ingameView = (InGameView) viewUI;

		_waveNo = 1;
		base.OnStart ();
	}

	public override void OnUpdate () {

	}
}
