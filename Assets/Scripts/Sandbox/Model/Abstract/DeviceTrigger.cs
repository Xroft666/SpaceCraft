using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceSandbox
{
	public delegate IEnumerator DeviceAction(DeviceQuery qry);
	public delegate void DeviceTrigger();
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
		public string commReceiver;
		public string commSender;
	}

	//public class CommArgs : EventArgs
	//{
	//	public string commReceiver;
	//}

	public class ArgsObject : EventArgs
	{
		public System.Object obj;
	}

	public class ArgsList : EventArgs
	{
		public System.Object[] objs;
	}
}
