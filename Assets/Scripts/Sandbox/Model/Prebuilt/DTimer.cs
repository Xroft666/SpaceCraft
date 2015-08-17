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
	private TextMesh m_timeText;


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

	public override void ActivateDevice (params object[] objects)
	{
		m_isActive = true;
		if( m_timeText != null )
			m_timeText.gameObject.SetActive(true);
	} 
	
	public override void DeactivateDevice(params object[] objects)
	{
		m_isActive = false;
		if( m_timeText != null )
			m_timeText.gameObject.SetActive(false);
	}

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
		GameObject text = new GameObject(EntityName);
		text.transform.SetParent( m_containerAttachedTo.View.transform, false );
		text.transform.localPosition = Vector3.right * 0.5f;
		text.transform.localScale = Vector3.one * 0.25f;

		m_timeText = text.AddComponent<TextMesh>();
		m_timeText.text = m_timerSetUp.ToString("0");

		m_timeText.gameObject.SetActive(m_isActive);
	}

	public override void Update()
	{
		if( m_fired )
			return;

		if( !m_started )
			return;

		m_timer += Time.deltaTime;
		m_timeText.text = (m_timerSetUp - m_timer).ToString("0");

		if( m_timer >= m_timerSetUp )
		{
			m_fired = true;

			DeviceEvent timerEvent = GetEvent("OnTimerTrigger");
			if( timerEvent != null )
				m_containerAttachedTo.IntegratedDevice.ScheduleEvent( timerEvent, null );

			m_timeText.gameObject.SetActive(false);
		}
	}

	public override void Destroy ()
	{
		GameObject.Destroy( m_timeText.gameObject );
	}

	#endregion
}
