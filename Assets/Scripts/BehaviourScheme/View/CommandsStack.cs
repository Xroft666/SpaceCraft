using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;

using System.Linq;
using UnityEngine.UI;

public class CommandsStack 
{
	private Transform m_stackTransform;

	private int m_Size;

	private Queue<string> commandsQueue = new Queue<string>(5);

	private ContainerView m_view;
	private List<Text> textComps = new List<Text>();

	public void InitializeStack( Transform stackTransform, int stackSize )
	{
		m_stackTransform = stackTransform;
		m_Size = stackSize;

		for( int i = -2; i <= 2; i ++ ) 
		{
			GameObject label = new GameObject("Command", typeof(RectTransform));
			RectTransform lTransfrm = label.transform as RectTransform;

			lTransfrm.SetParent( stackTransform, false );
			lTransfrm.sizeDelta = new Vector2(150f, 100f);
			lTransfrm.transform.localPosition = Vector3.right * i * 150f;

			Text lText = label.AddComponent<Text>();
			lText.text = "";
			lText.color = i == 0 ? Color.red : new Color(1f, 1f, 1f, 0.5f + 0.5f / (Mathf.Sign(i) * i) );
			lText.alignment = TextAnchor.MiddleCenter;
			lText.font = Font.CreateDynamicFontFromOSFont("Arial", 14);

			textComps.Add( lText );
		}
	}

	public void InitializeContainerView( ContainerView view )
	{
		Ship ship = view.m_contain as Ship;
		if( ship == null )
			return;

		m_view = view;

		ship.IntegratedDevice.Blueprint.onInitialize += InitializeCommandList;
		ship.IntegratedDevice.Blueprint.OnJobComplete += UpdateCommandsList;

		InitializeCommandList((m_view.m_contain as Ship).IntegratedDevice.Blueprint.executingCommandList);
	}

	public void InitializeCommandList( IEnumerable<string> commands )
	{
		foreach( Text text in textComps )
			text.text = "";

		foreach( string cmd in commands )
			commandsQueue.Enqueue( cmd );

		for( int i = textComps.Count / 2; i < textComps.Count; i++ )
			textComps[i].text = commandsQueue.Count > 0 ? commandsQueue.Dequeue() : "";
	}

	public void UpdateCommandsList()
	{
		for( int i = 0; i < textComps.Count - 1; i++ )
			textComps[i].text = textComps[i+1].text;

		textComps[ textComps.Count - 1].text = commandsQueue.Count > 0 ? commandsQueue.Dequeue() : "";
	}
	
	public void CleanCommandsStack()
	{
		if( m_view != null )
		{
			(m_view.m_contain as Ship).IntegratedDevice.Blueprint.onInitialize -= InitializeCommandList;
			(m_view.m_contain as Ship).IntegratedDevice.Blueprint.OnJobComplete -= UpdateCommandsList;
		}

		m_view = null;
		commandsQueue.Clear();

		foreach( Text text in textComps )
			text.text = "";
	}
}
