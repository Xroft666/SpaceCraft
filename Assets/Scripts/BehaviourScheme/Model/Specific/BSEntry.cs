using System;

namespace BehaviourScheme
{
	public class BSEntry : BSMultiChildNode
	{
		public void Initialize( EventArgs data )
		{
			m_outputData = data;
			Traverse();
		}

		public override void Traverse()
		{
			foreach( BSNode node in m_children )
				node.Traverse();
		}
	}
}
