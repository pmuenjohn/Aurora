using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("HP Stats")]
    public float maxhp = 1f;
    public float hp;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxhp;
    }

    public void TakeDamage(float amount, PlayerController instigator)
    {
        if(instigator)
        {
            instigator.StartCoroutine("PlayHitIndicator");
        }
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
