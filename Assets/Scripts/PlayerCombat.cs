using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public CombatStats cs;
    public Animator anim;
    private bool canAttack;
    private Transform targetEnemy;
    private bool isAlive;
    private int atkBonus;
    private float time;
    private bool powerDmg;
    void Start()
    {
        powerDmg = false;
        time = 0f;
        canAttack = false;
        cs = GetComponent<CombatStats>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DeathEnemy();
        SpecialAttack();
        CompareDistance();
        if (canAttack && Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        isAlive = cs.IsEntityAlive("Enemy");

        if (!isAlive )
        {
            
        }
    }

    void CompareDistance()
    {
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.TransformDirection(Vector3.forward), out RaycastHit hit, 2.0f, layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                canAttack = true;
                targetEnemy = hit.collider.transform;
                return; 
            }
        }

        canAttack = false;
        targetEnemy = null;
    }

    void Attack()
    {
        anim.SetTrigger("Attack");

        if (cs != null)
        {
            int numeroDmg = Random.Range(0, 100);
            if(numeroDmg < 10)
            {
                cs.ApplyDamage(0);
                cs.ShowHealth("Enemy");
            } else
            {
                int numeroCrit = Random.Range(0, 100);
                if (numeroCrit < 20)
                {
                    cs.ApplyDamage(20 + atkBonus);
                    cs.ShowHealth("Enemy");
                } else
                {
                    cs.ApplyDamage(10 + atkBonus);
                    cs.ShowHealth("Enemy");
                }

            }
        }

        canAttack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        atkBonus = 10;
        Destroy(other.gameObject);
        StartCoroutine(RemoveBuffAfterDelay(10f));
    }

    private IEnumerator RemoveBuffAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        atkBonus = 0;
    }

    private void SpecialAttack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(SpecialPoisonAttack(5f));
        }
    }

    private IEnumerator SpecialPoisonAttack(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            cs.ApplyDamage(1f);
            cs.ShowHealth("Enemy");
            elapsedTime += 1f;
            yield return new WaitForSeconds(duration);
        }

    }
    private IEnumerator DeathEnemyCounter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(GameObject.FindGameObjectWithTag("Enemy"));
    }

    private void DeathEnemy()
    {
        if (!cs.IsEntityAlive("Enemy"))
        {
            StartCoroutine(DeathEnemyCounter(3f));
        }
    }
}
