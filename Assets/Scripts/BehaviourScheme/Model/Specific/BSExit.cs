using UnityEngine.Events;
using SpaceSandbox;

using System;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		public DeviceTrigger m_entry;

		public override void Traverse()
		{
			m_entry();
		}
	}
}
