using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class ExampleSetup : MonoBehaviour {

	private void Start()
	{
		WorldManager.SpawnContainer (GenerateMotherBase(), new Vector3(5f, 0f, 5f), Quaternion.identity, 2 );


		Ship patrol = GeneratePatrolShip();

		// Generating missiles
		for( int i = 0; i < 40; i++ )
		{
			patrol.AddToCargo( GenerateMissile() );
		}

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
	

	private static Ship GenerateMissile()
	{
		Ship missile = new Ship(2){ EntityName = "Missile" };
		
		DEngine engine = new DEngine(){ EntityName = "engine", m_lookDirection = Vector3.forward, m_space = Space.Self };

		Device heatSeeker = GenerateHeatSeeker(3f);
		Device timeBomb = GenerateTimeBomb( 5f );
		DTimer activeTimer = new DTimer(){ EntityName = "activationtimer", m_timerSetUp = 2f };

		missile.IntegratedDevice.IntegrateDevice( engine );
		missile.IntegratedDevice.IntegrateDevice( timeBomb );
		missile.IntegratedDevice.IntegrateDevice( heatSeeker );
		missile.IntegratedDevice.IntegrateDevice( activeTimer );

		timeBomb.GetInternalDevice("warhead/ranger").m_isActive = false;
		heatSeeker.GetInternalDevice("ranger").m_isActive = false;
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
		WorldManager.SpawnContainer(new Ship(1f){ EntityName = "Target"}, 
		(Random.insideUnitCircle + Vector2.one) * (Camera.main.orthographicSize - 1f), 
		Quaternion.identity );
	}
	
	private static Ship GenerateShip()
	{
		Ship ship = new Ship(5f){ EntityName = "ship"};

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
		Ship ship = new Ship(0.3f){ EntityName = "patrolship"};

		GameObject[] markers = new GameObject[]
		{
			new GameObject("Marker1", typeof( TransformMarker )),
			new GameObject("Marker2", typeof( TransformMarker )),
			new GameObject("Marker3", typeof( TransformMarker )),
			new GameObject("Marker4", typeof( TransformMarker )),
		};

		markers[0].transform.position = Vector3.right * 15f;
		markers[1].transform.position = Vector3.left * 15f;
		markers[2].transform.position = Vector3.forward * 15f;
		markers[3].transform.position = Vector3.back * 15f;

		
		DEngine engine = new DEngine(){ EntityName = "engine", m_lookDirection = Vector3.forward, m_space = Space.Self };
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer"};
		DPatrolModule patrol = new DPatrolModule(){ EntityName = "patrol", 
				m_patrolPoints = new Vector3[] {
				markers[0].transform.position,
				markers[1].transform.position,
				markers[2].transform.position,
				markers[3].transform.position 
			}};

		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DRanger enemydetector = new DRanger(){ EntityName = "enemydetector", detectionRange = 5f };
		DMagnet magnet = new DMagnet();
		DTradeComputer trader = new DTradeComputer();
		
		

		ship.IntegratedDevice.IntegrateDevice( trader );
		ship.IntegratedDevice.IntegrateDevice( engine );
		ship.IntegratedDevice.IntegrateDevice( steerer );
		ship.IntegratedDevice.IntegrateDevice( patrol );
		ship.IntegratedDevice.IntegrateDevice( enemydetector );
		ship.IntegratedDevice.IntegrateDevice( launcher );
		ship.IntegratedDevice.IntegrateDevice( magnet );



		BSBranch rootDecision = ship.IntegratedDevice.Blueprint.CreateBranch();

		BSSequence patrolSequence = ship.IntegratedDevice.Blueprint.CreateSequence(); 
		BSAction disableEngine = ship.IntegratedDevice.Blueprint.CreateAction( "DeactivateDevice", engine );
		BSAction steerTowardsTarget = ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", steerer, patrol.GetQuery("CurrentTarget") );
		BSAction enableEngine = ship.IntegratedDevice.Blueprint.CreateAction( "ActivateDevice", engine );
		BSAction waitUntilReach = ship.IntegratedDevice.Blueprint.CreateAction( "ReachTarget", patrol, patrol.GetQuery("CurrentTarget") );
		BSAction nextPoint = ship.IntegratedDevice.Blueprint.CreateAction( "SetNextPoint", patrol );


		BSBranch miningDecision = ship.IntegratedDevice.Blueprint.CreateBranch();
		miningDecision.AddCondition( magnet.GetCheck("IsStorageble"), 
		                            enemydetector.GetQuery("CurrentTargetContainer") );




		BSSequence shootingSequence = ship.IntegratedDevice.Blueprint.CreateSequence();
		BSAction steerTowardsShootingTarget = 
			ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", steerer, 
			                                             enemydetector.GetQuery("CurrentTargetPosition") );
		BSAction shootTarget = ship.IntegratedDevice.Blueprint.CreateAction( "Fire", launcher );

		BSSequence collectingSequence = ship.IntegratedDevice.Blueprint.CreateSequence();
		BSAction attractAsteroid = ship.IntegratedDevice.Blueprint.CreateAction("Attract", magnet, 
		                                                                        enemydetector.GetQuery("CurrentTargetContainer"));
		BSAction storageAsteroid = ship.IntegratedDevice.Blueprint.CreateAction("Load", magnet, 
		                                                                        enemydetector.GetQuery("CurrentTargetContainer"));


		DeviceQuery tradeInfo = () =>
		{
			return ship.m_cargo.ComposeTradeOffer("Asteroid");
		};

		DeviceQuery stationPosition = () =>
		{
			return new PositionArgs(){ position = WorldManager.RequestContainerData("MotherBase").View.transform.position};
		};

		Device stationTrader = WorldManager.RequestContainerData("MotherBase").IntegratedDevice.GetInternalDevice("trader");


		BSSequence goingHomeSequence = ship.IntegratedDevice.Blueprint.CreateSequence();
		BSAction steerTowardsHome = ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", steerer, stationPosition );
		BSAction waitUntilReachHome = ship.IntegratedDevice.Blueprint.CreateAction( "ReachTarget", patrol, stationPosition );
		BSAction sellResouces = ship.IntegratedDevice.Blueprint.CreateAction( "LoadItemsFrom", stationTrader, tradeInfo );


		// interupt current commands stack
//		enemydetector.AddEvent("OnRangerEntered", ship.IntegratedDevice.Blueprint.InterruptExecution);


		ship.IntegratedDevice.Blueprint.m_entryPoint.AddChild(rootDecision);

		rootDecision.AddCondition( enemydetector.GetCheck("IsAnyTarget") );
		rootDecision.AddCondition( ship.IntegratedDevice.GetCheck("IsCargoFull") );


		rootDecision.AddChild(miningDecision);
		rootDecision.AddChild(goingHomeSequence);
		rootDecision.AddChild(patrolSequence);


		miningDecision.AddChild(collectingSequence);
		miningDecision.AddChild(shootingSequence);


		collectingSequence.AddChild( storageAsteroid );
		collectingSequence.AddChild( attractAsteroid );
		collectingSequence.AddChild( disableEngine );


		shootingSequence.AddChild( shootTarget );
		shootingSequence.AddChild( steerTowardsShootingTarget );
		shootingSequence.AddChild( disableEngine );

		patrolSequence.AddChild(nextPoint);
		patrolSequence.AddChild(waitUntilReach);
		patrolSequence.AddChild(enableEngine);
		patrolSequence.AddChild(steerTowardsTarget);
		patrolSequence.AddChild(disableEngine);


		goingHomeSequence.AddChild(sellResouces);
		goingHomeSequence.AddChild(waitUntilReachHome);
		goingHomeSequence.AddChild(enableEngine);
		goingHomeSequence.AddChild(steerTowardsHome);
		goingHomeSequence.AddChild(disableEngine);

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
			new DEngine(){ EntityName = "forward", m_lookDirection = Vector3.forward, m_space = Space.World },
			new DEngine(){ EntityName = "backward", m_lookDirection = Vector3.back, m_space = Space.World },
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

		BSEntry onClose = warheadDevice.Blueprint.CreateEntry( "OnRangerEntered", ranger );
		BSAction toDetonate = warheadDevice.Blueprint.CreateAction( "Detonate", detonator );
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
		BSAction toDetonate = timeBomb.Blueprint.CreateAction( "Detonate", timeBomb.GetInternalDevice("warhead/detonator") );
		timeBomb.Blueprint.ConnectElements( onTimer, toDetonate );

		return timeBomb;
	}

	// Heat seeker
	private static Device GenerateHeatSeeker( float detectionRange )
	{
		Device heatSeeker = new Device(){ EntityName = "heatseeker"};

		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };
		DSteerModule steerer = new DSteerModule() { EntityName = "steerer" };
		

		heatSeeker.IntegrateDevice( ranger );
		heatSeeker.IntegrateDevice( steerer );


		BSSequence onTargetFound = heatSeeker.Blueprint.CreateSequence();

		// add target signature
		BSEntry inRange = heatSeeker.Blueprint.CreateEntry( "OnRangerEntered", ranger );

		// steer towards the target
		BSAction toSteer = heatSeeker.Blueprint.CreateAction( "SteerTowards", steerer, ranger.GetQuery("CurrentTargetPosition"));

		inRange.AddChild( onTargetFound );
		onTargetFound.AddChild( toSteer );

		
		return heatSeeker;
	}

	private static Ship GenerateMotherBase()
	{
		Ship motherBase = new Ship(1000f){ EntityName = "MotherBase" };

		DMagnet magnet = new DMagnet(){ EntityName = "magnet" };
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = 5f };
		DTradeComputer trader = new DTradeComputer(){ EntityName = "trader"} ;


		motherBase.IntegratedDevice.IntegrateDevice( ranger );
		motherBase.IntegratedDevice.IntegrateDevice( magnet );
		motherBase.IntegratedDevice.IntegrateDevice( trader );


		BSBranch rootDecision = motherBase.IntegratedDevice.Blueprint.CreateBranch();
		rootDecision.AddCondition( ranger.GetCheck("IsAnyTarget") );

		return motherBase;
	}
}
