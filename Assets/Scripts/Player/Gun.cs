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
    public float tracerSpeed = 2f;

    [Header("Gun status - for monitoring")]
    public bool canShoot, reloading;

    [Header("References - not nullable")]
    public Camera cam;
    public Transform shootingPos;
    public TrailRenderer bulletTrail;
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
                //play empty mag sound effect
                StartReload();
            }
            else
            {
                Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                Vector3 hitPos;

                //raycasting to shoot
                if (Physics.Raycast(rayOrigin, cam.transform.forward, out raycastHit, Mathf.Infinity, (int)hittableLayers))
                {
                    hitPos = raycastHit.point;
                    GameObject hitObject = raycastHit.collider.gameObject;
                    if (hitObject.layer == (int)Layer.Enemy)
                    {
                        hitObject.GetComponent<Enemy>().TakeDamage(damage);
                    }
                    else if (hitObject.layer == (int)Layer.Switch)
                    {
                        hitObject.GetComponent<Switch>().ChangeActivationState(true);
                    }

                    //DEBUG
                    //Debug.DrawLine(shootingPos.position, raycastHit.point, Color.red, 0.3f);
                }
                else
                {
                    hitPos = rayOrigin + (cam.transform.forward * 500f);
                }

                TrailRenderer trail = Instantiate(bulletTrail, shootingPos.position, Quaternion.identity);
                StartCoroutine(TrailLerp(trail, hitPos));

                if (ammoLeft > 0)
                    ammoLeft--;
                canShoot = false;
                StartCoroutine(ShootingCooldownCoroutine(timeBetweenShots));
            }
        }
    }

    private IEnumerator TrailLerp(TrailRenderer trail, Vector3 hitPos)
    {
        Vector3 trailStartPos = trail.transform.position;
        float distance = Vector3.Distance(trailStartPos, hitPos);
        float startDistance = distance;
        while(distance > 0)
        {
            trail.transform.position = Vector3.Lerp(trailStartPos, hitPos, 1 - (distance / startDistance));
            distance -= Time.deltaTime * tracerSpeed;
            yield return null;
        }

        trail.transform.position = hitPos;
        //SFX ONHIT HERE
        //Instantiate(< SFX >, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(trail.gameObject);
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
