
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSSelect : BSMultuParentNode 
	{		
		public override void Activate(params object[] objects)
		{
			for( int i = 0; i < m_parents.Count; i ++ )
			{
				if( m_conditions[i].Invoke() )
				{
					// Retrive data from the respective parent
					// Remove the parent from the scheduled events list (somhow), or flag it out here
					// Pass its data here to the child

					m_connectNode.Activate( objects );
					break;
				}
			}
		}
	}
}