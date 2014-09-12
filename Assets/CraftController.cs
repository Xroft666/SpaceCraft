using UnityEngine;
using System.Collections;

public class CraftController : MonoBehaviour 
{
	public float gridScale = 1f;
	public GameObject containerPrefab;

	private GameObject containerGameObject = null;
	private Vector3 clickedMousePosition = Vector3.zero;
	private Vector3 originalWorldPosition = Vector3.zero;

	void Update()
	{
		if( Input.GetMouseButtonDown(0) )
		{
			Vector3 mousePosition = Input.mousePosition;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint( new Vector3(mousePosition.x, mousePosition.y, 
			                                                                    Mathf.Abs(Camera.main.transform.position.z)));
			
			if( !Physics2D.Raycast( new Vector2(worldPosition.x, worldPosition.y), Vector2.zero)) 
			{
				containerGameObject = Instantiate(containerPrefab) as GameObject;
				ContainerRepresentation containerRepresentation = containerGameObject.AddComponent(typeof(ContainerRepresentation)) as ContainerRepresentation;
				
				Vector3 descretePosition = worldPosition + Vector3.one * gridScale * 0.5f;
				descretePosition = new Vector3(Mathf.Floor(descretePosition.x ), 
				                               Mathf.Floor(descretePosition.y ),
				                               Mathf.Floor(descretePosition.z ));
				
				containerGameObject.transform.position = descretePosition;

				clickedMousePosition = mousePosition;
				originalWorldPosition = descretePosition;
			}
		}

		if( Input.GetMouseButtonUp(0) )
		{
			containerGameObject = null;
		}

		if( containerGameObject != null )
		{
			Vector3 mouseDifference = Input.mousePosition - clickedMousePosition;

			Vector2 worldSpaceDistance = new Vector2(Camera.main.aspect * mouseDifference.x / (float) Screen.width, mouseDifference.y / (float) Screen.height) * Camera.main.orthographicSize * 2f;
			Vector2 desceteMouseOffset = new Vector2(Mathf.Floor(worldSpaceDistance.x), Mathf.Floor(worldSpaceDistance.y));

			containerGameObject.transform.position = originalWorldPosition + new Vector3(desceteMouseOffset.x, desceteMouseOffset.y, 0f);
			containerGameObject.transform.localScale = new Vector3( desceteMouseOffset.x, desceteMouseOffset.y, 1f);
		}
	}
}
