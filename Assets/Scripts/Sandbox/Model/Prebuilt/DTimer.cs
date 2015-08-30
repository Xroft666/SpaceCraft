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

	private float m_timer = 0f;
	private bool m_fired = false;
	private TextMesh m_timeText;
	

	#region device's functions
	
	public IEnumerator ResetTimer( EventArgs args)
	{
		m_timer = 0f;
		m_fired = false;

		yield break;
	}

	#endregion

	#region device's interface implementation

	public override IEnumerator ActivateDevice ( EventArgs args )
	{
		m_isActive = true;
		if( m_timeText != null )
			m_timeText.gameObject.SetActive(true);

		yield break;
	} 
	
	public override IEnumerator DeactivateDevice( EventArgs args )
	{
		m_isActive = false;
		if( m_timeText != null )
			m_timeText.gameObject.SetActive(false);

		yield break;
	}

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnTimerComplete", null );
	
		AddAction("ResetTimer", ResetTimer );
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

		m_timer += Time.deltaTime;
		m_timeText.text = (m_timerSetUp - m_timer).ToString("0");

		if( m_timer >= m_timerSetUp )
		{
			m_fired = true;

			DeviceEvent timerEvent = GetEvent("OnTimerComplete");
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
