using UnityEngine.Events;
using SpaceSandbox;

using System;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		public Device m_device;
		public string m_entryName;

		public override BSNode GetCopy ()
		{
			return new BSExit ();
		}

		public override void Traverse()
		{
			BSEntry m_entry = m_device.m_blueprint.GetEntry( m_entryName );
			m_entry.Traverse();
		}
	}
}
