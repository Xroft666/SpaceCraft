using System;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSQuery : BSNode 
	{
		public Device m_device;
		public string m_queryName;

		public BSNode connectedNode;

		public override BSNode GetCopy ()
		{
			return new BSQuery ();
		}
	}
}
