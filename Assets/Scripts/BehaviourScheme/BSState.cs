using System.Collections;

namespace BehaviourScheme
{
	public class BSState : BSNode 
	{
		public enum StateType
		{
			Once, Repeate
		}

		private BSNode m_entry = null;
		private BSNode m_currentNode = null;

		private StateType m_type = StateType.Once;


		public void SetType( StateType type )
		{
			m_type = type;
		}

		public override void Activate()
		{
			m_entry.Activate();
		}

		public override IEnumerator ActivateAndWait()
		{
			yield return m_entry.ActivateAndWait();
		}
	}
}