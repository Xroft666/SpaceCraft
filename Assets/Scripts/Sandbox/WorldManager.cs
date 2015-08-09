using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;

using BehaviourScheme;

// Global objects pool manager
// keeps track on objects, spawns them, removes them, etc

public class WorldManager : MonoBehaviour 
{
	// Temporary variable. To be removed with something more sophisticated
	public Sprite m_visuals;
	// Temporary variable. I just dont like it
	public EventTriggerInitializer m_backgroundClicker;

	public static WorldManager World { get; private set; }

	public static ContainerView SpawnContainer( Container container, Vector3 position, Quaternion rotation )
	{
		GameObject newContainer = new GameObject( container.EntityName );
		newContainer.transform.position = position;
		newContainer.transform.rotation = rotation;

		ContainerView view = newContainer.AddComponent<ContainerView>();
		container.View = view;
		view.m_contain = container;

		InitializeVisuals( view );

		BoxCollider2D clickZone = newContainer.AddComponent<BoxCollider2D>();
		clickZone.isTrigger = true;

		EventTriggerInitializer inputInitializer = newContainer.AddComponent<EventTriggerInitializer>();
		inputInitializer.InitializeCallback( view );

		return view;
	}

	private static void InitializeVisuals(ContainerView view)
	{
//		GameObject body = GameObject.CreatePrimitive( PrimitiveType.Cube );
		GameObject body = new GameObject("body");

		SpriteRenderer sRenderer = body.AddComponent<SpriteRenderer>();
		sRenderer.sprite = World.m_visuals;

		body.transform.SetParent( view.transform, false );

		// and then go for each device and create their visuals
		// note: remove collider if exists
	}

	private void Awake()
	{
		World = this;
	}

	private void Start()
	{
		// Generate a ship that holds some amount of missiles
		// Ship can move, fire missiles, and a missile when shot
		// picks a target and destroys it

		// Include the whole ship-controlling input setting
		// under a new generated device

		// Initialize background trigger
		m_backgroundClicker.InitializeCallback( null );

		// Generating random targets
		for( int i = 0; i < 10; i++ )
		{
			GenerateTarget();
		}

		// Generating the ship
		Container ship = GenerateShip();

		// Generating missiles
		for( int i = 0; i < 2; i++ )
		{
			ship.AddToCargo( GenerateMissile() );
		}
	}

	public static void UnspawnContainer( Container container )
	{
		container.Destroy();
		GameObject.Destroy( container.View.gameObject );
	}



	private static Container GenerateMissile()
	{
		Container missile = new Container(){ EntityName = "Missile" };
		
		DEngine engine = new DEngine(){ EntityName = "engine"};
		DTimer timer = new DTimer(){ EntityName = "timer"};
		DDetonator detonator = new DDetonator(){ EntityName = "detonator"};
		
		engine.isEngaged = true;
		timer.m_timerSetUp = 5f;
		timer.m_started = true;
		
		missile.IntegratedDevice.IntegrateDevice( engine );
		missile.IntegratedDevice.IntegrateDevice( timer );
		missile.IntegratedDevice.IntegrateDevice( detonator );
		
		BSEntry onTimer = missile.Blueprint.CreateEntry( "OnTimerTrigger", timer );
		BSAction toDetonate = missile.Blueprint.CreateAction( "Detonate", detonator );
		missile.Blueprint.ConnectElements( onTimer, toDetonate );

		return missile;
	}

	private static void GenerateTarget()
	{
		SpawnContainer(new Container(){ EntityName = "Target"}, 
		              (Random.insideUnitCircle + Vector2.one) * (Camera.main.orthographicSize - 1f), 
		               Quaternion.identity );
	}

	private static Container GenerateShip()
	{
		Container ship = new Container(){ EntityName = "ship"};
		
		DEngine engine = new DEngine(){ EntityName = "engine"};
		DLauncher launcher = new DLauncher(){ EntityName = "launcher"};
		DSteeringModule steerer = new DSteeringModule(){ EntityName = "steerer"};
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
		
		SpawnContainer( ship, Vector3.zero, Quaternion.identity );

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
}
