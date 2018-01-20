using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class ExampleSetup : MonoBehaviour 
{
	// ships templates
	public ShipScriptableObject
		m_motherBaseTemplate,
		m_patrolShipTemplate,
		m_playerShipTemplate,
		m_missileTemplate;

	// compoud devices templates
	public DeviceScriptableObject
		m_heatSeekerTemplate,
		m_inclusEngineModuleTemplate,
		m_inclusInputModuleTemplate,
		m_navigatorTemplate,
		m_timeBombTemplate,
		m_warheadTemplate;

	// simple devices templates
	public DeviceScriptableObject
		m_detonatorTemplate,
		m_engineTemplate,
		m_inputModuleTemplate,
		m_launcherTemplate,
		m_magnetTemplate,
		m_patrolModuleTemplate,
		m_rangerTemplate,
		m_steerModuleTemplate,
		m_timerTemplate,
		m_tradeComputerTemplate;

		

	private void Start()
	{
		WorldManager.SpawnContainer (
			new Ship(m_motherBaseTemplate.m_ship),
			new Vector3(5f, 0f, 5f), 
			Quaternion.identity, 
			2 );

		var enemyShip = new Ship(m_patrolShipTemplate.m_ship);
		var myShip = new Ship(m_playerShipTemplate.m_ship);

		// Generating missiles
		for( int i = 0; i < 50; i++ )
		{
			enemyShip.AddToCargo(new Ship(m_missileTemplate.m_ship));
			myShip.AddToCargo(new Ship(m_missileTemplate.m_ship));
		}

		myShip.AddToCargo( new Device(m_heatSeekerTemplate.m_device) );
		myShip.AddToCargo( new Device(m_inclusEngineModuleTemplate.m_device) );
		myShip.AddToCargo( new Device(m_inclusInputModuleTemplate.m_device) );
		myShip.AddToCargo( new Device(m_timeBombTemplate.m_device) );
		myShip.AddToCargo( new Device(m_timeBombTemplate.m_device) );
		myShip.AddToCargo( new Device(m_warheadTemplate.m_device) );

		myShip.AddToCargo( new Device(m_detonatorTemplate.m_device));
		myShip.AddToCargo( new Device(m_engineTemplate.m_device));
		myShip.AddToCargo( new Device(m_inputModuleTemplate.m_device));
		myShip.AddToCargo( new Device(m_launcherTemplate.m_device));
		myShip.AddToCargo( new Device(m_magnetTemplate.m_device));
		myShip.AddToCargo( new Device(m_patrolModuleTemplate.m_device));
		myShip.AddToCargo( new Device(m_rangerTemplate.m_device));
		myShip.AddToCargo( new Device(m_steerModuleTemplate.m_device));
		myShip.AddToCargo( new Device(m_timerTemplate.m_device));
		myShip.AddToCargo( new Device(m_tradeComputerTemplate.m_device));


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


















	public static Ship GenerateMissile()
	{
		Ship missile = new Ship(2){ EntityName = "Missile" };

		DEngine engine = new DEngine(){ EntityName = "engine", m_lookDirection = Vector3.forward, m_space = Space.Self };

		Device heatSeeker = GenerateHeatSeeker(3f);
		Device timeBomb = GenerateTimeBomb( 5f );
		DTimer activeTimer = new DTimer(){ EntityName = "activationtimer", m_timerSetUp = 2f };

		missile.m_device.InstallDevice( engine );
		missile.m_device.InstallDevice( timeBomb );
		missile.m_device.InstallDevice( heatSeeker );
		missile.m_device.InstallDevice( activeTimer );

		timeBomb.GetInternalDevice("warhead/ranger").m_isActive = false;
		heatSeeker.GetInternalDevice("ranger").m_isActive = false;

		timeBomb.DeactivateDevice();
		heatSeeker.DeactivateDevice();


		BSEntry onTimer = missile.m_device.m_blueprint.CreateTrigger( "OnTimerComplete", activeTimer );
		BSAction toActivateWarhead = missile.m_device.m_blueprint.CreateAction( "ActivateDevice", timeBomb );
		BSAction toActivateSeeker = missile.m_device.m_blueprint.CreateAction( "ActivateDevice", heatSeeker );

		missile.m_device.m_blueprint.ConnectElements( onTimer, toActivateWarhead );
		missile.m_device.m_blueprint.ConnectElements( onTimer, toActivateSeeker );

		return missile;
	}

	public static Ship GenerateMyShip()
	{
		Ship ship = new Ship(0.3f){ EntityName = "MyShip"};

		Device navigator = GenerateNavigatorDevice();
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

	public static Ship GeneratePatrolShip()
	{
		Ship ship = new Ship(0.3f){ EntityName = "PatrolShip"};

		Device navigator = GenerateNavigatorDevice();
		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DRanger enemydetector = new DRanger(){ EntityName = "enemydetector", detectionRange = 5f };
		DMagnet magnet = new DMagnet(){EntityName = "Magnet"};
		DTradeComputer trader = new DTradeComputer(){EntityName = "TradeComputer"};

		ship.m_device.InstallDevice(navigator);
		ship.m_device.InstallDevice(launcher);
		ship.m_device.InstallDevice(enemydetector);
		ship.m_device.InstallDevice(magnet);
		ship.m_device.InstallDevice(trader);

		//SetUpFightingBlueprint(ship.IntegratedDevice);
		SetupMiningBlueprint( ship.m_device );

		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.forward * 3f, Quaternion.identity, 2 );
		shipView.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.red;

		return ship;
	}

	public static void SetUpFightingBlueprint( Device device )
	{
		BSEntry rootEntry = 
			device.m_blueprint.GetEntry("RootEntry");

		BSSelect rootDecision = device.m_blueprint.CreateBranch("Root");


		BSSequence patrolSequence = device.m_blueprint.CreateSequence("Patrol"); 
		BSAction nextPoint = device.m_blueprint.CreateAction( "SetNextPoint", device.GetInternalDevice("navigator/patrol") );
		BSExit moveToWaypoint = device.m_blueprint.CreateExit("MoveTo", device.GetInternalDevice("navigator") );



		BSSequence shootingSequence = device.m_blueprint.CreateSequence("Shooting");

		BSAction steerTowardsShootingTarget = 
			device.m_blueprint.CreateAction( "SteerTowards", device.GetInternalDevice("navigator/steerer") );
		BSQuery currentTargetPosition = device.m_blueprint.CreateQuery("CurrentTargetPosition", device.GetInternalDevice("enemydetector"));
		steerTowardsShootingTarget.ConnectToQuery( currentTargetPosition );
		BSAction shootTarget = device.m_blueprint.CreateAction( "Fire", device.GetInternalDevice("launcher") );
		BSAction disableEngineToCollect = device.m_blueprint.CreateAction("DeactivateDevice", device.GetInternalDevice("navigator/engine"));

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

	public static void SetupMiningBlueprint( Device device )
	{
		BSEntry rootEntry = 
			device.m_blueprint.GetEntry("RootEntry");

		BSSelect rootDecision = device.m_blueprint.CreateBranch("Root");


		BSSequence patrolSequence = device.m_blueprint.CreateSequence("Patrol"); 
		BSAction nextPoint = device.m_blueprint.CreateAction( "SetNextPoint", device.GetInternalDevice("navigator/patrol") );
		BSExit moveToWaypoint = device.m_blueprint.CreateExit("MoveTo", device.GetInternalDevice("navigator") );


		BSSelect miningDecision = device.m_blueprint.CreateBranch("MiningDecision");
		miningDecision.AddCondition( device.GetInternalDevice("magnet"), "IsStorageble", device.GetInternalDevice("enemydetector"), "CurrentTargetContainer");





		BSSequence shootingSequence = device.m_blueprint.CreateSequence("Shooting");
		BSAction steerTowardsShootingTarget = 
			device.m_blueprint.CreateAction( "SteerTowards", device.GetInternalDevice("navigator/steerer") );
		BSQuery currentTargetPosition = device.m_blueprint.CreateQuery("CurrentTargetPosition", device.GetInternalDevice("enemydetector"));
		steerTowardsShootingTarget.ConnectToQuery( currentTargetPosition );
		BSAction shootTarget = device.m_blueprint.CreateAction( "Fire", device.GetInternalDevice("launcher") );


		BSSequence collectingSequence = device.m_blueprint.CreateSequence("Collecting");
		BSQuery currentTargetContainer = device.m_blueprint.CreateQuery("CurrentTargetContainer", device.GetInternalDevice("enemydetector"));


		BSAction disableEngineToCollect = device.m_blueprint.CreateAction("DeactivateDevice", device.GetInternalDevice("navigator/engine"));
		BSAction attractAsteroid = device.m_blueprint.CreateAction("Attract", device.GetInternalDevice("magnet"));
		BSAction storageAsteroid = device.m_blueprint.CreateAction("Load", device.GetInternalDevice("magnet"));

		attractAsteroid.ConnectToQuery( currentTargetContainer );
		storageAsteroid.ConnectToQuery( currentTargetContainer );

		device.m_blueprint.AddQuery("TradeAsteroid", () =>
			{
				return device.m_container.m_cargo.ComposeTradeOffer("Asteroid");
			});

		device.m_blueprint.AddQuery("BasePosition", () =>
			{
				return new ArgsObject(){ obj = WorldManager.RequestContainerData("MotherBase").View.transform.position};
			});

		BSQuery tradeAsteroid = device.m_blueprint.CreateQuery( "TradeAsteroid", device );
		BSQuery motherBasePosition = device.m_blueprint.CreateQuery("BasePosition", device );

		Device stationTrader = WorldManager.RequestContainerData("MotherBase").m_device.GetInternalDevice("trader");


		BSSequence goingHomeSequence = device.m_blueprint.CreateSequence("Home");
		BSAction setTargetStation = device.m_blueprint.CreateAction( "SetTargetPosition", device.GetInternalDevice("navigator/patrol") );
		setTargetStation.ConnectToQuery(motherBasePosition);
		BSExit moveToStation = device.m_blueprint.CreateExit("MoveTo", device.GetInternalDevice("navigator") );
		BSAction sellResouces = device.m_blueprint.CreateAction( "LoadItemsFrom", stationTrader );
		sellResouces.ConnectToQuery( tradeAsteroid );

		rootEntry.AddChild( rootDecision );

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

	public static Device GeneratePilotCockpit()
	{
		Device input = GenerateInclusiveInputModule();
		Device engines = GenerateInclusiveEngineModule();
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer" };

		Device cockpitDevice = new Device(){ EntityName = "cockpit"};
		cockpitDevice.InstallDevice( input );
		cockpitDevice.InstallDevice( engines );
		cockpitDevice.InstallDevice( steerer );

		// Steering module

		BSEntry onMouseWorld = cockpitDevice.m_blueprint.CreateTrigger( "OnMouseWorldPosition", cockpitDevice.GetInternalDevice("input/mousePos"));
		BSAction toSteer = cockpitDevice.m_blueprint.CreateAction( "SteerTowards", cockpitDevice.GetInternalDevice("steerer"));
		cockpitDevice.m_blueprint.ConnectElements( onMouseWorld, toSteer );

		// Movement module

		BSEntry onForwardDown = cockpitDevice.m_blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/w"));
		BSAction toGoForward = cockpitDevice.m_blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/forward"));
		cockpitDevice.m_blueprint.ConnectElements( onForwardDown, toGoForward );

		BSEntry onBackwardDown = cockpitDevice.m_blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/s"));
		BSAction toGoBackward = cockpitDevice.m_blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/backward"));
		cockpitDevice.m_blueprint.ConnectElements( onBackwardDown, toGoBackward );

		BSEntry onLeftDown = cockpitDevice.m_blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/a"));
		BSAction toGoleft = cockpitDevice.m_blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/left"));
		cockpitDevice.m_blueprint.ConnectElements( onLeftDown, toGoleft );

		BSEntry onRightDown = cockpitDevice.m_blueprint.CreateTrigger( "OnInputHeld", cockpitDevice.GetInternalDevice("input/d"));
		BSAction toGoRight = cockpitDevice.m_blueprint.CreateAction( "MoveForward", cockpitDevice.GetInternalDevice("engines/right"));
		cockpitDevice.m_blueprint.ConnectElements( onRightDown, toGoRight );

		return cockpitDevice;
	}

	public static Device GenerateInclusiveInputModule()
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

	public static Device GenerateInclusiveEngineModule()
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
	public static Device GenerateWarhead( float detectionRange )
	{
		Device warheadDevice = new Device(){ EntityName = "warhead"};

		DDetonator detonator = new DDetonator(){ EntityName = "detonator"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };

		warheadDevice.InstallDevice( detonator );
		warheadDevice.InstallDevice( ranger );

		BSEntry onClose = warheadDevice.m_blueprint.CreateTrigger( "OnRangerEntered", ranger );
		BSAction toDetonate = warheadDevice.m_blueprint.CreateAction( "Detonate", detonator );
		warheadDevice.m_blueprint.ConnectElements( onClose, toDetonate );

		return warheadDevice;
	}


	public static Device GenerateTimeBomb( float time )
	{
		Device timeBomb = new Device(){ EntityName = "timebomb" };

		Device warhead = GenerateWarhead( 1f );
		DTimer timer = new DTimer() { EntityName = "timer", m_timerSetUp = time };

		timeBomb.InstallDevice( warhead );
		timeBomb.InstallDevice( timer );

		// Generating warhead
		BSEntry onTimer = timeBomb.m_blueprint.CreateTrigger( "OnTimerComplete", timer );
		BSAction toDetonate = timeBomb.m_blueprint.CreateAction( "Detonate", timeBomb.GetInternalDevice("warhead/detonator") );
		timeBomb.m_blueprint.ConnectElements( onTimer, toDetonate );

		return timeBomb;
	}

	// Heat seeker
	public static Device GenerateHeatSeeker( float detectionRange )
	{
		Device heatSeeker = new Device(){ EntityName = "heatseeker"};

		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };
		DSteerModule steerer = new DSteerModule() { EntityName = "steerer" };


		heatSeeker.InstallDevice( ranger );
		heatSeeker.InstallDevice( steerer );


		BSSequence onTargetFound = heatSeeker.m_blueprint.CreateSequence();

		// add target signature
		BSEntry inRange = heatSeeker.m_blueprint.CreateTrigger( "OnRangerEntered", ranger );

		// steer towards the target
		BSQuery targetPosition = heatSeeker.m_blueprint.CreateQuery( "CurrentTargetPosition", ranger );
		BSAction toSteer = heatSeeker.m_blueprint.CreateAction( "SteerTowards", steerer);
		toSteer.ConnectToQuery( targetPosition );

		inRange.AddChild( onTargetFound );
		onTargetFound.AddChild( toSteer );


		return heatSeeker;
	}

	public static Ship GenerateMotherBase()
	{
		Ship motherBase = new Ship(1000f){ EntityName = "MotherBase" };

		DMagnet magnet = new DMagnet(){ EntityName = "magnet" };
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = 5f };
		DTradeComputer trader = new DTradeComputer(){ EntityName = "trader"} ;


		motherBase.m_device.InstallDevice( ranger );
		motherBase.m_device.InstallDevice( magnet );
		motherBase.m_device.InstallDevice( trader );


		BSSelect rootDecision = motherBase.m_device.m_blueprint.CreateBranch();
		rootDecision.AddCondition( ranger, "IsAnyTarget");

		return motherBase;
	}

	public static Device GenerateNavigatorDevice()
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


		BSEntry entryPoint = navigator.m_blueprint.CreateEntry( "MoveTo", navigator );
		BSSequence waypoints = navigator.m_blueprint.CreateSequence();

		BSQuery currentTarget = navigator.m_blueprint.CreateQuery( "CurrentTarget", patrol );
		BSAction calculateWay = navigator.m_blueprint.CreateAction( "GetWaypointsList", patrol);
		calculateWay.ConnectToQuery( currentTarget );
		BSForeach foreachPosition = navigator.m_blueprint.CreateForeach( patrol.GetWaypoints );

		BSSequence moveToSequence = navigator.m_blueprint.CreateSequence();

		BSAction disableEngine = navigator.m_blueprint.CreateAction( "DeactivateDevice", engine );
		BSQuery navPosition = navigator.m_blueprint.CreateQuery( "CurrentNavigationPosition", patrol );
		BSAction steerTowardsTarget = navigator.m_blueprint.CreateAction( "SteerTowards", steerer );
		steerTowardsTarget.ConnectToQuery( navPosition );
		BSAction enableEngine = navigator.m_blueprint.CreateAction( "ActivateDevice", engine );

		BSAction waitUntilReach = navigator.m_blueprint.CreateAction( "ReachTarget", patrol);
		waitUntilReach.ConnectToQuery( navPosition );
		BSAction setNextWaypoint = navigator.m_blueprint.CreateAction( "SetNextNavigationPoint", patrol );

		entryPoint.AddChild(foreachPosition);

		foreachPosition.AddChild(moveToSequence);


		moveToSequence.AddChild(disableEngine);
		moveToSequence.AddChild(steerTowardsTarget);
		moveToSequence.AddChild(enableEngine);
		moveToSequence.AddChild(waitUntilReach);
		moveToSequence.AddChild(setNextWaypoint);

		return navigator;
	}
}
