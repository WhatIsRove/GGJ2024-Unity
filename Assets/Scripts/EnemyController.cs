using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask ground, playerLayer;

    public Vector3 walkDestin;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    public float timeToHit;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float maxHealth;
    public float currentHealth;

    public GameObject leftHP;
    public GameObject rightHP;

    Animator anim;

    public Sprite happy;
    public Sprite happySprite;
    public SpriteRenderer indicator;

    bool dead = false;

    public GameObject hand;

    public bool isRanged;
    public GameObject bulletPrefab;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        rightHP.SetActive(false);
        leftHP.transform.localScale = new Vector3(1, 1, 1);
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange && !dead) Patrolling();
        if (playerInSightRange && !playerInAttackRange && !dead) FollowPlayer();
        if (playerInAttackRange && !dead) AttackPlayer();

        if (currentHealth != maxHealth) rightHP.SetActive(true);
        if (currentHealth <= 0) leftHP.SetActive(false);
    }

    void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkDestin);
        }

        Vector3 distanceToDestin = transform.position - walkDestin;

        if (distanceToDestin.magnitude < 1f) walkPointSet = false;


    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkDestin = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkDestin, -transform.up, 2f, ground))
        {
            walkPointSet = true;
        }
    }

    void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            if (!isRanged)
            {
                anim.SetTrigger("Attack");
                Invoke(nameof(CheckPlayerDistance), timeToHit);
            } else
            {
                var aimDir = (player.position - hand.transform.position).normalized;
                Instantiate(bulletPrefab, hand.transform.position, Quaternion.LookRotation(aimDir));
            }
            
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void CheckPlayerDistance()
    {
        if (Vector3.Distance(transform.position, player.position) < attackRange+1)
        {
            FindObjectOfType<PlayerController>().TakeDamage();
            FindObjectOfType<AudioManager>().Play("Honk");
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth > 0)
        {
            if (currentHealth - amount < 0)
            {
                currentHealth = 0;
            } else currentHealth -= amount;
        }
            
        leftHP.transform.localScale = new Vector3(currentHealth/maxHealth, 1, 1);
        rightHP.transform.localScale = new Vector3(1-currentHealth/maxHealth, 1, 1);
        if (currentHealth <= 0) {
            currentHealth = 0;
            indicator.sprite = happy;
            GetComponent<SpriteRenderer>().sprite = happySprite;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

            dead = true;
            Destroy(hand);
            //Destroy(gameObject);
        }
    }
}
