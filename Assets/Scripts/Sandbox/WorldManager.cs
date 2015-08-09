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

		Rigidbody2D body = newContainer.gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0f;
		body.drag = 0.35f;

		BoxCollider2D clickZone = newContainer.AddComponent<BoxCollider2D>();
	//	clickZone.isTrigger = true;

		EventTriggerInitializer inputInitializer = newContainer.AddComponent<EventTriggerInitializer>();
		inputInitializer.InitializeCallback( view );

		return view;
	}

	public static void UnspawnContainer( Container container )
	{
		container.Destroy();
		//		GameObject.Destroy( container.View.gameObject );
		container.View.gameObject.SetActive( false );
	}

	private static void InitializeVisuals(ContainerView view)
	{
//		GameObject body = GameObject.CreatePrimitive( PrimitiveType.Cube );
		GameObject body = new GameObject("body");

		SpriteRenderer sRenderer = body.AddComponent<SpriteRenderer>();
		sRenderer.sprite = World.m_visuals;

		body.transform.SetParent( view.transform, false );
	}

	private void Awake()
	{
		World = this;

		// Initialize background trigger
		m_backgroundClicker.InitializeCallback( null );
	}
}
