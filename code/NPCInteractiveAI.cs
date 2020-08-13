using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractiveAI : MonoBehaviour
{


    [Header("▼▼▼▼ Animator ▼▼▼▼")]

    public Animator animator;

    [Header("▼▼▼▼ AIData ▼▼▼▼")]

    public AIData m_Data;

    [HideInInspector]
    public FSMSystem m_FSM;



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


        m_Data.m_fAttackRange = 3.0f;


        animator = GetComponent<Animator>();


        NPCActiveIdleState idlestate = new NPCActiveIdleState(animator);

        NPCActiveMainActionState mainActionstate = new NPCActiveMainActionState(animator);


        idlestate.AddTransition(eFSMTransition.GO_MainAction, mainActionstate);

        mainActionstate.AddTransition(eFSMTransition.Go_Idle, idlestate);

        m_FSM.AddState(idlestate);
        m_FSM.AddState(mainActionstate);

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
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fAttackRange);


        }
    }

    //Idle
    public class NPCActiveIdleState : FSMState
    {
        private float m_fIdleTim = 1.0f;
        Animator animator;

        public NPCActiveIdleState(Animator animator)
        {
            m_StateID = eFSMStateID.IdleStateID;

            this.animator = animator;


        }


        public override void DoBeforeEnter(AIData data)
        {
            m_fCurrentTime = 0;
        }

        public override void DoBeforeLeave(AIData data)
        {

        }

        public override void Do(AIData data)
        {
           
        }

        public override void CheckCondition(AIData data)
        {
            bool bAttack = false;
            GameObject go = AIFunction.CheckEnemyInSight(data, ref bAttack);

            if (go != null)
            {
                if (m_fCurrentTime >= m_fIdleTim)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.GO_MainAction);
                }
                data.m_TargetObject = go;
                animator.SetTrigger("confrontation");
                m_fCurrentTime += Time.deltaTime;
                return;
            }

        }

    }

    //Attack
    public class NPCActiveMainActionState : FSMState
    {
        private float fAttackTime = 0.0f;
        int type;
        Animator animator;

        public NPCActiveMainActionState(Animator animator)
        {
            m_StateID = eFSMStateID.AttackStateID;
            this.animator = animator;

        }

        public override void DoBeforeEnter(AIData data)
        {
            m_fCurrentTime = 0;
            animator.SetTrigger("mainAction0");
            type = Random.Range(2, 4);
            data.IdleAudioSource = data.Idle.GetComponent<AudioSource>();
            data.Idle2AudioSource = data.Idle2.GetComponent<AudioSource>();
            data.SpeakAudioSource = data.Speak.GetComponent<AudioSource>();
            data.HoldItemAudioSource = data.HoldItem.GetComponent<AudioSource>();
        }

        public override void DoBeforeLeave(AIData data)
        {
            animator.Play("ReturnIdle");
        }

        public override void Do(AIData data)
        {

            if (type == 0)
            {
                animator.SetTrigger("mainAction1");
                fAttackTime = 10.0f;
                if (!data.SpeakAudioSource.isPlaying && m_fCurrentTime <= 0.1f)
                {
                    data.SpeakAudioSource.Play();
                }
            }
            else if (type == 1)
            {
                animator.SetTrigger("mainAction2");
                fAttackTime = 10.0f;
                if (!data.HoldItemAudioSource.isPlaying && m_fCurrentTime <= 0.1f)
                {
                    data.HoldItemAudioSource.Play();
                }
            }
            else if (type == 2)
            {
                animator.SetTrigger("mainAction3");
                fAttackTime = 10.0f;
                if (!data.Idle2AudioSource.isPlaying && m_fCurrentTime <= 0.1f)
                {
                    data.Idle2AudioSource.Play();
                }
            }
            else if (type == 3)
            {
                animator.SetTrigger("mainAction0");
                fAttackTime = 10.0f;
                if (!data.IdleAudioSource.isPlaying && m_fCurrentTime <= 0.1f)
                {
                    data.IdleAudioSource.Play();
                }  
            }
            m_fCurrentTime += Time.deltaTime;

        }

        public override void CheckCondition(AIData data)
        {
            bool bAttack = false;
            GameObject go = AIFunction.CheckEnemyInSight(data, ref bAttack);
            if (go==null) {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
            else if (m_fCurrentTime >= fAttackTime && data.TraderUI.activeInHierarchy)
            {
                m_fCurrentTime = 0;
                type = Random.Range(0, 2);
                return;
            }
            else if (m_fCurrentTime >= fAttackTime)
            {
                m_fCurrentTime = 0;
                type = Random.Range(2, 4);
                animator.ResetTrigger("mainAction1");
                animator.ResetTrigger("mainAction2");
                animator.ResetTrigger("mainAction3");
                animator.ResetTrigger("mainAction0");
            }
        }
    }
}