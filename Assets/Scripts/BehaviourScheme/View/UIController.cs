using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using SpaceSandbox;

public class UIController : MonoBehaviour 
{
	public RectTransform m_selectListTransform;
	public GameObject m_selectionGroupPrefab;

	private Dictionary<ContainerView, GameObject> selections = new Dictionary<ContainerView, GameObject>();

	private void Awake()
	{

	}

	private void Start()
	{
		EntitySelection.onEntityClicked += OnContainerSelectedEvent;
		EntitySelection.onCleanSpaceClicked += HideAllSelections;
	}

	private void Update()
	{
		UtilsKeys();
		CameraControls();
		UpdateSelections();
	}
	
	private void UtilsKeys()
	{
		if( Input.GetKeyUp(KeyCode.R) )
		{
			EntitySelection.Cleanup();
			Application.LoadLevel( Application.loadedLevel );
		}
	}
	
	private void CameraControls()
	{
		if( EntitySelection.selectedContainer == null || !EntitySelection.selectedContainer.gameObject.activeInHierarchy )
		{
			Vector3 input = Vector3.zero; 
			if( Input.GetKey(KeyCode.UpArrow) )
				input += Vector3.up;
			
			if( Input.GetKey(KeyCode.DownArrow) )
				input += Vector3.down;
			
			if( Input.GetKey(KeyCode.LeftArrow) )
				input += Vector3.left;
			
			if( Input.GetKey(KeyCode.RightArrow) )
				input = Vector3.right;
			
			Camera.main.transform.position += input * Time.deltaTime;
		}
		else
		{
			Vector3 position = Vector3.zero;
			position.x = EntitySelection.selectedContainer.transform.position.x;
			position.y = EntitySelection.selectedContainer.transform.position.y;
			position.z = Camera.main.transform.position.z;
			
			Camera.main.transform.position = position;
		}
		
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis( "Mouse ScrollWheel"), 1f, 10f);
	}
	
	public void OnGUI()
	{
		GUILayout.Label("WASD to move your ship");
		GUILayout.Label("Space to fire missiles");
		GUILayout.Label("Arrows to move your camera");
		GUILayout.Label("Select any object to have your camera focused on it");
		GUILayout.Label("Scroll to zoom");
		GUILayout.Label("R to restart");
	}

	private void UpdateSelections()
	{
		foreach( KeyValuePair<ContainerView, GameObject> selection in selections )
		{
			if( selection.Value.activeInHierarchy )
			{
				// Should be changed for events instead if update checks
				if( !selection.Key.gameObject.activeInHierarchy )
				{
					selection.Value.SetActive(false);
					return;
				}

				selection.Value.transform.localPosition = Camera.main.WorldToScreenPoint( selection.Key.transform.position );
			}
		}
	}

	private void OnContainerSelectedEvent( ContainerView container )
	{
		HideAllSelections();

		if( selections.ContainsKey( container ) )
		{
			selections[container].SetActive(true);
			return;
		}

		GenerateNewSelection( container );
	}

	private void HideAllSelections()
	{
		foreach( GameObject selectionGO in selections.Values )
			selectionGO.SetActive( false );
	}

	private void GenerateNewSelection( ContainerView container )
	{
		Vector2 selectionScreenPos = Camera.main.WorldToScreenPoint( container.transform.position );

		GameObject newSelection = Instantiate(m_selectionGroupPrefab);
		newSelection.transform.parent = m_selectListTransform;
		
		RectTransform newSelectTransform = newSelection.GetComponent<RectTransform>();
		newSelectTransform.localPosition = selectionScreenPos;
		newSelectTransform.localRotation = Quaternion.identity;

		newSelectTransform.localScale = Vector3.one;

		newSelection.GetComponentInChildren<Text>().text = container.name;
		
		selections.Add( container, newSelection );
	}

	private void OnDestroy()
	{
		selections.Clear();
		EntitySelection.onEntityClicked -= OnContainerSelectedEvent;
		EntitySelection.onCleanSpaceClicked -= HideAllSelections;
	}
}
