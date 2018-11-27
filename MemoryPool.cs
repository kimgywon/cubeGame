using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool : MonoBehaviour
{

    private List<GameObject> m_Listprefep = new List<GameObject>();
    private Dictionary<string, List<GameObject>> m_map = new Dictionary<string, List<GameObject>>();
    private static MemoryPool s_Instance = null;

    private GameObject m_ParentObject;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = GetComponent<MemoryPool>();
            DontDestroyOnLoad(this);
           // m_ParentObject = transform.gameObject;

            Object[] m_Obj2 = Resources.LoadAll("asset/KUBIKOS - World/Prefabs");

            Object[] m_Obj3 = Resources.LoadAll("asset/ToonyTinyPeople/TT_Zombies/prefabs");

            foreach (var v in m_Obj2)
            {
                if (v as GameObject != null)
                    m_Listprefep.Add(v as GameObject);
            }
            foreach (var v in m_Obj3)
            {
                if (v as GameObject != null)
                    m_Listprefep.Add(v as GameObject);
            }
        }
    }


    public GameObject FindObj(string ObjName)
    {
        foreach (var v in m_Listprefep)
        {

            if (v.name == ObjName)
                return v;

        }

        Debug.LogError(string.Format("해당 이름이 없습니다."));

        return null;
    }

    public void initObject(string OBjName, int Res)
    {
        GameObject prefab = FindObj(OBjName);

        if (m_map.ContainsKey(OBjName) == false)
        {
            List<GameObject> list = new List<GameObject>();

            m_map.Add(OBjName, list);
        }

        List<GameObject> insertList = m_map[OBjName];

        GameObject parent = null;
        if (GameObject.Find(OBjName + "Parent") == null)
        {
            parent = new GameObject(OBjName + "Parent");
            parent.transform.parent = transform;
        }
        else
        {
            parent = GameObject.Find(OBjName + "Parent");
        }

        for (int i = 0; i < Res; i++)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(prefab);
            obj.SetActive(false);
            obj.transform.parent = parent.transform;
            obj.name = OBjName;
            insertList.Add(obj);
        }
    }

    public GameObject getObj(string OBjName)
    {
        if (m_map.ContainsKey(OBjName) == false)
            this.initObject(OBjName, 10);

        List<GameObject> SechList = m_map[OBjName];

        foreach (var v in SechList)
        {
            if (v.activeSelf == false)
            {
                v.SetActive(true);
                return v;

            }
        }

        initObject(OBjName, SechList.Count);

        return getObj(OBjName);
    }

    public static MemoryPool instance
    {

        get
        {
            if (s_Instance == null)
            {
                s_Instance = new GameObject("MemoryPoolManager").AddComponent<MemoryPool>();
                //오브젝트가 생성이 안되있을경우 생성. 
            }
            return s_Instance;
        }
    }


    void OnApplicationQuit()
    {
        s_Instance = null;
        //게임종료시 삭제. 
    }
}
