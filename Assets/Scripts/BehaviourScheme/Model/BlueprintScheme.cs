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
		public BlueprintScheme( Device device )
		{
			m_blueprintDevice = device;
		}

		private Device m_blueprintDevice;

		public List<BSNode> m_nodes = new List<BSNode>();

		public BSEntry m_entryPoint;
		public TasksRunner tasksRunner = new TasksRunner();

		public void RunLogicTree( DeviceEvent evt )
		{
			evt();
		}

		public void FireEvent( Device actionDevice, string actionName, Device queryDevice, string queryName )
			//DeviceAction evt, DeviceQuery data)
		{
			TasksRunner containersPlanner = m_blueprintDevice.m_containerAttachedTo.IntegratedDevice.Blueprint.tasksRunner;

			DeviceAction evt = actionDevice.GetFunction( actionName );

			DeviceQuery data = null;
			if( queryDevice != null )
				data = queryDevice.GetQuery( queryName );

			containersPlanner.ScheduleEvent( evt, data );
		}
	

		#region Nodes Creation 


		public BSAction CreateAction( string functionName, Device device)
		{
			DeviceAction action = device.GetFunction(functionName);

			BSAction node = new BSAction() { m_scheme = this, m_name = "Action", m_type = functionName, m_device = device, m_actionName = functionName };

			m_nodes.Add(node);
			return node;
		}

		public BSQuery CreateQuery( string queryName, Device device )
		{
			BSQuery node = new BSQuery() { m_scheme = this, m_name = "Query", m_type = queryName, m_device = device, m_queryName = queryName };
			m_nodes.Add(node);
			return node;
		}

		public BSCheck CreatePredecate( string checkName, Device device )
		{
			DeviceCheck check = device.GetCheck(checkName);
			
			BSCheck node = new BSCheck() { m_scheme = this, m_name = "Predecate", m_type = checkName, m_device = device, m_checkName = checkName };
			m_nodes.Add(node);
			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device)
		{
			BSEntry node = new BSEntry() { m_scheme = this, m_name = "Entry", m_type = eventName };
			device.AddEvent(eventName, node.Traverse);

			m_nodes.Add(node);
			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit() { m_scheme = this, m_entry = device.GetEvent(eventName), m_name = "Function", m_type = eventName };

			m_nodes.Add(node);
			return node;
		}

		public BSSelect CreateBranch( string name = "")
		{
			BSSelect node = new BSSelect() { m_scheme = this, m_type = "Selection", m_name = name };

			m_nodes.Add(node);
			return node;
		}

		public BSSequence CreateSequence(string name = "")
		{
			BSSequence node = new BSSequence() { m_scheme = this, m_type = "Sequence", m_name = name };
			
			m_nodes.Add(node);
			return node;
		}

		public BSEvaluate CreateEvaluate(string name = "")
		{
			BSEvaluate node = new BSEvaluate() { m_scheme = this, m_type = "Evaluation", m_name = name };

			m_nodes.Add(node);
			return node;
		}

		public BSForeach CreateForeach( DeviceQuery listQuery )
		{
			BSForeach node = new BSForeach() { m_scheme = this, m_listQuery = listQuery, m_type = "Foreach" };

			m_nodes.Add(node);
			return node;
		}

		// public BSWhile CreateWhile(){}



		public void ConnectElements( BSNode left, BSNode right )
		{
			left.AddChild( right );
		}

		#endregion
	}
}