
using System.Collections.Generic;

namespace SpaceSandbox
{
	[System.Serializable]
	public class Device : Entity 
	{
		public List<Resource> resources = new List<Resource>();
		public string deviceName;

		public delegate void DeviceCallback( object[] output );
		public DeviceCallback outputCallback;

		public virtual void OnStart(params object[] input){}
		public virtual void OnUpdate(){}
		public virtual void OnDelete(){}
		
		public virtual void OnActivate(params object[] input){}
		public virtual void OnDeactivate(params object[] input){}
	}
}