using System;
using System.Collections;
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSQuery : BSNode 
	{
		public SpaceSandbox.DeviceQuery m_query;
		public BSAction connectedActionNode;

		public override void Traverse()
		{

		}
	}
}
