using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSEntry : BSNode
	{
		public override BSNode GetCopy ()
		{
			return new BSEntry ();
		}

		public override void Traverse()
		{
			foreach( BSNode node in m_children )
				node.Traverse();
		}
	}
}
