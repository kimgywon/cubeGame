using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYAISTS
{
    ACTIONSELECT,  //액션 선택
    WAIT, //일정 시간 대기
    RUNTOPLAYER, //플레이어에개 다가간다
    ESCAPE, //플레이어에가 일정범위 이상 도망가면 원래 위치로 
    ATTACK, //공격한다
    FREEZ, //행동 정지 
}

public class Enemy : MonoBehaviour
{

    public int debug_selectRandomAiState = -1;

    [System.NonSerialized]
    public ENEMYAISTS m_AiState = ENEMYAISTS.ACTIONSELECT;

    protected EnemyControlloer m_EnemyControll;
    protected GameObject m_Player;
    protected PlayerController m_PlayerControll;
    protected Animator m_EnemyAnimator;

    protected float m_AiActionTimeLength = 0.0f;
    protected float m_AiActionTimeStart = 0.0f;

    public void Awake()
    {
        //  m_PlayerControll = m_Player.GetComponent<PlayerController>();
    }

    public virtual void FixedUpdateAI()
    {
    }

    public void EnemyInit()
    {
        m_EnemyControll = GetComponent<EnemyControlloer>();
        m_EnemyAnimator = GetComponent<Animator>();
        m_Player = PlayerController.GetGameObject();
        m_AiState = ENEMYAISTS.ACTIONSELECT;
        m_AiActionTimeLength = 0.0f;
        m_AiActionTimeStart = 0.0f;
    }

    public void FixedUpdate()
    {
        if (BeginEnemyCommonWork())
        {
            FixedUpdateAI();
            EndEnemyCommonWork();
        }
    }

    public bool BeginEnemyCommonWork()
    {
        if (m_EnemyControll.m_EnemyHp <= 0)  //살아 있는지 확인
        {
            return false;
        }

        if (!CheckAtion())
        {
            return false;
        }

        return true;
    }

    public void EndEnemyCommonWork()
    {
        float time = Time.fixedTime - m_AiActionTimeStart;
        if (time > m_AiActionTimeLength)
        {
            m_AiState = ENEMYAISTS.ACTIONSELECT;
        }
    }

    public bool CheckAtion()
    {
        AnimatorStateInfo stateInfo = m_EnemyAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == EnemyControlloer.ANISTS_ATTACK_NOMAL ||
            stateInfo.fullPathHash == EnemyControlloer.ANISTS_ATTACK_CRI ||
            stateInfo.fullPathHash == EnemyControlloer.ANISTS_DMG ||
             stateInfo.fullPathHash == EnemyControlloer.ANISTS_VIEW ||
            stateInfo.fullPathHash == EnemyControlloer.ANISTS_DEAD ||
            stateInfo.fullPathHash == EnemyControlloer.ANISTS_START)
        {
            return false;
        }

        return true;
    }

    public void SetAIState(ENEMYAISTS sts, float t)
    {
        m_AiState = sts;
        m_AiActionTimeStart = Time.fixedTime;
        m_AiActionTimeLength = t;
        MoveAction();
    }

    public void MoveAction()
    {
        switch (m_AiState)
        {
            case ENEMYAISTS.WAIT:
                m_EnemyAnimator.SetBool("IsRun", false);
                m_EnemyControll.EnemyIdel();
                break;
            case ENEMYAISTS.RUNTOPLAYER:
                m_EnemyAnimator.SetBool("IsRun", true);
                m_EnemyControll.EnemyRun(m_Player);
                break;
            case ENEMYAISTS.ESCAPE:
                m_EnemyAnimator.SetBool("IsRun", false);
                m_EnemyControll.EnemyRun(null);
                break;
            case ENEMYAISTS.FREEZ:
                m_EnemyAnimator.SetBool("IsRun", false);
                break;
        }
    }
}
