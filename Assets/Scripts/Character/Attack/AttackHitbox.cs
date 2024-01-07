using System.Collections;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 20; // Set the amount of damage this attack will deal
    public float hitCooldown = 0.5f; // Cooldown in seconds before the hitbox can hit again


    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the hitbox is on cooldown, don't check for hits

        if (other.gameObject.CompareTag("Enemy"))
        {
            // Fetch the Enemy script attached to the GameObject
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            // Apply damage to the enemy
            if (enemy != null)
            {
                Debug.Log("Enemy hit");
                audioManager.PlaySFX(audioManager.playerAttackAudio);
                enemy.TakeDamage(damage);
                StartCoroutine(HitCooldown()); // Start the hit cooldown
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            // Fetch the Troll_script script attached to the GameObject
            Troll_script enemy = other.gameObject.GetComponent<Troll_script>();

            // Apply damage to the troll
            if (enemy != null)
            {
                Debug.Log("Troll hit");
                audioManager.PlaySFX(audioManager.playerAttackAudio);
                enemy.TakeDamage(damage);
                StartCoroutine(HitCooldown()); // Start the hit cooldown
            }
        }
    }

    private IEnumerator HitCooldown()
    {
        
        yield return new WaitForSeconds(hitCooldown); // Wait for the cooldown duration
        
    }
}