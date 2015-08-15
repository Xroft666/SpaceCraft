
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultiConditional : BSNode 
	{		
		protected List<BSPredecate> m_conditions = null;

		public void AddCondition( BSPredecate condition )
		{
			if( !m_conditions.Contains( condition ) )
				m_conditions.Add( condition );
		}

		public void RemoveCondition( BSPredecate condition )
		{
			m_conditions.Remove( condition );
		}
	}
}