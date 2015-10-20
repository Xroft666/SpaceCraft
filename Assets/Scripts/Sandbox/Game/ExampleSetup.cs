using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class ExampleSetup : MonoBehaviour {

	private void Start()
	{
//		WorldManager.SpawnContainer (GenerateMotherBase(), new Vector3(5f, 0f, 5f), Quaternion.identity, 2 );


		Ship enemyShip = GeneratePatrolShip();
		Ship myShip = GenerateMyShip();

		// Generating missiles
		for( int i = 0; i < 50; i++ )
		{
			enemyShip.AddToCargo( GenerateMissile() );
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

		missile.IntegratedDevice.InstallDevice( engine );
		missile.IntegratedDevice.InstallDevice( timeBomb );
		missile.IntegratedDevice.InstallDevice( heatSeeker );
		missile.IntegratedDevice.InstallDevice( activeTimer );

		timeBomb.GetInternalDevice("warhead/ranger").m_isActive = false;
		heatSeeker.GetInternalDevice("ranger").m_isActive = false;
		Job.make( timeBomb.DeactivateDevice( null ), true);
    	Job.make( heatSeeker.DeactivateDevice( null), true );


		BSEntry onTimer = missile.IntegratedDevice.Blueprint.CreateTrigger( "OnTimerComplete", activeTimer );
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

	private static Ship GenerateMyShip()
	{
		Ship ship = new Ship(0.3f){ EntityName = "MyShip"};
		
		Device navigator = GenerateNavigator();
		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DRanger enemydetector = new DRanger(){ EntityName = "enemydetector", detectionRange = 5f };
		DMagnet magnet = new DMagnet(){EntityName = "Magnet"};
		DTradeComputer trader = new DTradeComputer(){EntityName = "TradeComputer"};
		
		ship.m_cargo.AddItem(navigator);
		ship.m_cargo.AddItem(launcher);
		ship.m_cargo.AddItem(enemydetector);
		ship.m_cargo.AddItem(magnet);
		ship.m_cargo.AddItem(trader);
		
		ContainerView shipView = WorldManager.SpawnContainer( ship, -Vector3.forward * 3f, Quaternion.identity, 2 );
		shipView.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.green;
		
		return ship;
	}

	private static Ship GeneratePatrolShip()
	{
		Ship ship = new Ship(0.3f){ EntityName = "PatrolShip"};
		
		Device navigator = GenerateNavigator();
		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DRanger enemydetector = new DRanger(){ EntityName = "enemydetector", detectionRange = 5f };
		DMagnet magnet = new DMagnet(){EntityName = "Magnet"};
		DTradeComputer trader = new DTradeComputer(){EntityName = "TradeComputer"};
		
		ship.IntegratedDevice.InstallDevice(navigator);
		ship.IntegratedDevice.InstallDevice(launcher);
		ship.IntegratedDevice.InstallDevice(enemydetector);
		ship.IntegratedDevice.InstallDevice(magnet);
		ship.IntegratedDevice.InstallDevice(trader);

		SetUpFightingBlueprint(ship.IntegratedDevice);
		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.forward * 3f, Quaternion.identity, 2 );
		shipView.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.red;

		return ship;
	}

	private static void SetUpFightingBlueprint( Device device )
	{
		BSEntry rootEntry = 
			device.GetEntry("RootEntry");

		BSSelect rootDecision = device.Blueprint.CreateBranch("Root");
		
		
		BSSequence patrolSequence = device.Blueprint.CreateSequence("Patrol"); 
		BSAction nextPoint = device.Blueprint.CreateAction( "SetNextPoint", device.GetInternalDevice("navigator/patrol") );
		BSExit moveToWaypoint = device.Blueprint.CreateExit("MoveTo", device.GetInternalDevice("navigator") );
		

		
		BSSequence shootingSequence = device.Blueprint.CreateSequence("Shooting");

		BSAction steerTowardsShootingTarget = 
			device.Blueprint.CreateAction( "SteerTowards", device.GetInternalDevice("navigator/steerer") );
		BSQuery currentTargetPosition = device.Blueprint.CreateQuery("CurrentTargetPosition", device.GetInternalDevice("enemydetector"));
		steerTowardsShootingTarget.ConnectToQuery( currentTargetPosition );
		BSAction shootTarget = device.Blueprint.CreateAction( "Fire", device.GetInternalDevice("launcher") );
		BSAction disableEngineToCollect = device.Blueprint.CreateAction("DeactivateDevice", device.GetInternalDevice("navigator/engine"));

//		device.Blueprint.m_entryPoint.AddChild(rootDecision);
		rootEntry.AddChild( rootDecision );
		
		rootDecision.AddCondition( device.GetInternalDevice("enemydetector"), "IsAnyTarget" );
				

		rootDecision.AddChild(patrolSequence);
		rootDecision.AddChild(shootingSequence);

		
		shootingSequence.AddChild( disableEngineToCollect );
		shootingSequence.AddChild( steerTowardsShootingTarget );
		shootingSequence.AddChild( shootTarget );
		
		
		patrolSequence.AddChild(nextPoint);
		patrolSequence.AddChild(moveToWaypoint);

	}
	
	private static void SetupMiningBlueprint( Device device )
	{
		BSSelect rootDecision = device.Blueprint.CreateBranch("Root");
				
		
		BSSequence patrolSequence = device.Blueprint.CreateSequence("Patrol"); 
		BSAction nextPoint = device.Blueprint.CreateAction( "SetNextPoint", device.GetInternalDevice("navigator/patrol") );
		BSExit moveToWaypoint = device.Blueprint.CreateExit("MoveTo", device.GetInternalDevice("navigator") );
		
		
		BSSelect miningDecision = device.Blueprint.CreateBranch("MiningDecision");
		miningDecision.AddCondition( device.GetInternalDevice("magnet"), "IsStorageble", device.GetInternalDevice("enemydetector"), "CurrentTargetContainer");
		
		
		
		
		
		BSSequence shootingSequence = device.Blueprint.CreateSequence("Shooting");
		BSAction steerTowardsShootingTarget = 
			device.Blueprint.CreateAction( "SteerTowards", device.GetInternalDevice("navigator/steerer") );
		BSQuery currentTargetPosition = device.Blueprint.CreateQuery("CurrentTargetPosition", device.GetInternalDevice("enemydetector"));
		steerTowardsShootingTarget.ConnectToQuery( currentTargetPosition );
		BSAction shootTarget = device.Blueprint.CreateAction( "Fire", device.GetInternalDevice("launcher") );
		
		
		BSSequence collectingSequence = device.Blueprint.CreateSequence("Collecting");
		BSQuery currentTargetContainer = device.Blueprint.CreateQuery("CurrentTargetContainer", device.GetInternalDevice("enemydetector"));
		
		
		BSAction disableEngineToCollect = device.Blueprint.CreateAction("DeactivateDevice", device.GetInternalDevice("navigator/engine"));
		BSAction attractAsteroid = device.Blueprint.CreateAction("Attract", device.GetInternalDevice("magnet"));
		BSAction storageAsteroid = device.Blueprint.CreateAction("Load", device.GetInternalDevice("magnet"));
		
		attractAsteroid.ConnectToQuery( currentTargetContainer );
		storageAsteroid.ConnectToQuery( currentTargetContainer );
		
		device.AddQuery("TradeAsteroid", () =>
		                               {
			return device.m_containerAttachedTo.m_cargo.ComposeTradeOffer("Asteroid");
		});
		
		device.AddQuery("BasePosition", () =>
		                               {
			return new ArgsObject(){ obj = WorldManager.RequestContainerData("MotherBase").View.transform.position};
		});
		
		BSQuery tradeAsteroid = device.Blueprint.CreateQuery( "TradeAsteroid", device );
		BSQuery motherBasePosition = device.Blueprint.CreateQuery("BasePosition", device );
		
		Device stationTrader = WorldManager.RequestContainerData("MotherBase").IntegratedDevice.GetInternalDevice("trader");
		
		
		BSSequence goingHomeSequence = device.Blueprint.CreateSequence("Home");
		BSAction setTargetStation = device.Blueprint.CreateAction( "SetTargetPosition", device.GetInternalDevice("navigator/patrol") );
		setTargetStation.ConnectToQuery(motherBasePosition);
		BSExit moveToStation = device.Blueprint.CreateExit("MoveTo", device.GetInternalDevice("navigator") );
		BSAction sellResouces = device.Blueprint.CreateAction( "LoadItemsFrom", stationTrader );
		sellResouces.ConnectToQuery( tradeAsteroid );
		
		device.Blueprint.m_entryPoint.AddChild(rootDecision);
		
		rootDecision.AddCondition( device.GetInternalDevice("enemydetector"), "IsAnyTarget" );
		
		rootDecision.AddCondition( device, "IsCargoFull" );
		
		
		
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
	}

	private static Device GeneratePilotCockpit()
	{
		Device input = GenerateInclusiveInputModule();
		Device engines = GenerateInclusiveEngineModule();
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer" };

		Device cockpitDevice = new Device(){ EntityName = "cockpit"};
		cockpitDevice.InstallDevice( input );
		cockpitDevice.InstallDevice( engines );
		cockpitDevice.InstallDevice( steerer );

		// Steering module

		BSEntry onMouseWorld = cockpitDevice.Blueprint.CreateTrigger( "OnMouseWorldPosition", cockpitDevice.GetInternalDevice("input/mousePos"));
		BSAction toSteer = cockpitDevice.Blueprint.CreateAction( "SteerTowards", cockpitDevice.GetInternalDevice("steerer"));
		cockpitDevice.Blueprint.ConnectElements( onMouseWorld, toSteer );

		// Movement module

		BSEntry onForwardDown = cockpitDevice.Blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/w"));
		BSAction toGoForward = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/forward"));
		cockpitDevice.Blueprint.ConnectElements( onForwardDown, toGoForward );
		
		BSEntry onBackwardDown = cockpitDevice.Blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/s"));
		BSAction toGoBackward = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/backward"));
		cockpitDevice.Blueprint.ConnectElements( onBackwardDown, toGoBackward );
		
		BSEntry onLeftDown = cockpitDevice.Blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/a"));
		BSAction toGoleft = cockpitDevice.Blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/left"));
		cockpitDevice.Blueprint.ConnectElements( onLeftDown, toGoleft );
		
		BSEntry onRightDown = cockpitDevice.Blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/d"));
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
		inclusiveDevice.InstallDevices( inputs );
		
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
		inclusiveDevice.InstallDevices( inputs );
		
		return inclusiveDevice;
	}

	// Can be a mine as it is
	private static Device GenerateWarhead( float detectionRange )
	{
		Device warheadDevice = new Device(){ EntityName = "warhead"};

		DDetonator detonator = new DDetonator(){ EntityName = "detonator"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };

		warheadDevice.InstallDevice( detonator );
		warheadDevice.InstallDevice( ranger );

		BSEntry onClose = warheadDevice.Blueprint.CreateTrigger( "OnRangerEntered", ranger );
		BSAction toDetonate = warheadDevice.Blueprint.CreateAction( "Detonate", detonator );
		warheadDevice.Blueprint.ConnectElements( onClose, toDetonate );
		
		return warheadDevice;
	}


	private static Device GenerateTimeBomb( float time )
	{
		Device timeBomb = new Device(){ EntityName = "timebomb" };

		Device warhead = GenerateWarhead( 1f );
		DTimer timer = new DTimer() { EntityName = "timer", m_timerSetUp = time };

		timeBomb.InstallDevice( warhead );
		timeBomb.InstallDevice( timer );

		// Generating warhead
		BSEntry onTimer = timeBomb.Blueprint.CreateTrigger( "OnTimerComplete", timer );
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
		

		heatSeeker.InstallDevice( ranger );
		heatSeeker.InstallDevice( steerer );


		BSSequence onTargetFound = heatSeeker.Blueprint.CreateSequence();

		// add target signature
		BSEntry inRange = heatSeeker.Blueprint.CreateTrigger( "OnRangerEntered", ranger );

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


		motherBase.IntegratedDevice.InstallDevice( ranger );
		motherBase.IntegratedDevice.InstallDevice( magnet );
		motherBase.IntegratedDevice.InstallDevice( trader );


		BSSelect rootDecision = motherBase.IntegratedDevice.Blueprint.CreateBranch();
		rootDecision.AddCondition( ranger, "IsAnyTarget");

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

		navigator.InstallDevice( engine );
		navigator.InstallDevice( steerer );
		navigator.InstallDevice( patrol );


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
