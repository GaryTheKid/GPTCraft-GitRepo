using UnityEngine;

public class MobBrain : MonoBehaviour
{
    public enum MobState
    {
        Idle,
        Roam,
        Chase,
        Attack
    }

    public MobState currentState = MobState.Idle;

    public float stateSwitchCD = 5f;
    public float statePerformCD = 3f;

    private float stateTimer = 0f;
    private bool isPerforming = false;

    private MobWorldController mobWorldController;
    private Mob mobClass;

    private IAggressiveMob iAggressiveMob;

    private void Awake()
    {
        mobWorldController = GetComponent<MobWorldController>();
    }

    void Update()
    {
        // 根据当前状态执行不同的行为
        switch (currentState)
        {
            case MobState.Idle:
                Idle();
                break;
            case MobState.Roam:
                Roam();
                break;
            case MobState.Chase:
                Chase();
                break;
            case MobState.Attack:
                Attack();
                break;
        }
    }

    public void SetMobBrain(Mob mobClass)
    {
        this.mobClass = mobClass;

        if (mobClass is IAggressiveMob)
        {
            iAggressiveMob = (IAggressiveMob)mobClass;
        }

        ResetMobBrain();
    }

    public void ResetMobBrain()
    {
        stateTimer = 0f;
        isPerforming = false;
        currentState = MobState.Idle;
    }

    void Idle()
    {
        if (mobWorldController.chaseTarget != null)
        {
            stateTimer = 0f;
            isPerforming = false;
            currentState = MobState.Chase;
        }

        stateTimer += Time.deltaTime;

        if (stateTimer > stateSwitchCD)
        {
            stateTimer = 0f;
            isPerforming = false;

            // 根据 50% 概率切换到 Idle 或 Roam
            if (Random.value < 0.5f)
            {
                currentState = MobState.Idle;
            }
            else
            {
                currentState = MobState.Roam;
            }
        }
    }

    void Roam()
    {
        if (mobWorldController.chaseTarget != null)
        {
            stateTimer = 0f;
            isPerforming = false;
            currentState = MobState.Chase;
        }

        stateTimer += Time.deltaTime;

        // perform state
        if (stateTimer > statePerformCD && !isPerforming)
        {
            isPerforming = true;

            // perform roam
            mobWorldController.UpdateWalkableNodes();
            mobWorldController.RoamToRandomCoord();
        }

        // switch state
        if (stateTimer > stateSwitchCD)
        {
            stateTimer = 0f;
            isPerforming = false;

            // 根据 50% 概率切换到 Idle 或 Roam
            if (Random.value < 0.5f)
            {
                currentState = MobState.Idle;
            }
            else
            {
                currentState = MobState.Roam;
            }
        }
    }

    void Chase()
    {
        if (iAggressiveMob == null)
        {
            return;
        }

        if (mobWorldController.chaseTarget != null && GetTargetDist() > iAggressiveMob.GetAttackRange())
        {
            // 追击
            stateTimer += Time.deltaTime;

            // perform state
            if (stateTimer > statePerformCD && !isPerforming)
            {
                isPerforming = true;
                stateTimer = 0f;

                // perform chase
                mobWorldController.UpdateWalkableNodes();
                mobWorldController.UpdateChaseTargetCoord();
                mobWorldController.ChaseTargetCoord();

                isPerforming = false;
            }

            return;
        }
        else if (GetTargetDist() <= iAggressiveMob.GetAttackRange())
        {
            stateTimer = 0f;
            isPerforming = false;

            currentState = MobState.Attack;

            return;
        }

        stateTimer = 0f;
        isPerforming = false;


        // 根据 50% 概率切换到 Idle 或 Roam
        if (Random.value < 0.5f)
        {
            currentState = MobState.Idle;
        }
        else
        {
            currentState = MobState.Roam;
        }
    }

    void Attack()
    {
        if (iAggressiveMob == null)
        {
            return;
        }

        if (mobWorldController.chaseTarget != null && GetTargetDist() <= iAggressiveMob.GetAttackRange())
        {
            // 攻击
            stateTimer += Time.deltaTime;

            // perform state
            if (stateTimer > (1f / iAggressiveMob.GetAttackSpeed()) && !isPerforming)
            {
                isPerforming = true;
                stateTimer = 0f;

                // perform chase
                var targetDamageController = mobWorldController.chaseTarget.GetComponent<PlayerDamageController>();
                if (targetDamageController != null)
                {
                    targetDamageController.TakeDamage(iAggressiveMob.GetAttackDamage());
                }

                isPerforming = false;
            }

            return;
        }
        else if (GetTargetDist() > iAggressiveMob.GetAttackRange())
        {
            stateTimer = 0f;
            isPerforming = false;

            currentState = MobState.Chase;

            return;
        }

        stateTimer = 0f;
        isPerforming = false;


        // 根据 50% 概率切换到 Idle 或 Roam
        if (Random.value < 0.5f)
        {
            currentState = MobState.Idle;
        }
        else
        {
            currentState = MobState.Roam;
        }
    }

    private float GetTargetDist()
    {
        if (mobWorldController.chaseTarget != null)
        {
            return Vector3.Distance(mobWorldController.chaseTarget.position, transform.position);
        }

        return -1f;
    }
}
