using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public GameObject playerTarget;
    //怪物初始位置
    Vector3 InitialPosition;
    //是否跟随玩家
    bool isFollowPlayer = false;
    Vector3 moveTargetPos => isFollowPlayer ? playerTarget.transform.position : InitialPosition;
    ////////////////////////////////////////////////怪物警戒标识//////////////////////////////
    public GameObject sign;
    public float alarmValue;
    //0 无警告，1黄色警告，2红色警告
    public int alarmLevel = 0;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        InitialPosition = transform.position;
        sign.transform.localPosition = new Vector3(0, 1.6f, 0);
        CloseSign();
    }
    void Update()
    {
        RefreshSign();
        RefreshLevel();
        RefreshMonsterPos();
    }
    //刷新标识位置和状态
    private void RefreshSign()
    {
        if (sign.activeSelf)
        {
            sign.transform.rotation = Camera.main.transform.rotation;
            sign.GetComponent<Renderer>().material.SetFloat("_Value", alarmValue);
        }
    }
    private void RefreshLevel()
    {
        switch (alarmLevel)
        {
            case 0:
                if (Vector3.Dot(playerTarget.transform.up, transform.up) > 0.3 &&
                    Vector3.Distance(playerTarget.transform.position, transform.position) < 10 &&
                    Vector3.Dot((playerTarget.transform.position - transform.position).normalized, transform.forward) > 0.5)
                {
                    alarmLevel = 1;
                    ShowQuestionSIgn();
                }
                break;
            case 1:
                if (Vector3.Dot(playerTarget.transform.up, transform.up) > 0.3 &&
                    Vector3.Distance(playerTarget.transform.position, transform.position) < 10 &&
                    Vector3.Dot((playerTarget.transform.position - transform.position).normalized, transform.forward) > 0.5)
                {
                    alarmValue += Time.deltaTime*0.5f;
                    if (alarmValue > 1)
                    {
                        alarmLevel = 2;
                        ShowExclamationSign();
                    }
                }
                else
                {
                    alarmValue -= Time.deltaTime;
                    alarmValue = math.max(alarmValue, 0);
                }
                sign.GetComponent<Renderer>().material.SetFloat("_Value", alarmValue);
                break;
            case 2:
                if (Vector3.Distance(playerTarget.transform.position, transform.position) > 10 ||
                    Vector3.Dot(playerTarget.transform.up, transform.up) < 0.3)
                {
                    alarmLevel = 1;
                    ShowQuestionSIgn();
                }
                break;
        }
    }
    private void RefreshMonsterPos()
    {
        // 检查角色是否到达目标点  
        float distanceToTarget = Vector3.Distance(transform.position, moveTargetPos);
        if (distanceToTarget <= agent.stoppingDistance)
        {
            agent.SetDestination(agent.transform.position);
            animator.SetBool("IsRun", false);
        }
        else
        {
            agent.SetDestination(moveTargetPos);
            animator.SetBool("IsRun", true);
        }
    }
    //显示问号标识
    private async void ShowQuestionSIgn()
    {
        sign.SetActive(true);
        sign.GetComponent<Renderer>().material.SetInt("_IsQuestion", 1);
        sign.transform.localScale = Vector3.one * 1.3f;
        await Task.Delay(200);
        sign.transform.localScale = Vector3.one;
        isFollowPlayer = false;
    }
    //显示叹号标识
    private async void ShowExclamationSign()
    {
        sign.GetComponent<Renderer>().material.SetInt("_IsQuestion", 0);
        sign.transform.localScale = Vector3.one * 1.3f;
        await Task.Delay(200);
        sign.transform.localScale = Vector3.one;
        isFollowPlayer = true;
    }
    //关闭标识
    private void CloseSign()
    {
        sign.SetActive(false);
    }
}
