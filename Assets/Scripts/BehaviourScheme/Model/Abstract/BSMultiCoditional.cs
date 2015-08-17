
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultiConditional : BSNode 
	{		
		protected List<SpaceSandbox.DeviceCheck> m_conditions = new List<SpaceSandbox.DeviceCheck>();

		public void AddCondition( SpaceSandbox.DeviceCheck condition )
		{
			if( !m_conditions.Contains( condition ) )
				m_conditions.Add( condition );
		}

		public void RemoveCondition( SpaceSandbox.DeviceCheck condition )
		{
			m_conditions.Remove( condition );
		}
	}
}