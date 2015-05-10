using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace SpaceSandbox
{
	[System.Serializable]
	public class Device : Entity 
	{
		protected List<Device> m_integratedDevices = new List<Device>();

		protected Dictionary<string, Job> m_functions = new Dictionary<string, Job>();
		protected Dictionary<string, UnityEvent> m_events = new Dictionary<string, UnityEvent>();


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

		public void SetUpIntegratedDevices( List<Device> devices )
		{
			m_integratedDevices = devices;
		}

		public void InstallDevice( Device device )
		{
			m_integratedDevices.Add( device );
		}

		public Container ConvertToContainer()
		{
			Container container = new Container();
			container.InstallEquipment(m_integratedDevices);

			return container;
		}


		public void Activate( string functionName )
		{
			m_functions[functionName].start();
		}

		public IEnumerator ActivateAndWait( string functionName )
		{
			yield return m_functions[functionName].startAsCoroutine();
		}


		public virtual void Initialize() {}
		public virtual void Update() {}
		public virtual void Delete() {}
	}
}