using UnityEngine.Events;
using SpaceSandbox;

using System;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		public Device m_device;
		public string m_entryName;

		public override void Traverse()
		{
			BSEntry m_entry = m_device.GetEntry( m_entryName );
			m_entry.Traverse();
		}
	}
}
