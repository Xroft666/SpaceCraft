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

	private static List<Container> objectsPool = new List<Container>();


	
	public static WorldManager World { get; private set; }

	public static ContainerView SpawnContainer( Container container, Vector3 position, Quaternion rotation, int owner = 0 )
	{
		container.InitializeView();

		container.View.transform.position = position;
		container.View.transform.rotation = rotation;

		container.View.m_owner = owner;


		objectsPool.Add( container );

		return container.View;
	}

	public static void UnspawnContainer( Container container )
	{
		objectsPool.Remove( container );

		container.Destroy();
//		container.View.gameObject.SetActive( false );
	}
	

	public static void GenerateAsteroid( Vector3 position, float rotation, float volume)
	{
		Asteroid astr = new Asteroid();

		astr.Containment.Amount = volume;

		astr.InitializeView( );

		astr.View.transform.position = position;
		astr.View.transform.rotation = Quaternion.Euler( 0f, 0f, rotation);
	}

	public static bool IsContainerDestroyed( ContainerView view )
	{
		return !view.gameObject.activeInHierarchy;
	}

	public static Ship RequestContainerData(string name)
	{
		foreach( Container cont in objectsPool )
			if( cont.EntityName.Equals( name ) )
				return cont as Ship;
		return null;
	}
	
	private void Awake()
	{
		World = this;
	}

}
