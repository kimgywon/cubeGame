using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;


public class BeanManager : MonoBehaviour
{
    private static BeanManager s_Instance = null;

    public List<ItemBean> m_CommonItemBean;
    public List<ItemBean> GetCommonItemBean()
    {
        return m_CommonItemBean;
    }
    public List<ItemBean> m_InCommonItemBean;
    public List<ItemBean> GetInCommonItemBean()
    {
        return m_InCommonItemBean;
    }
    public List<ItemBean> m_RareItemBean;
    public List<ItemBean> GetRareItemBean()
    {
        return m_RareItemBean;
    }
    public List<ItemBean> m_EpicItemBean;
    public List<ItemBean> GetEpicItemBean()
    {
        return m_EpicItemBean;
    }
    public List<ShopItemBean> m_WeapoinShopItemBean;
    public List<ShopItemBean> GetWeapoinShopItemBean()
    {
        return m_WeapoinShopItemBean;
    }
    public List<ShopItemBean> m_DefenseShopItemBean;
    public List<ShopItemBean> GetDefenseShopItemBean()
    {
        return m_DefenseShopItemBean;
    }
    public List<ItemViewUpgradeBean> m_ItemViewUpgradeBean;
    public List<ItemViewUpgradeBean> GetItemViewUpgradeBean()
    {
        return m_ItemViewUpgradeBean;
    }

    public PlayerBean m_PlayerBean;
    public PlayerBean GetPlayerBean()
    {
        return m_PlayerBean;
    }

    public GameBean m_GameMangerBean;
    public GameBean GetGameMangerBean()
    {
        return m_GameMangerBean;
    }

    public SoldOutBean m_SoldOutBean;
    public SoldOutBean GetSoldOutBean()
    {
        return m_SoldOutBean;
    }

    public List< EnemyBean> m_EnemyBean;
    public List<EnemyBean> GetEnemyBean()
    {
        return m_EnemyBean;
    }

    public List<MapInfoBean> m_MapinfoBean;
    public List<MapInfoBean> GetMapinfoBean()
    {
        return m_MapinfoBean;
    }

