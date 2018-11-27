using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct TouchCheck
{
    public bool Start;
    public bool Touch;
    public bool Press;
}

public class PadManager : MonoBehaviour
{

    private PlayerController m_PlayerController;
    private float m_Radius;
    private Dictionary<string, Vector3> FirstPosition;
    private bool m_IsRunButtonPress;
    private UISprite m_RunEfeectSprite;

    [HideInInspector]
    public List<GameObject> m_PusseButton;
    public ParticleSystem[] m_Particle;

    private float m_Start_FreesTime;
    private float m_FreesTime_Length;
    private float m_ATK_START_TIME;
    private readonly float m_ATK_SP_TIME = 2f;
    private bool m_ATKAngle = false;
    private bool m_ATK_SP = false;
    private bool m_ATK = false;

    private Vector3 moveposition;

    private TouchCheck[] m_TuochCheck;

    private Touch tempTouchs;
    // Use this for initialization
    void Start()
    {
        FirstPosition = new Dictionary<string, Vector3>();
        m_TuochCheck = new TouchCheck[2];

        for (int i = 0; i < m_TuochCheck.Length; i++)
        {
            m_TuochCheck[i].Start = false;
            m_TuochCheck[i].Touch = false;
            m_TuochCheck[i].Press = false;
        }

        m_PlayerController = PlayerController.GetPaleyrController();
        m_PusseButton.Add(transform.GetChild(1).gameObject);
        m_PusseButton.Add(transform.GetChild(3).gameObject);
        m_PusseButton.Add(transform.GetChild(4).gameObject);

        foreach (var v in m_PusseButton)
        {
            UIEventListener.Get(v).onPress = OnPress;
            UIEventListener.Get(v).onDrag = OnDrag;
            FirstPosition.Add(v.name, v.transform.localPosition);
        }
        m_RunEfeectSprite = transform.GetChild(5).gameObject.GetComponent<UISprite>();
        m_IsRunButtonPress = false;
        m_Radius = m_PusseButton[0].GetComponent<UISprite>().localSize.y * 0.5f;

        m_PlayerController.PaleyrIdel(); //ingamemanager 만들면 지워야함 


    }

