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

	private static List<Mesh> m_cachedMeshes = new List<Mesh>();


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
	

	public static void GenerateAsteroids()
	{
		for( int i = 0; i < 10; i++ )
		{
			Asteroid astr = new Asteroid();
			astr.InitializeView();

			astr.View.transform.position = UnityEngine.Random.insideUnitCircle * 15f;

			m_cachedMeshes.Add( astr.View.GetComponent<MeshFilter>().sharedMesh );
		}
	}

	private void Awake()
	{
		World = this;
	}

	private void Destroy()
	{
		for( int i = 0; i < m_cachedMeshes.Count; i++ )
			Destroy(m_cachedMeshes[i]);
		m_cachedMeshes.Clear();
	}
}
