namespace BehaviourScheme
{
	public class BSEntry : BSNode 
	{
		public override void Activate()
		{
//			UnityEngine.Debug.Log("BSEntry invoked");
			m_connectNode.Activate();
		}
	}
}
