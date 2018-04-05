using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class AdventurerCtrl : MonoBehaviour {
    public GameObject StoreBoardObj;
    private GameObject DoorPositionObj;
    public int nAdventurerVisitNumber { get; private set; }
    
    private List<GameObject> m_arrAdventurerObj;

    private List<AdventurerData> m_arrAdventurersData;
    private List<AdventurerExpData> m_arrAdventurerExpTable;
    private Controller m_ctrl;
    //List<AdventurerAction> actionList;
    // Use this for initialization
    void Awake()
    {
        initAdventurerData();
        initAdventurerExpTable();
    }

    void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_arrAdventurerObj = new List<GameObject>();
        DoorPositionObj = GameObject.Find("DoorPositon");
        nAdventurerVisitNumber = m_ctrl.reputationCtrl.nReputationLv * 3;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //랜덤으로 선택된 모험가 리스트
    public List<AdventurerData> getRandomAdventurers(int nRandomCount)
    {
        
        /*랜덤으로 모험가 선택*/
        GameData gameData = GameObject.Find("GameData").GetComponent<GameData>();
        List<AdventurerData> arrAdventurersData = new List<AdventurerData>();
        foreach(AdventurerData data in getAllAdventurerData())
        {
            if (!data.strCode.Equals("ADVENTURER_DUMMY"))
            {
                arrAdventurersData.Add(data);
            }
                
        }
        
        //모험가 리스트 만들기
        for(int i  = arrAdventurersData.Count; i < nRandomCount; i++)
        {
            AdventurerData data = new AdventurerData("ADVENTURER_DUMMY");
            arrAdventurersData.Add(data);
        }

        for(int i = 0; i< arrAdventurersData.Count;i++)
        {
            if (!arrAdventurersData[i].strCode.Equals("ADVENTURER_DUMMY"))       //더미 모험가가 아니고 네임드 모험가이면
            {
                int nRandom = UnityEngine.Random.Range(0, 101);                              //랜덤 돌림
                Debug.Log(arrAdventurersData[i].strName+":"+ arrAdventurersData[i].getLikeAbility());
                if(nRandom > arrAdventurersData[i].getLikeAbility())             //호감도가 랜덤 값보다 작으면
                {
                    arrAdventurersData[i] = new AdventurerData("ADVENTURER_DUMMY"); //더미로 바꿈.
                }
            }
        }

        //완성된 모험가 리스트를 섞기
        AdventurerData tmpData;
        for(int i = 0; i < arrAdventurersData.Count; i++)
        {
            int nRandom = UnityEngine.Random.Range(0, arrAdventurersData.Count);
            if(i != nRandom)
            {
                tmpData = arrAdventurersData[i];
                arrAdventurersData[i] = arrAdventurersData[nRandom];
                arrAdventurersData[nRandom] = tmpData;
            }
        }
        //Debug.Log(arrAdventurersData.Count);
        foreach (AdventurerData data in arrAdventurersData)
        {
            //Debug.Log(data.strName);
        }
        return arrAdventurersData;
    }

    //랜덤으로 모험가 액션 얻기
    private PotionData getRandomAdventurerAction()
    {

        //[모든 포션]
        GameData gameData = GameObject.Find("GameData").GetComponent<GameData>();
        List<PotionData> potionList = new List<PotionData>();
        foreach(PotionData data in m_ctrl.potionCtrl.getAllPotionData())
        {
            potionList.Add(data);
        }
        
        //[플레이어 레벨]
        int nPlayerLv = gameData.getPlayerLv(); 
        for (int i = potionList.Count -1; i >= 0; i--)
        {
            if (potionList[i].nPotionLv > nPlayerLv)   //레벨 이하면 삭제
            {
                potionList.RemoveAt(i);
            }
        }
        

        //[포션 종류 결정]
        //-모험가 액션 만들기
        ArrayList arrPotionType = new ArrayList();
        int nTmpPotionType = -1;
        for (int i = 0; i< potionList.Count; i++)
        {
            if(nTmpPotionType != potionList[i].nType)
            {
                nTmpPotionType = potionList[i].nType;
                arrPotionType.Add(nTmpPotionType);
            }
        }

        //랜덤으로 선택하기 위해 이벤트 효과에 따라 수치 조절
        NewsData EnvironmentData = new NewsData();
        int nEnvironmentType = 0;                               //이벤트 타입 
        int nEnvironmentEffectPercent = 200;                   //이벤트 효과 ex)200%증가          //추가 코딩
        foreach (NewsData data in m_ctrl.newsCtrl.getTodayNews())
        {
            if(data.eType == NewsData.E_TYPE.환경)
            {
                string strEnvironmentType = data.arrTargetCode[0].Substring(7,1);
                nEnvironmentType = Int32.Parse(strEnvironmentType);
                nEnvironmentEffectPercent = data.nValue;
            }
        }
        
        int nRandomRange = 10;                          //랜덤 수치
        int nRandomTotalRange = 0;                      //랜덤 수치 총합
        List<int> randomTable = new List<int>();        //랜덤 테이블 만들기
        for (int i = 0; i< arrPotionType.Count; i++)
        {
            if (i == nEnvironmentType)
            {
                randomTable.Add(nRandomRange + nRandomRange * nEnvironmentEffectPercent / 100);
                nRandomTotalRange = nRandomTotalRange + (nRandomRange + nRandomRange * nEnvironmentEffectPercent / 100);
            }
            else
            {
                randomTable.Add(nRandomRange);
                nRandomTotalRange = nRandomTotalRange + nRandomRange;
            }            
        }

        //-랜덤으로 포션 타입 선택
        int nRandom = UnityEngine.Random.Range(0, nRandomTotalRange + 1);   //랜덤으로 수치 선택
        int nCountRange = 0;
        int nSelectType = 0;
        for (int i = 0; i < randomTable.Count; i++)             //선택된 수치가 속한 범위 검색
        {
            nCountRange += randomTable[i];
            if (nRandom < nCountRange)
            {
                nSelectType = i;                                //타입 찾음
                break;
            }
        }
        
        for (int i = potionList.Count - 1; i >= 0 ; i--)
        {
            if (potionList[i].nType != nSelectType)   //선택된 포션 종류가 아니면
            {
                potionList.RemoveAt(i);                         //삭제
            }
        }
        int nRandomListIndex = UnityEngine.Random.Range(0, potionList.Count);
        
        return potionList[nRandomListIndex];
        
    }
   
    //모험가 생성
    public List<GameObject> createAdventurer()
    {
        m_arrAdventurerObj.Clear();
        nAdventurerVisitNumber = m_ctrl.reputationCtrl.nReputationLv * 3;
        //랜덤으로 모험가 선택 
        List<AdventurerData> randomAdventurerData = getRandomAdventurers(nAdventurerVisitNumber);

        //모험가에 넣을 행동 가져오기
        List<PotionData> arrPotionList = new List<PotionData>();
        for(int i = 0;i< nAdventurerVisitNumber; i++)
        {
            arrPotionList.Add(getRandomAdventurerAction());
        }
        
        //모험가 프리팹 생성
        for(int i = 0; i < nAdventurerVisitNumber; i++)
        {
            //Debug.Log(randomAdventurerData[i].strCode);
            GameObject AdventurerPrefeb = (GameObject)Resources.Load("Prefeb/Main/Adventurer/"+ randomAdventurerData[i].strCode, typeof(GameObject));
            GameObject adventurer = (GameObject)Instantiate(AdventurerPrefeb, Vector3.zero, transform.rotation);
            if (!randomAdventurerData[i].strCode.Equals("ADVENTURER_DUMMY"))
            {
                adventurer.transform.parent = GameObject.Find("Adventurers_Hero").transform;
                
            }else
            {
                adventurer.transform.parent = GameObject.Find("Adventurers_Dummy").transform;
            }
            
            adventurer.transform.localPosition = DoorPositionObj.transform.localPosition;
            adventurer.transform.localScale = Vector3.one;

            m_arrAdventurerObj.Add(adventurer);
        }

        //모험가에 데이터 넣기
        for (int i = 0; i< m_arrAdventurerObj.Count; i++)
        {
            AdventurerObj adven = m_arrAdventurerObj[i].GetComponent<AdventurerObj>();
            adven.setAdventurerData(randomAdventurerData[i], arrPotionList[i]);
            //Debug.Log(arrPotionList[i].strName);
        }
        //*/
        //Debug.Log(nAdventurerVisitNumber);
        return m_arrAdventurerObj;
    }

    /*-----------------------------------------------모험가 데이터--------------------------------------------------------*/
    //모험가 데이터 초기화
    private void initAdventurerData()
    {
        //모험가 xml가져오기
        m_arrAdventurersData = new List<AdventurerData>();
        XmlDocument AdventurerDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Adventurer", typeof(TextAsset)));
        XmlNodeList m_adventurerNodeList = AdventurerDoc.SelectNodes("List/Item");
        foreach (XmlNode node in m_adventurerNodeList)
        {
            
            AdventurerData data = new AdventurerData();
            data.strCode = node.Attributes[0].InnerText;
            data.strName = node.Attributes[1].InnerText;
            data.strJob = node.Attributes[2].InnerText;
            data.strCharCode = node.Attributes[3].InnerText;

            m_arrAdventurersData.Add(data);
            
        }
    }

    //모험가 모든 데이터 가져오기
    public List<AdventurerData> getAllAdventurerData()
    {
        foreach (AdventurerData data in m_arrAdventurersData)
        {
            //Debug.Log(data.strName +":"+ data.getLikeAbility());
        }
        return m_arrAdventurersData;
    }

    /*-----------------------------------------------모험가 경험치--------------------------------------------------------*/
    //모험가 경험치 데이터 초기화
    private void initAdventurerExpTable()
    {
        //모험가 xml가져오기
        m_arrAdventurerExpTable = new List<AdventurerExpData>();
        XmlDocument AdventurerExpDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/ExpTable", typeof(TextAsset)));
        XmlNodeList m_adventurerExpNodeList = AdventurerExpDoc.SelectNodes("List/Item");
        foreach (XmlNode node in m_adventurerExpNodeList)
        {
            AdventurerExpData data = new AdventurerExpData();
            data.nLv = Convert.ToInt32(node.Attributes[0].InnerText);
            data.nRequiredExp = Convert.ToInt32(node.Attributes[1].InnerText.Replace(",",""));
            data.nTotalRequiredExp = Convert.ToInt32(node.Attributes[2].InnerText.Replace(",", ""));
            data.nAddExp = Convert.ToInt32(node.Attributes[3].InnerText);
            
            m_arrAdventurerExpTable.Add(data);
        }
    }

    //모든 모험가 경험치 테이블 가져오기
    public List<AdventurerExpData> getAllAdventurerExpTable()
    {
        return m_arrAdventurerExpTable;
    }
}
