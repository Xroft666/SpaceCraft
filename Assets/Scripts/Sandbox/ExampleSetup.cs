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
			GenerateTarget();
		}
		
		// Generating the ship
		Container ship = GenerateShip();
		
		// Generating missiles
		for( int i = 0; i < 20; i++ )
		{
			ship.AddToCargo( GenerateMissile() );
		}

		GeneratePatrolShip();
	}

	private static Container GenerateMissile()
	{
		Container missile = new Container(){ EntityName = "Missile" };
		
		DEngine engine = new DEngine(){ EntityName = "engine", isEngaged = true, m_lookDirection = Vector3.up, m_space = Space.Self };

		Device heatSeeker = GenerateHeatSeeker(3f);
		Device timeBomb = GenerateTimeBomb( 3f );
		
		missile.IntegratedDevice.IntegrateDevice( engine );
		missile.IntegratedDevice.IntegrateDevice( timeBomb );
		missile.IntegratedDevice.IntegrateDevice( heatSeeker );

		return missile;
	}
	
	private static void GenerateTarget()
	{
		WorldManager.SpawnContainer(new Container(){ EntityName = "Target"}, 
		(Random.insideUnitCircle + Vector2.one) * (Camera.main.orthographicSize - 1f), 
		Quaternion.identity );
	}
	
	private static Container GenerateShip()
	{
		Container ship = new Container(){ EntityName = "ship"};

		Device cockpit = GeneratePilotCockpit();
		DLauncher launcher = new DLauncher(){ EntityName = "launcher", m_projectileName = "Missile" };
		DInputModule mouseInput = new DInputModule() { EntityName = "space", m_keyCode = KeyCode.Space };


		ship.IntegratedDevice.IntegrateDevice( launcher );
		ship.IntegratedDevice.IntegrateDevice( cockpit );
		ship.IntegratedDevice.IntegrateDevice( mouseInput );

		
		BSEntry onMouseUp = ship.IntegratedDevice.Blueprint.CreateEntry( "space/OnInputReleased", ship.IntegratedDevice);
		BSAction toFire = ship.IntegratedDevice.Blueprint.CreateAction( "launcher/Fire", ship.IntegratedDevice);
		ship.IntegratedDevice.Blueprint.ConnectElements( onMouseUp, toFire );

		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity );
		shipView.gameObject.layer = 8;
		
		return ship;
	}

	private static Container GeneratePatrolShip()
	{
		Container ship = new Container(){ EntityName = "patrolship"};
		
		DEngine engine = new DEngine(){ EntityName = "engine", m_lookDirection = Vector3.up, m_space = Space.Self };
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer"};
		DPatrolModule patrol = new DPatrolModule(){ EntityName = "patrol"};
		DTimer timer = new DTimer(){ EntityName = "timer" };

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

		patrol.SetPatrolPoints(markers[0].transform.position,
		                       markers[1].transform.position,
		                       markers[2].transform.position,
		                       markers[3].transform.position);

		ship.IntegratedDevice.IntegrateDevice( engine );
		ship.IntegratedDevice.IntegrateDevice( steerer );
		ship.IntegratedDevice.IntegrateDevice( patrol );
		ship.IntegratedDevice.IntegrateDevice( timer );

		
		BSEntry onPatrolPosition = ship.IntegratedDevice.Blueprint.CreateEntry( "TargetPosition", patrol);
		BSAction toSteer = ship.IntegratedDevice.Blueprint.CreateAction( "SteerTowards", steerer);
		ship.IntegratedDevice.Blueprint.ConnectElements( onPatrolPosition, toSteer );

		BSEntry onSteering = ship.IntegratedDevice.Blueprint.CreateEntry( "OnSteering", steerer);
		BSAction toDisableEngine = ship.IntegratedDevice.Blueprint.CreateAction( "DisengageEngine", engine);
		ship.IntegratedDevice.Blueprint.ConnectElements( onSteering, toDisableEngine );

		BSEntry onSteerComplete = ship.IntegratedDevice.Blueprint.CreateEntry( "OnSteerComplete", steerer);
		BSAction toEnableEngine = ship.IntegratedDevice.Blueprint.CreateAction( "EngageEngine", engine);
		ship.IntegratedDevice.Blueprint.ConnectElements( onSteerComplete, toEnableEngine );
		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity );
		
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
		DTimer timer = new DTimer() { EntityName = "timer", m_timerSetUp = time, m_started = true };

		timeBomb.IntegrateDevice( warhead );
		timeBomb.IntegrateDevice( timer );

		// Generating warhead
		BSEntry onTimer = timeBomb.Blueprint.CreateEntry( "OnTimerTrigger", timer );
		BSAction toDetonate = timeBomb.Blueprint.CreateAction( "warhead/detonator/Detonate", warhead );
		timeBomb.Blueprint.ConnectElements( onTimer, toDetonate );

		return timeBomb;
	}

	// Heat seeker
	private static Device GenerateHeatSeeker( float detectionRange )
	{
		Device heatSeeker = new Device(){ EntityName = "heatseeker"};
		
		DRadar radar = new DRadar(){ EntityName = "radar"};
		DRanger ranger = new DRanger(){ EntityName = "ranger", detectionRange = detectionRange };
		DSteerModule steerer = new DSteerModule() { EntityName = "steerer" };
		
		heatSeeker.IntegrateDevice( radar );
		heatSeeker.IntegrateDevice( ranger );
		heatSeeker.IntegrateDevice( steerer );
		
		BSEntry inRange = heatSeeker.Blueprint.CreateEntry( "ranger/OnRangerEntered", heatSeeker );
		BSAction toFollow = heatSeeker.Blueprint.CreateAction( "radar/SetTarget", heatSeeker );
		heatSeeker.Blueprint.ConnectElements( inRange, toFollow );

		BSEntry outRange = heatSeeker.Blueprint.CreateEntry( "ranger/OnRangerEscaped", heatSeeker );
		BSAction toNullTarget = heatSeeker.Blueprint.CreateAction( "radar/ResetTarget", heatSeeker );
		heatSeeker.Blueprint.ConnectElements( outRange, toNullTarget );

		BSEntry onTargetPos = heatSeeker.Blueprint.CreateEntry( "radar/TargetPosition", heatSeeker);
		BSAction toSteer = heatSeeker.Blueprint.CreateAction( "steerer/SteerTowards", heatSeeker);
		heatSeeker.Blueprint.ConnectElements( onTargetPos, toSteer );
		
		return heatSeeker;
	}
}
