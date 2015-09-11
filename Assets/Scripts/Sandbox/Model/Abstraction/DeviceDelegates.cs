using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceSandbox
{
	public delegate IEnumerator DeviceAction(EventArgs args);
	public delegate void DeviceEvent();
	public delegate bool DeviceCheck(EventArgs args);
	public delegate EventArgs DeviceQuery();

	public class ContainerArgs : EventArgs
	{
		public Container container;
	}

	public class StringArgs : EventArgs
	{
		public string name;
	}

	public class PositionArgs : EventArgs
	{
		public Vector3 position;
	}

	public class PositionsListArgs : EventArgs
	{
		public Vector3[] positions;
	}

	public class KeyCodeArgs : EventArgs
	{
		public KeyCode key;
	}

	public class TradingArgs : EventArgs
	{
		public string itemName;
		public int itemCount;
		public float resourceVolume;
		public float credits;	
		public Ship requestSender;
	}

	public class ArgsObject : EventArgs
	{
		public System.Object obj;
	}

	public class ArgsList : EventArgs
	{
		public List<System.Object> objs;
	}
}
