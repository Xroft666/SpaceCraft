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
		public List<BSNode> m_nodes;
		public BSEntry m_entryPoint;
	
		public BlueprintScheme()
		{
			m_nodes = new List<BSNode> ();
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
			DeviceCheck check = device.GetCheck(checkName);
			
			BSCheck node = new BSCheck() { m_name = "Predecate", m_type = checkName, m_device = device, m_checkName = checkName };
			m_nodes.Add(node);
			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device)
		{
			BSEntry node = new BSEntry() { m_name = "Entry", m_type = eventName };
			device.AddEntry( eventName, node );

			m_nodes.Add(node);
			return node;
		}

		public BSEntry CreateTrigger(string eventName, Device device)
		{
			BSEntry node = new BSEntry() { m_name = "Entry", m_type = eventName };
			device.AddTrigger( eventName, node.Traverse );
			
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