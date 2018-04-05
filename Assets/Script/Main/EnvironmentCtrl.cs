using UnityEngine;
using System.Collections.Generic;

public class EnvironmentCtrl : MonoBehaviour {
    //public value
    public int 환경변경확률;

    //private obj
    private Controller m_ctrl;
    private NewsData m_EnvironmentNews;
	// Use this for initialization
	void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_EnvironmentNews = new NewsData();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public NewsData getEnvironmentNews()
    {
        //초기 환경이 있는지 확인
        string strEnvironmentNewscode = PlayerPrefs.GetString("GameEnvironmentCode", "NONE");
        if (strEnvironmentNewscode.Equals("NONE"))  //이전에 설정된 환경이 없으면
        {
            m_EnvironmentNews = getRandomEnvironmentNews();
            PlayerPrefs.SetString("GameEnvironmentCode", m_EnvironmentNews.strCode);    //변경된 환경 코드 저장
            Debug.Log("환경초기화");
        }else                                       //이전에 설정된 환경이 있으면 
        {
            m_EnvironmentNews = new NewsData(strEnvironmentNewscode);
        }

        //랜덤으로 환경을 바꿀것인지 결정
        int nTotalValue = 100;
        int nRandom = Random.Range(0, nTotalValue);
        if(환경변경확률 > nRandom)    //변경 조건을 충족 하면
        {
            m_EnvironmentNews = getRandomEnvironmentNews();                             //랜덤으로 환경 선택
            PlayerPrefs.SetString("GameEnvironmentCode", m_EnvironmentNews.strCode);    //변경된 환경 코드 저장
            Debug.Log("환경변경");
            return m_EnvironmentNews;
            
        }
        else                        //변경이 안되면
        {
            
            Debug.Log("환경유지");
            return m_EnvironmentNews;
            
        }
    }

    //랜덤으로 환경 뉴스 골라주기
    private NewsData getRandomEnvironmentNews()
    {
        List<NewsData> arrEnabledEnvironmentNews = new List<NewsData>();
        foreach (NewsData data in m_ctrl.newsCtrl.getAllNewsData())
        {
            if (data.eType == NewsData.E_TYPE.환경)
            {
                PotionData pData = new PotionData(data.arrTargetCode[0]);   //포션중 가장 낮은 래벨의 포션
                if (pData.nPotionLv <= m_ctrl.gameData.getPlayerLv())       //포션레벨이 플레이어 레벨에 충족이 되면
                {
                    arrEnabledEnvironmentNews.Add(data);                    //환경 배열에 추가
                }
            }
        }
        int nRandom = Random.Range(0, arrEnabledEnvironmentNews.Count);
        return arrEnabledEnvironmentNews[nRandom];                          //그 중에 랜덤으로 하나 선택
    }
}
