using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardEffect : MonoBehaviour
{
    [Header("▼▼▼▼ GameObject ▼▼▼▼")]

    public GameObject Player;
    public GameObject MonsterParent;
    public GameObject Reward;
    public AIData data;

    [HideInInspector]
    public bool firstTarget;
    public bool canMove;
    public bool reset;

    float parameter;
    // Start is called before the first frame update
    void Start()
    {
        canMove = false;
        firstTarget = true;
        data.m_Go = Reward;
        data.m_fAttackRange = 0.2f;
        data.m_fSight = 100f;
        data.m_fMaxSpeed = 1.5f;
        data.m_fMaxRot = 0.03f;
        data.m_TargetObject = Player;
        reset = false;



    }

    // Update is called once per frame
    void Update()
    {
        //Rewards.transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, Time.deltaTime * 5.0f);
        //transform.position = 
        if (reset)
        {
            transform.position = MonsterParent.transform.position;
            reset = false;
            firstTarget = true;
        }
        if (canMove && Reward!=null)
        {
            Vector3 vec = data.m_TargetObject.transform.position - MonsterParent.transform.position;
            parameter = vec.magnitude;
            //Debug.Log(parameter);

            //Debug.Log(data.m_fMaxRot);
            //Debug.Log(MonsterParent.transform.position);
            if (firstTarget)
            {
                if (parameter>6f)
                {
                    data.m_vTarget = (data.m_TargetObject.transform.position + MonsterParent.transform.position) / 2 + MonsterParent.transform.right * parameter;
                    data.m_fMaxRot = parameter * 0.03f;

                }
                else
                {
                    data.m_vTarget = data.m_TargetObject.transform.position;
                    data.m_fMaxRot = parameter;

                }
                firstTarget = false;
            }

            
            if ((data.m_vTarget - data.m_Go.transform.position).magnitude < parameter)
            {
                data.m_vTarget = data.m_TargetObject.transform.position;
            }

            if (SteeringBehavior.CollisionAvoid(data) == false)
            {
                SteeringBehavior.Seek(data);
            }

            SteeringBehavior.Move(data);
        }
    }

    public void DropReward()
    {
        //Debug.Log("dddd");
        Reward.SetActive(true);
        canMove = true;
        
    }
}
