using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void ApplyDamage(float damage);
    float GetHealth();
    void ShowHealth();
}
