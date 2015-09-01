using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSEntry : BSMultiChildNode
	{
		public IEnumerator Initialize( EventArgs data )
		{
	//		m_outputData = data;
			Traverse();

			yield break;
		}

		public override void Traverse()
		{
			foreach( BSNode node in m_children )
				node.Traverse();
		}
	}
}
