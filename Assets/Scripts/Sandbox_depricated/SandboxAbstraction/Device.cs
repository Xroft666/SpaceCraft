
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Device : Entity 
	{
		public List<Resource> resources = new List<Resource>();

		public delegate void DeviceCallback( object[] output );
		public DeviceCallback outputCallback;

		public virtual void OnStart(){}
		public virtual void OnUpdate(){}
		
		public virtual void OnActivate(params object[] input){}
	}
}