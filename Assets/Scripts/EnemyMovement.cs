using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private List<Vector3> patrolPoints = new List<Vector3>();
    private Transform target;
    private Animator anim;
    private int actualPoint;
    private int actualState;
    public float speed;
    private float speedRotation;
    private float timeBeetweenAttacks;
    private float stopDistance;
    public bool patrolDirection;
    private bool isStopping;
    private float stopTimer;
    public CombatStats combatStats;

    float counterTime;
    float timeAttack;
    float counterFight;
    void Start()
    {
        stopTimer = 0;
        isStopping = false;
        patrolDirection = true;
        timeBeetweenAttacks = 2f;
        stopDistance = 2.0f;
        combatStats = GetComponent<CombatStats>();
        anim = GetComponent<Animator>();
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == "Attack")
            {
                timeAttack = ac.animationClips[i].length;
                i = ac.animationClips.Length;
            }
        }

        if (target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        actualPoint = 0;
        actualState = 0;
        speed = 1;
        speedRotation = 10f;

        patrolPoints.Add(new Vector3(30f, 0, 30f));
        patrolPoints.Add(new Vector3(-30f, 0, 30f));
        patrolPoints.Add(new Vector3(-30f, 0, -30f));
        patrolPoints.Add(new Vector3(30f, 0, -30f));
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= 15f && Vector3.Distance(transform.position, target.transform.position) >= stopDistance)
        {
            actualState = 1;
        }
        else if (Vector3.Distance(transform.position, target.transform.position) <= stopDistance)
        {
            actualState = 2; 
        }
        else if (isStopping)
        {
            actualState = 4;
        }
        else if (!isStopping) 
        {
            actualState = 0;
        }

        StateManager();
        PatrolDirection(patrolDirection);
    }

    void PatrolDirection (bool direction)
    {
        patrolPoints.Clear();

        if (direction)
        {
            patrolPoints.Add(new Vector3(30f, 0, 30f));
            patrolPoints.Add(new Vector3(-30f, 0, 30f));
            patrolPoints.Add(new Vector3(-30f, 0, -30f));
            patrolPoints.Add(new Vector3(30f, 0, -30f));
        }
        else
        {
            patrolPoints.Add(new Vector3(30f, 0, -30f));
            patrolPoints.Add(new Vector3(-30f, 0, -30f));
            patrolPoints.Add(new Vector3(-30f, 0, 30f));
            patrolPoints.Add(new Vector3(30f, 0, 30f));
        }

    }

    void StateManager()
    {
        switch(actualState)
        {
            case 0:
                DoPatrol();
                break;
            case 1:
                DoAggro();
                break;
            case 2:
                DoAttack();
                break;
            case 3:
                //death
                break;
            case 4:
                Debug.Log("Entro en case 4");
                StopMoving();
                break;
        }
    }

    void StopMoving()
    {
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("Idle", true);

        if (stopTimer > 2f)
        {
            isStopping = false;
        }
        stopTimer += Time.deltaTime;
    }
    void DoPatrol()
    {
        if (!isStopping)
        {
            speed = 1;
            anim.SetBool("IdleAttack", false);
            anim.SetBool("Walking", true);
            anim.SetBool("Running", false);
            anim.SetBool("Idle", false);

            moveEnemyToPosition(patrolPoints[actualPoint]);

            if (Vector3.Distance(transform.position, patrolPoints[actualPoint]) < 1)
            {
                if (actualPoint == patrolPoints.Count - 1)
                {
                    actualPoint = 0;
                }
                else
                {
                    actualPoint++;
                }
            }
        }

    }

    void DoAggro()
    {
        if (Vector3.Distance(transform.position, target.position) > stopDistance && (Vector3.Distance(transform.position, target.position) <= 15f))
        {
            speed = 2;
            anim.SetBool("IdleAttack", false);
            anim.SetBool("Walking", false);
            anim.SetBool("Running", true);
            anim.SetBool("Idle", false);

            moveEnemyToPosition(target.position);
            isStopping = true;
        } 
    }

    void moveEnemyToPosition(Vector3 position)
    {
        Vector3 direccion = position - transform.position;
        direccion = new Vector3 (direccion.x, 0, direccion.z);
        Quaternion rot = Quaternion.LookRotation(direccion, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speedRotation * Time.deltaTime);
        transform.Translate (speed * Time.deltaTime * transform.worldToLocalMatrix.MultiplyVector(transform.forward));
    }

    void DoAttack()
    {
        /*
        anim.SetBool("Walking", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Running", false);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("IdleAttack"))
        {
            anim.SetTrigger("IdleAttack");
        }

        TimeBeetweenAttacks();
        
        */
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("IdleAttack", true);
        Vector3 direccion = target.transform.position - transform.position; 
        direccion = new Vector3(direccion.x, 0, direccion.z); Quaternion rot = Quaternion.LookRotation(direccion, transform.up); 
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speedRotation / 4 * Time.deltaTime); 
        counterFight = counterFight + Time.deltaTime; 
        if (counterFight > timeAttack + 0.5f) { 
            counterFight = 0;
            print("Lanzo rayo");

            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.TransformDirection(Vector3.forward) * 10, out RaycastHit hit, 2.5f)) { 
                print("Te detecto"); 
                anim.SetTrigger("Attack");
                combatStats.ApplyDamage(10);
            }
        }
    /*
    timeBeetweenAttacks = timeBeetweenAttacks - Time.deltaTime;
    if (timeBeetweenAttacks <= 0)
    {
        timeBeetweenAttacks =  ;
    }
    */
}

    void TimeBeetweenAttacks()
    {
        timeBeetweenAttacks -= Time.deltaTime;

        if (timeBeetweenAttacks <= 0f)
        {
            Debug.Log("Attack");
            anim.SetBool("Attack", true);
            combatStats.ApplyDamage(10);
            timeBeetweenAttacks = 2f;
        }
    }
}
