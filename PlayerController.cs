using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public int m_PlayerHp { get; set; }
    public int m_PlayerAtk { get; set; }
    public int m_PlayerDef { get; set; }
    public int m_PlayerExp { get; set; }
    public double m_playerMoveSpeed { get; set; }
    public double m_playerAtkSpeed { get; set; }
    public double m_playerCriChance { get; set; }
    public double m_playerCriDamage { get; set; }
    public double m_playerHpRegen { get; set; }
    public  int PlayerHpMax { get; private set; }

    public readonly static int ANISTS_IDLE = Animator.StringToHash("Base Layer.Paleyr_Idel");
    public readonly static int ANISTS_MOVE = Animator.StringToHash("Base Layer.Paleyr_Move");
    public readonly static int ANISTS_RUN = Animator.StringToHash("Base Layer.Paleyr_Run");
    public readonly static int ANISTS_ATTACK_NOMAL = Animator.StringToHash("Base Layer.Paleyr_ATK_Nomal");
    public readonly static int ANISTS_ATTACK_CRI = Animator.StringToHash("Base Layer.Paleyr_ATK_Cri");
    public readonly static int ANISTS_ATTACK_SP = Animator.StringToHash("Base Layer.Paleyr_ATK_SP");
    public readonly static int ANISTS_DMG = Animator.StringToHash("Base Layer.Paleyr_DMG");
    public readonly static int ANISTS_DEAD = Animator.StringToHash("Base Layer.Paleyr_Dead");
    public readonly static int ANISTS_TRIED = Animator.StringToHash("Base Layer.Paleyr_Tried");
    public readonly static int ANISTS_ATK_WAITING = Animator.StringToHash("Base Layer.Paleyr_Atk_Waiting");
    public readonly static int ANISTS_VICTORY = Animator.StringToHash("Base Layer.Paleyr_Victory");


    private Animator m_aniControll;
    public BoxCollider m_AtteckCollider;
    public CapsuleCollider m_PlayerCollider;
    private ParticleSystem m_ATK_Particle;
    private ParticleSystem m_ATK_possible_Particle;

    private bool m_IsMoving = false;
    private bool m_IsATK_Particle = false;
    private bool m_IsATK_Possible_Particle = false;

    public bool m_ATK_Cri_DMG { get; private set; }
    public List<GameObject> m_EnemyList { get; set; }
    private InGameUIManager m_UiManager;

    private bool m_EndPanel;

    // Use this for initialization
    void Awake()
    {
        PlayerBean bean = BeanManager.instance.m_PlayerBean;
        m_PlayerHp = bean.m_PlayerHp;
        PlayerHpMax = bean.m_PlayerHp;
        m_PlayerAtk = bean.m_PlayerAtk + bean.m_PlayerWeapoin.m_ItemAbility;
        m_PlayerDef = bean.m_PlayerDef + bean.m_PlayerArmor.m_ItemAbility + bean.m_PlayerHelmet.m_ItemAbility + bean.m_PlayerBoot.m_ItemAbility;
        m_PlayerExp = bean.m_PlayerExp;
        m_playerMoveSpeed = bean.m_playerMoveSpeed;
        m_playerAtkSpeed = bean.m_playerAtkSpeed;
        m_playerCriChance = bean.m_playerCriChance;
        m_playerCriDamage = bean.m_playerCriDamage;
        m_playerHpRegen = bean.m_playerHpRegen;

        m_ATK_Cri_DMG = false;
        m_AtteckCollider.gameObject.SetActive(false);
        m_aniControll = PlayerController.GetAnimator();
        m_ATK_Particle = MemoryPool.instance.getObj("ATK_Particle").GetComponent<ParticleSystem>();
        m_ATK_possible_Particle = MemoryPool.instance.getObj("ATK_SP_possible").GetComponent<ParticleSystem>();
        m_ATK_Particle.gameObject.SetActive(false);
        m_ATK_possible_Particle.gameObject.SetActive(false);

        m_UiManager = GameObject.Find("InGameUImanager").GetComponent<InGameUIManager>();
    }

    private void OnDrawGizmos()
    {
        //  CollisionCheck();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        AnimatorStateInfo stateInfo = PlayerController.GetAnimator().GetCurrentAnimatorStateInfo(0);

        if (m_IsMoving)
        {

            if (stateInfo.fullPathHash == ANISTS_MOVE || stateInfo.fullPathHash == ANISTS_RUN)
            {

                if (CollisionCheck())
                {
                    float speed = stateInfo.fullPathHash == PlayerController.ANISTS_MOVE ? 0.25f : 0.5f;
                    transform.Translate(Vector3.forward * (float)m_playerMoveSpeed * speed * Time.fixedDeltaTime);
                }
            }
        }

        if (m_IsATK_Particle)
        {
            m_ATK_Particle.gameObject.transform.position = m_AtteckCollider.transform.position;

            if (m_IsATK_Possible_Particle)
                m_ATK_possible_Particle.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
    }

    private bool CollisionCheck()
    {

        float maxDistance = m_PlayerCollider.radius * 1.5f;
        Vector3[] position = new Vector3[] { transform.localPosition + new Vector3(0, m_PlayerCollider.height / 2, 0), transform.localPosition + new Vector3(m_PlayerCollider.radius * 0.1f, m_PlayerCollider.height / 2, 0), transform.localPosition + new Vector3(-m_PlayerCollider.radius * 0.1f, m_PlayerCollider.height / 2, 0) };

        RaycastHit hit;
        // Physics.Raycast (레이저를 발사할 위치, 발사 방향, 충돌 결과, 최대 거리)

        /*
        for (int i = 0; i < 3; i++)
        {
            bool isHit = Physics.BoxCast(position[i], transform.lossyScale / 2, transform.forward, out hit, transform.rotation, maxDistance);

            Gizmos.color = Color.red;

            Gizmos.DrawRay(position[i], transform.forward * maxDistance);
        }
        return true;
*/
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(position[i], transform.forward, out hit, maxDistance))
            {
                return false;
            }
        }
        return true;


    }


    public void EnemyApplyDamage(int Damage)
    {
        m_PlayerHp -=  Damage;
        m_UiManager.PlayerBarUpdate(PlayerHpMax, m_PlayerHp);
        if (m_PlayerHp <= 0)
        {
            PlayerDead();
        }

    }


    public void MoveAngle(Vector3 Angle)
    {
        transform.eulerAngles = Angle;
    }

    public void SetMoving(bool Type)
    {
        m_IsMoving = Type;
    }



    public void SetIsAtteck(int Type)
    {
        m_AtteckCollider.gameObject.SetActive(Type == 1 ? true : false);
    }

    public bool IsGetMoving()
    {
        return m_IsMoving;
    }

    public void PaleyrIdel()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_aniControll.SetTrigger("Idel");
    }

    public void PlayerMove()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_aniControll.SetTrigger("Move");
    }

    public void PlayerRun()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_aniControll.SetTrigger("Run");
    }

    public void PlayerThired()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_aniControll.SetTrigger("Tried");

    }

    public void PlayerDead()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_aniControll.SetTrigger("Dead");
        m_EndPanel = false;
        Invoke("EndPanel", 3.0f);
    }

    public void PlayerVictory()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_aniControll.SetTrigger("Victory");
        m_EndPanel = true;
        Invoke("EndPanel", 3.0f);
    }

    public void PlayerATK_NOMAL()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_ATK_Cri_DMG = false;
        m_IsATK_Possible_Particle = false;
        m_IsATK_Particle = false;
        m_aniControll.SetTrigger("ATK_Nomal");

    }

    public void PlayerATK_Cri()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_ATK_Cri_DMG = true;
        m_IsATK_Possible_Particle = false;
        m_IsATK_Particle = false;
        m_aniControll.SetTrigger("ATK_Cri");

    }

    public void PlayerATK_Sp()
    {
        Vector3 pos = transform.eulerAngles;
        pos.x = 0f;
        transform.eulerAngles = pos;
        m_ATK_Cri_DMG = false;
        m_IsATK_Possible_Particle = false;
        m_IsATK_Particle = false;
        m_aniControll.SetTrigger("ATK_Sp");

    }

    public void PlayerATK_Waiting()
    {
        m_aniControll.SetTrigger("Atk_Waiting");
    }

    public bool Get_ATK_SP_Particle()
    {
        return m_IsATK_Particle;
    }

    public void Set_ATK_SP_Particle(bool Type)
    {
        m_ATK_Particle.gameObject.transform.position = m_AtteckCollider.transform.position;
        m_ATK_Particle.gameObject.SetActive(Type);
        m_IsATK_Particle = Type;
    }

    public void Set_ATK_Possible_Particle(bool Type)
    {
        m_ATK_possible_Particle.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        m_ATK_possible_Particle.gameObject.SetActive(Type);
        m_IsATK_Possible_Particle = Type;
    }

    public void EnemyLootAt()
    {
        float lenth = 1000000f;
        GameObject Enemy = null;

        if (m_EnemyList == null)
            return;

        foreach(var v in m_EnemyList)
        {
            if(v.gameObject.activeSelf == false)
            {
                continue;
            }

            if (Vector3.Distance(transform.localPosition , v.transform.localPosition)<lenth)
            {
                Enemy = v;
                lenth = Vector3.Distance(transform.localPosition, v.transform.localPosition);
            }
        }

        if (Vector3.Distance(transform.localPosition, Enemy.transform.localPosition) < 0.3f)
        {
            if(Enemy != null)
                transform.LookAt(Enemy.transform);
        }

        //transform.LookAt(GameObject.Find("TT_zombie_M_15").transform);

    }


    public void EndPanel()
    {
        if(m_EndPanel )
        {
            m_UiManager.GameEndPanel(true);
        }
        else
        {
            m_UiManager.GameEndPanel(false);
        }
    }


    public void TriggerReset(string name)
    {
        m_aniControll.ResetTrigger(name);
    }

    public static GameObject GetGameObject()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static Transform GetTransform()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }

    public static PlayerController GetPaleyrController()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public static Animator GetAnimator()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    public static PadManager GetPadManger()
    {
        return GameObject.FindGameObjectWithTag("PadManager").GetComponent<PadManager>();
    }
}
