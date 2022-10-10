using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    public float turnSpeedMultiplier;
    public GameObject projectilePrefab;

    public float fireRate = 200;
    public int bulletsPerShot = 1;
    public float spreadMultiplier = 0;

    public int magainzeSize = 5;
    public int currentMagazineAmount = 5;
    
    public float reloadTime = 2.5f;

}
