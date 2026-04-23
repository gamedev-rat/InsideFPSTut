using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Linq;

public class Enemy : MonoBehaviour
{

    public int health = 100;

    public AudioClip shootingSFX;
    public float shootingVol = 0.25f;

    
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject weaponFlash;
    public float bloom; //inaccuracy
    public float fireRate;
    private float lastShotTime = 0f;




    public Material hitMat;



    private Rigidbody rb;
    private Renderer rend;
    private Material originalMaterial;

    private NavMeshAgent agent;
    //AI Settings
    public int currentPointIndex =0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public float idleTime;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth = 30f;

    public Transform[] patrolPoints;
    private float idleTimeCounter;
    private Transform playerTransform;
    private bool canSeePlayer;
    private Vector3 lastKnownPlayerPosition;

    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Idle;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        GameObject patrolPointParent = GameObject.FindWithTag("PatrolPoint");
        patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointParent.transform).ToArray();
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            Debug.Log("damaged");
            health -= 10;

            if(health <= 0){
                Die();
            }
            else
            {
                StartCoroutine(Blink());
            }
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }


    IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = originalMaterial;
    }




    private void Update()
    {
        LookForPlayer();


        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrolling();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Chasing:
                Chasing();
                break;
        }

        rb.linearVelocity = Vector3.zero;

        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }

    private void LookForPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        if(Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform;

            if(canSeePlayer && state != State.Attacking)
            {
                state = State.Chasing;
            }
        }
    }

    private void Idle()
    {
        agent.ResetPath(); // has to stand still

        idleTimeCounter -= Time.deltaTime;

        if(idleTimeCounter < 0)
        {
            state = State.Patrolling;
            idleTimeCounter = idleTime;
        }
    }
    private void Patrolling()
    {
        if(Vector3.Distance(currentTarget, transform.position) < positionThreshold) //are we close to our target?
        {
            float chance = Random.Range(0,100);

            if(chance < 10) //10% chance of idling at this point before moving on to next target
            {
                state = State.Idle;
                return;
            }

            currentPointIndex++;
            currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;

        }
        else
        {
            agent.SetDestination(currentTarget);
        }
    }

    private void Attacking()
    {
        idleTimeCounter = idleTime;
        agent.ResetPath(); //standing still

        Shoot();

        if(Vector3.Distance(transform.position, playerTransform.position) > attackDistance || !canSeePlayer) //if player is too far away or cant see anymore
        {
            if(health < minChasingHealth)
            {
                state = State.Patrolling; //cautious
            }
            else
            {
                state = State.Chasing;
            }
        
        }
    }

    private void Chasing()
    {
        idleTimeCounter = idleTime;
        agent.SetDestination(lastKnownPlayerPosition);

        if(health < minChasingHealth){
            state = State.Patrolling;
        }
        else if(Vector3.Distance(transform.position, playerTransform.position) <= attackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        else if(Vector3.Distance(transform.position, playerTransform.position)  > maxVisionDistance)
        {
            state = State.Patrolling;
        }
        else if(Vector3.Distance(transform.position, playerTransform.position)  < positionThreshold && !canSeePlayer)
        {
            state = State.Patrolling;
        }
    }

    private void LookAtPlayer()
    {
        if(canSeePlayer)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
    }
    private void SetLastKnownPlayerPosition()
    {
        if( canSeePlayer)
        {
            lastKnownPlayerPosition = playerTransform.position;
        }
    }



    private void Shoot()
    {
        if(Time.time > lastShotTime + fireRate)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.Normalize();

            Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

            float maxInaccuracy = 10f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomYaw = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomYaw + 90, 0f);


            AudioManager.Instance.PlaySFX(shootingSFX,shootingVol);

            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
            Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            lastShotTime = Time.time;
        }
    }

}
