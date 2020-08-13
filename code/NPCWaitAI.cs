using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWaitAI : MonoBehaviour
{


    [Header("▼▼▼▼ Animator ▼▼▼▼")]

    public Animator animator;

    [Header("▼▼▼▼ AIData ▼▼▼▼")]

    public AIData m_Data;

    [HideInInspector]
    public FSMSystem m_FSM;


    //private float timeCount = 0;

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

        m_Data.m_fAttackRange = 2.0f;
        m_Data.m_fSight = 2.8f * m_Data.m_Go.transform.localScale.x;
        m_Data.m_fMaxSpeed = 0.04f;
        m_Data.m_fMaxRot = 0.07f;

        animator = GetComponent<Animator>();

        NPCWaitIdleState idlestate = new NPCWaitIdleState(animator);
        NPCWaitAttackState attackstate = new NPCWaitAttackState(animator);

        idlestate.AddTransition(eFSMTransition.Go_Attack, attackstate);
        attackstate.AddTransition(eFSMTransition.Go_Idle, idlestate);

        m_FSM.AddState(idlestate);
        m_FSM.AddState(attackstate);

    }

    //Update
    void Update()
    {

        m_FSM.DoState();

    }



    //DrawGizmos
    private void OnDrawGizmos()
    {
        if (m_Data != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fAttackRange);

        }
    }

    //Idle
    public class NPCWaitIdleState : FSMState
    {

        private float m_fIdleTim;
        Animator animator;

        public NPCWaitIdleState(Animator animator)
        {
            m_StateID = eFSMStateID.IdleStateID;
            m_fIdleTim = Random.Range(3.0f, 4.0f);
            this.animator = animator;


        }


        public override void DoBeforeEnter(AIData data)
        {
            animator.SetTrigger("Stand");
            m_fCurrentTime = 0.0f;

            m_fIdleTim = Random.Range(1.0f, 3.0f);

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

            Collider[] hitColliders = Physics.OverlapSphere(data.m_Go.transform.position, data.m_fAttackRange);

            if (hitColliders.Length!=0)
            {
                for (int i=0;i<hitColliders.Length;i++) {
                    string objName= hitColliders[i].gameObject.name;
                    Animator objAni = hitColliders[i].gameObject.GetComponent<Animator>();

                    if (hitColliders[i].gameObject.name.Length>=13) {
                        objName = hitColliders[i].gameObject.name.Substring(0, 13);
                    }

                    if (objName.Length>=13 &&hitColliders[i].gameObject.name.Substring(0, 13) == "VilligerSpeak" && objAni.GetCurrentAnimatorStateInfo(0).IsName("Speak"))// && stateInfo
                    {
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
                    }
                }
            }
            else if (m_fCurrentTime > m_fIdleTim)
            {
                m_fCurrentTime = 0;
                return;
            }
        }

    }


    //Attack
    public class NPCWaitAttackState : FSMState
    {
        Animator animator;

        public NPCWaitAttackState(Animator animator)
        {
            m_StateID = eFSMStateID.AttackStateID;
            this.animator = animator;

        }

        public override void DoBeforeEnter(AIData data)
        {
            animator.SetTrigger("Work");
        }

        public override void DoBeforeLeave(AIData data)
        {

        }

        public override void Do(AIData data)
        {

        }

        public override void CheckCondition(AIData data)
        {
            Collider[] hitColliders = Physics.OverlapSphere(data.m_Go.transform.position, data.m_fAttackRange);
            if (hitColliders.Length != 0)
            {
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    string objName = hitColliders[i].gameObject.name;
                    Animator objAni = hitColliders[i].gameObject.GetComponent<Animator>();

                    if (hitColliders[i].gameObject.name.Length >= 13)
                    {
                        objName = hitColliders[i].gameObject.name.Substring(0, 13);
                    }

                    if (objName.Length >= 13 && hitColliders[i].gameObject.name.Substring(0, 13) == "VilligerSpeak" && objAni.GetCurrentAnimatorStateInfo(0).IsName("Speak"))// && stateInfo
                    {
                        return;
                    }
                }
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
            else
            {
                Debug.Log("trans");
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }


        }
    }
}