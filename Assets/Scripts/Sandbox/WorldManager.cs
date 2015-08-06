using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;

using BehaviourScheme;


public class WorldManager : MonoBehaviour 
{
	public static WorldManager World { get; private set; }

	private override void Awake()
	{
		World = this;
	}

	private override void Start()
	{
		// Generate a ship that holds some amount of missiles
		// Ship can move, fire missiles, and a missile when shot
		// picks a target and destroys it
	}

	private override void Update()
	{

	}

	private override void OnDestroy()
	{

	}
}
