using System;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSQuery : BSNode 
	{
		//public SpaceSandbox.DeviceQuery m_query;
		public Device m_device;
		public string m_queryName;

		public BSAction connectedActionNode;

	}
}
