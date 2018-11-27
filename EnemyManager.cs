using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public List<List<GameObject>> m_EnemyList { get;  set; }
    private characterBean m_CharacterBean;


	// Update is called once per frame
	void Update () {

	}

    public void CharacterBeanLoad()
    {
        m_EnemyList = new List<List<GameObject>>();
        m_CharacterBean = new characterBean();
        string path = "Enemy" + PlayerPrefs.GetInt("stage") + ".json";

        BeanManager.instance.CharacterBeanLoad(m_CharacterBean, path);

        for (int i = 0; i < m_CharacterBean.m_ObjectName.Count; i++)
        {
            MemoryPool.instance.initObject(m_CharacterBean.m_ObjectName[i], m_CharacterBean.m_ObjectNum[i]);
        }

        string fristName = m_CharacterBean.m_ObjPosition[0].Split(',')[0];
        List<GameObject> ListObject = new List<GameObject>();

        for (int i = 0; i < m_CharacterBean.m_ObjPosition.Count; i++)
        {
            string[] Info = m_CharacterBean.m_ObjPosition[i].Split(','); // object name , x , y , z

            GameObject obj = MemoryPool.instance.getObj(Info[0]);

            EnemyControlloer com = obj.GetComponent<EnemyControlloer>();

            com.m_EnemyName = Info[0];
            obj.transform.localPosition = new Vector3(float.Parse(Info[1]), float.Parse(Info[2]), float.Parse(Info[3]));
            obj.transform.localScale = new Vector3(float.Parse(Info[4]), float.Parse(Info[5]), float.Parse(Info[6]));
            // obj.transform.localRotation = Quaternion.Euler(float.Parse(Info[7]), float.Parse(Info[8]), float.Parse(Info[9]));
            obj.transform.localRotation = Quaternion.Euler(float.Parse(Info[7]), Random.Range(0, 360), float.Parse(Info[9]));

            com.EnemyInit();

            if (fristName.Equals(Info[0]))
            {
                ListObject.Add(obj);
            }
            else
            {
                fristName = Info[0];
                m_EnemyList.Add(ListObject);
                ListObject = new List<GameObject>();
                ListObject.Add(obj);
            }
        }
        m_EnemyList.Add(ListObject);


        foreach(var v in m_EnemyList)
        {
            foreach(var k in v)
            {
                k.SetActive(false);
            }
        }
    }


}
