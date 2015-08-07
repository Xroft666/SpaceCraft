using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;

using BehaviourScheme;


public class WorldManager : MonoBehaviour 
{
	public static WorldManager World { get; private set; }

	private void Awake()
	{
		World = this;
	}

	private void Start()
	{
		// Generate a ship that holds some amount of missiles
		// Ship can move, fire missiles, and a missile when shot
		// picks a target and destroys it

	}

	private void Update()
	{

	}

	private void OnDestroy()
	{

	}
}
