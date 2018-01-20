using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeviceScriptableObject))]
public class DeviceSctiptableEditor : Editor 
{
	private SerializedProperty m_deviceProperty;

	private void OnEnable()
	{
		m_deviceProperty = serializedObject.FindProperty ("m_device");
	}

	public override void OnInspectorGUI ()
	{

	}

	[MenuItem("Sandbox Assets/Device Object", false, 100)]
	public static void CreateDeviceObject()
	{
		var newObj = CreateInstance<DeviceScriptableObject> ();
		AssetDatabase.CreateAsset (newObj, "Assets/Data/Devices/New Device.asset");
		AssetDatabase.SaveAssets ();
	}
}
