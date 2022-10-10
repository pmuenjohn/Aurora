using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    public float turnSpeedMultiplier;
    public GameObject projectilePrefab;

    public float fireRate = 200;
    public int bulletsPerShot = 1;
    public float spreadAngle;

    public int magazineSize = 5;
    public int currentMagazineAmount = 5;
    
    public float reloadTime = 2.5f;
    public bool isReloading = false;

    public Transform muzzle;
    public ParticleSystem muzzleFlash;

    private Vector3 lastMuzzlePosition;
    public Vector3 muzzleWorldVelocity;

    private float lastTimeShot;

    void Update()
    {
        if (Time.deltaTime > 0)
        {
            muzzleWorldVelocity = (muzzle.position - lastMuzzlePosition) / Time.deltaTime;
            lastMuzzlePosition = muzzle.position;
        }
    }

    public bool FireWeapon()
    {
        if(isReloading)
            return false;
        if(currentMagazineAmount <= 0)
        {
            StartCoroutine(StartReload());
            return false;
        }
        if(lastTimeShot + 60/fireRate < Time.time)
        {
            currentMagazineAmount -= 1;

            for(int i = 0; i < bulletsPerShot; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(muzzle);
                GameObject newProjectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(shotDirection));
                newProjectile.GetComponent<EnemyProjectile>().InitialiseProjectile(this);
            }

            if(muzzleFlash != null)
            {
                muzzleFlash.Clear();
                muzzleFlash.Play();
            }

            lastTimeShot = Time.time;

            return true;
        }
        return false;
    }

    IEnumerator StartReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentMagazineAmount = magazineSize;
        isReloading = false;
    }

    public Vector3 GetShotDirectionWithinSpread(Transform origin)
    {
        float spreadAngleRatio = spreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(origin.forward, Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }
}
