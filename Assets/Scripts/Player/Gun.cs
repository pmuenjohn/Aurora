using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun stats - modifiable")]
    public float damage;
    public float timeBetweenShots;
    public float bulletsPerHit = 1;
    public float spreadMultiplier = 0;
    public int magazineSize;
    public int magazinesLeft; //negative value means infinite magazine
    public int ammoLeft; //negative value means infinite ammo
    public float reloadTime;
    public float reloadSpeed;
    public bool automaticShooting;

    [Header("Gun status - for monitoring")]
    public bool canShoot, reloading;

    [Header("References - not nullable")]
    public Camera cam;
    public Transform shootingPos;
    public RaycastHit raycastHit;
    public LayerMask hittableLayers;

    private void OnEnable()
    {
        canShoot = true;
    }

    public void Shoot()
    {
        //if cooldown between shots finished
        if (canShoot)
        {
            //if out of ammo and is trying to shoot, reload
            if (ammoLeft == 0)
            {
                StartReload();
            }
            else
            {
                Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

                //raycasting to shoot
                if (Physics.Raycast(rayOrigin, cam.transform.forward, out raycastHit, Mathf.Infinity, (int)hittableLayers))
                {
                    //TODO: insert shoot mechanic
                    Debug.Log(raycastHit);
                }

                ammoLeft--;
                canShoot = false;
                StartCoroutine(ShootingCooldownCoroutine(timeBetweenShots));
            }
        }
    }

    private IEnumerator ShootingCooldownCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        EndShootCooldown();
    }

    private void EndShootCooldown()
    {
        canShoot = true;
    }

    public void StartReload()
    {
        if (!reloading)
        {
            reloading = true;
            //FinishReload after a certain amount of time has passed
            StartCoroutine(ReloadingCoroutine(reloadTime));
        }
    }
    private IEnumerator ReloadingCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        FinishReload();
    }

    public void InterruptReload()
    {
        reloading = false;
    }


    private void FinishReload()
    {
        if (reloading)
        {
            if (magazinesLeft != 0)
            {
                ammoLeft = magazineSize;
                magazinesLeft--;
            }
            reloading = false;
        }
    }
}
