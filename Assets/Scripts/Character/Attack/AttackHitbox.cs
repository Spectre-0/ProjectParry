using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 20;  // Set the amount of damage this attack will deal

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hitbox collided with an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Fetch the Enemy script attached to the GameObject
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            // Apply damage to the enemy
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Optionally, destroy the hitbox after it hits an enemy
            Destroy(gameObject);
        }
    }
}