    public static BeanManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new GameObject("Manager").AddComponent<BeanManager>();
                // Debug.Log("Fail");
            }
            return s_Instance;
        }
    }

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = GetComponent<BeanManager>();
            DontDestroyOnLoad(this);

            m_CommonItemBean = new List<ItemBean>();
            ItemBeanLoad(m_CommonItemBean, "commonItem.json");

            m_InCommonItemBean = new List<ItemBean>();
            ItemBeanLoad(m_InCommonItemBean, "IncommonItem.json");

            m_RareItemBean = new List<ItemBean>();
            ItemBeanLoad(m_RareItemBean, "RareItem.json");

            m_EpicItemBean = new List<ItemBean>();
            ItemBeanLoad(m_EpicItemBean, "EpicItem.json");

            m_WeapoinShopItemBean = new List<ShopItemBean>();
            WeapoinShopItemBeanLoad(m_WeapoinShopItemBean, "WeapoinShopItem.json");

            m_DefenseShopItemBean = new List<ShopItemBean>();
            WeapoinShopItemBeanLoad(m_DefenseShopItemBean, "DefenseShopItem.json");

            m_ItemViewUpgradeBean = new List<ItemViewUpgradeBean>();
            ItemViewUpgradeLoad(m_ItemViewUpgradeBean, "ShopUpgrade.json");

            m_PlayerBean = new PlayerBean();
            PlayerBeanLoad(m_PlayerBean, "PlayerBean.json");

            m_GameMangerBean = new GameBean();
            GameMansgerBeanLoad(m_GameMangerBean, "GameInfo.json");

            m_SoldOutBean = new SoldOutBean();
            SoldOutBeanLoad(m_SoldOutBean, "SoldOut.json");

            m_EnemyBean = new List<EnemyBean>();
            EnemyBeanLoad(m_EnemyBean, "EnemyInfo.json");

            m_MapinfoBean = new List<MapInfoBean>();
            MapinfoBeanLoad(m_MapinfoBean, "MapInfo.json");
        }
    }

    void OnApplicationQuit()
    {
        SaveFunc(BeanManager.instance.m_WeapoinShopItemBean, "WeapoinShopItem.json");
        SaveFunc(BeanManager.instance.m_DefenseShopItemBean, "DefenseShopItem.json");
        BeanManager.instance.m_PlayerBean.Reset();
        SaveFunc(BeanManager.instance.m_PlayerBean, "PlayerBean.json");
        SaveFunc(BeanManager.instance.m_GameMangerBean, "GameInfo.json");
        SaveFunc(BeanManager.instance.m_SoldOutBean, "SoldOut.json");
        SaveFunc(BeanManager.instance.m_MapinfoBean, "MapInfo.json");

        Invoke("End", 5f);
        //게임종료시 삭제. 
    }

    void End()
    {
        s_Instance = null;
    }
    /////////////////////////////////////  Load Func() start

    public void GameMansgerBeanLoad(GameBean Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        Bean.m_PlayrMoney = int.Parse(data["m_PlayrMoney"].ToString());
        Bean.m_PlayerDiamond = int.Parse(data["m_PlayerDiamond"].ToString());
        Bean.m_PlayerKey = int.Parse(data["m_PlayerKey"].ToString());
        Bean.m_PlayerWave = int.Parse(data["m_PlayerWave"].ToString());
    }


    public void ItemBeanLoad(List<ItemBean> Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            ItemBean item = new ItemBean();
            item.m_ItemName = data[i]["m_ItemName"].ToString();
            item.m_SpriteName = data[i]["m_SpriteName"].ToString();
            item.m_ItemSpecialAbilityName = data[i]["m_ItemSpecialAbilityName"].ToString();
            item.m_ItemSpecialAbility = int.Parse(data[i]["m_ItemSpecialAbility"].ToString());
            item.m_ItemAbility = int.Parse(data[i]["m_ItemAbility"].ToString());
            item.m_ItemPrice = int.Parse(data[i]["m_ItemPrice"].ToString());
            item.m_ItemReinforce = int.Parse(data[i]["m_ItemReinforce"].ToString());
            item.m_ItemRating = data[i]["m_ItemRating"].ToString();
            item.m_ItemPart = data[i]["m_ItemPart"].ToString();


            Bean.Add(item);
        }
    }

    public void WeapoinShopItemBeanLoad(List<ShopItemBean> Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            ShopItemBean item = new ShopItemBean();
            item.m_ItemName = data[i]["m_ItemName"].ToString();
            item.m_SpriteName = data[i]["m_SpriteName"].ToString();
            item.m_ItemSpecialAbilityName = data[i]["m_ItemSpecialAbilityName"].ToString();
            item.m_ItemSpecialAbility = int.Parse(data[i]["m_ItemSpecialAbility"].ToString());
            item.m_ItemAbility = int.Parse(data[i]["m_ItemAbility"].ToString());
            item.m_ItemPrice = int.Parse(data[i]["m_ItemPrice"].ToString());
            item.m_ItemReinforce = int.Parse(data[i]["m_ItemReinforce"].ToString());
            item.m_ItemRating = data[i]["m_ItemRating"].ToString();
            item.m_ItemPart = data[i]["m_ItemPart"].ToString();
            item.m_ItemGridNumber = int.Parse(data[i]["m_ItemGridNumber"].ToString());

            Bean.Add(item);
        }
    }

    public void ItemViewUpgradeLoad(List<ItemViewUpgradeBean> Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            string str = "Upgrade" + i;
            ItemViewUpgradeBean item = new ItemViewUpgradeBean();

            for (int j = 0; j < data[str].Count; j++)
            {
                item.Rating[j] = data[str][j]["Rating"].ToString();

            }
            Bean.Add(item);
        }
    }

    public void PlayerBeanLoad(PlayerBean Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        int PlayerItemNum = 4;

        if (data == null)
            return;

        string[] str = { "m_PlayerArmor", "m_PlayerHelmet", "m_PlayerBoot", "m_PlayerWeapoin" };

        Bean.m_PlayerLevel = int.Parse(data["m_PlayerLevel"].ToString());
        Bean.m_PlayerHp = int.Parse(data["m_PlayerHp"].ToString());
        Bean.m_PlayerMp = int.Parse(data["m_PlayerMp"].ToString());
        Bean.m_PlayerAtk = int.Parse(data["m_PlayerAtk"].ToString());
        Bean.m_PlayerDef = int.Parse(data["m_PlayerDef"].ToString());
        Bean.m_PlayerExp = int.Parse(data["m_PlayerExp"].ToString());
        Bean.m_playerMoveSpeed = double.Parse(data["m_playerMoveSpeed"].ToString());
        Bean.m_playerAtkSpeed = double.Parse(data["m_playerAtkSpeed"].ToString());
        Bean.m_playerCriChance = double.Parse(data["m_playerCriChance"].ToString());
        Bean.m_playerCriDamage = double.Parse(data["m_playerCriDamage"].ToString());
        Bean.m_playerHpRegen = double.Parse(data["m_playerHpRegen"].ToString());

        for (int i = 0; i < PlayerItemNum; i++)
        {
            string ps = str[i];

            ItemBean item = new ItemBean();
            if (data[ps] != null)
            {
                item.m_ItemName = data[ps]["m_ItemName"].ToString();
                item.m_SpriteName = data[ps]["m_SpriteName"].ToString();
                item.m_ItemSpecialAbilityName = data[ps]["m_ItemSpecialAbilityName"].ToString();
                item.m_ItemSpecialAbility = int.Parse(data[ps]["m_ItemSpecialAbility"].ToString());
                item.m_ItemAbility = int.Parse(data[ps]["m_ItemAbility"].ToString());
                item.m_ItemPrice = int.Parse(data[ps]["m_ItemPrice"].ToString());
                item.m_ItemReinforce = int.Parse(data[ps]["m_ItemReinforce"].ToString());
                item.m_ItemRating = data[ps]["m_ItemRating"].ToString();
                item.m_ItemPart = data[ps]["m_ItemPart"].ToString();

            }
            if (i == 0)
                Bean.m_PlayerArmor = item;
            else if (i == 1)
                Bean.m_PlayerHelmet = item;
            else if (i == 2)
                Bean.m_PlayerBoot = item;
            else if (i == 3)
                Bean.m_PlayerWeapoin = item;
        }
    }
    public void SoldOutBeanLoad(SoldOutBean Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;


        for (int j = 0; j < data["m_WeSoldOut"].Count; j++)
        {
            if (data["m_WeSoldOut"][j].ToString() == "False")
                Bean.m_WeSoldOut[j] = false;
            else
                Bean.m_WeSoldOut[j] = true;
        }

        for (int j = 0; j < data["m_DefSoldOut"].Count; j++)
        {
            if (data["m_DefSoldOut"][j].ToString() == "False")
                Bean.m_DefSoldOut[j] = false;
            else
                Bean.m_DefSoldOut[j] = true;
        }

    }

    public void MapBeanLoad(MapBean Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int j = 0; j < data["m_ObjectNum"].Count; j++)
        {
            Bean.m_ObjectNum.Add(int.Parse(data["m_ObjectNum"][j].ToString()));
            Bean.m_ObjectName.Add(data["m_ObjectName"][j].ToString());
        }

        for(int i = 0; i< data["m_ObjPosition"].Count; i++)
        {
            Bean.m_ObjPosition.Add(data["m_ObjPosition"][i].ToString());
        }
    }

    public void CharacterBeanLoad(characterBean Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int j = 0; j < data["m_ObjectNum"].Count; j++)
        {
            Bean.m_ObjectNum.Add(int.Parse(data["m_ObjectNum"][j].ToString()));
            Bean.m_ObjectName.Add(data["m_ObjectName"][j].ToString());
        }

        for (int i = 0; i < data["m_ObjPosition"].Count; i++)
        {
            Bean.m_ObjPosition.Add(data["m_ObjPosition"][i].ToString());
        }
    }


    public void EnemyBeanLoad(List<EnemyBean> Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            EnemyBean item = new EnemyBean();
            item.m_EnemyAtk =int.Parse( data[i]["m_EnemyAtk"].ToString());
            item.m_EnemyAtkCri = int.Parse(data[i]["m_EnemyAtkCri"].ToString());
            item.m_EnemyAtkSpeed =double.Parse( data[i]["m_EnemyAtkSpeed"].ToString());
            item.m_EnemyDef = int.Parse(data[i]["m_EnemyDef"].ToString());
            item.m_EnemyHp = int.Parse(data[i]["m_EnemyHp"].ToString());
            item.m_EnemyName = data[i]["m_EnemyName"].ToString();
            item.m_EnemySpeed = double.Parse(data[i]["m_EnemySpeed"].ToString());

            Bean.Add(item);
        }
    }

    public void MapinfoBeanLoad(List<MapInfoBean> Bean, string Path)
    {
        JsonData data = LoadFunc(Path);

        if (data == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            MapInfoBean item = new MapInfoBean();
            item.m_MapNumber = int.Parse(data[i]["m_MapNumber"].ToString());
            item.m_MapStar = int.Parse(data[i]["m_MapStar"].ToString());
            item.m_MapAvatar = int.Parse(data[i]["m_MapAvatar"].ToString());
            item.m_Boss = int.Parse(data[i]["m_Boss"].ToString());
            item.m_MapPass = int.Parse(data[i]["m_MapPass"].ToString());

            Bean.Add(item);
        }
    }
    /////////////////////////////////////  Load Func() end


    public void SaveFunc(object Data, string Path)
    {
        JsonData Json = JsonMapper.ToJson(Data);

        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ?
            Application.persistentDataPath : Application.dataPath) + "/Resources/Bean/";

        string filePath = folderPath + Path;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            File.WriteAllText(filePath, Json.ToString());
        }
        else
            File.WriteAllText(filePath, Json.ToString());
    }

    public JsonData LoadFunc(string Path)
    {
        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ?
Application.persistentDataPath : Application.dataPath) + "/Resources/Bean/";

        string filePath = folderPath + Path;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (File.Exists(filePath))
        {
            string Jsonstring = File.ReadAllText(filePath);
            return JsonMapper.ToObject(Jsonstring.ToString());
        }
        else
        {
            TextAsset text = Resources.Load("Bean/" + Path.Substring(0, Path.Length - 5)) as TextAsset;
            string Jsonstring = text.ToString();

            if (text == null)
            {
                return null;
            }

            return JsonMapper.ToObject(Jsonstring.ToString());
        }
    }
}
