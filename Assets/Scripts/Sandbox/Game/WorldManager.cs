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

	public static Dictionary<string, Ship> s_containersCache = new Dictionary<string, Ship>();
	public static WorldManager World { get; private set; }

	public static ContainerView SpawnContainer( Ship container, Vector3 position, Quaternion rotation, int owner = 0 )
	{
		if( s_containersCache.ContainsKey( container.EntityName ) )
			s_containersCache.Add( container.EntityName, container );

//		container = new Ship( container );

		container.InitializeView();

		container.View.transform.position = position;
		container.View.transform.rotation = rotation;

		container.View.m_owner = owner;


		return container.View;
	}

	public static void UnspawnContainer( Container container )
	{
		container.Destroy();
//		container.View.gameObject.SetActive( false );
	}
	

	public static void GenerateAsteroid( Vector3 position, float rotation, float volume)
	{
		Asteroid astr = new Asteroid(){ EntityName = "Asteroid"};

		astr.Containment.Amount = volume;

		astr.InitializeView( );

		astr.View.transform.position = position;
		astr.View.transform.rotation = Quaternion.Euler( 0f, rotation, 0f);
	}

	public static bool IsContainerDestroyed( ContainerView view )
	{
		return !view.gameObject.activeInHierarchy;
	}

	public static Ship RequestContainerData(string name)
	{
		GameObject obj = GameObject.Find(name);
		if( obj == null )
			return null;

		ContainerView view = obj.GetComponent<ContainerView>();
		if( view == null )
			return null;

		Ship ship = view.m_contain as Ship;
		return ship;
	}
	
	private void Awake()
	{
		World = this;
	}

}
