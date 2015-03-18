using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{

	[HideInInspector] public ShipBuilderBrain owner = null;

	void OnCollisionEnter2D(Collision2D collision)
	{
		Destroy(gameObject);
	}

}
