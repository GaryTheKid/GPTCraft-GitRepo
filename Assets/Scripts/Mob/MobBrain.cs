using UnityEngine;

public class MobBrain : MonoBehaviour
{
    public enum MobState
    {
        Idle,
        Roam,
        Flee,
        Chase,
        Attack
    }
    public MobState currentState = MobState.Idle;

    private float stateTimer = 0f;
    private bool isPerforming = false;

    private MobWorldController mobWorldController;
    private Mob mobClass;
    private MobData mobData;
    private IAggressiveMob iAggressiveMob;


    #region Unity Functions
    private void Awake()
    {
        mobWorldController = GetComponent<MobWorldController>();
    }
    void Update()
    {
        // 根据当前状态执行不同的行为
        switch (currentState)
        {
            case MobState.Idle: Idle(); break;
            case MobState.Roam: Roam(); break;
            case MobState.Flee: Flee(); break;
            case MobState.Chase: Chase(); break;
            case MobState.Attack: Attack(); break;
        }
    }
    #endregion


    #region Setting
    public void SetMobBrain(Mob mobClass, MobData mobData)
    {
        this.mobClass = mobClass;
        this.mobData = mobData;

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
    #endregion


    #region States
    void Idle()
    {
        if (mobWorldController.chaseTarget != null)
        {
            stateTimer = 0f;
            isPerforming = false;
            currentState = MobState.Chase;
        }

        if (mobData.isTerrified)
        {
            stateTimer = 0f;
            isPerforming = false;
            currentState = MobState.Flee;
        }

        stateTimer += Time.deltaTime;

        if (stateTimer > mobClass.idleStateLength)
        {
            stateTimer = 0f;
            isPerforming = false;

            if (Random.value > mobClass.mobActivity)
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

        if (mobData.isTerrified)
        {
            stateTimer = 0f;
            isPerforming = false;
            currentState = MobState.Flee;
        }

        stateTimer += Time.deltaTime;

        // perform state
        if (stateTimer > mobClass.roamPerformTimer && !isPerforming)
        {
            isPerforming = true;

            // perform roam
            mobWorldController.UpdateWalkableNodes();
            mobWorldController.RoamToRandomCoord();
        }

        // switch state
        if (stateTimer > mobClass.roamStateLength)
        {
            stateTimer = 0f;
            isPerforming = false;

            if (Random.value > mobClass.mobActivity)
            {
                currentState = MobState.Idle;
            }
            else
            {
                currentState = MobState.Roam;
            }
        }
    }
    void Flee()
    {
        if (mobWorldController.chaseTarget != null)
        {
            stateTimer = 0f;
            isPerforming = false;
            currentState = MobState.Chase;
        }

        stateTimer += Time.deltaTime;

        // perform state
        if (stateTimer > mobClass.terrifiedPerformTimer && !isPerforming)
        {
            isPerforming = true;

            // perform roam
            mobWorldController.UpdateWalkableNodes();
            mobWorldController.FleeToRandomCoord();
        }

        // switch state
        if (stateTimer > mobClass.terrifiedStateLength)
        {
            stateTimer = 0f;
            isPerforming = false;

            if (Random.value > mobClass.mobActivity)
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
            if (stateTimer > mobClass.chasePerformTimer && !isPerforming)
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

        if (Random.value > mobClass.mobActivity)
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
                var targetDamageController = mobWorldController.chaseTarget.GetComponent<PlayerHealthController>();
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


        if (Random.value > mobClass.mobActivity)
        {
            currentState = MobState.Idle;
        }
        else
        {
            currentState = MobState.Roam;
        }
    }
    #endregion


    #region Utilities 
    private float GetTargetDist()
    {
        if (mobWorldController.chaseTarget != null)
        {
            return Vector3.Distance(mobWorldController.chaseTarget.position, transform.position);
        }

        return -1f;
    }
    #endregion
}
