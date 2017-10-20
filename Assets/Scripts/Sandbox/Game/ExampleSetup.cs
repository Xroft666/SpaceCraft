using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class ExampleSetup : MonoBehaviour {

	private void Start()
	{
		WorldManager.SpawnContainer (TemplatesGenerator.GenerateMotherBase(), new Vector3(5f, 0f, 5f), Quaternion.identity, 2 );


		Ship enemyShip = TemplatesGenerator.GeneratePatrolShip();
		Ship myShip = TemplatesGenerator.GenerateMyShip();

		// Generating missiles
		for( int i = 0; i < 50; i++ )
		{
			enemyShip.AddToCargo( TemplatesGenerator.GenerateMissile() );
			myShip.AddToCargo( TemplatesGenerator.GenerateMissile() );
		}

		myShip.AddToCargo( TemplatesGenerator.GenerateHeatSeeker(1f) );
		myShip.AddToCargo( TemplatesGenerator.GenerateInclusiveEngineModule() );
		myShip.AddToCargo( TemplatesGenerator.GenerateInclusiveInputModule() );
		myShip.AddToCargo( TemplatesGenerator.GenerateNavigator() );
		myShip.AddToCargo( TemplatesGenerator.GenerateTimeBomb(1f) );
		myShip.AddToCargo( TemplatesGenerator.GenerateWarhead(1f) );

		myShip.AddToCargo( new DDetonator(){EntityName = "Detonator"} );
		myShip.AddToCargo( new DEngine(){EntityName = "Engine"} );
		myShip.AddToCargo( new DInputModule(){EntityName = "InputModule"} );
		myShip.AddToCargo( new DLauncher(){EntityName = "Launcher"} );
		myShip.AddToCargo( new DMagnet(){EntityName = "Magnet"} );
		myShip.AddToCargo( new DPatrolModule(){EntityName = "PatrolModule"} );
		myShip.AddToCargo( new DRanger(){EntityName = "Ranger"} );
		myShip.AddToCargo( new DSteerModule(){EntityName = "SteerModule"} );
		myShip.AddToCargo( new DTimer(){EntityName = "Timer"} );
		myShip.AddToCargo( new DTradeComputer(){EntityName = "TradeComputer"} );


		//Random.seed = 7;
		for( int i = 0; i < 100; i++ )
		{
			Vector3 pos = Vector3.zero;
			pos = UnityEngine.Random.insideUnitCircle * 50f;
			pos = new Vector3(pos.x, 0f, pos.y);

			float radius = Random.Range(0.1f, 2.4f);

			if( Physics.CheckSphere(pos, radius) )
				continue;

			WorldManager.GenerateAsteroid( pos, Random.Range(0f,360f), radius);
		}
	
	}

	private static void GenerateTarget()
	{
		WorldManager.SpawnContainer(new Ship(1f){ EntityName = "Target"}, 
			(Random.insideUnitCircle + Vector2.one) * (Camera.main.orthographicSize - 1f), 
			Quaternion.identity );
	}
}
