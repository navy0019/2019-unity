using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWorkAI : MonoBehaviour
{

    public GameObject wayPointObj;
    public List<Node> workPoint;

    [Header("▼▼▼▼ Animator ▼▼▼▼")]

    public Animator animator;

    [Header("▼▼▼▼ AIData ▼▼▼▼")]

    public AIData m_Data;

    [HideInInspector]
    public FSMSystem m_FSM;

    [HideInInspector]
    public Astar astarPath;
    [HideInInspector]
    public terrainData tData;

    [HideInInspector]
    public List<Vector3> path;


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

        m_Data.m_fProbeLength = 1.5f;
        m_Data.m_fRadius = 0.5f;

        m_Data.m_fAttackRange = 1.0f;
        m_Data.m_fSight = 2.8f * m_Data.m_Go.transform.localScale.x;
        m_Data.m_fMaxSpeed = 0.04f;
        m_Data.m_fMaxRot = 0.07f;

        animator = GetComponent<Animator>();


        tData = new terrainData(wayPointObj);
        tData.Init();
        List<Node> wayPoints = tData.points;

        astarPath = new Astar();
        astarPath.InitList();

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (gos != null || gos.Length > 0)
        {
            foreach (GameObject go in gos)
            {
                if (go.name != this.name)
                {
                    m_Data.Obstacles.Add(go.GetComponent<Obstacle>());
                    //Debug.Log(go.name);
                }

            }
            m_Data.Obstacles.Add(player.GetComponent<Obstacle>());
        }

        NPCWorkIdleState idlestate = new NPCWorkIdleState(animator);
        NPCWorkMoveToState mtstate = new NPCWorkMoveToState(animator, astarPath, tData, workPoint);
        NPCWorkAttackState attackstate = new NPCWorkAttackState(animator);

        idlestate.AddTransition(eFSMTransition.Go_MoveTo, mtstate);
        idlestate.AddTransition(eFSMTransition.Go_Attack, attackstate);

        mtstate.AddTransition(eFSMTransition.Go_Idle, idlestate);
        mtstate.AddTransition(eFSMTransition.Go_Attack, attackstate);

        attackstate.AddTransition(eFSMTransition.Go_Idle, idlestate);

        m_FSM.AddState(idlestate);
        m_FSM.AddState(mtstate);
        m_FSM.AddState(attackstate);

    }

    //Update
    void Update()
    {

        m_FSM.DoState();
        RaycastHit hit;
        if (Physics.Linecast(transform.position + transform.up, transform.position - transform.up * 5f, out hit, 1 << LayerMask.NameToLayer("Ground")))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

    }



    //DrawGizmos
    private void OnDrawGizmos()
    {
        if (m_Data != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fRadius);
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.m_Data.m_fProbeLength);
            Gizmos.color = Color.yellow;
            Vector3 vLeftStart = this.transform.position - this.transform.right * m_Data.m_fRadius;
            Vector3 vLeftEnd = vLeftStart + this.transform.forward * m_Data.m_fProbeLength;
            Gizmos.DrawLine(vLeftStart, vLeftEnd);
            Vector3 vRightStart = this.transform.position + this.transform.right * m_Data.m_fRadius;
            Vector3 vRightEnd = vRightStart + this.transform.forward * m_Data.m_fProbeLength;
            Gizmos.DrawLine(vRightStart, vRightEnd);
            Gizmos.DrawLine(vLeftEnd, vRightEnd);

            Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fAttackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.up, transform.position - transform.up * 5f);

        }
    }

    //Idle
    public class NPCWorkIdleState : FSMState
    {

        private float m_fIdleTim;
        Animator animator;

        public NPCWorkIdleState(Animator animator)
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

            if (m_fCurrentTime > m_fIdleTim)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_MoveTo);


            }
        }

    }

    //Move To
    public class NPCWorkMoveToState : FSMState
    {
        private int m_iCurrentWanderPt;

        Animator animator;

        List<Node> workPoint;
        List<Vector3> path;
        Astar astar;
        terrainData tData;

        int pathIndex = 0;
        int endIndex;
        int type = 1;

        public NPCWorkMoveToState(Animator animator, Astar astar, terrainData tData, List<Node> workPoint)
        {
            m_StateID = eFSMStateID.MoveToStateID;
            m_iCurrentWanderPt = -1;
            this.animator = animator;
            this.astar = astar;
            this.tData = tData;
            this.workPoint = workPoint;
        }


        public override void DoBeforeEnter(AIData data)
        {
            Node nowPos = tData.GetNodeFromPosition(data.m_Go.transform.position);
            //Debug.Log("now "+nowPos.name);
            int iNewPt = Random.Range(0, workPoint.Count);
            List<int> indexList = new List<int>();
            for (int i = 0; i < workPoint.Count; i++)
            {
                indexList.Add(i);
            }
            if (nowPos == workPoint[iNewPt])
            {
                indexList.Remove(iNewPt);
                iNewPt = Random.Range(0, indexList.Count);
                iNewPt = indexList[iNewPt];
            }
            //Debug.Log("target "+ tData.points[iNewPt].name);

            path = astar.startAStar(nowPos, workPoint[iNewPt]);

            pathIndex = 0;
            animator.SetTrigger("Walk");

            data.m_vTarget = path[pathIndex];
            data.m_bMove = true;
        }

        public override void DoBeforeLeave(AIData data)
        {
        }

        public override void Do(AIData data)
        {

            if ((data.m_vTarget - data.m_Go.transform.position).magnitude < data.m_fAttackRange && pathIndex != path.Count - 1)
            {

                pathIndex++;
                data.m_vTarget = path[pathIndex];
                //Debug.Log("+++++++++");
            }

            if (SteeringBehavior.CollisionAvoid(data) == false)
            {
                SteeringBehavior.Seek(data);
            }



            SteeringBehavior.Move(data);

        }

        public override void CheckCondition(AIData data)
        {

            if ((data.m_vTarget - data.m_Go.transform.position).magnitude < data.m_fAttackRange && pathIndex >= path.Count - 1)
            {

                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
            }
        }
    }


    //Attack
    public class NPCWorkAttackState : FSMState
    {
        private float fAttackTime = 0.0f;
        Animator animator;

        public NPCWorkAttackState(Animator animator)
        {
            m_StateID = eFSMStateID.AttackStateID;
            this.animator = animator;

        }

        public override void DoBeforeEnter(AIData data)
        {
            m_fCurrentTime = 0;
            fAttackTime = Random.Range(5f, 7f);
            animator.SetTrigger("Work");
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

            if (m_fCurrentTime >= fAttackTime)
            {
                m_fCurrentTime = 0;
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }


        }
    }
}