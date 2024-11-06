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
    private float speed;
    private float speedRotation;
    private float timeBeetweenAttacks;
    private float stopDistance;
    void Start()
    {
        timeBeetweenAttacks = 0f;
        stopDistance = 2.0f;

        anim = GetComponent<Animator>();

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
        /*
        if (Vector3.Distance(transform.position, target.position) <= 15f && Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            actualState = 1;
        }
        else if (Vector3.Distance(transform.position, target.position) <= stopDistance)
        {
            actualState = 2; // Cambia a estado de ataque
        }
        else
        {
            actualState = 0;
        }

        StateManager();
        */

        anim.SetTrigger("IdleAttack");
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
        }
    }

    void DoPatrol()
    {
        speed = 1;
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

    void DoAggro()
    {
        if (Vector3.Distance(transform.position, target.position) > stopDistance && (Vector3.Distance(transform.position, target.position) <= 15f))
        {
            speed = 2;
            anim.SetBool("Walking", false);
            anim.SetBool("Running", true);
            anim.SetBool("Idle", false);
            moveEnemyToPosition(target.position);
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
        //anim.SetBool("Walking", false);
        //anim.SetBool("Idle", false);
        anim.SetTrigger("IdleAttack");
        //TimeBeetweenAttacks();
    }

    void TimeBeetweenAttacks()
    {
        timeBeetweenAttacks += Time.deltaTime;

        if (timeBeetweenAttacks >= 2.0f)
        {
            anim.SetBool("Attack", true);
            timeBeetweenAttacks = 0f;
        } else
        {
            anim.SetBool("Attack", false);
        }
    }
}
