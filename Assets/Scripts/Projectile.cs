using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour {

	float maxLife = 60000;

	// Use this for initialization
	void Start () {
		//GetComponent<CircleCollider2D>().isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		maxLife-=Time.deltaTime;
		if(maxLife<0){
			Destroy(gameObject);
		}
	}


	void OnCollisionEnter2D(Collision2D col){
		Destroy(gameObject);
	}
}
