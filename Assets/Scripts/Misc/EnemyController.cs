using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        BulletController bulletCtrl = collision.collider.GetComponent<BulletController>();
		if (bulletCtrl != null && bulletCtrl.owner != null)
        {
			bulletCtrl.owner.Stats.EnemyDamage.Hit(1f);
        }
    }
}
