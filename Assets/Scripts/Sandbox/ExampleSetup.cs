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

	private void Update()
	{
		if( Input.GetKeyUp(KeyCode.R) )
		{
			EntitySelection.Cleanup();
			Application.LoadLevel( Application.loadedLevel );
		}

		Vector3 input = Vector3.zero; 
		if( Input.GetKey(KeyCode.UpArrow) )
			input += Vector3.up;

		if( Input.GetKey(KeyCode.DownArrow) )
			input += Vector3.down;

		if( Input.GetKey(KeyCode.LeftArrow) )
			input += Vector3.left;

		if( Input.GetKey(KeyCode.RightArrow) )
			input = Vector3.right;

		Camera.main.transform.position += input * Time.deltaTime;
	}
	
	private static Container GenerateMissile()
	{
		Container missile = new Container(){ EntityName = "Missile" };
		
		DEngine engine = new DEngine(){ EntityName = "engine"};
		DTimer timer = new DTimer(){ EntityName = "timer"};
		DDetonator detonator = new DDetonator(){ EntityName = "detonator"};
		DRanger ranger = new DRanger(){ EntityName = "ranger" };
		DHeatDetector detector = new DHeatDetector(){ EntityName = "detector" };
		DSteerModule steerer = new DSteerModule() { EntityName = "steerer" };
		
		engine.isEngaged = true;
		timer.m_timerSetUp = 3f;
		timer.m_started = true;
		
		missile.IntegratedDevice.IntegrateDevice( engine );
		missile.IntegratedDevice.IntegrateDevice( timer );
		missile.IntegratedDevice.IntegrateDevice( detonator );
		missile.IntegratedDevice.IntegrateDevice( ranger );
		missile.IntegratedDevice.IntegrateDevice( detector );
		missile.IntegratedDevice.IntegrateDevice( steerer );
		
		// Generating warhead
		BSEntry onTimer = missile.Blueprint.CreateEntry( "OnTimerTrigger", timer );
		BSAction toDetonate = missile.Blueprint.CreateAction( "Detonate", detonator );
		missile.Blueprint.ConnectElements( onTimer, toDetonate );
		
		// Generating navigation
		BSEntry onObjectInRange = missile.Blueprint.CreateEntry( "OnRangerEntered", ranger );
		BSAction toMarkTarget = missile.Blueprint.CreateAction( "SetTarget", detector );
		missile.Blueprint.ConnectElements( onObjectInRange, toMarkTarget );

		BSEntry onObjectOutRange = missile.Blueprint.CreateEntry( "OnRangerEscaped", ranger );
		BSAction toNullTarget = missile.Blueprint.CreateAction( "ResetTarget", detector );
		missile.Blueprint.ConnectElements( onObjectOutRange, toNullTarget );
		
		BSEntry onTargetPos = missile.Blueprint.CreateEntry( "TargetPosition", detector);
		BSAction toSteer = missile.Blueprint.CreateAction( "SteerTowards", steerer);
		missile.Blueprint.ConnectElements( onTargetPos, toSteer );
		
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
		
		DEngine engine = new DEngine(){ EntityName = "engine"};
		DLauncher launcher = new DLauncher(){ EntityName = "launcher"};
		DSteerModule steerer = new DSteerModule(){ EntityName = "steerer"};
		Device input = GenerateInclusiveInputModule();
		
		launcher.SetProjectile("Missile");
		
		ship.IntegratedDevice.IntegrateDevice( engine );
		ship.IntegratedDevice.IntegrateDevice( launcher );
		ship.IntegratedDevice.IntegrateDevice( steerer );
		ship.IntegratedDevice.IntegrateDevice( input );
		
		BSEntry onMouseUp = ship.Blueprint.CreateEntry( "input/mouse0/OnInputReleased", ship.IntegratedDevice);
		BSAction toFire = ship.Blueprint.CreateAction( "launcher/Fire", ship.IntegratedDevice);
		ship.Blueprint.ConnectElements( onMouseUp, toFire );
		
		BSEntry onForwardDown = ship.Blueprint.CreateEntry( "input/w/OnInputHeld", ship.IntegratedDevice);
		BSAction toGoForward = ship.Blueprint.CreateAction( "engine/Move", ship.IntegratedDevice);
		ship.Blueprint.ConnectElements( onForwardDown, toGoForward );
		
		BSEntry onMouseWorld = ship.Blueprint.CreateEntry( "input/mousePos/OnMouseWorldPosition", ship.IntegratedDevice);
		BSAction toSteer = ship.Blueprint.CreateAction( "steerer/SteerTowards", ship.IntegratedDevice);
		ship.Blueprint.ConnectElements( onMouseWorld, toSteer );
		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity );
		shipView.gameObject.layer = 8;
		
		return ship;
	}
	
	private static Device GenerateInclusiveInputModule()
	{
		List<Device> inputs = new List<Device>() 
		{
			new DInputModule(){ EntityName = "mouse0", m_keyCode = KeyCode.Mouse0 },
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

	private static Container GeneratePatrolShip()
	{
		Container ship = new Container(){ EntityName = "patrolship"};
		
		DEngine engine = new DEngine(){ EntityName = "engine"};
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

		
		BSEntry onPatrolPosition = ship.Blueprint.CreateEntry( "TargetPosition", patrol);
		BSAction toSteer = ship.Blueprint.CreateAction( "SteerTowards", steerer);
		ship.Blueprint.ConnectElements( onPatrolPosition, toSteer );

		BSEntry onSteering = ship.Blueprint.CreateEntry( "OnSteering", steerer);
		BSAction toDisableEngine = ship.Blueprint.CreateAction( "DisengageEngine", engine);
		ship.Blueprint.ConnectElements( onSteering, toDisableEngine );

		BSEntry onSteerComplete = ship.Blueprint.CreateEntry( "OnSteerComplete", steerer);
		BSAction toEnableEngine = ship.Blueprint.CreateAction( "EngageEngine", engine);
		ship.Blueprint.ConnectElements( onSteerComplete, toEnableEngine );
		
		ContainerView shipView = WorldManager.SpawnContainer( ship, Vector3.zero, Quaternion.identity );
		
		return ship;
	}
}
