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
    private float stopDistance;
    public bool patrolDirection;
    private bool isStopping;
    public CombatStats combatStats;
    public string playerEntity;
    public string enemyEntity;
    private bool entityIsAlive;
    private bool doDamage;
    float counterTime;
    float timeAttack;
    float counterFight;
    void Start()
    {
        playerEntity = "Player";
        enemyEntity = "Enemy";

        timeAttack = 2;
        doDamage = false;
        isStopping = false;
        patrolDirection = true;
        stopDistance = 2.0f;

        GameObject enemy = GameObject.FindWithTag("Enemy");
        if (enemy != null)
        {
            combatStats = enemy.GetComponent<CombatStats>();
        }

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
        /*
        entityIsAlive = combatStats.IsEntityAlive("Enemy");

        if (Vector3.Distance(transform.position, target.transform.position) <= 15f && Vector3.Distance(transform.position, target.transform.position) >= stopDistance)
        {
            actualState = 1;
        }
        else if (!entityIsAlive)
        {
            Debug.Log(enemyEntity);
            actualState = 3;
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
        */
        if (combatStats.GetHealth() <= 0) // Aquí verificamos el CombatStats del enemigo
        {
            actualState = 3; // Cambia al estado de muerte
        }
        else if (Vector3.Distance(transform.position, target.transform.position) <= 15f && Vector3.Distance(transform.position, target.transform.position) >= stopDistance)
        {
            actualState = 1; // Cambia al estado de "agro"
        }
        else if (Vector3.Distance(transform.position, target.transform.position) <= stopDistance)
        {
            actualState = 2; // Cambia al estado de "ataque"
        }
        else if (isStopping)
        {
            actualState = 4; // Cambia al estado de "parada"
        }
        else
        {
            actualState = 0; // Cambia al estado de "patrullaje"
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
                Debug.Log("Entro en case3");
                DoDie();
                break;
        }
    }

    void DoDie()
    {
        anim.SetBool("IdleAttack", false);
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("Idle", false);
        anim.SetTrigger("Death");
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

            isStopping = false;
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
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("IdleAttack", true);
        Vector3 direccion = target.transform.position - transform.position;
        direccion = new Vector3(direccion.x, 0, direccion.z); Quaternion rot = Quaternion.LookRotation(direccion, transform.up); 
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speedRotation / 4 * Time.deltaTime); 

        counterFight -= Time.deltaTime;

        if (counterFight <= 0)
        {
            counterFight = timeAttack;
            doDamage = false;
        }

        if(counterFight > timeAttack -0.1f && !doDamage)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.TransformDirection(Vector3.forward) * 10, out RaycastHit hit, 2.5f))
            {
                anim.SetBool("IdleAttack", true);
                anim.SetTrigger("Attack");
                doDamage = true;

            }
        }
        if (counterFight < timeAttack / 2 && doDamage)
        {
            combatStats.ApplyDamage(10);
            combatStats.ShowHealth(playerEntity);
            doDamage = false;
        }
    }
}
