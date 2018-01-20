using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpaceSandbox;
using BehaviourScheme;

using UnityEditor;

public static class TemplatesGenerator
{
	[MenuItem("Sandbox Assets/Device Templates/Missile")]
	public static void GenerateMissile()
	{
		var newObj = ScriptableObject.CreateInstance<ShipScriptableObject>();
		newObj.m_ship = ExampleSetup.GenerateMissile();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Missile.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Container Templates/Player Ship")]
	public static void GenerateMyShip()
	{
		var newObj = ScriptableObject.CreateInstance<ShipScriptableObject>();
		newObj.m_ship = ExampleSetup.GenerateMyShip();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Player Ship.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Container Templates/Patrol Ship")]
	public static void GeneratePatrolShip()
	{
		var newObj = ScriptableObject.CreateInstance<ShipScriptableObject>();
		newObj.m_ship = ExampleSetup.GeneratePatrolShip();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Patrol Ship.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Container Templates/Mother Base")]
	public static void GenerateMotherBase()
	{
		var newObj = ScriptableObject.CreateInstance<ShipScriptableObject>();
		newObj.m_ship = ExampleSetup.GenerateMotherBase();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Mother Base.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Cockpit")]
	public static void GeneratePilotCockpit()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GeneratePilotCockpit();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Cockpit.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Input Module")]
	public static void GenerateInclusiveInputModule()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GenerateInclusiveInputModule();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Input Module.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Engine Module")]
	public static void GenerateInclusiveEngineModule()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GenerateInclusiveEngineModule();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Engine Module.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Warhead")]
	public static void GenerateWarhead()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GenerateWarhead(1f);

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Warhead.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Timebomb")]
	public static void GenerateTimeBomb()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GenerateTimeBomb(5f);

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Timebomb.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Heat Seeker")]
	public static void GenerateHeatSeeker()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GenerateHeatSeeker(1f);

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Heat Seeker.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Device Templates/Navigator")]
	public static void GenerateNavigatorDevice()
	{
		var newObj = ScriptableObject.CreateInstance<DeviceScriptableObject>();
		newObj.m_device = ExampleSetup.GenerateNavigatorDevice();

		AssetDatabase.CreateAsset(newObj, "Assets/Data/Devices/New Navigator.asset");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Sandbox Assets/Blueprint Templates/Fighter")]
	public static void SetUpFightingBlueprint()
	{

	}

	[MenuItem("Sandbox Assets/Blueprint Templates/Miner")]
	public static void SetupMiningBlueprint()
	{

	}
}
