using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

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
		protected List<Device> m_integratedDevices = new List<Device>();


		public Dictionary<string, DeviceAction> m_actions = new Dictionary<string, DeviceAction>();
		public Dictionary<string, DeviceEvent> m_events = new Dictionary<string, DeviceEvent>();
	//	public Dictionary<string, DeviceQuery> m_exits = new Dictionary<string, DeviceQuery>();
		public Dictionary<string, DeviceCheck> m_checks = new Dictionary<string, DeviceCheck>();
		public Dictionary<string, DeviceQuery> m_queries = new Dictionary<string, DeviceQuery>();
	

		public void AssignContainer( Ship container )
		{
			m_containerAttachedTo = container;
			foreach( Device device in m_integratedDevices )
				device.AssignContainer( container );
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

		public void GetCompleteEventsList( string hierarchy, ref Dictionary<string, DeviceEvent> functionsList )
		{
			hierarchy += "/" + EntityName;

			foreach( KeyValuePair<string, DeviceEvent> function in m_events )
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
				device.GetCompleteEventsList(hierarchy, ref functionsList);		
		}


		public Device GetInternalDevice(string path)
		{
			Device device = this;
			bool existenceFlag = true;

			string[] hierarchy = path.Split('/');
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
			return m_actions[name];
		}

		public void RemoveFunction ( string name )
		{
			m_actions.Remove( name );
		}


		public void AddQuery ( string name, DeviceQuery query )
		{
			m_queries.Add( name, query );
		}
		
		public DeviceQuery GetQuery ( string name )
		{
			if( string.IsNullOrEmpty( name ) )
			   return null;

			return m_queries[name];
		}
		
		public void RemoveQuery ( string name )
		{
			m_queries.Remove( name );
		}


		public void AddEvent ( string name, DeviceEvent trigger )
		{
			if( m_events.ContainsKey(name) )
				m_events[name] += trigger ;
			else
				m_events.Add(name, trigger);
		}

		public DeviceEvent GetEvent( string name )
		{
			return m_events[name];
		}
		
		public void RemoveEvent ( string name )
		{
			m_events.Remove( name );
		}

		// Predecates

		public void AddCheck( string name, DeviceCheck check )
		{
			m_checks.Add( name, check );
		}

		public DeviceCheck GetCheck( string name )
		{
			return m_checks[name];
		}
		
		public void RemoveCheck ( string name )
		{
			m_checks.Remove( name );
		}


		/// <summary>
		/// Installs the equipment. Sets up a list of devices at one go.
		/// And then initializes them
		/// </summary>
		/// <param name="devices">Devices. List of devices</param>
		public void IntegrateDevices( List<Device> devices )
		{
			m_integratedDevices.AddRange( devices );
			foreach( Device device in devices )
			{
				device.AssignContainer( m_containerAttachedTo );
				device.OnDeviceInstalled();
			}
		}

		/// <summary>
		/// Installs the device. Installs a single device and initializes it
		/// </summary>
		/// <param name="device">Device.</param>
		public void IntegrateDevice( Device device )
		{
			device.AssignContainer( m_containerAttachedTo );
			m_integratedDevices.Add( device );
			device.OnDeviceInstalled();
		}

		public List<Device> GetDevicesList()
		{
			return m_integratedDevices;
		}


//		public void ScheduleEvent(DeviceAction evt, DeviceQuery data = null)
//		{
//			Blueprint.ScheduleEvent( evt, data );
//		}

		public void ExecuteLogic()
		{
			if( !Blueprint.tasksRunner.IsRunning )
			{
				Blueprint.RunLogicTree( GetEvent( "RootEntry" ) );
				Blueprint.tasksRunner.ExecuteTasksQeue();
			}

//			foreach( Device device in m_integratedDevices )
//				device.ExecuteLogic();
		}


		#region Common events and function

		public virtual IEnumerator ActivateDevice( DeviceQuery qry )//EventArgs args )
		{
			m_isActive = true;

			foreach( Device device in m_integratedDevices )
				yield return Job.make(device.ActivateDevice( qry /*args*/ ) ).startAsCoroutine();
		}

		public virtual IEnumerator DeactivateDevice( DeviceQuery qry )//EventArgs args )
		{
			m_isActive = false;
			
			foreach( Device device in m_integratedDevices )
				yield return Job.make( device.DeactivateDevice( qry/*args*/) ).startAsCoroutine();
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