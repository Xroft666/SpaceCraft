using UnityEngine;
using System;
using System.Collections.Generic;

using SpaceSandbox;

using UnityEngine.UI;

public class DeveloperInterface : MonoBehaviour 
{
	private RectTransform m_actionsContent, m_eventsContent, m_exitsContent, m_controlsContent;

	private float m_buttonsDistance = 20f;

	private void Awake()
	{
		m_actionsContent = transform.FindChild("Actions/Content") as RectTransform;
		m_eventsContent = transform.FindChild("Events/Content") as RectTransform;
		m_exitsContent = transform.FindChild("Exits/Content") as RectTransform;
		m_controlsContent = transform.FindChild("Controls/Content") as RectTransform;
	}

	public void InitializeInteface( Ship selectedContainer)
	{
		Device upMostDevice = selectedContainer.IntegratedDevice;

		InitializeActions(upMostDevice);
		InitializeEvents(upMostDevice);
		InitializeExits(upMostDevice);
		InitializeControls(upMostDevice);
	}

	private void InitializeActions(Device device)
	{
		CleanContent(m_actionsContent);

		Dictionary<string, DeviceAction> actions = new Dictionary<string, DeviceAction>();
		device.GetCompleteActionsList("", ref actions);

		FillUpContent( new List<string>(actions.Keys), m_actionsContent );
	}

	private void InitializeEvents(Device device)
	{
		CleanContent(m_eventsContent);

		Dictionary<string, DeviceEvent> events = new Dictionary<string, DeviceEvent>();
		device.GetCompleteEventsList("", ref events);

		FillUpContent( new List<string>(events.Keys), m_eventsContent );
	}

	private void InitializeExits(Device device)
	{
		CleanContent(m_exitsContent);
//		FillUpContent( device.m_events, m_exitsContent );
	}

	private void InitializeControls(Device device)
	{
		CleanContent(m_controlsContent);
//		FillUpContent( device.m_actions, m_controlsContent );
	}

	private void FillUpContent( List<string> content, RectTransform contentTransform )
	{
		int actionsNum = content.Count;
		int count = -actionsNum / 2;

		contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, actionsNum * 20f);

		foreach( string item in content )
		{
			string itemName = item;

			GameObject newAction = new GameObject(item, typeof(RectTransform));
			newAction.transform.SetParent( contentTransform, false );

			RectTransform transf = newAction.GetComponent<RectTransform>();
			transf.sizeDelta = new Vector2(250f, 20f);

			Button button = newAction.AddComponent<Button>();

			button.onClick.AddListener( () =>
			{
				// Create Node

				Debug.Log(itemName + " clicked");
			});

			Text text = newAction.AddComponent<Text>();
			text.text = item;
			text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
			text.alignment = TextAnchor.MiddleCenter;
			
			newAction.transform.localPosition = Vector3.up * count * m_buttonsDistance;
			
			count++;
		}
	}

	private void CleanContent( Transform content )
	{
		foreach( Transform child in content )
			GameObject.Destroy( child.gameObject );
	}
}
