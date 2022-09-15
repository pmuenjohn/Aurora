using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxhp = 1f;
    public float hp;
    public float speed = 5f;
    public float damage = 1f;
    public float fireRate = 1f;
    public float spread = 0f;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxhp;
    }

    private void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        Debug.Log(hp);
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
