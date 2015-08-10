using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using SpaceSandbox;

public class InGameUIController : MonoBehaviour 
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
	}

	private void Update()
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
		foreach( GameObject selectionGO in selections.Values )
			selectionGO.SetActive( false );

		if( container == null )
			return;

		if( selections.ContainsKey( container ) )
		{
			selections[container].SetActive(true);
			return;
		}

		GenerateNewSelection( container );
	}

	private void GenerateNewSelection( ContainerView container )
	{
//		Vector2 selectionViewPos = Camera.main.WorldToViewportPoint( container.transform.position );
//		Vector2 offsetVector = selectionViewPos - Vector2.one * 0.5f;
		
		Vector2 selectionScreenPos = Camera.main.WorldToScreenPoint( container.transform.position );

		GameObject newSelection = Instantiate(m_selectionGroupPrefab);
		newSelection.transform.parent = m_selectListTransform;
		
		RectTransform newSelectTransform = newSelection.GetComponent<RectTransform>();
		newSelectTransform.localPosition = selectionScreenPos;
		newSelectTransform.localRotation = Quaternion.identity;
		//rotation = Quaternion.Euler( Vector3.forward * Vector2.Angle(Vector2.right, offsetVector.normalized) );
		newSelectTransform.localScale = Vector3.one;
//		newSelectTransform.sizeDelta = new Vector2(1f, Screen.height * offsetVector.magnitude);
//		newSelectTransform.pivot = new Vector2(0.5f, 0f);

		newSelection.GetComponentInChildren<Text>().text = container.name;
		
		selections.Add( container, newSelection );
	}

	private void OnDestroy()
	{
		selections.Clear();
		EntitySelection.onEntityClicked -= OnContainerSelectedEvent;
	}
}
