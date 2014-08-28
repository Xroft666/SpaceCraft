using UnityEngine;
using System.Collections;

using pacman;
using pacman.controllers;
using pacman.controllers.examples;
using pacman.game;

using java.util;

public class Executor : MonoBehaviour 
{
	public Transform pacmanTransform;
	public Transform[] ghostsTransforms;

	private Material pillMaterial;

	Game game = new Game(0L);

	Controller pacManController = new StarterPacMan();
	Controller ghostController = new StarterGhosts();

	private GameObject[] pills;
	private GameObject[] powerPills;

	Constants.GHOST[] ghosts = new Constants.GHOST[]
	{
		Constants.GHOST.BLINKY,
     	Constants.GHOST.INKY,
     	Constants.GHOST.PINKY,
		Constants.GHOST.SUE
	};

	string[] ghostNames = new string[]
	{
		"blinky",
		"inky",
		"pinky",
		"sue"
	};

	IEnumerator Start()
	{
		float delay = 0.25f;

		pillMaterial = new Material( Shader.Find("Self-Illumin/VertexLit") );
		pillMaterial.color = Color.yellow;

		pills = new GameObject[game.getCurrentMaze().pillIndices.Length];
		for( int i = 0; i < game.getCurrentMaze().pillIndices.Length; i++ )
		{
			int nodeIdx = game.getCurrentMaze().pillIndices[i];

			GameObject pill = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			pill.transform.position = new Vector3(game.getCurrentMaze().graph[nodeIdx].x, game.getCurrentMaze().graph[nodeIdx].y, 0f);

			pill.renderer.sharedMaterial = pillMaterial;

			pills[i] = pill;
		}

		powerPills = new GameObject[game.getCurrentMaze().powerPillIndices.Length];
		for( int i = 0; i < game.getCurrentMaze().powerPillIndices.Length; i++ )
		{
			int nodeIdx = game.getCurrentMaze().powerPillIndices[i];
			
			GameObject pill = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			pill.transform.position = new Vector3(game.getCurrentMaze().graph[nodeIdx].x, game.getCurrentMaze().graph[nodeIdx].y, 0f);
			pill.transform.localScale = Vector3.one * 3f;

			pill.renderer.sharedMaterial = pillMaterial;

			powerPills[i] = pill;
		}

		while (!game.gameOver())
		{
			game.advanceGame((Constants.MOVE)pacManController.getMove(game.copy(), -1L), (EnumMap)ghostController.getMove(game.copy(), -1L));
			UpdateVisuals();

			yield return new WaitForSeconds(delay);
		}
	}

	public void UpdateVisuals()
	{
		int pacmanNode = game.getPacmanCurrentNodeIndex();
		pacmanTransform.transform.position = new Vector3(game.getCurrentMaze().graph[pacmanNode].x, game.getCurrentMaze().graph[pacmanNode].y, 0f);

			
		Constants.MOVE pacmanmove = game.getPacmanLastMoveMade();

		if( pacmanmove == Constants.MOVE.DOWN )
			pacmanTransform.animation.Play( "pacman" + "_down" );
		else	
			if( pacmanmove == Constants.MOVE.UP )
				pacmanTransform.animation.Play( "pacman" + "_up" );
		else	
			if( pacmanmove == Constants.MOVE.LEFT )
				pacmanTransform.animation.Play( "pacman" + "_left" );
		else	
			if( pacmanmove == Constants.MOVE.RIGHT )
				pacmanTransform.animation.Play( "pacman" + "_right" );


		int[] ghostsNodes = new int[4]; 
		for( int i = 0; i < 4; i++ )
		{
			Constants.MOVE move = game.getGhostLastMoveMade( ghosts[i] );
			
			if( move == Constants.MOVE.DOWN )
				ghostsTransforms[i].animation.Play( ghostNames[i] + "_down" );
			else	
			if( move == Constants.MOVE.UP )
				ghostsTransforms[i].animation.Play( ghostNames[i] + "_up" );
			else	
			if( move == Constants.MOVE.LEFT )
				ghostsTransforms[i].animation.Play( ghostNames[i] + "_left" );
			else	
			if( move == Constants.MOVE.RIGHT )
				ghostsTransforms[i].animation.Play( ghostNames[i] + "_right" );

			ghostsNodes[i] = game.getGhostCurrentNodeIndex(ghosts[i]);
		}


		for( int i = 0; i < 4; i ++ )
			ghostsTransforms[i].transform.position = new Vector3(game.getCurrentMaze().graph[ghostsNodes[i]].x, game.getCurrentMaze().graph[ghostsNodes[i]].y, 0f);

		if( game.wasPillEaten() )
		{
			pills[ game.getPillIndex(pacmanNode) ].renderer.enabled = false;
		}

		if( game.wasPowerPillEaten() )
		{
			powerPills[ game.getPowerPillIndex(pacmanNode) ].renderer.enabled = false;
		}
	}


}
