
using System;
using System.Collections.Generic;
using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		private List<BSEntry> m_entries = new List<BSEntry>();
		private List<BSState> m_states = new List<BSState>();
		private List<BSExit> m_exits = new List<BSExit>();


		private List<BSNode> m_nodes = new List<BSNode>();

		public void CreateState()
		{
			BSState node = new BSState();

			m_states.Add( node );
			m_nodes.Add( node );
		}

		public void CreateAction( Action action )
		{
			BSAction node = new BSAction();
			node.SetAction( action );

			m_nodes.Add( node );
		}
		
		public void CreateEntry(  )
		{
			BSEntry node = new BSEntry();

			m_entries.Add( node );
			m_nodes.Add( node );
		}
		
		public void CreateExit()
		{
			BSExit node = new BSExit();
			node.SetScheme( this );

			m_exits.Add( node );
			m_nodes.Add( node );
		}

		public void CreateSelect()
		{
			BSSelect node = new BSSelect();
			m_nodes.Add( node );
		}

		public void CreateEvaluate()
		{
			BSEvaluate node = new BSEvaluate();
			m_nodes.Add( node );
		}


		public void ConnectElements( BSNode left, BSNode right )
		{
			left.AddChild( right );
			m_nodes.Add( node );
		}


		public void UpdateScheme()
		{

		}

		public void OnExitNode( BSNode exitNode )
		{

		}
	}
}