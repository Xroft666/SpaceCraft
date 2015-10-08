
using System.Collections.Generic;
using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSSelect : BSNode 
	{
		public List<Device> m_conditionsDevice = new List<Device>();
		public List<string> m_conditionsName = new List<string>();

		public List<Device> m_dataDevice = new List<Device>();
		public List<string> m_dataName = new List<string>();
		
		public void AddCondition( Device conditionDevice, string conditionName, Device dataDevice = null, string dataName = null )
		{
			if( !m_conditionsName.Contains( conditionName ) )
			{
				m_conditionsDevice.Add( conditionDevice );
				m_conditionsName.Add( conditionName );
				
				m_dataDevice.Add( dataDevice );
				m_dataName.Add( dataName );
			}
		}
		
		public void RemoveCondition( string conditionName )
		{
			int idx = m_conditionsName.IndexOf( conditionName );

			m_conditionsDevice.RemoveAt(idx);
			m_conditionsName.RemoveAt(idx);
			m_dataDevice.RemoveAt(idx);
			m_dataName.RemoveAt(idx);
		}

		public override void Traverse()
		{
			for( int i = 0; i < m_children.Count - 1; i ++ )
			{
				System.EventArgs args = null;
				DeviceQuery query = null;
				DeviceCheck check = null;

				check = m_conditionsDevice[i].GetCheck( m_conditionsName[i] );

				if( check == null )
				{
					UnityEngine.Debug.Log("Predecate missing");
					return;
				}

				if( m_dataDevice[i] != null )
				{
					query = m_dataDevice[i].GetQuery( m_dataName[i] );
					args = query.Invoke();
				}

				if( check.Invoke( args ) )
				{
					m_children[i].Traverse();
					return;
				}
			}

			// Fallback [else] connection
			if( m_children.Count > 0 )
			m_children[m_children.Count - 1].Traverse();
		}
	}
}