using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public PlayerMotor playerMotor;
    public int maxHealth = 100;
    private int currentHealth;
    public NavMeshAgent enemy;
    public Transform player;
    public float stoppingDistance = 1.5f;
    public float attackCooldown = 2.0f;
    public float leapDistance = 2.0f;
    public float rotationSpeed = 5.0f;
    public float attackSpeed = 5.0f;
    private float nextAttackTime;
    public float retreatDistance = 0.5f; // Add this variable at the top with your other public variables

    private Vector3 originalPosition;
    private bool isLeaping;
    private Renderer rend;  // For changing color
    private Color originalColor;

    public float headbuttStoppingDistance = 0.1f;  // New variable

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        Rigidbody rb = GetComponent<Rigidbody>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerMotor = playerObject.GetComponent<PlayerMotor>();
        }
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        rend = GetComponent<Renderer>();  // For changing color
        originalColor = rend.material.color;

        isLeaping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLeaping)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance > stoppingDistance)
            {
                enemy.isStopped = false;
                enemy.SetDestination(player.position);
            }
            else
            {
                enemy.isStopped = true;
                originalPosition = transform.position;  // Update original position here

                RotateTowardsPlayer();

                if (Time.time >= nextAttackTime)
                {
                    RotateTowardsPlayer();
                    StartCoroutine(HeadbuttAttack());
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }



    

    IEnumerator HeadbuttAttack()
    {
        isLeaping = true;
        rend.material.color = Color.blue;  // Turn blue during attack

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 leapTarget = new Vector3(
            player.position.x - directionToPlayer.x * headbuttStoppingDistance,
            transform.position.y,  // Use the Y-coordinate of the enemy itself
            player.position.z - directionToPlayer.z * headbuttStoppingDistance
        );

        Vector3 startPos = transform.position;

        enemy.isStopped = true;  // Disable NavMeshAgent temporarily

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPos, leapTarget);
        float fracJourney = 0;

        // Move towards the player
        while (fracJourney < 1)
        {
            float distCovered = (Time.time - startTime) * attackSpeed;
            fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPos, leapTarget, fracJourney);
            yield return null;
        }

        // Make sure you've reached the target to avoid "shaking"
        transform.position = leapTarget;

        // Apply damage logic here
        playerMotor.TakeDamage(10); // The player takes 10 damage

        // Calculate retreat position
        Vector3 retreatPosition = new Vector3(
            transform.position.x - (player.position.x - transform.position.x) * retreatDistance,
            transform.position.y,  // Use the Y-coordinate of the enemy itself
            transform.position.z - (player.position.z - transform.position.z) * retreatDistance
        );

        startTime = Time.time;
        journeyLength = Vector3.Distance(leapTarget, retreatPosition);
        fracJourney = 0;

        // Move back a bit
        while (fracJourney < 1)
        {
            float distCovered = (Time.time - startTime) * attackSpeed;
            fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(leapTarget, retreatPosition, fracJourney);
            yield return null;
        }

        // Make sure you've reached the retreat position
        transform.position = retreatPosition;

        enemy.isStopped = false;  // Enable NavMeshAgent again
        rend.material.color = originalColor;  // Return to original color after attack
        isLeaping = false;
    }







    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.tag == "Player")
    //     {
    //         PlayerMotor playerMotor = collision.gameObject.GetComponent<PlayerMotor>();
    //         if (playerMotor != null)
    //         {
    //             playerMotor.TakeDamage(10f); // The player takes 10 damage
    //             Debug.Log("Player took 10 damage");
    //         }
    //     }
    // }
}
