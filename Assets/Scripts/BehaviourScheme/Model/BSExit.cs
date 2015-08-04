using UnityEngine.Events;
using SpaceSandbox;

using System;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		private SpaceSandbox.DeviceEvent exitEvent;
		public SpaceSandbox.DeviceEvent ExitEvent
		{
			get{ return exitEvent; }
			set{ exitEvent = value; }
		}

		public override void Activate(params SpaceSandbox.Entity[] objects)
		{
			exitEvent.Invoke(objects);
		}
	}
}
