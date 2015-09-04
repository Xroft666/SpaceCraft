using System;
using System.Collections;
using UnityEngine;

namespace SpaceSandbox
{
	public delegate IEnumerator DeviceEvent(EventArgs args);
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

//	public class TypedEventArg<T> : EventArgs
//	{
//		private T _Value;
//		
//		public TypedEventArg(T value)
//		{
//			_Value = value;
//		}
//		
//		public T Value
//		{
//			get { return _Value; }
//			set { _Value = value; }
//		}
//	}
}
