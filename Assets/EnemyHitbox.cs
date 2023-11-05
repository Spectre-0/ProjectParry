using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 20;  // Set the amount of damage this attack will deal

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hitbox collided with an enemy
        if (other.gameObject.CompareTag("Player"))
        {
            // Fetch the Enemy script attached to the GameObject
            PlayerMotor player = other.gameObject.GetComponent<PlayerMotor>();

            // Apply damage to the enemy
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // Optionally, destroy the hitbox after it hits an enemy
            Destroy(gameObject);
        }
    }
}