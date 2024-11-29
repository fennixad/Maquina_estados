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
    void Start()
    {
        canAttack = false;
        cs = GetComponent<CombatStats>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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
            cs.ApplyDamage(10);
            cs.ShowHealth("Enemy");
        }

        canAttack = false;
    }
}
