using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour 
{
	private List<Vector3> flyingPath = new List<Vector3>();
	private float timer = 0f;
	private float speed = 0.05f;

	void Start()
	{

	}

	void Update()
	{
		if( flyingPath.Count > 0 )
		{

			if( timer < 1f )
			{
				int goFromIdx =(int)( timer * (flyingPath.Count)) ;
				int goToIdx =(int)( timer * flyingPath.Count + 1f) ;
			
				timer += Time.deltaTime * speed / (flyingPath[goToIdx] - flyingPath[goFromIdx]).sqrMagnitude;

				transform.position = Vector3.Lerp( flyingPath[goFromIdx], flyingPath[goToIdx], timer * flyingPath.Count - goFromIdx);

			}
			else
			{
//				flyingPath.Clear();
				timer = 0f;
			}
		}				


		if( Input.GetMouseButtonUp(1) )
		{
			Vector3 goToPos = Camera.main.ScreenToWorldPoint( new Vector3(Input.mousePosition.x,
			                                                    Input.mousePosition.y,
			                                                    10f) );
			IEnumerator curve = Interpolate.NewEase( Interpolate.Ease( Interpolate.EaseType.EaseInOutQuad ), 
			                                        transform.position,
			                                        goToPos, 3 );

			                                        //Interpolate.NewCatmullRom( new Vector3[] { transform.position, goToPos }, 10, false ).GetEnumerator();

			flyingPath.Clear();
			timer = 0f;

			while( curve.MoveNext() )
				flyingPath.Add( (Vector3) curve.Current );

			for( int i = 0; i < flyingPath.Count - 1; i++ )
			{
				Debug.DrawLine(flyingPath[i], flyingPath[i+1], Color.red, 1f);
			}
		}        


	}
}