    void FixedUpdate()
    {
        if (m_Start_FreesTime != 0)
            FressCheck();


        for (int j = 0; j < m_TuochCheck.Length; j++)
        {
            if (m_TuochCheck[j].Start)
                m_TuochCheck[j].Touch = false;
        }

        bool touchPress = false;

        //      /*
        for (int i = 0; i < Input.touchCount; i++)
        {
            tempTouchs = Input.GetTouch(i);

            Vector2 pos = tempTouchs.position;    // 터치한 위치
            Vector3 theTouch = new Vector3(pos.x, pos.y, 0.0f);    // 변환 안하고 바로 Vector3로 받아도 되겠지.

           if (UICamera.Raycast(theTouch) == false)
            {
                if (pos.x > Screen.width / 2)
                {
                    if (m_TuochCheck[0].Start == true || m_TuochCheck[1].Start)
                        touchPress = true;
                }
                continue;
            }


            RaycastHit hit;
            Ray ray = UICamera.mainCamera.ScreenPointToRay(theTouch);

            if (Physics.Raycast(ray, out hit, 100))    // 레이저를 끝까지 쏴블자. 충돌 한넘이 있으면 return true다.
            {
                if (hit.collider.name == "Move_Pad_Con")
                    continue;


                if (tempTouchs.phase == TouchPhase.Began)    // 딱 처음 터치 할때 발생한다
                {
                    if (hit.collider.name == "Run_Pad_Bg")
                    {
                        m_TuochCheck[0].Start = true;
                        m_TuochCheck[0].Touch = true;

                        if (m_IsRunButtonPress == true)
                            return;

                        if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                            return;

                        m_IsRunButtonPress = true;

                        GameButtonManager.instance.ButtonDown(m_PusseButton[2]);

                        ActionReset(new int[] { 0, 1, 2, 9 });
                        m_PlayerController.PlayerRun();
                    }
                    else if (hit.collider.name == "ATK_Pad_Con")
                    {
                        m_TuochCheck[1].Start = true;
                        m_TuochCheck[1].Touch = true;

                        if (m_ATK)
                            return;

                        if (m_Start_FreesTime != 0)
                            return;
                        if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 10 }))
                            return;

                        m_PlayerController.Set_ATK_SP_Particle(true);

                        GameButtonManager.instance.ButtonDown(m_PusseButton[1]);
                        m_ATK_START_TIME = Time.fixedTime;
                        m_ATK = true;

                    }
                }
                else if (tempTouchs.phase == TouchPhase.Stationary)    // 터치하고 대기 발생한다.
                {
                    if (hit.collider.name == "Run_Pad_Bg")
                    {
                        m_TuochCheck[0].Touch = true;

                        if (ActionCheck(new int[] { 3, 4, 5 }) == false)
                        {
                            GameButtonManager.instance.ButtonUp(m_PusseButton[2]);
                            m_IsRunButtonPress = false;
                            m_Particle[0].gameObject.SetActive(false);
                            m_PlayerController.SetMoving(false);
                        }
                    }
                    else if (hit.collider.name == "ATK_Pad_Con")
                    {
                        m_TuochCheck[1].Touch = true;

                        if (!m_ATK)
                            return;

                        if (m_ATK_START_TIME == 0)
                            return;

                        if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                            return;

                        float time = Time.fixedTime - m_ATK_START_TIME;
                        if (time > m_ATK_SP_TIME) //특별공격
                        {
                            m_PlayerController.Set_ATK_Possible_Particle(true);
                        }
                    }
                }
                else if (tempTouchs.phase == TouchPhase.Ended)    // 터치 따악 떼면 발생한다.
                {
                    if (hit.collider.name == "Run_Pad_Bg")
                    {
                        if (m_TuochCheck[0].Start == false)
                            return;

                        m_TuochCheck[0].Start = false;
                        m_TuochCheck[0].Touch = false;
                        m_TuochCheck[0].Press = false;

                        if (m_IsRunButtonPress == false)
                            return;

                        if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                            return;

                        GameButtonManager.instance.ButtonUp(m_PusseButton[2]);
                        m_IsRunButtonPress = false;
                        m_Particle[0].gameObject.SetActive(false);

                        ActionReset(new int[] { 0, 1, 2, 9 });

                        if (m_PlayerController.IsGetMoving())
                        {
                            m_PlayerController.PlayerMove();
                        }
                        else
                        {
                            m_PlayerController.PaleyrIdel();
                        }
                    }
                    else if (hit.collider.name == "ATK_Pad_Con")
                    {
                        if (m_TuochCheck[1].Start == false)
                            return;

                        m_TuochCheck[1].Start = false;
                        m_TuochCheck[1].Touch = false;
                        m_TuochCheck[1].Press = false;

                        if (!m_ATK)
                            return;

                        if (m_ATK_START_TIME == 0)
                            return;

                        if (m_Start_FreesTime != 0)
                            return;
                        if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 10 }))
                            return;
                        m_PlayerController.Set_ATK_SP_Particle(false);
                        m_PlayerController.Set_ATK_Possible_Particle(false);
                        m_PlayerController.EnemyLootAt();
                        m_PlayerController.SetMoving(false);
                        m_ATKAngle = true;
                        float time = Time.fixedTime - m_ATK_START_TIME;
                        m_ATK_START_TIME = 0;
                        ActionReset(new int[] { 0, 1, 2, 9 });

                        if (time > m_ATK_SP_TIME) //특별공격
                        {
                            // SetFress(1.4f + (float)m_PlayerController.m_playerAtkSpeed);
                            SetFress(2.5f);

                            m_PlayerController.PlayerATK_Sp();
                            m_ATK_SP = true;
                        }
                        else
                        {
                            SetFress(0.5f + (float)m_PlayerController.m_playerAtkSpeed);

                            if (m_PlayerController.m_playerCriChance > Random.Range(1, 100))  //크리티컬 공격
                            {
                                m_PlayerController.PlayerATK_Cri();
                            }
                            else  //일반 공격
                            {
                                m_PlayerController.PlayerATK_NOMAL();
                            }
                        }
                    }
                }
            }

        }

        if (touchPress)
        { 
            if (m_TuochCheck[0].Start == true && m_TuochCheck[0].Touch == false )
            {
                if (m_TuochCheck[0].Start == false)
                    return;

                m_TuochCheck[0].Start = false;
                m_TuochCheck[0].Touch = false;
                m_TuochCheck[0].Press = false;
                if (m_IsRunButtonPress == false)
                    return;

                if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                    return;

                GameButtonManager.instance.ButtonUp(m_PusseButton[2]);
                m_IsRunButtonPress = false;
                m_Particle[0].gameObject.SetActive(false);

                ActionReset(new int[] { 0, 1, 2, 9 });

                if (m_PlayerController.IsGetMoving())
                {
                    m_PlayerController.PlayerMove();
                }
                else
                {
                    m_PlayerController.PaleyrIdel();
                }
            }

            if (m_TuochCheck[1].Start == true && m_TuochCheck[1].Touch == false)
            {
                if (m_TuochCheck[1].Start == false)
                    return;

                m_TuochCheck[1].Start = false;
                m_TuochCheck[1].Touch = false;
                m_TuochCheck[1].Press = false;
                if (!m_ATK)
                    return;

                if (m_ATK_START_TIME == 0)
                    return;

                if (m_Start_FreesTime != 0)
                    return;
                if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 10 }))
                    return;
                m_PlayerController.Set_ATK_SP_Particle(false);
                m_PlayerController.Set_ATK_Possible_Particle(false);
                m_PlayerController.EnemyLootAt();
                m_PlayerController.SetMoving(false);
                m_ATKAngle = true;
                float time = Time.fixedTime - m_ATK_START_TIME;
                m_ATK_START_TIME = 0;
                ActionReset(new int[] { 0, 1, 2, 9 });

                if (time > m_ATK_SP_TIME) //특별공격
                {
                    // SetFress(1.4f + (float)m_PlayerController.m_playerAtkSpeed);
                    SetFress(2.5f);

                    m_PlayerController.PlayerATK_Sp();
                    m_ATK_SP = true;
                }
                else
                {
                    SetFress(0.5f + (float)m_PlayerController.m_playerAtkSpeed);

                    if (m_PlayerController.m_playerCriChance > Random.Range(1, 100))  //크리티컬 공격
                    {
                        m_PlayerController.PlayerATK_Cri();
                    }
                    else  //일반 공격
                    {
                        m_PlayerController.PlayerATK_NOMAL();
                    }
                }
            }
        }

        //   */
        /*
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (m_IsRunButtonPress == true)
                return;

            if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                return;

            m_IsRunButtonPress = true;
            GameButtonManager.instance.ButtonDown(m_PusseButton[2]);

            ActionReset(new int[] { 0, 1, 2, 9 });
            m_PlayerController.PlayerRun();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (ActionCheck(new int[] { 3, 4, 5 }) == false)
            {
                GameButtonManager.instance.ButtonUp(m_PusseButton[2]);
                m_IsRunButtonPress = false;
                m_Particle[0].gameObject.SetActive(false);
                m_PlayerController.SetMoving(false);
            }
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (m_IsRunButtonPress == false)
                return;

            if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                return;

            GameButtonManager.instance.ButtonUp(m_PusseButton[2]);
            m_IsRunButtonPress = false;
            m_Particle[0].gameObject.SetActive(false);

            ActionReset(new int[] { 0, 1, 2, 9 });

            if (m_PlayerController.IsGetMoving())
            {
                m_PlayerController.PlayerMove();
            }
            else
            {
                m_PlayerController.PaleyrIdel();
            }

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (m_ATK)
                return;

            if (m_Start_FreesTime != 0)
                return;
            if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 10 }))
                return;

            m_PlayerController.Set_ATK_SP_Particle(true);

            GameButtonManager.instance.ButtonDown(m_PusseButton[1]);
            m_ATK_START_TIME = Time.fixedTime;
            m_ATK = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (!m_ATK)
                return;

            if (m_ATK_START_TIME == 0)
                return;

            if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }))
                return;

            float time = Time.fixedTime - m_ATK_START_TIME;
            if (time > m_ATK_SP_TIME) //특별공격
            {
                m_PlayerController.Set_ATK_Possible_Particle(true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            if (!m_ATK)
                return;

            if (m_ATK_START_TIME == 0)
                return;

            if (m_Start_FreesTime != 0)
                return;
            if (!ActionCheck(new int[] { 3, 4, 5, 6, 7, 8, 10 }))
                return;
            m_PlayerController.Set_ATK_SP_Particle(false);
            m_PlayerController.Set_ATK_Possible_Particle(false);
            m_PlayerController.EnemyLootAt();
            m_PlayerController.SetMoving(false);
            m_ATKAngle = true;
            float time = Time.fixedTime - m_ATK_START_TIME;
            m_ATK_START_TIME = 0;
            ActionReset(new int[] { 0, 1, 2, 9 });

            if (time > m_ATK_SP_TIME) //특별공격
            {
                // SetFress(1.4f + (float)m_PlayerController.m_playerAtkSpeed);
                SetFress(2.5f);

                m_PlayerController.PlayerATK_Sp();
                m_ATK_SP = true;
            }
            else
            {
                SetFress(0.5f + (float)m_PlayerController.m_playerAtkSpeed);

                if (m_PlayerController.m_playerCriChance > Random.Range(1, 100))  //크리티컬 공격
                {
                    m_PlayerController.PlayerATK_Cri();
                }
                else  //일반 공격
                {
                    m_PlayerController.PlayerATK_NOMAL();
                }
            }
        }
        */

        if (m_IsRunButtonPress)
        {
            if (!ActionCheck(new int[] { 8 }))
                return;

            m_Particle[0].gameObject.SetActive(true);
            m_Particle[1].gameObject.SetActive(false);
            m_RunEfeectSprite.fillAmount -= Time.deltaTime * 0.1f;

            if (m_RunEfeectSprite.fillAmount <= 0)
            {
                m_RunEfeectSprite.fillAmount = 0;
                m_Particle[0].gameObject.SetActive(false);
                StartCoroutine(PlayerTried());
            }
            else
            {
                float Pos_x;
                float Pos_y;
                float r = 360 * m_RunEfeectSprite.fillAmount;

                float angle = 360 - r - 270;
                Pos_x = m_PusseButton[2].transform.localPosition.x + (Mathf.Cos((angle) * (Mathf.PI / 180)) * (80));
                Pos_y = m_PusseButton[2].transform.localPosition.y + (Mathf.Sin((angle) * (Mathf.PI / 180)) * (80));

                m_Particle[0].transform.localPosition = new Vector3(Pos_x, Pos_y, 0);
            }
        }
        else if (!m_IsRunButtonPress && m_RunEfeectSprite.fillAmount != 1)
        {
            if (!ActionCheck(new int[] { 8 }))
                return;

            m_Particle[0].gameObject.SetActive(false);

            if (m_RunEfeectSprite.fillAmount >= 1)
            {
                m_RunEfeectSprite.fillAmount = 1;
            }
            else
            {
                m_Particle[1].gameObject.SetActive(true);
                m_RunEfeectSprite.fillAmount += Time.deltaTime * 0.1f;
                float Pos_x;
                float Pos_y;
                float r = 360 * m_RunEfeectSprite.fillAmount;

                float angle = 360 - r - 270;
                Pos_x = m_PusseButton[2].transform.localPosition.x + (Mathf.Cos((angle) * (Mathf.PI / 180)) * (80));
                Pos_y = m_PusseButton[2].transform.localPosition.y + (Mathf.Sin((angle) * (Mathf.PI / 180)) * (80));

                m_Particle[1].transform.localPosition = new Vector3(Pos_x, Pos_y, 0);
            }
        }
        else
        {
            m_Particle[0].gameObject.SetActive(false);
            m_Particle[1].gameObject.SetActive(false);
        }
    }


    public void OnPress(GameObject sender, bool isDown)
    {
        if (isDown) // button Down
        {
            switch (sender.name)
            {
                case "Move_Pad_Con":
                    if (!ActionCheck(new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 }))
                        return;

                    if (m_ATK_SP)
                    {
                        m_ATKAngle = false;
                        m_ATK_SP = false;
                    }

                    m_PlayerController.SetMoving(true);
                    moveposition = FirstPosition[sender.name];
                    ActionReset(new int[] { 0, 1, 9 });
                    m_PlayerController.PlayerMove();
                    break;
                case "Run_Pad_Bg":

                    break;
                case "ATK_Pad_Con":

                    break;
            }
        }
        else  //button up
        {
            switch (sender.name)
            {
                case "Move_Pad_Con":
                    sender.transform.localPosition = FirstPosition[sender.name];
                    m_PlayerController.SetMoving(false);

                    if (!ActionCheck(new int[] { 0, 3, 4, 5, 6, 7, 8, 10 }))
                    {
                        return;
                    }

                    moveposition = FirstPosition[sender.name];

                    ActionReset(new int[] { 1, 2, 9 });
                    m_PlayerController.PaleyrIdel();

                    break;
                case "Run_Pad_Bg":

                    break;
                case "ATK_Pad_Con":

                    break;
            }
            //플레이어 이동 멈춤
        }

    }

    public void PlayerStop()
    {
        ActionReset(new int[] { 1, 2, 9 });
        m_PlayerController.PaleyrIdel();
    }

    public void OnDrag(GameObject sender, Vector2 delta)
    {
        switch (sender.name)
        {
            case "Move_Pad_Con":
                if (ActionCheck(new int[] { 0, 7, 8, 10 }) == false)
                {
                    sender.transform.localPosition = FirstPosition[sender.name];
                    return;
                }
                if (ActionCheck(new int[] { 0, 1, 2, 9, 3, 4, 5 }) == false)
                {
                    if (ActionCheck(new int[] { 9 }) == false && m_ATKAngle == true)
                    {
                        ActionReset(new int[] { 9, 2, 1, 0 });
                        m_PlayerController.SetMoving(true);
                        m_PlayerController.PlayerMove();
                        m_ATKAngle = false;
                        m_ATK_SP = false;
                    }
                    else
                    {
                        moveposition += new Vector3(delta.x, delta.y, 0);

                        Vector3 Pos = (moveposition - FirstPosition[sender.name]).normalized;
                        float Dis = Vector3.Distance(moveposition, FirstPosition[sender.name]);

                        Transform pvetransform = sender.transform;

                        if (Dis < m_Radius)
                        {
                            sender.transform.localPosition = FirstPosition[sender.name] + Pos * Dis;
                        }
                        else
                        {
                            sender.transform.localPosition = FirstPosition[sender.name] + Pos * m_Radius;
                        }

                        if (m_ATKAngle == false)
                            m_PlayerController.MoveAngle(new Vector3(0, Mathf.Atan2(Pos.x, Pos.y) * Mathf.Rad2Deg, 0));
                    }
                }
                break;
            case "ATK_Pad_Con":

                break;

            case "Run_Pad_Bg":

                break;
        }

    }

    public void SetFress(float t)
    {
        m_Start_FreesTime = Time.fixedTime;
        m_FreesTime_Length = t;
    }

    public void FressCheck()
    {
        if (ActionCheck(new int[] { 3, 4, 6, 7, 8, 10 }) == false)
            return;

        float time = Time.fixedTime - m_Start_FreesTime;

        if (m_ATKAngle == false)
        {
            if (time > m_FreesTime_Length)
            {
                m_Start_FreesTime = 0f;
                m_ATK = false;
                GameButtonManager.instance.ButtonUp(m_PusseButton[1]);

                if (ActionCheck(new int[] { 9 }) == false)
                {
                    ActionReset(new int[] { 0, 1, 2, 9 });
                    m_PlayerController.PaleyrIdel();
                }
            }

            return;
        }

        if (time > m_FreesTime_Length)
        {
            m_Start_FreesTime = 0f;
            m_ATKAngle = false;
            m_ATK = false;
            GameButtonManager.instance.ButtonUp(m_PusseButton[1]);

            if (ActionCheck(new int[] { 9 }) == false)
            {
                ActionReset(new int[] { 0, 1, 2, 9 });
                m_PlayerController.PaleyrIdel();
            }
            else if (ActionCheck(new int[] { 5 }) == false && ActionCheck(new int[] { 0, 1, 2 }) == true)
            {
                SetFress((float)m_PlayerController.m_playerAtkSpeed);
                ActionReset(new int[] { 0, 1, 2, 5, 9 });


                m_PlayerController.PlayerATK_Waiting();
                GameButtonManager.instance.ButtonDown(m_PusseButton[1]);
                m_ATKAngle = true;
                m_ATK = true;
            }
        }
    }

    public IEnumerator PlayerTried()
    {
        // 여기에는 플레이어 Tried 액션 시작
        m_PlayerController.PlayerThired();
        m_PlayerController.Set_ATK_SP_Particle(false);
        m_PlayerController.Set_ATK_Possible_Particle(false);
        m_Start_FreesTime = 0f;
        m_ATK_START_TIME = 0;
        GameButtonManager.instance.ButtonUp(m_PusseButton[1]);
        m_ATKAngle = false;
        m_ATK = false;
        m_PlayerController.SetMoving(false);

        m_IsRunButtonPress = false;

        float time = 0f;
        m_RunEfeectSprite.fillAmount = 1;
        Color col = m_RunEfeectSprite.color;
        while (time < 3.0f)
        {
            time += 0.2f;
            m_RunEfeectSprite.color = Color.red;
            yield return new WaitForSeconds(0.2f);

            time += 0.2f;
            m_RunEfeectSprite.color = col;
            yield return new WaitForSeconds(0.2f);
        }
        m_RunEfeectSprite.color = col;

        // 여기에는 플레이어 Tried 액션 종료
        m_PlayerController.PaleyrIdel();
    }

    public void ActionReset(int[] array)
    {

        AnimatorStateInfo stateInfo = PlayerController.GetAnimator().GetCurrentAnimatorStateInfo(0);

        foreach (var v in array)
        {
            switch (v)
            {
                case 0:
                    m_PlayerController.TriggerReset("Idel");
                    break;
                case 1:
                    m_PlayerController.TriggerReset("Move");
                    break;
                case 2:
                    m_PlayerController.TriggerReset("Run");
                    break;
                case 3:
                    m_PlayerController.TriggerReset("ATK_Nomal");
                    break;
                case 4:
                    m_PlayerController.TriggerReset("ATK_Cri");
                    break;
                case 5:
                    m_PlayerController.TriggerReset("ATK_Sp");
                    break;
                case 6:

                    break;
                case 7:
                    m_PlayerController.TriggerReset("Dead");
                    break;
                case 8:
                    m_PlayerController.TriggerReset("Tried");
                    break;
                case 9:
                    m_PlayerController.TriggerReset("Atk_Waiting");
                    break;
                case 10:
                    m_PlayerController.TriggerReset("Victory");
                    break;
            }
        }
    }


    public bool ActionCheck(int[] array)
    {

        AnimatorStateInfo stateInfo = PlayerController.GetAnimator().GetCurrentAnimatorStateInfo(0);

        foreach (var v in array)
        {
            switch (v)
            {
                case 0:
                    if (PlayerController.ANISTS_IDLE == stateInfo.fullPathHash)
                        return false;
                    break;
                case 1:
                    if (PlayerController.ANISTS_MOVE == stateInfo.fullPathHash)
                        return false;
                    break;
                case 2:
                    if (PlayerController.ANISTS_RUN == stateInfo.fullPathHash)
                        return false;
                    break;
                case 3:
                    if (PlayerController.ANISTS_ATTACK_NOMAL == stateInfo.fullPathHash)
                        return false;
                    break;
                case 4:
                    if (PlayerController.ANISTS_ATTACK_CRI == stateInfo.fullPathHash)
                        return false;
                    break;
                case 5:
                    if (PlayerController.ANISTS_ATTACK_SP == stateInfo.fullPathHash)
                        return false;
                    break;
                case 6:
                    if (PlayerController.ANISTS_DMG == stateInfo.fullPathHash)
                        return false;
                    break;
                case 7:
                    if (PlayerController.ANISTS_DEAD == stateInfo.fullPathHash)
                        return false;
                    break;
                case 8:
                    if (PlayerController.ANISTS_TRIED == stateInfo.fullPathHash)
                        return false;
                    break;
                case 9:
                    if (PlayerController.ANISTS_ATK_WAITING == stateInfo.fullPathHash)
                        return false;
                    break;
                case 10:
                    if (PlayerController.ANISTS_VICTORY == stateInfo.fullPathHash)
                        return false;
                    break;
            }
        }

        return true;
    }
}

