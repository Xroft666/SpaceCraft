using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;

public class EventTriggerInitializer : MonoBehaviour 
{
	public ContainerRepresentation m_container;

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
		eventTrigger.delegates.Add(pointerEnterEntry);
	}
}
