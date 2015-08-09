using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

/// <summary>
/// Timer Device. Counts to the specified second and triggers the event
/// </summary>
public class DTimer : Device 
{
	// Exported values
	public float m_timerSetUp = 0f;
	public bool m_started = false;

	private float m_timer = 0f;

	private bool m_fired = false;

	public void SetUpTimer( float time )
	{
		m_timerSetUp = time;
	}

	#region device's functions

	public void StartTimer(params object[] objects)
	{
		m_started = true;
	}
	
	public void ResetTimer(params object[] objects)
	{
		m_timer = 0f;
		m_fired = false;
		m_started = false;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnTimerTrigger", null );

		AddAction("StartTimer", StartTimer );
		AddAction("ResetTimer", ResetTimer );
	}

	public void OnTimerTrigger(params Entity[] objs)
	{
		
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		if( m_fired )
			return;

		if( !m_started )
			return;

		m_timer += Time.deltaTime;

		if( m_timer >= m_timerSetUp )
		{
			m_fired = true;

			DeviceEvent timerEvent = GetEvent("OnTimerTrigger");
			if( timerEvent != null )
				timerEvent.Invoke();
		}
	}

	#endregion
}
