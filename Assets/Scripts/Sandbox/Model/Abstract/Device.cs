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

	public class Device : Entity, IDestroyable, IUpdatable
	{
		public Ship m_container = null;
		public bool m_isActive = true;

		/// <summary>
		/// The m_blueprint. The blueprint logic scheme storage.
		/// </summary>
		public BlueprintScheme m_blueprint { get; private set; }

		/// <summary>
		/// The m_integrated devices. If the device is compud, this list will
		/// store all the simplier devices in it
		/// </summary>
		public List<Device> m_devices = new List<Device>();


	

		public Device()
		{

		}

		public Device(Device other)
		{
			foreach (var childDevice in other.m_devices)
				m_devices.Add (new Device (childDevice));

			m_blueprint = new BlueprintScheme (other.m_blueprint);
		}

		public void AssignContainer( Ship container )
		{
			m_container = container;
			foreach( Device device in m_devices )
				device.AssignContainer( container );
		}

		public void UnassignContainer( )
		{
			m_container = null;
			foreach( Device device in m_devices )
				device.UnassignContainer( );
		}

		public void PlugBlueprintScheme(BlueprintScheme scheme)
		{
			m_blueprint = scheme;
		}

		public void UnplugBlueprintScheme()
		{
			m_blueprint = null;
		}

		public void GetCompleteActionsList( string hierarchy, ref Dictionary<string, DeviceAction> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, DeviceAction> function in m_blueprint.m_actions )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				functionsList.Add(key, function.Value);
			}
			
			foreach( Device device in m_devices )
				device.GetCompleteActionsList(hierarchy, ref functionsList);		
		}

		public void GetCompleteTriggersList( string hierarchy, ref Dictionary<string, DeviceTrigger> functionsList )
		{
			hierarchy += "/" + EntityName;

			foreach( KeyValuePair<string, DeviceTrigger> function in m_blueprint.m_triggers )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}

				functionsList.Add( key, function.Value);
			}
					
			foreach( Device device in m_devices )
				device.GetCompleteTriggersList(hierarchy, ref functionsList);		
		}

		public void GetCompleteQueriesList( string hierarchy, ref Dictionary<string, DeviceQuery> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, DeviceQuery> function in m_blueprint.m_queries )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				
				functionsList.Add( key, function.Value);
			}
			
			foreach( Device device in m_devices )
				device.GetCompleteQueriesList(hierarchy, ref functionsList);		
		}

		public void GetCompleteChecksList( string hierarchy, ref Dictionary<string, DeviceCheck> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, DeviceCheck> function in m_blueprint.m_checks )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				
				functionsList.Add( key, function.Value);
			}
			
			foreach( Device device in m_devices )
				device.GetCompleteChecksList(hierarchy, ref functionsList);		
		}

		public void GetCompleteExitsList( string hierarchy, ref Dictionary<string, BSEntry> functionsList )
		{
			hierarchy += "/" + EntityName;
			
			foreach( KeyValuePair<string, BSEntry> function in m_blueprint.m_entries )
			{
				string key = hierarchy + "." + function.Key;
				if( functionsList.ContainsKey( key ) )
				{
					Debug.LogError("Key already exists: " + key );
					continue;
				}
				
				functionsList.Add( key, function.Value);
			}
			
			foreach( Device device in m_devices )
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
				foreach( Device inclusiveDevice in device.m_devices )
				{
					if( inclusiveDevice.EntityName.ToLower().Equals( hierarchy[i].ToLower() ) )
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

		public void InstallDevices( List<Device> devices )
		{
			m_devices.AddRange( devices );
			foreach( Device device in devices )
			{
				device.AssignContainer( m_container );
				device.OnDeviceInstalled();
			}
		}

		public void InstallDevice( Device device )
		{
			device.AssignContainer( m_container );
			m_devices.Add( device );
			device.OnDeviceInstalled();
		}

		public void UninstallDevice( Device device )
		{
			device.UnassignContainer( );
			m_devices.Remove( device );
			device.OnDeviceUninstalled();
		}

		public List<Device> GetDevicesList()
		{
			return m_devices;
		}

		public void FireEvent( string actionName, DeviceQuery query )
		{
			DeviceAction evt = m_blueprint.GetFunction( actionName );
			m_container.m_tasksRunner.ScheduleEvent( evt, query );
		}

		#region Common events and function

		public IEnumerator ActivateDevice(DeviceQuery qry)
		{
			ActivateDevice ();
			yield return null;
		}

		public IEnumerator DeactivateDevice(DeviceQuery qry)
		{
			DeactivateDevice ();
			yield return null;
		}

		public virtual void ActivateDevice()
		{
			m_isActive = true;

			foreach (var device in m_devices)
				device.ActivateDevice ();
		}

		public virtual void DeactivateDevice()
		{
			m_isActive = false;

			foreach (var device in m_devices)
				device.DeactivateDevice ();
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
			m_blueprint.AddAction ("ActivateDevice", ActivateDevice);
			m_blueprint.AddAction ("DeactivateDevice", DeactivateDevice);
		}

		public virtual void OnDeviceUninstalled()
		{
			m_blueprint.RemoveAction ("ActivateDevice");
			m_blueprint.RemoveAction ("DeactivateDevice");
		}

		public virtual void Initialize() 
		{
			foreach( Device device in m_devices )
				(device as IUpdatable).Initialize();
		}

		public virtual void Update() 
		{
			if( !m_isActive )
				return;

			foreach( Device device in m_devices )
				(device as IUpdatable).Update();
		}

		public virtual void Destroy() 
		{
			foreach( Device device in m_devices )
				(device as IDestroyable).Destroy();
		}

		#endregion
	}
}