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
	private float m_timer = 0f;
	private float m_timerSetUp = 0f;

	private bool m_started = false;
	private bool m_fired = false;

	public void SetUpTimer( float time )
	{
		m_timerSetUp = time;
	}

	#region device's functions

	public IEnumerator StartTimer()
	{
		m_started = true;

		yield return null;
	}
	
	public IEnumerator ResetTimer()
	{
		m_timer = 0f;
		m_fired = false;
		m_started = false;

		yield return null;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnTimerTrigger", new UnityEvent() );

		AddFunction("StartTimer", Job.make(StartTimer()) );
		AddFunction("ResetTimer", Job.make(ResetTimer()) );
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

			UnityEvent timerEvent = GetEvent("OnTimerTrigger");
			timerEvent.Invoke();
		}
	}

	public override void Delete()
	{
		UnityEvent timerEvent = GetEvent("OnTimerTrigger");
		timerEvent.RemoveAllListeners();
		RemoveEvent("OnTimerTrigger");
	}

	#endregion
}
