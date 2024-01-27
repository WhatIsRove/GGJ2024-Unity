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
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float maxHealth;
    float currentHealth;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) FollowPlayer();
        if (playerInAttackRange) AttackPlayer();
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
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Destroy(gameObject);
        }
    }
}
