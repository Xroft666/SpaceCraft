using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;

public class UCABuilderUIController : MonoBehaviour 
{

	public Transform m_hardwareList;
	public Transform m_hardwareBP;
	public Transform m_softwareBP;
	public Transform m_softwareAPI;

	public void LoadDevicesList( Container container )
	{
		foreach( Entity ent in container.GetCargoList() )
		{
			Device device = ent as Device;
			if( device == null )
				continue;

			// create a device UI representation
		}
	}

	public Device GenerateDevice( List<Device> devices, BlueprintScheme blueprint )
	{
		return new Device();
	}

	public Entity GenerateItem( Container container, BlueprintScheme blueprint )
	{
		return new Container();
	}
}
