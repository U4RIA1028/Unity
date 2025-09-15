using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // 부서진 신체에 따른 값 계산
    public void OnLimbDestroyed(LimbType type)
    {
        switch (type)
        {
            case LimbType.Head:
                Debug.Log("Head Destroyed");
                GetDamage(100);
                break;

            case LimbType.LeftArm:
                Debug.Log("LeftArm Destroyed");
                isLeftArmDestroy = true;

                UpdateAnimatiorState();
                GetDamage(30);
                break;

            case LimbType.RightArm:
                Debug.Log("RightArm Destroyed");
                isRightArmDestroy = true;

                UpdateAnimatiorState();
                GetDamage(30);
                break;

            case LimbType.Leg:
                Debug.Log("Leg Destroyed");
                isLegDestroy = true;
                isWalk = false;
                isRun = false;
                isCrawl = true;

                UpdateAnimatiorState();
                GetDamage(20);
                break;

        }
    }

    // 애니메이션 상태 변경
    private void UpdateAnimatiorState()
    {
        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isRun", isRun);
        anim.SetBool("isCrawl", isCrawl);
        anim.SetBool("isLeftArmDestroy", isLeftArmDestroy);
        anim.SetBool("isRightArmDestroy", isRightArmDestroy);
        anim.SetBool("isLegDestroy", isLegDestroy);
    }

    // 지정 구역 순회
    private void Patrol()
    {
        destination = patrolPoints[patrolPointIndex].position;
        agent.speed = walkSpeed;

        if (isLegDestroy)
        {
            isRun = false;
            isCrawl = true;
        }
        else
        {
            isRun = false;
            isWalk = true;
        }

        UpdateAnimatiorState();

        agent.SetDestination(destination);

        if (!agent.pathPending && agent.remainingDistance < 1.2f)
        {
            patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
        }
    }

    // 플레이어 추격
    private void Chase()
    {
        destination = player.position;

        if (isLegDestroy)
        {
            isWalk = false;
            isCrawl = true;
        }
        else
        {
            isWalk = false;
            isRun = true;
            agent.speed = chaseSpeed;
        }

        UpdateAnimatiorState();

        agent.SetDestination(destination);
    }

    // 공격 코루틴
    private IEnumerator AttackCoroutine()
    {
        isAttack = true;
        agent.ResetPath();

        yield return new WaitForSeconds(0.5f);
        transform.LookAt(player);

        // 공격 애니메이션이 어느정도 진행 된 후 데미지를 입히게
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;
        int layerMask = LayerMask.GetMask("Player");
        Debug.DrawRay(origin, direction * attackRange, Color.red, 1f);
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange, layerMask))
        {
            Debug.Log("Player Damaged");
        }

        isAttack = false;
    }
}
