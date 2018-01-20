using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		public List<BSNode> m_nodes = new List<BSNode> ();
		public BSEntry m_entryPoint;

		public Dictionary<string, DeviceAction> m_actions = new Dictionary<string, DeviceAction>();
		public Dictionary<string, DeviceTrigger> m_triggers = new Dictionary<string, DeviceTrigger>();
		public Dictionary<string, BSEntry> m_entries = new Dictionary<string, BSEntry>();
		public Dictionary<string, DeviceCheck> m_checks = new Dictionary<string, DeviceCheck>();
		public Dictionary<string, DeviceQuery> m_queries = new Dictionary<string, DeviceQuery>();
	
		public BlueprintScheme()
		{
			
		}

		public BlueprintScheme(BlueprintScheme other)
		{
			foreach(var node in other.m_nodes)
				m_nodes.Add(node.GetCopy());
		}

		public void AddAction ( string name, DeviceAction function )
		{
			m_actions.Add( name, null );
			m_actions[name] += function;
		}

		public DeviceAction GetFunction ( string name )
		{
			if( !m_actions.ContainsKey(name) )
				return null;

			return m_actions[name];
		}

		public void RemoveAction ( string name )
		{
			m_actions.Remove( name );
		}


		public void AddQuery ( string name, DeviceQuery query )
		{
			m_queries.Add( name, query );
		}

		public DeviceQuery GetQuery ( string name )
		{
			if( !m_queries.ContainsKey(name) )
				return null;

			return m_queries[name];
		}

		public void RemoveQuery ( string name )
		{
			m_queries.Remove( name );
		}


		public void AddTrigger ( string name, DeviceTrigger trigger )
		{
			if( m_triggers.ContainsKey(name) )
				m_triggers[name] += trigger ;
			else
				m_triggers.Add(name, trigger);
		}

		public DeviceTrigger GetTrigger( string name )
		{
			if( !m_triggers.ContainsKey(name) )
				return null;

			return m_triggers[name];
		}

		public void RemoveTrigger ( string name )
		{
			m_triggers.Remove( name );
		}

		public void AddEntry ( string name, BSEntry trigger )
		{
			m_entries.Add(name, trigger);
		}

		public BSEntry GetEntry( string name )
		{
			if( !m_entries.ContainsKey(name) )
				return null;

			return m_entries[name];
		}

		public void RemoveEntry ( string name )
		{
			m_entries.Remove( name );
		}

		// Predecates

		public void AddCheck( string name, DeviceCheck check )
		{
			m_checks.Add( name, check );
		}

		public DeviceCheck GetCheck( string name )
		{
			if( !m_checks.ContainsKey(name) )
				return null;

			return m_checks[name];
		}

		public void RemoveCheck ( string name )
		{
			m_checks.Remove( name );
		}
			
		#region Nodes Creation 


		public BSAction CreateAction( string functionName, Device device)
		{
			BSAction node = new BSAction() { m_name = "Action", m_type = functionName, m_actionName = functionName };
			node.onEventFired += device.FireEvent;

			m_nodes.Add(node);
			return node;
		}

		public BSQuery CreateQuery( string queryName, Device device )
		{
			BSQuery node = new BSQuery() { m_name = "Query", m_type = queryName, m_device = device, m_queryName = queryName };
			m_nodes.Add(node);
			return node;
		}

		public BSCheck CreatePredecate( string checkName, Device device )
		{
			DeviceCheck check = device.m_blueprint.GetCheck(checkName);
			
			BSCheck node = new BSCheck() { m_name = "Predecate", m_type = checkName, m_device = device, m_checkName = checkName };
			m_nodes.Add(node);
			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device)
		{
			BSEntry node = new BSEntry() { m_name = "Entry", m_type = eventName };
			device.m_blueprint.AddEntry( eventName, node );

			m_nodes.Add(node);
			return node;
		}

		public BSEntry CreateTrigger(string eventName, Device device)
		{
			BSEntry node = new BSEntry() { m_name = "Entry", m_type = eventName };
			device.m_blueprint.AddTrigger( eventName, node.Traverse );
			
			m_nodes.Add(node);
			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit() { m_entryName = eventName, m_device = device, m_name = "Exit", m_type = eventName };

			m_nodes.Add(node);
			return node;
		}

		public BSSelect CreateBranch( string name = "")
		{
			BSSelect node = new BSSelect() { m_type = "Selection", m_name = name };

			m_nodes.Add(node);
			return node;
		}

		public BSSequence CreateSequence(string name = "")
		{
			BSSequence node = new BSSequence() { m_type = "Sequence", m_name = name };
			
			m_nodes.Add(node);
			return node;
		}

		public BSEvaluate CreateEvaluate(string name = "")
		{
			BSEvaluate node = new BSEvaluate() { m_type = "Evaluation", m_name = name };

			m_nodes.Add(node);
			return node;
		}

		public BSForeach CreateForeach( DeviceQuery listQuery )
		{
			BSForeach node = new BSForeach() { m_listQuery = listQuery, m_type = "Foreach" };

			m_nodes.Add(node);
			return node;
		}

		public void ConnectElements( BSNode left, BSNode right )
		{
			left.AddChild( right );
		}

		#endregion
	}
}