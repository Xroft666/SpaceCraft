using UnityEngine.Events;
using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		private UnityEvent exitEvent = new UnityEvent();
		public UnityEvent ExitEvent
		{
			get{ return exitEvent; }
		}

		public override void Activate()
		{
			exitEvent.Invoke();
		}
	}
}
