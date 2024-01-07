using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class Troll_script : MonoBehaviour
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
    [SerializeField] private float viewDistance = 10.0f;
    private float nextAttackTime;
    private bool isAttacking;
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

    public int enemyValue = 100000;


    


    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }


    

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
        if (playerInSight && !death && !isAttacking)
        {
            HandleTrollBehavior();
        }
    }
    private void UpdateHealthUI()
    {
        // Calculate the percentage of health remaining
        float healthPercent = (float)currentHealth / maxHealth;

        // Update the health slider to reflect current health
        if (healthSlider.value != healthPercent)
        {
            Debug.Log("Health Slider: " + healthPercent);

            healthSlider.value = healthPercent;
        }

        // Smoothly transition the ease health slider to match the health slider
        if (Mathf.Abs(easeHealthSlider.value - healthPercent) > Mathf.Epsilon)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, healthLerpSpeed);
        }
    }
    private void CheckForPlayerInFOV()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = Vector3.Distance(transform.position, player.position);

        if ((angle < fovAngle * 0.5f && distance <= maxDetectionDistance) || distance <= viewDistance)
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

    private void HandleTrollBehavior()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        RotateTowardsPlayer();
        
        // If the enemy is not within the stopping distance and not attacking, pursue the player
        if (distance > stoppingDistance && !isAttacking)
        {
            enemy.isStopped = false;
            enemy.SetDestination(player.position);
        }
        else
        {
            // If within stopping distance, stop and prepare to attack
            enemy.isStopped = true;

            if (!isAttacking && Time.time >= nextAttackTime)
            {
                StartCoroutine(ChooseAttack());
                nextAttackTime = Time.time + attackCooldown;
            }
        }

        // Set the walking animation based on the enemy's isStopped status
        // The walk animation will play when the enemy is moving, and stop when the enemy stops
        anim.SetBool("walk", !enemy.isStopped);
    }

    [Header("Attack Parameters")]

    public float attack1HitStart = 0.2f;
    public float attack1HitLastFor = 0.01f;

    public float attack2HitStart = 0.5f;
    public float attack2HitLastFor= 0.01f;

IEnumerator ChooseAttack()
{
    isAttacking = true;
    
    // Choose which attack animation to trigger
    bool isAttack1 = Random.Range(0, 2) == 0;
    string attackTrigger = isAttack1 ? "attack1" : "attack2";
    anim.SetTrigger(attackTrigger);
    Debug.Log("Attack Triggered");

    // Wait for the attack animation to almost finish to enable the hitbox
    float attackAnimationTime = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    
    // If it's attack1, we might want to spawn the hitbox later than if it's attack2
    float hitboxSpawnTime = 0f;
    float hitboxActiveTime = 0f;

    if (isAttack1)
    {
        hitboxSpawnTime = attackAnimationTime * attack1HitStart;
        hitboxActiveTime = attackAnimationTime * attack1HitLastFor;
    }
    else
    {
        hitboxSpawnTime = attackAnimationTime * attack2HitStart;
        hitboxActiveTime = attackAnimationTime * attack2HitLastFor;
    }

    yield return new WaitForSeconds(hitboxSpawnTime);

    // Enable the hitbox collider to detect hits
    hitboxCollider.enabled = true;
    audioManager.PlaySFX(audioManager.mobAttackAudio);

    // Wait for a brief moment while the hitbox is active (adjust this time as necessary)
    yield return new WaitForSeconds(hitboxActiveTime);
    

    // Disable the hitbox collider after the attack
    hitboxCollider.enabled = false;

    isAttacking = false;
    nextAttackTime = Time.time + attackCooldown; // Ensure the next attack has a delay based on the cooldown
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
        if (death) return; // Ignore damage if already dead.

        currentHealth -= damage;
        if (currentHealth <= 0 && !death)
        {
            Die();
        }
        else
        {
            
            StartCoroutine(ShowHit(false));
        }
    }

    private void Die()
    {
        death = true;
        enemy.isStopped = true; // Stop the NavMeshAgent from moving.
        anim.SetTrigger("death");
        StartCoroutine(ShowHit(true));
        float animationLength = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Invoke("DestroyObject", animationLength);
    }

    private void DestroyObject()
    {
        slugHitbox.OnPlayerHit -= HandlePlayerHit;
        player.GetComponent<PlayerMotor>().AddMoney(enemyValue);
        Destroy(gameObject);
    }

    private IEnumerator ShowHit(bool death)
    {
        foreach (Renderer r in renderers)
        {
            r.material.color = Color.red;
            anim.SetTrigger("take_damage");
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
