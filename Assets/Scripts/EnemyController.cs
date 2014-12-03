using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        BulletController bulletCtrl = collision.collider.GetComponent<BulletController>();
        if (bulletCtrl != null)
        {
            Debug.Log("I am a bullet: " + collision.collider.name + " and my owner is: " + bulletCtrl.ownerID);
        }
    }
}
