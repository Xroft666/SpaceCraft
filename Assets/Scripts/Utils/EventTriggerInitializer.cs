using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;

public class EventTriggerInitializer : MonoBehaviour 
{
	public ContainerView m_container;

	private void Awake()
	{
		// Adding on click event callback dynamically
		
		EventTrigger.TriggerEvent pointerClickEvent;
		pointerClickEvent = new EventTrigger.TriggerEvent();
		pointerClickEvent.AddListener((eventData) => EntitySelection.OnEntityClicked(m_container));
		
		EventTrigger.Entry pointerEnterEntry;
		pointerEnterEntry = new EventTrigger.Entry();
		pointerEnterEntry.eventID = EventTriggerType.PointerClick;
		pointerEnterEntry.callback = pointerClickEvent;
		
		EventTrigger eventTrigger = GetComponent<EventTrigger>();
		#if UNITY_5_1
		eventTrigger.triggers.Add(pointerEnterEntry);
		#else
		eventTrigger.delegates.Add(pointerEnterEntry);
		#endif
	}
}
