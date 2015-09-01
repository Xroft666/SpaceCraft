using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class ExampleSetup : MonoBehaviour {

	private void Start()
	{
		// Generating random targets
		for( int i = 0; i < 5; i++ )
		{
//			GenerateTarget();
		}
		
		// Generating the ship
//		Ship ship = GenerateShip();
//		ship.View.transform.position = Vector3.left * 8f;

		Ship patrol = GeneratePatrolShip();

		// Generating missiles
		for( int i = 0; i < 20; i++ )
		{
//			patrol.AddToCargo( GenerateMissile() );
//			ship.AddToCargo( GenerateMissile() );
		}

//		WorldManager.SpawnContainer( GenerateMissile(), Vector3.zero, Quaternion.identity );


		for( int i = 0; i < 100; i++ )
		{

			Vector3 pos = Vector3.zero;
			pos = UnityEngine.Random.insideUnitCircle * 50f;

	//		pos.x = (float) GaussRandom();
	//		pos.y = (float) GaussRandom();


	//		WorldManager.GenerateAsteroid( pos, Random.Range(0f,360f), Random.Range(0.1f, 2.4f));
		}
	}


	private double GaussRandom()
	{
		int seed = 6666666;
		double sum = 0.0;
	
		for( int i = 0; i < 3; i++ )
		{
			long holdseed = seed;
			seed ^= seed << 13;
			seed ^= seed >> 17;
			seed ^= seed << 5;
			long r = (System.Int64) (holdseed + seed);
			sum += (double) r * (1.0 / 0x7fffffffffffffff);
		}

		return sum;
	}
	private static Ship GenerateMissile()
	{
		Ship missile = new Ship(){ EntityName = "Missile" };
		
		DEngine engine = new DEngine(){ EntityName = "engine", m_lookDirection = Vector3.up, m_space = Space.Self };

		Device heatSeeker = GenerateHeatSeeker(3f);
		Device timeBomb = GenerateTimeBomb( 5f );
		DTimer activeTimer = new DTimer(){ EntityName = "activationtimer", m_timerSetUp = 2f };

		missile.IntegratedDevice.IntegrateDevice( engine );
		missile.IntegratedDevice.IntegrateDevice( timeBomb );
		missile.IntegratedDevice.IntegrateDevice( heatSeeker );
		missile.IntegratedDevice.IntegrateDevice( activeTimer );

		Job.make( timeBomb.DeactivateDevice( null ), true);
    	Job.make( heatSeeker.DeactivateDevice( null), true );


		BSEntry onTimer = missile.IntegratedDevice.Blueprint.CreateEntry( "OnTimerComplete", activeTimer );
		BSAction toActivateWarhead = missile.IntegratedDevice.Blueprint.CreateAction( "ActivateDevice", timeBomb );
		BSAction toActivateSeeker = missile.IntegratedDevice.Blueprint.CreateAction( "ActivateDevice", heatSeeker );

		missile.IntegratedDevice.Blueprint.ConnectElements( onTimer, toActivateWarhead );
		missile.IntegratedDevice.Blueprint.ConnectElements( onTimer, toActivateSeeker );

		return missile;
	}
	
	private static void GenerateTarget()
	{
		WorldManager.SpawnContainer(new Ship(){ EntityName = "Target"}, 
		(Random.insideUnitCircle + Vector2.one) * (Camera.main.orthographicSize - 1f), 
		Quaternion.identity );
	}
	
	private static Ship GenerateShip()
	{
		Ship ship = new Ship(){ EntityName = "ship"};

		Device cockpit = GeneratePilotCockpit();
		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DInputModule mouseInput = new DInputModule() { EntityName = "space", m_keyCode = KeyCode.Space };


		ship.IntegratedDevice.IntegrateDevice( launcher );
		ship.IntegratedDevice.IntegrateDevice( cockpit );
		ship.IntegratedDevice.IntegrateDevice( mouseInput );

		
		BSEntry onMouseUp = ship.IntegratedDevice.Blueprint.CreateEntry( "space/OnInputReleased", ship.IntegratedDevice);
		BSAction toFire = ship.IntegratedDevice.Blueprint.CreateAction( "launcher/Fire", ship.IntegratedDevice);
		ship.IntegratedDevice.Blueprint.ConnectElements( onMouseUp, toFire );

		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity, 1 );
		
		return ship;
	}

	private static Ship GeneratePatrolShip()
	{
		Ship ship = new Ship(){ EntityName = "patrolship"};

		GameObject[] markers = new GameObject[]
		{
			new GameObject("Marker1", typeof( TransformMarker )),
			new GameObject("Marker2", typeof( TransformMarker )),
			new GameObject("Marker3", typeof( TransformMarker )),
			new GameObject("Marker4", typeof( TransformMarker )),
		};

		markers[0].transform.position = Vector3.right * 3f;
		markers[1].transform.position = Vector3.left * 3f;
		markers[2].transform.position = Vector3.up * 3f;
		markers[3].transform.position = Vector3.down * 3f;

		
		DEngine engine = new DEngine(){ EntityName = "engine", m_lookDirection = Vector3.up, m_space = Space.Self };
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer"};
		DPatrolModule patrol = new DPatrolModule(){ EntityName = "patrol", 
				m_patrolPoints = new Vector3[] {
				markers[0].transform.position,
				markers[1].transform.position,
				markers[2].transform.position,
				markers[3].transform.position 
			}};

		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };

		Device enemyDetector = GenerateEnemyDetector( 5f );


		ship.IntegratedDevice.IntegrateDevice( engine );
		ship.IntegratedDevice.IntegrateDevice( steerer );
		ship.IntegratedDevice.IntegrateDevice( patrol );
		ship.IntegratedDevice.IntegrateDevice( enemyDetector );
		ship.IntegratedDevice.IntegrateDevice( launcher );


		BSBranch rootDecision = ship.IntegratedDevice.Blueprint.CreateBranch();

		BSSequence patrolSequence = ship.IntegratedDevice.Blueprint.CreateSequence(); 
		BSAction disableEngine = ship.IntegratedDevice.Blueprint.CreateAction( "DeactivateDevice", engine );
		BSAction steerTowardsTarget = ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", steerer, patrol.GetQuery("CurrentTarget") );
		BSAction enableEngine = ship.IntegratedDevice.Blueprint.CreateAction( "ActivateDevice", engine );
		BSAction waitUntilReach = ship.IntegratedDevice.Blueprint.CreateAction( "ReachTarget", patrol, patrol.GetQuery("CurrentTarget") );
		BSAction nextPoint = ship.IntegratedDevice.Blueprint.CreateAction( "SetNextPoint", patrol );

		BSSequence miningSequence = ship.IntegratedDevice.Blueprint.CreateSequence();
		BSAction steerTowardsShootingTarget = 
			ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", steerer, 
			                                              ship.IntegratedDevice.GetInternalDevice("enemydetector/radar").GetQuery("CurrentTarget") );
		BSAction shootTarget = ship.IntegratedDevice.Blueprint.CreateAction( "Fire", launcher );


		ship.IntegratedDevice.Blueprint.m_entryPoint.AddChild(rootDecision);

