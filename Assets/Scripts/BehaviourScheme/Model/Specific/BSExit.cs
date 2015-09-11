using UnityEngine.Events;
using SpaceSandbox;

using System;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		public DeviceEvent m_entry;

		public override void Traverse()
		{
			// send back to scheme, device function signature
			//m_scheme.ScheduleTask( m_entry );
			m_entry.Invoke();
		}
	}
}
