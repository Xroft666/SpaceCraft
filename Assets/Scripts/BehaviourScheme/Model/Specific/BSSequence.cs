
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSSequence : BSNode 
	{
		public override BSNode GetCopy ()
		{
			return new BSSequence ();
		}

		public override void Traverse()
		{
			for( int i = 0; i < m_children.Count; i ++ )
			{
				m_children[i].Traverse();
			}
		}
	}
}