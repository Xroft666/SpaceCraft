
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultiConditional : BSNode 
	{		
		protected List<SpaceSandbox.DeviceCheck> m_conditions = new List<SpaceSandbox.DeviceCheck>();
		protected List<SpaceSandbox.DeviceQuery> m_conditionData = new List<SpaceSandbox.DeviceQuery>();

		public void AddCondition( SpaceSandbox.DeviceCheck condition, SpaceSandbox.DeviceQuery data = null )
		{
			if( !m_conditions.Contains( condition ) )
			{
				m_conditions.Add( condition );
				m_conditionData.Add( data );
			}
		}

		public void RemoveCondition( SpaceSandbox.DeviceCheck condition )
		{
			m_conditions.Remove( condition );
			m_conditionData.RemoveAt( m_conditions.IndexOf( condition ) );
		}
	}
}