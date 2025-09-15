using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 발사 후 계산
    private void Shoot()
    {
        crosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        PlaySE(currentGun.fireSound);
        if (!isFineSightMode)
        {
            currentGun.muzzleFlash.Play();
        }
        Hit();

        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
        StartCoroutine(CameraRecoilCoroutine());
    }

    private void Hit()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward +
        new Vector3(Random.Range(-crosshair.GetAccuracy() - currentGun.accuracy, crosshair.GetAccuracy() + currentGun.accuracy),
                    Random.Range(-crosshair.GetAccuracy() - currentGun.accuracy, crosshair.GetAccuracy() + currentGun.accuracy),
                    0) ,
            out hitInfo, currentGun.range))
        {
            if (hitInfo.transform.GetComponent<Limb>())
            {
                Limb limb = hitInfo.transform.GetComponent<Limb>();
                limb.GetHit();

                GameObject blood = Instantiate(blood_Effect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                Destroy(blood, 2.0f);
            }
            else if (hitInfo.transform.CompareTag("Zombie"))
            {
                ZombieController zombie = hitInfo.transform.root.GetComponent<ZombieController>();
                zombie.GetDamage(20);

                GameObject blood = Instantiate(blood_Effect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                Destroy(blood, 2.0f);
            }
            else
            {
                GameObject clone = Instantiate(hit_Effect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                Destroy(clone, 2.0f);
            }

        }
    }

    // 재장전 코루틴
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            PlaySE(currentGun.ReloadSound);

            yield return new WaitForSeconds(currentGun.reloadTime);

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            if (currentGun.currentBulletCount == 0)
            {
                if (!audioSource.isPlaying)
                {
                    PlaySE(currentGun.gunEmptySound);
                }
            }
            else
            {
                Debug.Log("소유한 총알이 없습니다");
            }
        }
    }

    // 반동 코루틴
    IEnumerator RetroActionCoroutine()
    {
        // 반동 z값으로 주기
        Vector3 recoilBack = new Vector3(originPos.x, originPos.y, currentGun.retroActionForce);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.fineSightOriginPos.x, currentGun.fineSightOriginPos.y, currentGun.retroActionFineSightForce);


        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;

            //반동 시작
            while (currentGun.transform.localPosition.z >= currentGun.retroActionForce + 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            //반동 시작
            while (currentGun.transform.localPosition.z >= currentGun.retroActionFineSightForce + 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    // 카메라 반동 코루틴
    IEnumerator CameraRecoilCoroutine()
    {
        if (isFineSightMode)
        {
            yield break;
        }
        
        Vector3 recoilRotation = new Vector3(-2f, Random.Range(-1f, 1f), 0f);
        Vector3 currentRotation = camera.transform.localEulerAngles;

        Vector3 targetRotation = currentRotation + recoilRotation;

        // 반동주기
        float elapsed = 0f;
        float duration = 0.1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            camera.transform.localEulerAngles = Vector3.Lerp(currentRotation, targetRotation, elapsed);
            yield return null;
        }

        // 원위치 복귀
        elapsed = 0f;
        duration = 0.2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            camera.transform.localEulerAngles = Vector3.Lerp(targetRotation, currentRotation, elapsed);
            yield return null;
        }
    }

    // 벽에 가까이가면 총을 들도록 상태 판별
    private void CheckHighReadyState()
    {
        if (highRreadyTrigger.GetIsTouchingWall() && !isFineSightMode)
        {
            if (!playerController.isHighReady)
            {
                playerController.SetHighReady(true);
            }
        }
        else
        {
            if (playerController.isHighReady)
            {
                playerController.SetHighReady(false);
            }
        }
    }

}
