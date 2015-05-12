using UnityEngine.Events;
using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		private UnityEvent exitEvent = null;
		public UnityEvent ExitEvent
		{
			get{ return exitEvent; }
			set{ exitEvent = value; }
		}

		public override void Activate()
		{
//			UnityEngine.Debug.Log("BSExit invoked");
			exitEvent.Invoke();
		}
	}
}
