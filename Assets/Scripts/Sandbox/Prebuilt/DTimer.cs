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
	private bool m_fired = false;

	public void SetUpTimer( float time )
	{
		m_timerSetUp = time;
	}

	public void Reset()
	{
		m_timer = 0f;
		m_fired = true;
	}

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnTimerTrigger", new UnityEvent() );
	}

	public override void Initialize()
	{
		m_timer = 0f;
	}

	public override void Update()
	{
		if( m_fired )
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
		m_timer = 0f;

		UnityEvent timerEvent = GetEvent("OnTimerTrigger");
		timerEvent.RemoveAllListeners();
	}
}
