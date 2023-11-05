using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent enemy;
    public Transform player;
    private Animator anim;
    private Renderer[] renderers;
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    [Header("Health Parameters")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider easeHealthSlider;
    private float healthLerpSpeed = 0.05f;

    [Header("Attack Parameters")]
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float attackSpeed = 5.0f;
    private float nextAttackTime;
    private bool isLeaping;
    [SerializeField] private GameObject attackHitboxPrefab;
    [SerializeField] private Transform hitboxPos;
    private Vector3 originalPosition;
    private Vector3 playerPosition;
    private bool didHitPlayer = false;
    private EnemyHitbox slugHitbox;
    private Collider hitboxCollider;
    private bool firstAttackDone = false;

    [Header("Detection Parameters")]
    [SerializeField] private float fovAngle = 110f;
    private bool playerInSight;
    [SerializeField] private float maxDetectionDistance = 10f;

    private bool death = false;

    private void Start()
    {
        currentHealth = maxHealth;
        renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            originalColors[r] = r.material.color;
        }
        enemy = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        slugHitbox = GetComponentInChildren<EnemyHitbox>();
        hitboxCollider = slugHitbox.GetComponent<Collider>();
        // Subscribe to the event
        slugHitbox.OnPlayerHit += HandlePlayerHit;

        hitboxCollider.enabled = false;
        isLeaping = false;
    }

    private void Update()
    {
        UpdateHealthUI();
        CheckForPlayerInFOV();
        if (anim.GetBool("isWobbling") == false)
        {
            anim.SetBool("idleWobble", true);
        }
        if (playerInSight && !death && !isLeaping)
        {
            HandleSlugBehavior();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, healthLerpSpeed);
        }
    }

    private void CheckForPlayerInFOV()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = Vector3.Distance(transform.position, player.position);

        if ((angle < fovAngle * 0.5f && distance <= maxDetectionDistance) || distance <= 10.0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, directionToPlayer.normalized, out hit, maxDetectionDistance) &&
                hit.collider.gameObject == player.gameObject)
            {
                playerInSight = true;
                return;
            }
        }
        firstAttackDone = false;
        playerInSight = false;
    }


    private void HandleSlugBehavior()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        RotateTowardsPlayer();
        if (distance > stoppingDistance && playerInSight)
        {

            enemy.isStopped = false;
            enemy.SetDestination(player.position);
            Debug.Log("Moving");
            anim.SetBool("isWobbling", true);
            anim.SetBool("idleWobble", false);
        }
        else
        {
            anim.SetBool("isWobbling", false);
            enemy.isStopped = true;
            originalPosition = transform.position;
            if (!firstAttackDone)
            {
                Debug.Log("First Attack");
                StartCoroutine(FirstAttackDelay());
            }
            if (!isLeaping && firstAttackDone)
            {
                if (Time.time >= nextAttackTime)
                {
                   
                    StartCoroutine(HeadbuttAttack());
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        anim.SetBool("isWobbling", true);
    }

    IEnumerator FirstAttackDelay()
    {
        yield return new WaitForSeconds(0.75f);
        firstAttackDone = true;
    }

    IEnumerator HeadbuttAttack()
{
        if (!death)
        {
            isLeaping = true;

            float prepareDistance = 2.0f;

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            Vector3 retreatDirection = -transform.forward;
            retreatDirection.y = 0;
            retreatDirection.Normalize();
            float adjustedPrepareDistance = CheckRetreatDistance(retreatDirection, prepareDistance);
            Vector3 retreatPosition = transform.position + retreatDirection * adjustedPrepareDistance;


            float retreatSpeed = 0.05f;
            float fracJourney = 0;
            float threshold = 0.1f;
            while (Vector3.Distance(transform.position, retreatPosition) > threshold)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 25.0f);

                transform.position = Vector3.Lerp(transform.position, retreatPosition, fracJourney);

                fracJourney += retreatSpeed * Time.deltaTime;  
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);


            hitboxCollider.enabled = true;


            Vector3 leapTarget = transform.position + transform.forward * (stoppingDistance + prepareDistance);

            Vector3 startPos = transform.position;
            float startTime = Time.time;
            float journeyLength = stoppingDistance;
            fracJourney = 0;
            anim.SetTrigger("doAttack");
            // Leap towards the player
            while (fracJourney < 1)
            {

                if (didHitPlayer)
                {
                    hitboxCollider.enabled = false;
                    break;
                }

                float distCovered = (Time.time - startTime) * attackSpeed;
                fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPos, leapTarget, fracJourney);
                yield return null;
            }
            anim.SetTrigger("StopAttack");
            if (didHitPlayer)
            {
                didHitPlayer = false;
            }


            enemy.Warp(transform.position); 
            enemy.isStopped = false;
            hitboxCollider.enabled = false;
            isLeaping = false;
            anim.SetBool("isWobbling", true);
        }
}

    float CheckRetreatDistance(Vector3 retreatDirection, float originalDistance)
    {
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Enemy");
        if (Physics.Raycast(transform.position, retreatDirection, out hit, originalDistance+3.0f, layerMask))
        {
            return hit.distance - 3.0f;  
        }
        return originalDistance; 
    }


    private void HandlePlayerHit()
    {
        didHitPlayer = true;
    }


    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        } else
        {
            StartCoroutine(ShowHit(false));
        }
        

    }

    private void Die()
    {
        death = true;
        
        anim.SetTrigger("Die");
        StartCoroutine(ShowHit(true));
        float animationLength = anim.runtimeAnimatorController.animationClips[2].length;
        Invoke("DestroyObject", animationLength);
    }

    private void DestroyObject()
    {
        slugHitbox.OnPlayerHit -= HandlePlayerHit;
        Destroy(gameObject);
    }

    private IEnumerator ShowHit(bool death)
    {
        foreach (Renderer r in renderers)
        {
            r.material.color = Color.red;
        }
        if (!death)
        {
            yield return new WaitForSeconds(0.1f);
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
