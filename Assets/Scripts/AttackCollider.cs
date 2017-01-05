using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Attack
{
	public float minDamage;
	public float maxDamage;
}

public class AttackCollider : MonoBehaviour
{
	public bool isAttacking;
	public bool isPlayer;

	public PolygonCollider2D weaponCollider;
	public Attack attack;




	void Update ()
	{
		if (isAttacking) {
			if (isPlayer) {
				foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
					Collider2D collider = enemy.GetComponent<Collider2D> ();
					if (weaponCollider.IsTouching (collider)) {
						enemy.SendMessage ("Damage", (float)Random.Range (attack.minDamage, attack.maxDamage), SendMessageOptions.DontRequireReceiver);
					}
				}
			} else {
				foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Player")) {
					Collider2D collider = enemy.GetComponent<Collider2D> ();
					if (weaponCollider.IsTouching (collider)) {
						enemy.SendMessage ("Damage", (float)Random.Range (attack.minDamage, attack.maxDamage), SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}
}
