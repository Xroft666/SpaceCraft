using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	/// <summary>
	/// Device. Represent a single executable entity, that stores
	/// functions and events. Can be compaund, that stores simplier
	/// devices inside
	/// </summary>

	public class Device : Entity
	{
		public Device()
		{
			Blueprint = new BlueprintScheme( this );
		}

		public Ship m_containerAttachedTo = null;
		public bool m_isActive = true;

		/// <summary>
		/// The m_blueprint. The blueprint logic scheme storage.
		/// </summary>
		public BlueprintScheme Blueprint { get;  private set; }


		/// <summary>
		/// The m_integrated devices. If the device is compud, this list will
		/// store all the simplier devices in it
		/// </summary>
		public List<Device> m_integratedDevices = new List<Device>();


		public Dictionary<string, DeviceAction> m_actions = new Dictionary<string, DeviceAction>();
		public Dictionary<string, DeviceTrigger> m_triggers = new Dictionary<string, DeviceTrigger>();
		public Dictionary<string, BSEntry> m_entries = new Dictionary<string, BSEntry>();
		public Dictionary<string, DeviceCheck> m_checks = new Dictionary<string, DeviceCheck>();
		public Dictionary<string, DeviceQuery> m_queries = new Dictionary<string, DeviceQuery>();
	

		public void AssignContainer( Ship container )
		{
			m_containerAttachedTo = container;
			foreach( Device device in m_integratedDevices )
				device.AssignContainer( container );
		}

		public void UnassignContainer( )
		{
			m_containerAttachedTo = null;
			foreach( Device device in m_integratedDevices )
				device.UnassignContainer( );
		}

		public void GetCompleteActionsList( string hierarchy, ref Dictionary<string, DeviceAction> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, DeviceAction> function in m_actions )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				functionsList.Add(key, function.Value);
			}
			
			foreach( Device device in m_integratedDevices )
				device.GetCompleteActionsList(hierarchy, ref functionsList);		
		}

		public void GetCompleteTriggersList( string hierarchy, ref Dictionary<string, DeviceTrigger> functionsList )
		{
			hierarchy += "/" + EntityName;

			foreach( KeyValuePair<string, DeviceTrigger> function in m_triggers )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}

				functionsList.Add( key, function.Value);
			}
					
			foreach( Device device in m_integratedDevices )
				device.GetCompleteTriggersList(hierarchy, ref functionsList);		
		}

		public void GetCompleteQueriesList( string hierarchy, ref Dictionary<string, DeviceQuery> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, DeviceQuery> function in m_queries )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				
				functionsList.Add( key, function.Value);
			}
			
			foreach( Device device in m_integratedDevices )
				device.GetCompleteQueriesList(hierarchy, ref functionsList);		
		}

		public void GetCompleteChecksList( string hierarchy, ref Dictionary<string, DeviceCheck> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, DeviceCheck> function in m_checks )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				
				functionsList.Add( key, function.Value);
			}
			
			foreach( Device device in m_integratedDevices )
				device.GetCompleteChecksList(hierarchy, ref functionsList);		
		}

		public void GetCompleteExitsList( string hierarchy, ref Dictionary<string, BSEntry> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, BSEntry> function in m_entries )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				
				functionsList.Add( key, function.Value);
			}
			
			foreach( Device device in m_integratedDevices )
				device.GetCompleteExitsList(hierarchy, ref functionsList);		
		}


		public Device GetInternalDevice(string path)
		{
			Device device = this;
			bool existenceFlag = true;

			string[] hierarchy = path.Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries);

			for( int i = 0; i < hierarchy.Length; i++ )
			{
				existenceFlag = false;
				foreach( Device inclusiveDevice in device.m_integratedDevices )
				{
					if( inclusiveDevice.EntityName.Equals( hierarchy[i] ) )
					{
						existenceFlag = true;
						device = inclusiveDevice;
						break;
					}
				}
			}

			if( !existenceFlag )
				device = null;

			return device;
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
			//if( m_entries.ContainsKey(name) )
			//	m_entries[name] += trigger ;
			//else
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


		public void InstallDevices( List<Device> devices )
		{
			m_integratedDevices.AddRange( devices );
			foreach( Device device in devices )
			{
				device.AssignContainer( m_containerAttachedTo );
				device.OnDeviceInstalled();
			}
		}

		public void InstallDevice( Device device )
		{
			device.AssignContainer( m_containerAttachedTo );
			m_integratedDevices.Add( device );
			device.OnDeviceInstalled();
		}

		public void UninstallDevice( Device device )
		{
			device.UnassignContainer( );
			m_integratedDevices.Remove( device );
			device.OnDeviceUninstalled();
		}

		public List<Device> GetDevicesList()
		{
			return m_integratedDevices;
		}

		public void ExecuteLogic()
		{
			if( !m_isActive )
				return;

			if( !Blueprint.tasksRunner.IsRunning )
			{
				//Blueprint.RunLogicTree( GetTrigger( "RootEntry" ) );
				Blueprint.RunLogicTree( GetEntry("RootEntry").Traverse );
				Blueprint.tasksRunner.ExecuteTasksQeue();
			}
		}


		#region Common events and function

		public virtual IEnumerator ActivateDevice( DeviceQuery qry )
		{
			m_isActive = true;

			foreach( Device device in m_integratedDevices )
				yield return Job.make(device.ActivateDevice( qry ) ).startAsCoroutine();
		}

		public virtual IEnumerator DeactivateDevice( DeviceQuery qry )
		{
			m_isActive = false;
			
			foreach( Device device in m_integratedDevices )
				yield return Job.make( device.DeactivateDevice( qry) ).startAsCoroutine();
		}

		#endregion

		#region Device Callback Interface

		/// <summary>
		/// OnDeviceInstalled. Initializes device when installed on container.
		/// If is compound, will call the respected function in children
		/// If not, will call the overwriten virtual function
		/// </summary>
		public virtual void OnDeviceInstalled() 
		{
			AddAction("ActivateDevice", ActivateDevice );
			AddAction("DeactivateDevice", DeactivateDevice );
		}

		public virtual void OnDeviceUninstalled()
		{
			RemoveAction("ActivateDevice" );
			RemoveAction("DeactivateDevice" );
		}

		public virtual void Initialize() 
		{
			foreach( Device device in m_integratedDevices )
				device.Initialize();
		}

		public virtual void Update() 
		{
			if( !m_isActive )
				return;

			foreach( Device device in m_integratedDevices )
				device.Update();
		}

		public override void Destroy() 
		{
			foreach( Device device in m_integratedDevices )
				device.Destroy();
		}

		#endregion
	}
}