using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 20;

    // Declare a delegate and an event
    public delegate void PlayerHitHandler();
    public event PlayerHitHandler OnPlayerHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMotor player = other.gameObject.GetComponent<PlayerMotor>();
            if (player != null)
            {
                player.TakeDamage(damage);

                // Trigger the event when the player is hit
                OnPlayerHit?.Invoke();
            }
        }
    }
}
