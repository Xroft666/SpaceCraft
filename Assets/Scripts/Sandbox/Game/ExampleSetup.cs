using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class ExampleSetup : MonoBehaviour {

	private void Start()
	{
		WorldManager.SpawnContainer (GenerateMotherBase(), new Vector3(5f, 0f, 5f), Quaternion.identity, 2 );


		Ship ship = GeneratePatrolShip();
//		Ship ship = GenerateShip();

		// Generating missiles
		for( int i = 0; i < 40; i++ )
		{
			ship.AddToCargo( GenerateMissile() );
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
//		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DRanger ranger = new DRanger() {EntityName = "ranger", detectionRange = 3f };
		DMagnet magnet = new DMagnet(){ EntityName = "magnet" };
		DInputModule mouseInput = new DInputModule() { EntityName = "space", m_keyCode = KeyCode.Space };


//		ship.IntegratedDevice.IntegrateDevice( launcher );
		ship.IntegratedDevice.IntegrateDevice( ranger );
		ship.IntegratedDevice.IntegrateDevice( magnet );
		ship.IntegratedDevice.IntegrateDevice( cockpit );
		ship.IntegratedDevice.IntegrateDevice( mouseInput );


		BSEntry onMouseDown = ship.IntegratedDevice.Blueprint.CreateEntry( "OnInputPressed", mouseInput);
		BSAction toAtract = ship.IntegratedDevice.Blueprint.CreateAction( "Attract", magnet);
		BSQuery currentTargetContainer = ship.IntegratedDevice.Blueprint.CreateQuery("CurrentTargetContainer", ranger);
		toAtract.ConnectToQuery( currentTargetContainer );
		ship.IntegratedDevice.Blueprint.ConnectElements( onMouseDown, toAtract );

		BSEntry onMouseUp = ship.IntegratedDevice.Blueprint.CreateEntry( "OnInputReleased", mouseInput);
		BSAction toRepulse = ship.IntegratedDevice.Blueprint.CreateAction( "Repulse", magnet);
		toAtract.ConnectToQuery( currentTargetContainer );
		ship.IntegratedDevice.Blueprint.ConnectElements( onMouseUp, toRepulse );

		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity, 1 );
		
		return ship;
	}

	private static Ship GeneratePatrolShip()
	{
		Ship ship = new Ship(0.3f){ EntityName = "patrolship"};
	
		Device navigator = GenerateNavigator();
		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DRanger enemydetector = new DRanger(){ EntityName = "enemydetector", detectionRange = 5f };
		DMagnet magnet = new DMagnet();
		DTradeComputer trader = new DTradeComputer();
		
		

		ship.IntegratedDevice.IntegrateDevice( trader );
		ship.IntegratedDevice.IntegrateDevice( navigator );
		ship.IntegratedDevice.IntegrateDevice( enemydetector );
		ship.IntegratedDevice.IntegrateDevice( launcher );
		ship.IntegratedDevice.IntegrateDevice( magnet );



		BSBranch rootDecision = ship.IntegratedDevice.Blueprint.CreateBranch("Root");



		BSSequence patrolSequence = ship.IntegratedDevice.Blueprint.CreateSequence("Patrol"); 
		BSAction nextPoint = ship.IntegratedDevice.Blueprint.CreateAction( "SetNextPoint", navigator.GetInternalDevice("patrol") );
		BSExit moveToWaypoint = ship.IntegratedDevice.Blueprint.CreateExit("MoveTo", navigator );


		BSBranch miningDecision = ship.IntegratedDevice.Blueprint.CreateBranch("MiningDec");
		miningDecision.AddCondition( magnet.GetCheck("IsStorageble"), 
		                            enemydetector.GetQuery("CurrentTargetContainer") );




		BSSequence shootingSequence = ship.IntegratedDevice.Blueprint.CreateSequence("Shooting");
		BSAction steerTowardsShootingTarget = 
			ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", navigator.GetInternalDevice("steerer") );
		BSQuery currentTargetPosition = ship.IntegratedDevice.Blueprint.CreateQuery("CurrentTargetPosition", enemydetector);
		steerTowardsShootingTarget.ConnectToQuery( currentTargetPosition );
		BSAction shootTarget = ship.IntegratedDevice.Blueprint.CreateAction( "Fire", launcher );


		BSSequence collectingSequence = ship.IntegratedDevice.Blueprint.CreateSequence("Collecting");
		BSQuery currentTargetContainer = ship.IntegratedDevice.Blueprint.CreateQuery("CurrentTargetContainer", enemydetector);


		BSAction disableEngineToCollect = ship.IntegratedDevice.Blueprint.CreateAction("DeactivateDevice", navigator.GetInternalDevice("engine"));
		BSAction attractAsteroid = ship.IntegratedDevice.Blueprint.CreateAction("Attract", magnet);
		BSAction storageAsteroid = ship.IntegratedDevice.Blueprint.CreateAction("Load", magnet);

		attractAsteroid.ConnectToQuery( currentTargetContainer );
		storageAsteroid.ConnectToQuery( currentTargetContainer );


		BSQuery tradeAsteroid = ship.IntegratedDevice.Blueprint.CreateQuery("TradeAsteroid", () =>
		{
			return ship.m_cargo.ComposeTradeOffer("Asteroid");
		});

		BSQuery motherBasePosition = ship.IntegratedDevice.Blueprint.CreateQuery("BasePosition", () =>
		{
			return new ArgsObject(){ obj = WorldManager.RequestContainerData("MotherBase").View.transform.position};
		});

		Device stationTrader = WorldManager.RequestContainerData("MotherBase").IntegratedDevice.GetInternalDevice("trader");


		BSSequence goingHomeSequence = ship.IntegratedDevice.Blueprint.CreateSequence("Home");
		BSAction setTargetStation = ship.IntegratedDevice.Blueprint.CreateAction( "SetTargetPosition", navigator.GetInternalDevice("patrol") );
		setTargetStation.ConnectToQuery(motherBasePosition);
		BSExit moveToStation = ship.IntegratedDevice.Blueprint.CreateExit("MoveTo", navigator );
		BSAction sellResouces = ship.IntegratedDevice.Blueprint.CreateAction( "LoadItemsFrom", stationTrader );
		sellResouces.ConnectToQuery( tradeAsteroid );

		ship.IntegratedDevice.Blueprint.m_entryPoint.AddChild(rootDecision);

		rootDecision.AddCondition( enemydetector.GetCheck("IsAnyTarget") );
		rootDecision.AddCondition( ship.IntegratedDevice.GetCheck("IsCargoFull") );


		rootDecision.AddChild(miningDecision);
		rootDecision.AddChild(goingHomeSequence);
		rootDecision.AddChild(patrolSequence);


		miningDecision.AddChild(collectingSequence);
		miningDecision.AddChild(shootingSequence);


		collectingSequence.AddChild( disableEngineToCollect );
		collectingSequence.AddChild( attractAsteroid );
		collectingSequence.AddChild( storageAsteroid );


		shootingSequence.AddChild( disableEngineToCollect );
		shootingSequence.AddChild( steerTowardsShootingTarget );
		shootingSequence.AddChild( shootTarget );


		patrolSequence.AddChild(nextPoint);
		patrolSequence.AddChild(moveToWaypoint);

	
		goingHomeSequence.AddChild(setTargetStation);
		goingHomeSequence.AddChild(moveToStation);
		goingHomeSequence.AddChild(sellResouces);





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

		BSEntry onMouseWorld = cockpitDevice.Blueprint.CreateEntry( "OnMouseWorldPosition", cockpitDevice.GetInternalDevice("input/mousePos"));
		BSAction toSteer = cockpitDevice.Blueprint.CreateAction( "SteerTowards", cockpitDevice.GetInternalDevice("steerer"));
		cockpitDevice.Blueprint.ConnectElements( onMouseWorld, toSteer );

		// Movement module

		BSEntry onForwardDown = cockpitDevice.Blueprint.CreateEntry( "OnInputHeld", cockpitDevice.GetInternalDevice("input/w"));
		BSAction toGoForward = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/forward"));
		cockpitDevice.Blueprint.ConnectElements( onForwardDown, toGoForward );
		
		BSEntry onBackwardDown = cockpitDevice.Blueprint.CreateEntry( "OnInputHeld", cockpitDevice.GetInternalDevice("input/s"));
		BSAction toGoBackward = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/backward"));
		cockpitDevice.Blueprint.ConnectElements( onBackwardDown, toGoBackward );
		
		BSEntry onLeftDown = cockpitDevice.Blueprint.CreateEntry( "OnInputHeld", cockpitDevice.GetInternalDevice("input/a"));
		BSAction toGoleft = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/left"));
		cockpitDevice.Blueprint.ConnectElements( onLeftDown, toGoleft );
		
		BSEntry onRightDown = cockpitDevice.Blueprint.CreateEntry( "OnInputHeld", cockpitDevice.GetInternalDevice("input/d"));
		BSAction toGoRight = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/right"));
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
		BSQuery targetPosition = heatSeeker.Blueprint.CreateQuery( "CurrentTargetPosition", ranger );
		BSAction toSteer = heatSeeker.Blueprint.CreateAction( "SteerTowards", steerer);
		toSteer.ConnectToQuery( targetPosition );

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

	private static Device GenerateNavigator()
	{
		Device navigator = new Device() {EntityName = "navigator"};


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

		navigator.IntegrateDevice( engine );
		navigator.IntegrateDevice( steerer );
		navigator.IntegrateDevice( patrol );


		BSEntry entryPoint = navigator.Blueprint.CreateEntry( "MoveTo", navigator );
		BSSequence waypoints = navigator.Blueprint.CreateSequence();

		BSQuery currentTarget = navigator.Blueprint.CreateQuery( "CurrentTarget", patrol );
		BSAction calculateWay = navigator.Blueprint.CreateAction( "GetWaypointsList", patrol);//, patrol.CurrentTarget );
		calculateWay.ConnectToQuery( currentTarget );
		BSForeach foreachPosition = navigator.Blueprint.CreateForeach( patrol.GetWaypoints );



		BSSequence moveToSequence = navigator.Blueprint.CreateSequence();



		BSAction disableEngine = navigator.Blueprint.CreateAction( "DeactivateDevice", engine );
		BSQuery navPosition = navigator.Blueprint.CreateQuery( "CurrentNavigationPosition", patrol );
		BSAction steerTowardsTarget = navigator.Blueprint.CreateAction( "SteerTowards", steerer );
		steerTowardsTarget.ConnectToQuery( navPosition );
		BSAction enableEngine = navigator.Blueprint.CreateAction( "ActivateDevice", engine );

		BSAction waitUntilReach = navigator.Blueprint.CreateAction( "ReachTarget", patrol);
		waitUntilReach.ConnectToQuery( navPosition );
		BSAction setNextWaypoint = navigator.Blueprint.CreateAction( "SetNextNavigationPoint", patrol );

		entryPoint.AddChild(foreachPosition);
//		entryPoint.AddChild(waypoints);
//		waypoints.AddChild(calculateWay);
//		waypoints.AddChild(foreachPosition);

		foreachPosition.AddChild(moveToSequence);


		moveToSequence.AddChild(disableEngine);
		moveToSequence.AddChild(steerTowardsTarget);
		moveToSequence.AddChild(enableEngine);
		moveToSequence.AddChild(waitUntilReach);
		moveToSequence.AddChild(setNextWaypoint);



		return navigator;
	}
}
