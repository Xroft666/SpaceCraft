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
			Blueprint = new BlueprintScheme();
		}

		protected Ship m_containerAttachedTo = null;
		protected bool m_isActive = true;

		/// <summary>
		/// The m_blueprint. The blueprint logic scheme storage.
		/// </summary>
		public BlueprintScheme Blueprint { get;  private set; }


		/// <summary>
		/// The m_integrated devices. If the device is compud, this list will
		/// store all the simplier devices in it
		/// </summary>
		protected List<Device> m_integratedDevices = new List<Device>();

		/// <summary>
		/// The m_functions. List of functions that are exposed to the logic scheme
		/// </summary>
		protected Dictionary<string, DeviceEvent> m_actions = new Dictionary<string, DeviceEvent>();
		/// <summary>
		/// The m_events. List of trigger events that are exposed to the logic scheme
		/// </summary>
		public Dictionary<string, DeviceEvent> m_events = new Dictionary<string, DeviceEvent>();

		public Dictionary<string, DeviceCheck> m_checks = new Dictionary<string, DeviceCheck>();

		public void AssignContainer( Ship container )
		{
			m_containerAttachedTo = container;
			foreach( Device device in m_integratedDevices )
				device.AssignContainer( container );
		}

		/// <summary>
		/// Gets the functions list. Returns the list of all the functions and their names
		/// including the current "compound" list if it is not end-point device
		/// </summary>
		/// <returns>The functions list.</returns>
		public Dictionary<string, DeviceEvent> GetCompleteFunctionsList()
		{
			Dictionary<string, DeviceEvent> functionsList = new Dictionary<string, DeviceEvent>(m_actions);

			foreach( Device device in m_integratedDevices )
			{
				foreach(KeyValuePair<string, DeviceEvent> function in device.GetCompleteFunctionsList())
				{
					functionsList.Add(function.Key, function.Value);
				}
			}

			return functionsList;
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



		public void AddAction ( string name, DeviceEvent function )
		{
			m_actions.Add( name, null );
			m_actions[name] += function;
		}

		public DeviceEvent GetFunction ( string name )
		{
			return m_actions[name];
		}

		public void RemoveFunction ( string name )
		{
			m_actions.Remove( name );
		}


		/// <summary>
		/// Gets the complete events list. 
		/// </summary>
		/// <returns>The complete events list.</returns>

		public Dictionary<string, DeviceEvent> GetCompleteEventsList()
		{
			Dictionary<string, DeviceEvent> eventsList = new Dictionary<string, DeviceEvent>(m_events);
			
			foreach( Device device in m_integratedDevices )
			{
				foreach(KeyValuePair<string, DeviceEvent> dEvent in device.GetCompleteEventsList())
				{
					eventsList.Add(dEvent.Key, dEvent.Value);
				}
			}
			
			return eventsList;
		} 

		public void AddEvent ( string name, DeviceEvent trigger )
		{
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


		public void ScheduleEvent(DeviceEvent evt, EventArgs data)
		{
			Blueprint.AddScheduledEvent( evt, data );
		}

		public void ExecuteLogic()
		{
			Blueprint.ExecuteSceduledEvents();
			foreach( Device device in m_integratedDevices )
				device.ExecuteLogic();
		}

		public void CleanScheduledEvents()
		{
			Blueprint.ClearEventsAndData();
			foreach( Device device in m_integratedDevices )
				device.CleanScheduledEvents();
		}

		#region Common events and function

		public virtual void ActivateDevice( EventArgs args )
		{
			m_isActive = true;

			foreach( Device device in m_integratedDevices )
				device.ActivateDevice( args );
		}

		public virtual void DeactivateDevice( EventArgs args )
		{
			m_isActive = false;
			
			foreach( Device device in m_integratedDevices )
				device.DeactivateDevice( args );
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