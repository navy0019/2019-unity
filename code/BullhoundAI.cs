using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullhoundAI : MonoBehaviour
{
    [Header("▼▼▼▼ GameObject ▼▼▼▼")]

    public GameObject m_Target;
    public GameObject neck;

    [Header("▼▼▼▼ Animator ▼▼▼▼")]

    public Animator animator;

    [Header("▼▼▼▼ AIData ▼▼▼▼")]

    public AIData m_Data;

    [HideInInspector]
    public FSMSystem m_FSM;

    public bool isBOSS;

    private float timeCount = 0;

    //Awake
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //Start
    void Start()
    {
        m_FSM = new FSMSystem(m_Data);
        m_Data.m_Go = this.gameObject;
        m_Data.m_FSMSystem = m_FSM;

        m_Data.arcAngle = 65;

        m_Data.arcLength = 15 * m_Data.m_Go.transform.localScale.x;
        m_Data.m_fAttackRange = 3.5f * (m_Data.m_Go.transform.localScale.x - 0.5f);
        m_Data.m_fSight = 2.8f * m_Data.m_Go.transform.localScale.x;
        m_Data.m_fMaxSpeed = 0.04f * (m_Data.m_Go.transform.localScale.x - 0.5f);
        m_Data.m_fMaxRot = 0.1f * (m_Data.m_Go.transform.localScale.x - 0.5f);




        HoundIdleState idlestate = new HoundIdleState(animator, isBOSS);
        HoundMoveToState mtstate = new HoundMoveToState(animator);
        HoundChaseState chasestate = new HoundChaseState(animator, isBOSS);
        HoundAttackState attackstate = new HoundAttackState(animator, isBOSS);
        HoundConfrontationState confrontationState = new HoundConfrontationState(animator,isBOSS);
        HoundDeadState dstate = new HoundDeadState(animator);

        idlestate.AddTransition(eFSMTransition.Go_MoveTo, mtstate);
        idlestate.AddTransition(eFSMTransition.Go_Confrontation, confrontationState);
        idlestate.AddTransition(eFSMTransition.Go_Attack, attackstate);

        mtstate.AddTransition(eFSMTransition.Go_Idle, idlestate);
        mtstate.AddTransition(eFSMTransition.Go_Confrontation, confrontationState);

        chasestate.AddTransition(eFSMTransition.Go_Attack, attackstate);
        chasestate.AddTransition(eFSMTransition.Go_Confrontation, confrontationState);

        attackstate.AddTransition(eFSMTransition.Go_Idle, idlestate);
        attackstate.AddTransition(eFSMTransition.Go_Chase, chasestate);
        attackstate.AddTransition(eFSMTransition.Go_Confrontation, confrontationState);

        confrontationState.AddTransition(eFSMTransition.Go_Idle, idlestate);
        confrontationState.AddTransition(eFSMTransition.Go_Attack, attackstate);
        confrontationState.AddTransition(eFSMTransition.Go_Chase, chasestate);

        m_FSM.AddGlobalTransition(eFSMTransition.Go_Dead, dstate);
        m_FSM.AddState(idlestate);
        m_FSM.AddState(mtstate);
        m_FSM.AddState(chasestate);
        m_FSM.AddState(attackstate);
        m_FSM.AddState(dstate);
        m_FSM.AddState(confrontationState);




    }

    //Update
    void Update()
    {
        if (!animator.GetBool("deathb"))
        {
            m_FSM.DoState();
        }
    }

    //LateUpdate
    private void LateUpdate()
    {

        Vector3 relativePos = m_Target.transform.position - neck.transform.position;


        string state = m_FSM.CurrentState.ToString();
        state = state.Substring(state.Length - 18);
        if (state == "ConfrontationState" && !animator.GetBool("deathb"))
        {
            if (timeCount >= 1)
            {
                timeCount = 1;
            }
            //Debug.Log("ConfrontationState");
            relativePos = Quaternion.Euler(0, -90, 0) * relativePos;
            Quaternion rotation = Quaternion.LookRotation(relativePos, -Vector3.up);

            Quaternion RotationOnlyY = Quaternion.Euler(neck.transform.rotation.eulerAngles.x, rotation.eulerAngles.y, neck.transform.rotation.eulerAngles.z);
            float rotAngle = Quaternion.Angle(neck.transform.rotation, rotation);
            neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, RotationOnlyY, timeCount);

            timeCount += Time.deltaTime;
        }
        else
        {
            timeCount = 0;
        }

    }

    //DrawGizmos
    private void OnDrawGizmos()
    {
        if (m_Data != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_Data.m_fAttackRange);
            //扇形
            float halfAngle = m_Data.arcAngle / 2;
            float add = 0.0f;
            Gizmos.color = Color.blue;
            Quaternion leftMax = Quaternion.Euler(0, -halfAngle, 0);
            Quaternion rightMax = Quaternion.Euler(0, halfAngle, 0);
            Vector3 left = (transform.position + leftMax * transform.forward * m_Data.arcLength);
            Vector3 right = (transform.position + rightMax * transform.forward * m_Data.arcLength);
            Gizmos.DrawLine(transform.position, left);
            Gizmos.DrawLine(transform.position, right);
            Gizmos.DrawWireSphere(transform.position, m_Data.m_fSight);

            for (float i = -halfAngle; i <= halfAngle; i += 5.0f)
            {
                Quaternion rotate = Quaternion.Euler(0, -halfAngle + add, 0);
                right = (transform.position + rotate * transform.forward * m_Data.arcLength);
                Gizmos.DrawLine(left, right);
                left = right;
                add += 5.0f;
            }
            Gizmos.color = Color.red;
            Vector3 neckForward = -neck.transform.right;
            neckForward.y = 0;
            Vector3 neckPos = neck.transform.position;
            neckPos.y = transform.position.y;
            Gizmos.DrawLine(neckPos, neckPos + neckForward * m_Data.arcLength);
            //Gizmos.DrawLine(footFL.position, footFL.position - Vector3.up * 100.0f);


        }
    }

    //Idle
    public class HoundIdleState : FSMState
    {

        private float m_fIdleTim;
        Animator animator;
        bool isBOSS;
        public HoundIdleState(Animator animator, bool TF)
        {
            m_StateID = eFSMStateID.IdleStateID;
            m_fIdleTim = Random.Range(1.0f, 3.0f);
            this.animator = animator;
            this.isBOSS = TF;

        }


        public override void DoBeforeEnter(AIData data)
        {

            animator.SetTrigger("idle01");
            data.m_fAttackRange = 3.5f * (data.m_Go.transform.localScale.x - 0.5f);
            data.m_fSight = 2.8f * data.m_Go.transform.localScale.x;
            data.m_fMaxRot = 0.1f;
            data.m_fMaxSpeed = 0.04f;

            m_fCurrentTime = 0.0f;
            m_fIdleTim = Random.Range(1.0f, 3.0f);

            //Debug.Log("Go idle");

        }

        public override void DoBeforeLeave(AIData data)
        {

        }

        public override void Do(AIData data)
        {

            m_fCurrentTime += Time.deltaTime;
        }

        public override void CheckCondition(AIData data)
        {
            bool bAttack = false;
            GameObject go = AIFunction.CheckEnemyInArc2(data, ref bAttack);

            if (go != null && CharacterParameter.PlayerHP > 0)
            {
                data.m_TargetObject = go;
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Confrontation);
                return;
            }
            if (m_fCurrentTime > m_fIdleTim && !isBOSS)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_MoveTo);
            }
        }
    }

    //Move To
    public class HoundMoveToState : FSMState
    {
        private int m_iCurrentWanderPt;
        private GameObject[] m_WanderPoints;
        Animator animator;

        public HoundMoveToState(Animator animator)
        {
            m_StateID = eFSMStateID.MoveToStateID;
            m_iCurrentWanderPt = -1;
            m_WanderPoints = GameObject.FindGameObjectsWithTag("Hound");
            this.animator = animator;
        }


        public override void DoBeforeEnter(AIData data)
        {

            animator.SetTrigger("walk");
            int iNewPt = Random.Range(0, m_WanderPoints.Length);
            List<int> indexList = new List<int>();
            for (int i = 0; i < m_WanderPoints.Length; i++)
            {
                indexList.Add(i);
            }
            if (m_iCurrentWanderPt == iNewPt)
            {
                indexList.Remove(iNewPt);
                iNewPt = Random.Range(0, indexList.Count);
                iNewPt = indexList[iNewPt];
            }
            m_iCurrentWanderPt = iNewPt;
            data.m_vTarget = m_WanderPoints[m_iCurrentWanderPt].transform.position;
            data.m_bMove = true;

        }

        public override void DoBeforeLeave(AIData data)
        {

        }

        public override void Do(AIData data)
        {
            if (SteeringBehavior.CollisionAvoid(data) == false)
            {

                SteeringBehavior.Seek(data);
            }

            SteeringBehavior.Move(data);
            //Debug.Log(data.m_bMove);
        }

        public override void CheckCondition(AIData data)
        {
            bool bAttack = false;
            GameObject go = AIFunction.CheckEnemyInArc2(data, ref bAttack);
            if (go != null && CharacterParameter.PlayerHP > 0)
            {
                data.m_TargetObject = go;

                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Confrontation);

                return;

            }
            if (data.m_bMove == false)
            {
                //Debug.Log("mt idle01");
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
        }
    }

    //Chase
    public class HoundChaseState : FSMState
    {
        Animator animator;
        bool isBOSS;
        public HoundChaseState(Animator animator, bool TF)
        {
            m_StateID = eFSMStateID.ChaseStateID;
            this.animator = animator;
            this.isBOSS = TF;

        }


        public override void DoBeforeEnter(AIData data)
        {
            data.m_fSight = data.arcLength;

            data.m_fMaxSpeed = 0.4f * (data.m_Go.transform.localScale.x - 0.5f);
            data.m_fMaxRot = 1f * (data.m_Go.transform.localScale.x - 0.5f);


            animator.SetTrigger("run_fast");
            Vector3 vec = data.m_TargetObject.transform.position - data.m_Go.transform.position;
            if (vec.magnitude > 6.5f * (data.m_Go.transform.localScale.x-0.5f))
            {
                data.m_fAttackRange = 6f * (data.m_Go.transform.localScale.x - 0.5f);
                if (isBOSS)
                {
                    data.attackType = Random.Range(3, 4);
                }
                else
                {
                    data.attackType = Random.Range(4, 5);
                }

            }
            else
            {

                data.attackType = Random.Range(0, 2);
                    




            }




            //Debug.Log("chase");
        }

        public override void DoBeforeLeave(AIData data)
        {
            //effect.GetComponent<ParticleSystem>().Stop();
        }

        public override void Do(AIData data)
        {

            data.m_vTarget = data.m_TargetObject.transform.position;
            if (SteeringBehavior.CollisionAvoid(data) == false)
            {
                SteeringBehavior.Seek(data);
            }

            SteeringBehavior.Move(data);

        }

        public override void CheckCondition(AIData data)
        {

            bool bAttack = false;
            bool bCheck = AIFunction.CheckTargetEnemyInArc2(data, data.m_TargetObject, ref bAttack);
            if (CharacterParameter.PlayerHP <= 0)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
                animator.Play("idle01");
            }
            else if (bAttack)
            {
                //Debug.Log("attack" );
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
            }
            else if (bCheck == false)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Confrontation);
                return;
            }
        }
    }

    //Attack
    public class HoundAttackState : FSMState
    {
        private float fAttackTime = 0.0f;
        Animator animator;
        int turnAround = 0;
        bool canMove = false;
        int maxTurn;
        bool isBOSS;
        bool firestjump;
        public HoundAttackState(Animator animator, bool TF)
        {
            m_StateID = eFSMStateID.AttackStateID;
            this.animator = animator;
            this.isBOSS = TF;
        }

        public override void DoBeforeEnter(AIData data)
        {
            data.m_fSight = data.arcLength;
            data.m_fMaxRot = 0.05f;
            m_fCurrentTime = 0.0f;
            turnAround = 0;
            canMove = false;
            firestjump = false;
            //Debug.Log(data.attackType);

        }

        public override void DoBeforeLeave(AIData data)
        {
            animator.ResetTrigger("att01");
            animator.ResetTrigger("att02");
            animator.ResetTrigger("jump");
            animator.ResetTrigger("runAttack");
            animator.ResetTrigger("attackJump");
            animator.ResetTrigger("emote");
            animator.speed = 1;
            turnAround = 0;
            canMove = false;

            //Debug.Log("leave attack");
        }

        public override void Do(AIData data)
        {
            Vector3 vec = data.m_TargetObject.transform.position - data.m_Go.transform.position;
            if (data.attackType == 0)
            {
                fAttackTime = 1.0f;
                animator.SetTrigger("att01");
            }
            else if (data.attackType == 1)
            {

                fAttackTime = 1.9f;
                animator.SetTrigger("att02");
            }
            else if (data.attackType == 2)
            {
                fAttackTime = 3.2f;//1.6f
                animator.SetTrigger("emote");
            }

            else if (data.attackType == 3)
            {
                fAttackTime = 1.3f;
                animator.speed = 1.3f;
                animator.SetTrigger("attackJump");

                if (!firestjump)
                {
                    data.m_vTarget = data.m_TargetObject.transform.position - data.m_Go.transform.forward*2f;
                    canMove = true;
                    firestjump = true;
                }
                if (Vector3.Dot(data.m_Go.transform.forward, vec.normalized) <0.5f)
                {
                    canMove = false;

                }
                if (canMove)
                {
                    if (SteeringBehavior.CollisionAvoid(data) == false)
                    {
                        SteeringBehavior.Seek(data);
                    }
                    SteeringBehavior.Move(data);
                }
            }
            else if (data.attackType == 4)
            {
                //Debug.Log(data.attackType);
                fAttackTime = 2.5f;

                if (turnAround == 0)
                {
                    maxTurn = (int)vec.magnitude - 2;
                    animator.SetTrigger("jump");
                    turnAround++;
                    data.m_vTarget = data.m_TargetObject.transform.position + data.m_Go.transform.forward * 1.5f;

                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("jump_start") || animator.GetCurrentAnimatorStateInfo(0).IsName("jump_pose"))
                {
                    canMove = true;
                    animator.speed = 1.5f;
                }
                else
                {
                    animator.speed = 1;
                }
                //Debug.Log(Mathf.Abs(Vector3.Dot(data.m_Go.transform.forward, vec.normalized)));
                //Debug.Log(m_fCurrentTime);
                if (Vector3.Dot(data.m_Go.transform.forward, vec.normalized) >= 0.8f && m_fCurrentTime > 1.5f)
                {
                    m_fCurrentTime = fAttackTime;
                    turnAround = maxTurn;
                    canMove = false;

                }
                if ((data.m_vTarget - data.m_Go.transform.position).magnitude < 1.8f && turnAround < maxTurn)
                {
                    turnAround++;
                    data.m_vTarget = data.m_vTarget + data.m_Go.transform.forward * 0.8f + data.m_Go.transform.right;
                }
                if (canMove)
                {
                    if (SteeringBehavior.CollisionAvoid(data) == false)
                    {
                        SteeringBehavior.Seek(data);
                    }
                    SteeringBehavior.Move(data);
                }
            }

            if (m_fCurrentTime >= fAttackTime)
            {
                canMove = false;
                animator.speed = 1;
                animator.SetTrigger("idle02");

            }
            m_fCurrentTime += Time.deltaTime;

        }

        public override void CheckCondition(AIData data)
        {
            bool bAttack = false;
            bool bCheck = AIFunction.CheckTargetEnemyInArc2(data, data.m_TargetObject, ref bAttack);

            if (m_fCurrentTime >= fAttackTime)
            {

                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Confrontation);
            }
            return;

        }
    }

    //Confrontation
    public class HoundConfrontationState : FSMState
    {
        private float fWaitTime = 0.0f;
        Animator animator;
        Vector3 rightVec;
        int ran;
        bool isBOSS;
        public HoundConfrontationState(Animator animator, bool TF)
        {
            m_StateID = eFSMStateID.ConfrontationStateID;
            this.animator = animator;
            this.isBOSS = TF;
        }


        public override void DoBeforeEnter(AIData data)
        {
            data.arcLength = 17.0f * data.m_Go.transform.localScale.x;
            data.m_fAttackRange = 3.5f * (data.m_Go.transform.localScale.x - 0.5f);
            data.m_fSight = data.arcLength;
            data.m_fMaxRot = 4f;
            animator.SetTrigger("idle02");
            ran = Random.Range(0, 2);
            fWaitTime = Random.Range(3.5f, 4.5f);
            m_fCurrentTime = 0.0f;
            animator.speed = 1;
        }

        public override void DoBeforeLeave(AIData data)
        {
            animator.ResetTrigger("idle02");
            animator.ResetTrigger("turn_left");
            animator.ResetTrigger("turn_right");
        }

        public override void Do(AIData data)
        {
            rightVec = data.m_Go.transform.right;
            Transform player = data.m_TargetObject.transform;
            Transform monster = data.m_Go.transform;

            Vector3 vec = data.m_TargetObject.transform.position - monster.position;

            Quaternion rq = Quaternion.LookRotation(player.position - monster.position);
            Quaternion RotationOnlyY = Quaternion.Euler(player.rotation.eulerAngles.x, rq.eulerAngles.y, player.rotation.eulerAngles.z);
            monster.rotation = Quaternion.Lerp(monster.rotation, RotationOnlyY, data.m_fMaxRot * Time.deltaTime);

            if (m_fCurrentTime > 1.0 && m_fCurrentTime < fWaitTime * 0.8 && vec.magnitude < data.arcLength && ran == 0)
            {

                animator.SetTrigger("turn_right");
                if (isBOSS) {
                    monster.position += rightVec / 25;
                }
                else{
                    monster.position += rightVec / 30;
                }

            }
            else if (m_fCurrentTime > 1.0 && m_fCurrentTime < fWaitTime * 0.8 && vec.magnitude < data.arcLength && ran == 1)
            {

                animator.SetTrigger("turn_left");
                if (isBOSS)
                {
                    monster.position -= rightVec / 25;
                }
                else
                {
                    monster.position -= rightVec / 30;
                }

            }
            else
            {
                animator.SetTrigger("idle02");
            }

            if (m_fCurrentTime >= fWaitTime)
            {
                m_fCurrentTime = 0.0f;
            }
            m_fCurrentTime += Time.deltaTime;
        }

        public override void CheckCondition(AIData data)
        {
            bool bAttack = false;
            bool bCheck = AIFunction.CheckTargetEnemyInArc2(data, data.m_TargetObject, ref bAttack);
            GameObject go = AIFunction.CheckEnemyInArc2(data, ref bAttack);
            Vector3 vec = data.m_TargetObject.transform.position - data.m_Go.transform.position;

            if (CharacterParameter.PlayerHP <= 0)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
            else if (m_fCurrentTime >= fWaitTime)
            {
                int rant = Random.Range(0, 2);
                if (go != null)
                {
                    data.m_TargetObject = go;
                    if (isBOSS && vec.magnitude< 9f && rant!=1)
                    {
                        data.attackType = 2;
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
                        return;
                    }
                    else if (bAttack)
                    {
                        if (isBOSS)
                        {
                            data.attackType = Random.Range(0, 3);
                        }
                        else
                        {
                            data.attackType = Random.Range(0, 2);
                        }


                        //Debug.Log("c go attack " + data.attackType);
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
                        return;
                    }
                    else if (bCheck)
                    {
                        //Debug.Log(vec.magnitude);
                        //Debug.Log("c go chase");
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                        return;
                    }
                }
                else
                {
                    //Debug.Log("c go Idel");
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
                    return;
                }
            }

        }
    }

    //Dead
    public class HoundDeadState : FSMState
    {
        Animator animator;

        public HoundDeadState(Animator animator)
        {
            m_StateID = eFSMStateID.DeadStateID;
            this.animator = animator;
        }


        public override void DoBeforeEnter(AIData data)
        {

        }

        public override void DoBeforeLeave(AIData data)
        {

        }

        public override void Do(AIData data)
        {
            Debug.Log("Do Dead State");
        }

        public override void CheckCondition(AIData data)
        {

        }
    }


}
