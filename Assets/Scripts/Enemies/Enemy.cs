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

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        rend = GetComponent<Renderer>();  // For changing color
        originalColor = rend.material.color;

        isLeaping = false;

        anim = GetComponent<Animator>();
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
                anim.SetBool("isWobbling", true); // Start wobble animation
                anim.SetBool("isPreparing", false);
            }
            else
            {
                anim.SetBool("isWobbling", false); // Stop wobble animation
                anim.SetBool("isPreparing", true);
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
        anim.SetTrigger("doAttack");

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;  // Zero out the y component if you only care about horizontal movement
        float distanceBefore = 1.4f;  // Set your own value for how close "just before" is

        // Normalize the vector so that it has a length of 1, then scale it to your 'distanceBefore'
        Vector3 leapTarget = player.position - toPlayer.normalized * distanceBefore;
        leapTarget.y = transform.position.y;


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
        if (playerMotor != null) // Make sure the playerMotor is not null
        {
            Debug.Log("Applying damage...");
            playerMotor.TakeDamage(10); // The player takes 10 damage
        }

        // Calculate retreat position
        Vector3 retreatPosition = new Vector3(
            transform.position.x - (player.position.x - transform.position.x) * retreatDistance,
            transform.position.y,
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


}
