using UnityEngine;
using System.Collections;

using SpaceSandbox;

public class DeviceItem : MonoBehaviour 
{
	public Entity ItemContainment { get; private set; }

	public void InitializeItem( Entity resource )
	{
		ItemContainment = resource;
	}
}
