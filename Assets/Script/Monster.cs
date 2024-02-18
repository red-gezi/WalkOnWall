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
    //�����ʼλ��
    Vector3 InitialPosition;
    //�Ƿ�������
    bool isFollowPlayer = false;
    Vector3 moveTargetPos => isFollowPlayer ? playerTarget.transform.position : InitialPosition;
    ////////////////////////////////////////////////���ﾯ���ʶ//////////////////////////////
    public GameObject sign;
    public float alarmValue;
    //0 �޾��棬1��ɫ���棬2��ɫ����
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
    //ˢ�±�ʶλ�ú�״̬
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
        // ����ɫ�Ƿ񵽴�Ŀ���  
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
    //��ʾ�ʺű�ʶ
    private async void ShowQuestionSIgn()
    {
        sign.SetActive(true);
        sign.GetComponent<Renderer>().material.SetInt("_IsQuestion", 1);
        sign.transform.localScale = Vector3.one * 1.3f;
        await Task.Delay(200);
        sign.transform.localScale = Vector3.one;
        isFollowPlayer = false;
    }
    //��ʾ̾�ű�ʶ
    private async void ShowExclamationSign()
    {
        sign.GetComponent<Renderer>().material.SetInt("_IsQuestion", 0);
        sign.transform.localScale = Vector3.one * 1.3f;
        await Task.Delay(200);
        sign.transform.localScale = Vector3.one;
        isFollowPlayer = true;
    }
    //�رձ�ʶ
    private void CloseSign()
    {
        sign.SetActive(false);
    }
}
