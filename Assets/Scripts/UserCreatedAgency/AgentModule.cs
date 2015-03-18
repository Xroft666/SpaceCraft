using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;

namespace UserCreatedAgency
{
	/// <summary>
	/// Physical devices to install on a ship.
	/// Controls and exectures AI behaviour flow 
	/// </summary>
	public class AgentModule : Device 
	{
		List<BehaviourScheme> _schemes;
	}

}