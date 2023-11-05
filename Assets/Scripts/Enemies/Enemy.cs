using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public PlayerMotor playerMotor;
    public int maxHealth = 100;
    private int currentHealth;
    private float healthLerpSpeed = 0.05f;
    public int EnemeyLoot = 100;

    public Slider healthSlider;
    public Slider easeHealthSlider;

    public Transform hitboxPos;
    public NavMeshAgent enemy;
    public Transform player;
    public float stoppingDistance = 1.5f;
    public float attackCooldown = 2.0f;
    public float leapDistance = 2.0f;
    public float rotationSpeed = 5.0f;
    public float attackSpeed = 5.0f;
    private float nextAttackTime;
    public float retreatDistance = 0.5f; // Add this variable at the top with your other public variables

    public GameObject attackHitboxPrefab;
    public float attackDistance = 1.0f;

    private Vector3 originalPosition;
    private bool isLeaping;
    private Renderer[] renderers;  // For changing color
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
    private Color originalColor;

    private Animator anim;

    public float fovAngle = 110f;
    private bool playerInSight;
    public float maxDetectionDistance = 10f;

    private bool death = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        renderers = GetComponentsInChildren<Renderer>();  // For changing color
        foreach (Renderer r in renderers)
        {
            originalColors[r] = r.material.color;
        }
        isLeaping = false;

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        if(healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if(healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, healthLerpSpeed);
        }
        CheckForPlayerInFOV();
        if (playerInSight && !death)
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
    }



    void CheckForPlayerInFOV()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleBetweenEnemyAndPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check for player in FOV
        bool isInFOV = angleBetweenEnemyAndPlayer < fovAngle * 0.5f && distanceToPlayer <= maxDetectionDistance;

        // Proximity check
        bool isTooClose = distanceToPlayer <= 10.0f;
        Debug.Log("Checking for player...");
        if (isInFOV || isTooClose)
        {
            RaycastHit hit;
            float raycastDistance = isInFOV ? maxDetectionDistance : 10.0f; // Use the appropriate distance based on the condition that was met

            Debug.Log("Player is in FOV or too close.");
            

            if (Physics.Raycast(transform.position + transform.up, directionToPlayer.normalized, out hit, raycastDistance))
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                Debug.DrawLine(transform.position + transform.up, hit.point, Color.green, 2f);
                if (hit.collider.gameObject == player.gameObject)
                {

                    playerInSight = true;
                    return; // Exit early since the player is detected
                }
            }
        }
        // If the code reaches here, the player is not in sight
        playerInSight = false;
    }


    IEnumerator HeadbuttAttack()
    {
        if (!death) { 
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

            // Create a hitbox in front of the player
            Vector3 attackPosition = hitboxPos.position;
            GameObject hitbox = Instantiate(attackHitboxPrefab, attackPosition, transform.rotation);


            // Destroy the hitbox after a short time (e.g., 0.5 seconds)
            Destroy(hitbox, 0.2f);

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
            death = true;
            StartCoroutine(ShowHit());
            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("Die");

            float animationLength = anim.runtimeAnimatorController.animationClips[3].length;
            Invoke("DestroyObject", animationLength);
        }
        StartCoroutine(ShowHit());
    }

    void DestroyObject()
    {
<<<<<<< HEAD
        death = true;
        
        anim.SetTrigger("Die");
        StartCoroutine(ShowHit(true));
        float animationLength = anim.runtimeAnimatorController.animationClips[2].length;
        Invoke("DestroyObject", animationLength);
    }

    private void DestroyObject()
    {
        slugHitbox.OnPlayerHit -= HandlePlayerHit;
        GetEnemyLoot();
        Destroy(gameObject);
    }
    public void GetEnemyLoot()
    {
        player.GetComponent<PlayerMotor>().AddMoney(EnemeyLoot);
    }
    private IEnumerator ShowHit(bool death)
=======
        Destroy(gameObject);
    }


    IEnumerator ShowHit()
>>>>>>> parent of 9ceb031 (Slug DONE)
    {
        foreach (Renderer r in renderers)
        {
            r.material.color = Color.red;
        }
        if (!death)
        {
            yield return new WaitForSeconds(0.1f);

            // Revert back to original colors
            foreach (Renderer r in renderers)
            {
                if (originalColors.ContainsKey(r))
                {
                    r.material.color = originalColors[r];
                }
            }
        }

    }
}
