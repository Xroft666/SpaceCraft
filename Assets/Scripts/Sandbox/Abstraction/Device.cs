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
		protected Container m_containerAttachedTo = null;

		/// <summary>
		/// The m_integrated devices. If the device is compud, this list will
		/// store all the simplier devices in it
		/// </summary>
		protected List<Device> m_integratedDevices = null;

		/// <summary>
		/// The m_functions. List of functions that are exposed to the logic scheme
		/// </summary>
		protected Dictionary<string, Job> m_functions = new Dictionary<string, Job>();
		/// <summary>
		/// The m_events. List of trigger events that are exposed to the logic scheme
		/// </summary>
		protected Dictionary<string, UnityEvent> m_events = new Dictionary<string, UnityEvent>();


		/// <summary>
		/// Gets the functions list. Returns the list of all the functions and their names
		/// including the current "compound" list if it is not end-point device
		/// </summary>
		/// <returns>The functions list.</returns>
		public Dictionary<string, Job> GetCompleteFunctionsList()
		{
			Dictionary<string, Job> functionsList = new Dictionary<string, Job>(m_functions);

			foreach( Device device in m_integratedDevices )
			{
				foreach(KeyValuePair<string, Job> function in device.GetCompleteFunctionsList())
				{
					functionsList.Add(function.Key, function.Value);
				}
			}

			return functionsList;
		}

		public void AddFunction ( string name, Job function )
		{
			m_functions[name] = function;
		}

		public Job GetFunction ( string name )
		{
			return m_functions[name];
		}

		public void RemoveFunction ( string name )
		{
			m_functions.Remove( name );
		}


		/// <summary>
		/// Gets the complete events list. 
		/// </summary>
		/// <returns>The complete events list.</returns>

		public Dictionary<string, UnityEvent> GetCompleteEventsList()
		{
			Dictionary<string, UnityEvent> eventsList = new Dictionary<string, UnityEvent>(m_events);

			foreach( Device device in m_integratedDevices )
			{
				foreach(KeyValuePair<string, UnityEvent> dEvent in device.GetCompleteEventsList())
				{
					eventsList.Add(dEvent.Key, dEvent.Value);
				}
			}
			
			return eventsList;
		} 

		public void AddEvent ( string name, UnityEvent trigger )
		{
			m_events.Add(name, trigger);
		}

		public UnityEvent GetEvent( string name )
		{
			return m_events[name];
		}
		
		public void RemoveEvent ( string name )
		{
			m_events.Remove( name );
		}

		/// <summary>
		/// Installs the equipment. Sets up a list of devices at one go.
		/// And then initializes them
		/// </summary>
		/// <param name="devices">Devices. List of devices</param>
		public void InstallEquipment( List<Device> devices )
		{
			m_integratedDevices = devices;
			foreach( Device device in devices )
				device.OnDeviceInstalled();
		}

		/// <summary>
		/// Installs the device. Installs a single device and initializes it
		/// </summary>
		/// <param name="device">Device.</param>
		public void InstallDevice( Device device )
		{
			if( m_integratedDevices == null )
				m_integratedDevices = new List<Device>();

			m_integratedDevices.Add( device );
			device.OnDeviceInstalled();
		}

		/// <summary>
		/// Activates the specified action by name.
		/// </summary>
		/// <param name="functionName">Function name to be activated.</param>
		public void Activate( string functionName )
		{
			m_functions[functionName].start();
		}

		/// <summary>
		/// Activates the action by name and returns coroutine
		/// </summary>
		/// <returns> Returns Coroutine.</returns>
		/// <param name="functionName">Function name to be activated.</param>
		public IEnumerator ActivateAndWait( string functionName )
		{
			yield return m_functions[functionName].startAsCoroutine();
		}

		#region Device Callback Interface

		/// <summary>
		/// OnDeviceInstalled. Initializes device when installed on container.
		/// If is compound, will call the respected function in children
		/// If not, will call the overwriten virtual function
		/// </summary>
		public virtual void OnDeviceInstalled() 
		{
			foreach( Device device in m_integratedDevices )
				device.OnDeviceInstalled();
		}
		/// <summary>
		/// OnDeviceInstalled. Initializes device when installed on container.
		/// If is compound, will call the respected function in children
		/// If not, will call the overwriten virtual function
		/// </summary>
		public virtual void Initialize() 
		{
			foreach( Device device in m_integratedDevices )
				device.Initialize();
		}
		/// <summary>
		/// OnDeviceInstalled. Initializes device when installed on container.
		/// If is compound, will call the respected function in children
		/// If not, will call the overwriten virtual function
		/// </summary>
		public virtual void Update() 
		{
			foreach( Device device in m_integratedDevices )
				device.Update();
		}
		/// <summary>
		/// OnDeviceInstalled. Initializes device when installed on container.
		/// If is compound, will call the respected function in children
		/// If not, will call the overwriten virtual function
		/// </summary>
		public virtual void Delete() 
		{
			foreach( Device device in m_integratedDevices )
				device.Delete();
		}

		public virtual void OnObjectEntered() 
		{
			foreach( Device device in m_integratedDevices )
				device.OnObjectEntered();
        }

		public virtual void OnObjectEscaped() 
        {
            foreach( Device device in m_integratedDevices )
				device.OnObjectEscaped();
        }
        
        #endregion
	}
}