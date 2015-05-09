using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SpaceSandbox
{
	[System.Serializable]
	public class Device : Entity 
	{
		protected BlueprintScheme blueprint = null;

		protected List<Device> m_integratedDevices = new List<Device>();
		protected Dictionary<string, Job> m_functions = new Dictionary<string, Job>();
		protected Dictionary<string, Action> m_events = new Dictionary<string, Action>();



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

		public void AddEvent ( string name, Action action )
		{
			m_events[name] = action;
		}

		public Action GetEvent( string name )
		{
			return m_events[name];
		}
		
		public void RemoveEvent ( string name )
		{
			m_events.Remove( name );
		}




		public void Activate( string functionName )
		{
			m_functions[functionName].start();
		}

		public IEnumerator ActivateAndWait( string functionName )
		{
			yield return m_functions[functionName].startAsCoroutine();
		}


//		public List<Resource> resources = new List<Resource>();

//		public delegate void DeviceCallback( object[] output );
//		public DeviceCallback outputCallback;
//
//		public virtual void OnStart(params object[] input){}
//		public virtual void OnUpdate(){}
//		public virtual void OnDelete(){}
//		
//		public virtual void OnActivate(params object[] input){}
//		public virtual void OnDeactivate(params object[] input){}
	}
}