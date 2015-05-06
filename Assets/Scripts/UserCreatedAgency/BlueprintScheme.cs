
using System.Collections.Generic;
using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		private BSState m_defaultState;

		private List<BSEntry> m_entries;
		private List<BSState> m_states;
		private List<BSExit> m_exits;

		public void UpdateScheme()
		{

		}
	}
}