//		rootDecision.AddCondition( ship.IntegratedDevice.GetCheck( "IsCargoFull") );
		rootDecision.AddCondition( ship.IntegratedDevice.GetInternalDevice("enemydetector/radar").GetCheck("IsAnyTarget") );

		rootDecision.AddChild(miningSequence);
		rootDecision.AddChild(patrolSequence);

		miningSequence.AddChild( disableEngine );
		miningSequence.AddChild( steerTowardsShootingTarget );
		miningSequence.AddChild( shootTarget );

		patrolSequence.AddChild(nextPoint);
		patrolSequence.AddChild(waitUntilReach);
		patrolSequence.AddChild(enableEngine);
		patrolSequence.AddChild(steerTowardsTarget);
		patrolSequence.AddChild(disableEngine);

//		patrolSequence.AddChild(disableEngine);
//		patrolSequence.AddChild(steerTowardsTarget);
//		patrolSequence.AddChild(enableEngine);
//		patrolSequence.AddChild(waitUntilReach);
//		patrolSequence.AddChild(nextPoint);

		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity, 2 );
		
		return ship;
	}

	private static Device GeneratePilotCockpit()
	{
		Device input = GenerateInclusiveInputModule();
		Device engines = GenerateInclusiveEngineModule();
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer" };

		Device cockpitDevice = new Device(){ EntityName = "cockpit"};
		cockpitDevice.IntegrateDevice( input );
		cockpitDevice.IntegrateDevice( engines );
		cockpitDevice.IntegrateDevice( steerer );

		// Steering module

		BSEntry onMouseWorld = cockpitDevice.Blueprint.CreateEntry( "input/mousePos/OnMouseWorldPosition", cockpitDevice);
		BSAction toSteer = cockpitDevice.Blueprint.CreateAction( "steerer/SteerTowards", cockpitDevice);
		cockpitDevice.Blueprint.ConnectElements( onMouseWorld, toSteer );

		// Movement module

		BSEntry onForwardDown = cockpitDevice.Blueprint.CreateEntry( "input/w/OnInputHeld", cockpitDevice);
		BSAction toGoForward = cockpitDevice.Blueprint.CreateAction( "engines/forward/MoveForward", cockpitDevice);
		cockpitDevice.Blueprint.ConnectElements( onForwardDown, toGoForward );
		
		BSEntry onBackwardDown = cockpitDevice.Blueprint.CreateEntry( "input/s/OnInputHeld", cockpitDevice);
		BSAction toGoBackward = cockpitDevice.Blueprint.CreateAction( "engines/backward/MoveForward", cockpitDevice);
		cockpitDevice.Blueprint.ConnectElements( onBackwardDown, toGoBackward );
		
		BSEntry onLeftDown = cockpitDevice.Blueprint.CreateEntry( "input/a/OnInputHeld", cockpitDevice);
		BSAction toGoleft = cockpitDevice.Blueprint.CreateAction( "engines/left/MoveForward", cockpitDevice);
		cockpitDevice.Blueprint.ConnectElements( onLeftDown, toGoleft );
		
		BSEntry onRightDown = cockpitDevice.Blueprint.CreateEntry( "input/d/OnInputHeld", cockpitDevice);
		BSAction toGoRight = cockpitDevice.Blueprint.CreateAction( "engines/right/MoveForward", cockpitDevice);
		cockpitDevice.Blueprint.ConnectElements( onRightDown, toGoRight );

		return cockpitDevice;
	}

	private static Device GenerateInclusiveInputModule()
	{
		List<Device> inputs = new List<Device>() 
		{
			new DInputModule(){ EntityName = "w", m_keyCode = KeyCode.W },
			new DInputModule(){ EntityName = "s", m_keyCode = KeyCode.S },
			new DInputModule(){ EntityName = "a", m_keyCode = KeyCode.A },
			new DInputModule(){ EntityName = "d", m_keyCode = KeyCode.D },
			new DInputModule(){ EntityName = "mousePos" }
		};
		
		Device inclusiveDevice = new Device(){ EntityName = "input"};
		inclusiveDevice.IntegrateDevices( inputs );
		
		return inclusiveDevice;
	}

	private static Device GenerateInclusiveEngineModule()
	{
		List<Device> inputs = new List<Device>() 
		{
			new DEngine(){ EntityName = "forward", m_lookDirection = Vector3.up, m_space = Space.World },
			new DEngine(){ EntityName = "backward", m_lookDirection = Vector3.down, m_space = Space.World },
			new DEngine(){ EntityName = "left", m_lookDirection = Vector3.left, m_space = Space.World },
			new DEngine(){ EntityName = "right", m_lookDirection = Vector3.right, m_space = Space.World }
		};
		
		Device inclusiveDevice = new Device(){ EntityName = "engines"};
		inclusiveDevice.IntegrateDevices( inputs );
		
		return inclusiveDevice;
	}

	// Can be a mine as it is
	private static Device GenerateWarhead( float detectionRange )
	{
		Device warheadDevice = new Device(){ EntityName = "warhead"};

		DDetonator detonator = new DDetonator(){ EntityName = "detonator"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };

		warheadDevice.IntegrateDevice( detonator );
		warheadDevice.IntegrateDevice( ranger );

		BSEntry onClose = warheadDevice.Blueprint.CreateEntry( "ranger/OnRangerEntered", warheadDevice );
		BSAction toDetonate = warheadDevice.Blueprint.CreateAction( "detonator/Detonate", warheadDevice );
		warheadDevice.Blueprint.ConnectElements( onClose, toDetonate );
		
		return warheadDevice;
	}


	private static Device GenerateTimeBomb( float time )
	{
		Device timeBomb = new Device(){ EntityName = "timebomb" };

		Device warhead = GenerateWarhead( 1f );
		DTimer timer = new DTimer() { EntityName = "timer", m_timerSetUp = time };

		timeBomb.IntegrateDevice( warhead );
		timeBomb.IntegrateDevice( timer );

		// Generating warhead
		BSEntry onTimer = timeBomb.Blueprint.CreateEntry( "OnTimerComplete", timer );
		BSAction toDetonate = timeBomb.Blueprint.CreateAction( "warhead/detonator/Detonate", warhead );
		timeBomb.Blueprint.ConnectElements( onTimer, toDetonate );

		return timeBomb;
	}

	// Heat seeker
	private static Device GenerateHeatSeeker( float detectionRange )
	{
		Device heatSeeker = new Device(){ EntityName = "heatseeker"};
		
		DFriendOrFoeUnit radar = new DFriendOrFoeUnit(){ EntityName = "radar"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };
		DSteerModule steerer = new DSteerModule() { EntityName = "steerer" };
		
		heatSeeker.IntegrateDevice( radar );
		heatSeeker.IntegrateDevice( ranger );
		heatSeeker.IntegrateDevice( steerer );
		
		BSEntry inRange = heatSeeker.Blueprint.CreateEntry( "ranger/OnRangerEntered", heatSeeker );
		BSAction toFollow = heatSeeker.Blueprint.CreateAction( "radar/AddTarget", heatSeeker );
		heatSeeker.Blueprint.ConnectElements( inRange, toFollow );

		BSEntry outRange = heatSeeker.Blueprint.CreateEntry( "ranger/OnRangerEscaped", heatSeeker );
		BSAction toNullTarget = heatSeeker.Blueprint.CreateAction( "radar/RemoveTarget", heatSeeker );
		heatSeeker.Blueprint.ConnectElements( outRange, toNullTarget );

		BSEntry onTargetPos = heatSeeker.Blueprint.CreateEntry( "radar/TargetPosition", heatSeeker);
		BSAction toSteer = heatSeeker.Blueprint.CreateAction( "steerer/SteerTowards", heatSeeker);
		heatSeeker.Blueprint.ConnectElements( onTargetPos, toSteer );
		
		return heatSeeker;
	}

	private static Device GenerateEnemyDetector( float detectionRange )
	{
		Device detector = new Device(){ EntityName = "enemydetector"};
		
		DFriendOrFoeUnit radar = new DFriendOrFoeUnit(){ EntityName = "radar"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };
		
		detector.IntegrateDevice( radar );
		detector.IntegrateDevice( ranger );

		
		return detector;
	}

	private static Device GenerateAsteroidGrabber()
	{
		Device grabber = new Device(){ EntityName = "grabber"};
		
		DManipulator manipulator = new DManipulator(){ EntityName = "manipulator"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = 1.5f };
		
		grabber.IntegrateDevice( manipulator );
		grabber.IntegrateDevice( ranger );
		

		BSEntry inRange = grabber.Blueprint.CreateEntry( "grabber/ranger/OnRangerEntered", grabber );
		BSAction toFollow = grabber.Blueprint.CreateAction( "grabber/manipulator/Load", grabber );
		grabber.Blueprint.ConnectElements( inRange, toFollow );


		return grabber;
	}

	private static Device GenerateAsteroidAttractor()
	{
		Device attractor = new Device(){ EntityName = "attractor"};
		
		DMagnet magnet = new DMagnet(){ EntityName = "magnet"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = 2.5f };
		
		attractor.IntegrateDevice( magnet );
		attractor.IntegrateDevice( ranger );


		BSEntry inRange = attractor.Blueprint.CreateEntry( "attractor/ranger/OnRangerEntered", attractor );
		BSAction toFollow = attractor.Blueprint.CreateAction( "attractor/magnet/Attract", attractor );
		attractor.Blueprint.ConnectElements( inRange, toFollow );
		
		BSEntry outRange = attractor.Blueprint.CreateEntry( "attractor/ranger/OnRangerEscaped", attractor );
		BSAction toNullTarget = attractor.Blueprint.CreateAction( "attractor/magnet/RemoveTarget", attractor );
		attractor.Blueprint.ConnectElements( outRange, toNullTarget );


		return attractor;
	}
}
