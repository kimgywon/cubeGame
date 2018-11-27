using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControlloer : MonoBehaviour
{
    public string m_EnemyName { get; set; } //EnemyManger에서 받아야함
    public int m_EnemyHp { get; set; }
    public int m_EnemyDef { get; set; }
    public double m_EnemySpeed { get; set; }
    public int m_EnemyAtk { get; set; }
    public double m_EnemyAtkSpeed { get; set; }
    public double m_ViewTiem { get; set; }
    public int m_EnemyAtkCri { get; set; }
    public int m_EnemyWabe { get; set; }                //EnemyManger에서 받아야함

    public bool m_ATK_Cri_DMG { get; private set; }

    public GameObject m_FirstPosition { get; set; }

    public readonly static int ANISTS_START = Animator.StringToHash("Base Layer.Enemy_Start");
    public readonly static int ANISTS_VIEW = Animator.StringToHash("Base Layer.Enemy_View");
    public readonly static int ANISTS_IDLE = Animator.StringToHash("Base Layer.Enemy_Idel");
    public readonly static int ANISTS_RUN = Animator.StringToHash("Base Layer.Enemy_Run");
    public readonly static int ANISTS_ATTACK_NOMAL = Animator.StringToHash("Base Layer.Enemy_Attack_Nomal");
    public readonly static int ANISTS_ATTACK_CRI = Animator.StringToHash("Base Layer.Enemy_Attack_Cri");
    public readonly static int ANISTS_DMG = Animator.StringToHash("Base Layer.Enemy_DMG");
    public readonly static int ANISTS_DEAD = Animator.StringToHash("Base Layer.Enemy_Dead");


    private NavMeshAgent m_nav;

    private Animator m_aniControll;
    public BoxCollider m_AtteckCollider;

    private bool m_IsMoving = false;
    private GameObject m_Target;

    private NavMeshData m_NavMeshData;
    protected PlayerController m_PlayerControll;


    private ParticleSystem m_Enemy_End;
    private ParticleSystem m_Enemy_Shoot_Effect;
    private ParticleSystem m_Player_Shoot_Effect;

    private InGameUIManager m_UiManager;
    private UISprite m_EnemyArrow;

    private bool m_PlayerAtk = false;
    private Vector3 m_ViewVector;

    private Camera m_Uicamera;

    // Use this for initialization
    void Start()
    {
        //m_EnemyHp = 100; // ingame 만들면 지워야함 
        //m_EnemySpeed = 1.2f; // ingame 만들면 지워야함 

        //PlayerPrefs.SetInt("stage", 1);  // ingame 만들면 지워야함 

        //string path = "Prefab/NavMesh/NavMesh " + PlayerPrefs.GetInt("stage");
        //m_NavMeshData = Resources.Load(path) as NavMeshData;
        //NavMesh.AddNavMeshData(m_NavMeshData);

        //m_aniControll = GetComponent<Animator>();
        //m_nav = GetComponent<NavMeshAgent>();
        //m_FirstPosition = new GameObject();
        //m_FirstPosition.transform.localPosition = transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        //m_nav.speed = (float)m_EnemySpeed;
        //m_AtteckCollider.enabled = false;

        //m_Enemy_End = MemoryPool.instance.getObj("EnemyEnd").GetComponent<ParticleSystem>();
        //m_Enemy_Shoot_Effect = MemoryPool.instance.getObj("Enemy_Shoot_Effect").GetComponent<ParticleSystem>();
        //m_Player_Shoot_Effect = MemoryPool.instance.getObj("Pleyer_Shoot_Effect").GetComponent<ParticleSystem>();
        //m_Player_Shoot_Effect.Pause();
        //m_Enemy_Shoot_Effect.Pause();
        //m_Enemy_End.gameObject.SetActive(false);
        //m_PlayerControll = PlayerController.GetPaleyrController();

        //m_ATK_Cri_DMG = false;

        //m_UiManager = GameObject.Find("InGameUImanager").GetComponent<InGameUIManager>();
        //m_EnemyArrow = m_UiManager.GetEnemySpritel().GetComponent<UISprite>();

    }

    public void EnemyInit()
    {
        EnemyBean bean = null;
        foreach (var v in BeanManager.instance.m_EnemyBean)
        {
            if (v.m_EnemyName == m_EnemyName)
            {
                bean = v;
                break;
            }
        }

        //m_EnemyHp = bean.m_EnemyHp;
        m_EnemyHp = 100;
        // m_EnemyDef = bean.m_EnemyDef;
        m_EnemyDef = 0;
         m_EnemySpeed = bean.m_EnemySpeed;
        // m_EnemyAtk = bean.m_EnemyAtk + 10;
        m_EnemyAtk = 25;
        m_EnemyAtkSpeed = bean.m_EnemyAtkSpeed;
        m_EnemyAtkCri = bean.m_EnemyAtkCri;

        string path = "Prefab/NavMesh/NavMesh " + PlayerPrefs.GetInt("stage");
        m_NavMeshData = Resources.Load(path) as NavMeshData;
        NavMesh.AddNavMeshData(m_NavMeshData);

        m_aniControll = GetComponent<Animator>();
        m_nav = GetComponent<NavMeshAgent>();
        m_FirstPosition = new GameObject("FirstPosition");
        m_FirstPosition.transform.parent = transform;
        m_FirstPosition.transform.localPosition  = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        m_nav.speed = (float)m_EnemySpeed + Random.Range(0.1f,0.4f);
        m_AtteckCollider.enabled = false;

        m_Enemy_End = MemoryPool.instance.getObj("EnemyEnd").GetComponent<ParticleSystem>();
        m_Enemy_Shoot_Effect = MemoryPool.instance.getObj("Enemy_Shoot_Effect").GetComponent<ParticleSystem>();
        m_Player_Shoot_Effect = MemoryPool.instance.getObj("Pleyer_Shoot_Effect").GetComponent<ParticleSystem>();
        m_Player_Shoot_Effect.Pause();
        m_Enemy_Shoot_Effect.Pause();
        m_Enemy_End.gameObject.SetActive(false);
        m_PlayerControll = PlayerController.GetPaleyrController();

        m_ATK_Cri_DMG = false;

        m_UiManager = GameObject.Find("InGameUImanager").GetComponent<InGameUIManager>();

        m_Uicamera = GameObject.FindGameObjectWithTag("UIMainCamera").GetComponent<Camera>();
        m_EnemyArrow = m_UiManager.GetEnemySpritel().GetComponent<UISprite>();
        m_ViewVector = new Vector3(NGUITools.screenSize.x / 2, NGUITools.screenSize.y / 2, 0);
        //m_ViewVector = new Vector3(Screen.width / 2, Screen.height / 2, 0); //안드로이드버젼
        gameObject.GetComponent<Enemy_Nomal>().EnemyInit();

 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_EnemyHp <= 0)
            return;

        if (m_IsMoving)
        {
            m_nav.SetDestination(m_Target.transform.position);
        }

        AnimatorStateInfo stateInfo = m_aniControll.GetCurrentAnimatorStateInfo(0);

        //if (stateInfo.fullPathHash != EnemyControlloer.ANISTS_VIEW)
        //{
        //    Vector3 position = Camera.main.WorldToViewportPoint(transform.localPosition);

        //    if (Mathf.Abs(position.x)>1 || Mathf.Abs(position.y) > 1)
        //        EnemyArrow(position);
        //}

        EnemyArrow();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_EnemyHp <= 0)
            return;

        if (other.tag == "PlayerHitBox")
        {
            float  Player_DMG = m_PlayerControll.m_ATK_Cri_DMG == false ? (float)m_PlayerControll.m_PlayerAtk : (float)m_PlayerControll.m_PlayerAtk + (float)m_PlayerControll.m_playerCriDamage;
            int DMG = Mathf.Abs(m_EnemyDef - (int)Player_DMG);
            m_Enemy_Shoot_Effect.gameObject.transform.position = other.gameObject.transform.position;
            m_Enemy_Shoot_Effect.Stop();
            EnemyApplyDamage(DMG);
        }
        else if(other.tag == "PlayerBody" && m_AtteckCollider.enabled == true)
        {
            if (m_PlayerControll.m_PlayerHp <= 0)
                return;
            if (m_PlayerAtk)
                return;

            m_PlayerAtk = true;
            m_AtteckCollider.enabled = false;
            m_Player_Shoot_Effect.transform.position = m_AtteckCollider.transform.position;
            m_Player_Shoot_Effect.Play();

            float Player_DMG = m_ATK_Cri_DMG == false ? (float)m_EnemyAtk : (float)m_EnemyAtk + 10f;
            int DMG = Mathf.Abs( m_PlayerControll.m_PlayerDef - (int)Player_DMG);

            m_PlayerControll.EnemyApplyDamage(DMG <=0 ? 1 : DMG);
        }
    }

    public void EnemyArrow()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.localPosition);

        Vector3 position2 = (Camera.main.WorldToScreenPoint(transform.localPosition) + (Vector3.up * 1.5f)) - m_ViewVector;

        Color com = m_EnemyArrow.color;
        com.a = 1;
        m_EnemyArrow.color = com;

        if (position.x < 0)
        {
            m_EnemyArrow.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            m_EnemyArrow.gameObject.transform.localPosition = new Vector3(-613, Mathf.Abs(position2.y) < 325 ? position2.y : position2.y >0 ? 325 : -325, 0);
        }
        else if (position.x > 1)
        {
            m_EnemyArrow.gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
            m_EnemyArrow.gameObject.transform.localPosition = new Vector3(613, Mathf.Abs(position2.y) < 325 ? position2.y : position2.y > 0 ? 325 : -325, 0);
        }
        else if (position.y < 0)
        {
            m_EnemyArrow.gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
            m_EnemyArrow.gameObject.transform.localPosition = new Vector3(Mathf.Abs(position2.x) < 613 ? position2.x : position2.x > 0 ? 613 : -613, -325, 0);
        }
        else if (position.y > 1)
        {
            m_EnemyArrow.gameObject.transform.eulerAngles = new Vector3(0, 0, 270);
            m_EnemyArrow.gameObject.transform.localPosition = new Vector3(Mathf.Abs(position2.x) < 613 ? position2.x : position2.x > 0 ? 613 : -613, 325, 0);
        }
        else
        {
            com.a = 0;
            m_EnemyArrow.color = com;
        }
    }


    public void EnableNav()
    {
        m_nav.enabled = true;
    }

    public void EnemyIdel()
    {
        m_nav.enabled = false;
        m_IsMoving = false;
        m_PlayerAtk = false;
        m_aniControll.SetTrigger("Idle");
    }

    public void EnemyRun(GameObject Target)
    {
        if (Target == null)
            m_Target = m_FirstPosition;
        else
            m_Target = Target;

        m_nav.enabled = true;
        m_IsMoving = true;
        m_PlayerAtk = false;
        m_aniControll.SetTrigger("Run");
    }

    public void EnemyAtteck_Nomal()
    {
        m_nav.enabled = false;
        m_IsMoving = false;
        m_ATK_Cri_DMG = false;
        m_PlayerAtk = false;
        m_aniControll.SetTrigger("Attack_Nomal");
    }

    public void EnemyAtteck_Cri()
    {
        m_nav.enabled = false;
        m_IsMoving = false;
        m_ATK_Cri_DMG = true;
        m_PlayerAtk = false;
        m_aniControll.SetTrigger("Attack_Cri");
    }

    public void EnemyDMG()
    {
        m_nav.enabled = false;
        m_IsMoving = false;
        m_PlayerAtk = false;
        m_aniControll.SetTrigger("DMG");
    }

    public void EnemyDead()
    {
        m_nav.enabled = false;
        m_IsMoving = false;
        m_UiManager.GameScoreUpdate(100);
        m_aniControll.SetTrigger("Dead");
    }


    public void SetIsAtteck(int Type)
    {
        m_AtteckCollider.enabled = Type == 1 ? true : false;
    }

    public void EnemyApplyDamage(int Damage)
    {
        DMGLabel label = m_UiManager.GetEnemyLabel().GetComponent<DMGLabel>();

        //Vector3 p = Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * 1.5f));
        //Vector3 uip = m_Uicamera.ScreenToWorldPoint(p);
        //uip.z = 0; 

        Vector3 ap;

        if(m_PlayerControll.transform.localPosition.x >transform.localPosition.x)
        {
            ap = Vector3.right * 0.2f;
        }
        else
        {
            ap = Vector3.left * 0.35f;
        }

        Vector3 position2 = Camera.main.WorldToScreenPoint(transform.localPosition + (Vector3.up * 1.1f) + ap) - m_ViewVector;

        label.init(position2, Damage, m_PlayerControll.m_ATK_Cri_DMG);

        m_EnemyHp -= Mathf.Abs(m_EnemyDef - Damage);

        m_Enemy_Shoot_Effect.Play();

        if (m_EnemyHp <= 0)
        {
            int num = Random.Range(0, 100);
            if (40 > num)
            {
                GameObject Coin = MemoryPool.instance.getObj("EventCoin");
                Coin.GetComponent<Coin>().init(transform.position);
            }

            EnemyDead();
        }
        else
        {
            AnimatorStateInfo stateInfo = m_aniControll.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.fullPathHash == EnemyControlloer.ANISTS_ATTACK_NOMAL ||
                stateInfo.fullPathHash == EnemyControlloer.ANISTS_ATTACK_CRI)
                return;

            EnemyDMG();
        }
    }

    public void EnemyEnd()
    {
        gameObject.SetActive(false);
        m_Enemy_End.gameObject.SetActive(false);
    }

    public void EnemyFadeOut()
    {
        StartCoroutine(PadeOut());
    }

    IEnumerator PadeOut()
    {
        float time = 0f;

        Vector3 a = transform.position;
        Vector3 b = transform.position + new Vector3(0f, -1.5f, 0f);

        while (time <= 1f)
        {
            time += Time.deltaTime;

            transform.position = Vector3.Lerp(a, b, time);

            yield return null;
        }
    }
    public void EnemyEndParticle()
    {
        m_Enemy_End.gameObject.transform.position = transform.position + new Vector3(0f,0.3f,0f);
        m_Enemy_End.gameObject.SetActive(true);
    }

    public bool ActionMoveToNear(GameObject go, float near)
    {
        if (Vector3.Distance(transform.position, go.transform.position) < near)
        {
            return true;
        }
        return false;
    }

    public bool ActionMoveToFar(GameObject go, float Far)
    {
        if (Vector3.Distance(transform.position, go.transform.position) > Far)
        {
            return true;
        }
        return false;
    }

    public bool ActionMoveESCAPE()
    {
        if (Vector3.Distance(transform.localPosition, m_FirstPosition.transform.localPosition) > 0.5f)
            return true;
        else
            return false;
    }
}
