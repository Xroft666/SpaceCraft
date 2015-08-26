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

	//private static List<Mesh> m_cachedMeshes = new List<Mesh>();


	public static WorldManager World { get; private set; }

	public static ContainerView SpawnContainer( Container container, Vector3 position, Quaternion rotation )
	{
		container.InitializeView();

		container.View.transform.position = position;
		container.View.transform.rotation = rotation;
		
		return container.View;
	}

	public static void UnspawnContainer( Container container )
	{
		container.Destroy();
		container.View.gameObject.SetActive( false );
	}
	

	public static void GenerateAsteroid( Vector3 position, float volume, List<Vector2> shape = null )
	{
		Asteroid astr = new Asteroid();

		astr.Containment.Amount = volume;
		astr.vertices = shape;

		astr.InitializeView( );

		astr.View.transform.position = position;

	//	m_cachedMeshes.Add( astr.View.GetComponent<MeshFilter>().sharedMesh );
	}
	
	private void Awake()
	{
		World = this;
	}

	private void Destroy()
	{
	//	for( int i = 0; i < m_cachedMeshes.Count; i++ )
	//		Destroy(m_cachedMeshes[i]);
	//	m_cachedMeshes.Clear();
	}
}
