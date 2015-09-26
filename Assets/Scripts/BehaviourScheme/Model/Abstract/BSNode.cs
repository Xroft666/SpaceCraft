using System;
using SpaceSandbox;

using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSNode 
	{
		public BlueprintScheme m_scheme;
		public NodeView m_view;

		public string m_type;
		public string m_name;

		public List<BSNode> m_parents = new List<BSNode>();
		public List<BSNode> m_children = new List<BSNode>();

		protected List<SpaceSandbox.DeviceCheck> m_conditions = new List<SpaceSandbox.DeviceCheck>();
		protected List<SpaceSandbox.DeviceQuery> m_conditionData = new List<SpaceSandbox.DeviceQuery>();
		
		public void AddCondition( SpaceSandbox.DeviceCheck condition, SpaceSandbox.DeviceQuery data = null )
		{
			if( !m_conditions.Contains( condition ) )
			{
				m_conditions.Add( condition );
				m_conditionData.Add( data );
			}
		}
		
		public void RemoveCondition( SpaceSandbox.DeviceCheck condition )
		{
			m_conditions.Remove( condition );
			m_conditionData.RemoveAt( m_conditions.IndexOf( condition ) );
		}
		
		public void AddChild( BSNode node )
		{
			m_children.Add( node );
			node.SetParent( this );
		}
		
		public void RemoveChild( BSNode node )
		{
			m_children.Remove(node);
			node.RemoveParent( this );
		}

		public void SetParent( BSNode node )
		{
			if( !m_parents.Contains( node ) )
				m_parents.Add( node );
		}
		
		public void RemoveParent( BSNode node )
		{
			m_parents.Remove( node  );
		}

		public virtual void Traverse(){}
	}
}
