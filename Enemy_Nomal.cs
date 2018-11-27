using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Nomal : Enemy {
    public readonly float ATTACKSPEED = 4f;
    public readonly float ATTACKLenth = 0.5f;

    public override void FixedUpdateAI()
    {
        switch (m_AiState)
        {
            case ENEMYAISTS.ACTIONSELECT:
                {
                   
                    if (m_EnemyControll.ActionMoveToNear(m_Player, ATTACKLenth))//플레이어 범위내 접근
                    {
                        Attack();
                        SetAIState(ENEMYAISTS.FREEZ, ATTACKSPEED + (float)m_EnemyControll.m_EnemyAtkSpeed);
                  
                    }
                    else if (m_EnemyControll.ActionMoveToNear(m_Player, 8.0f))
                    {
                        SetAIState(ENEMYAISTS.RUNTOPLAYER, 0.5f);
                    }
                    else if (m_EnemyControll.ActionMoveESCAPE())
                    {
                        SetAIState(ENEMYAISTS.ESCAPE, 1.0f);
                    }
                    else
                    {
                        SetAIState(ENEMYAISTS.WAIT, 5f);
                    }
                }
                break;
            case ENEMYAISTS.WAIT:
                if (m_EnemyControll.ActionMoveToNear(m_Player, ATTACKLenth))
                {
                    Attack();
                    SetAIState(ENEMYAISTS.FREEZ, ATTACKSPEED + (float)m_EnemyControll.m_EnemyAtkSpeed);
          
                }
                else if (m_EnemyControll.ActionMoveToNear(m_Player, 8.0f))
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 0.5f);
                }
                
                break;
            case ENEMYAISTS.RUNTOPLAYER:
                if (m_EnemyControll.ActionMoveToNear(m_Player, ATTACKLenth))
                {
                    Attack();
                    SetAIState(ENEMYAISTS.FREEZ, ATTACKSPEED + (float)m_EnemyControll.m_EnemyAtkSpeed);
       
                }

                break;
            case ENEMYAISTS.ESCAPE:
                if (Vector3.Distance(transform.localPosition, m_EnemyControll.m_FirstPosition.transform.localPosition) < 0.5f)
                    SetAIState(ENEMYAISTS.ACTIONSELECT,0f);
                break;
        }
    }

    public void Attack()
    {
        if (Random.Range(0, 100) - m_EnemyControll.m_EnemyAtkCri <= 0)
            m_EnemyControll.EnemyAtteck_Cri();
        else
            m_EnemyControll.EnemyAtteck_Nomal();
    }
}
