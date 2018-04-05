using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class NewsCtrl : MonoBehaviour {
    public GameObject boardMainContentObj;
    public Button NewsOpenBtn;
    public Text boardDetialText;
    public GameObject newsItemPrefeb;
    public GameObject NewsWindow;
    public List<GameObject> arrItems;

    private List<NewsData> arrNextNews;
    private List<NewsData> arrTodayNews;
    private List<NewsData> m_arrAllNewsData;
    private List<string> m_arrNewsDetail;
    private bool m_bNewsLooking;    //뉴스 보는 중?
    private int m_nNewsItemCount;
    enum NEWS_TYPE
    {
        E_ENVIROMENT,
        E_ADVENTURER,
        E_ALCHEMY
    }
	// Use this for initialization
	void Awake () {
        m_nNewsItemCount = 0;
        arrItems = new List<GameObject>();
        arrNextNews = new List<NewsData>();
        arrTodayNews = new List<NewsData>();
        m_arrNewsDetail = new List<string>();
        m_bNewsLooking = false;
        initNewsData();

        //뉴스 만들기
        arrNextNews = getNextNews();
        Debug.Log("다음 뉴스 카운트:"+arrNextNews.Count);
        if (arrNextNews.Count > 0)
        {
            setTodayNews(getNextNews());
            clearNextNews();
        }
        
    }
	
	// Update is called once per frame
	void Update () {
       
    }

    
    //스크롤뷰 뉴스 아이템 추가하기
    private void addNewsItem(NewsData data)
    {
        GameObject obj = (GameObject)Instantiate(newsItemPrefeb, Vector3.zero, newsItemPrefeb.transform.rotation);
        RectTransform objRect = obj.GetComponent<RectTransform>();
        obj.transform.parent = boardMainContentObj.transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);

        //content 크기 얻기
        RectTransform content = boardMainContentObj.GetComponent<RectTransform>();
        float contentHeight = content.rect.height;
        float firstItemY = objRect.rect.height / -2;
        float ItemX = objRect.rect.width / 2;

        //아이템 위치 조정        
        obj.transform.localPosition = new Vector3(ItemX + 1, firstItemY - (objRect.rect.height * m_nNewsItemCount++), 0f);

        //아이템 데이터 넣기
        obj.GetComponent<NewsItemObj>().setNewsTitle(data.strTitle);
        obj.GetComponent<NewsItemObj>().setNewsIcon(data.eType);
        obj.GetComponent<NewsItemObj>().setNewsData(data);
        arrItems.Add(obj);
        m_arrNewsDetail.Add(data.strContents);
    }
    
    //판매 스크롤뷰 컨텐트 사이즈 설정
    private void setScrollContentSize(int nItemAmount)
    {
        //아이템 height구하기
        RectTransform objRect = newsItemPrefeb.GetComponent<RectTransform>();

        //content 크기 설정
        RectTransform content = boardMainContentObj.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(0f, objRect.rect.height * nItemAmount);
    }

    public void createNews(List<NewsData> arrNewsList)
    {
        //초기화
        foreach(GameObject destroyObj in arrItems)
        {
            Destroy(destroyObj);
        }
        arrItems.Clear();
        m_nNewsItemCount = 0;

        //아이템 디스플레이 조절
        int nNewsNum = arrNewsList.Count;
        setScrollContentSize(arrNewsList.Count);

        //아이템 생성
        foreach(NewsData data in arrNewsList)
        {
            addNewsItem(data);
        }
        //뉴스 디테일 초기화면
        if(arrItems.Count > 0)
        {
            arrItems[0].GetComponent<NewsItemObj>().selectItem();
        }else
        {
            boardDetialText.text = "";
        }
        
    }
    
    public void openNews()
    {
        RectTransform rectTrans = NewsWindow.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(-rectTrans.rect.width / 2, 0);
        
        m_bNewsLooking = true;
        createNews(getTodayNews());
    }
    public void closeNews()
    {
        RectTransform rectTrans = NewsWindow.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(1500, 0);

        //초기화
        m_nNewsItemCount = 0;
        m_bNewsLooking = false;
    }

    public bool isNewsLooking()
    {
        return m_bNewsLooking;
    }

    /*------------------------------------오늘 뉴스 저장-----------------------------------*/
    public List<NewsData> getTodayNews()
    {
        arrTodayNews.Clear();    //다음 뉴스 스택 초기화
        string strRawNewsCodes = PlayerPrefs.GetString("TodayNewsCodes", "");  //다음 뉴스 가져오기
        //Debug.Log(strRawNewsCodes);
        string[] arrRawNewsCodes = strRawNewsCodes.Split('#');

        if (arrRawNewsCodes[0].Contains("News"))
        {
            foreach (string strNewsCode in arrRawNewsCodes)
            {
                arrTodayNews.Add(new NewsData(strNewsCode));
            }
        }

        return arrTodayNews;
    }

    public void setTodayNews(List<NewsData> newsList)
    {
        string strRawNewsCodes = "";
        strRawNewsCodes = "";
        foreach (NewsData strNewsData in newsList)
        {
            strRawNewsCodes += (strNewsData.strCode + "#");
        }
        if (!strRawNewsCodes.Equals(""))
        {
            strRawNewsCodes = strRawNewsCodes.Remove(strRawNewsCodes.Length - 1);
        }
        
        //Debug.Log(strRawNewsCodes);
        PlayerPrefs.SetString("TodayNewsCodes", strRawNewsCodes);
    }

    public void addTodayNews(NewsData data)
    {
        string strRawNewsCodes = PlayerPrefs.GetString("TodayNewsCodes", "");  //다음 뉴스 가져오기
        string[] arrRawNewsCodes = strRawNewsCodes.Split('#');
        strRawNewsCodes = "";
        foreach (string strNewsCode in arrRawNewsCodes)
        {
            if (!strNewsCode.Equals(""))
            {
                strRawNewsCodes += (strNewsCode + "#");
            }
        }

        strRawNewsCodes += data.strCode;
        Debug.Log(strRawNewsCodes);
        PlayerPrefs.SetString("TodayNewsCodes", strRawNewsCodes);
    }

    public void clearTodayNews()
    {
        PlayerPrefs.SetString("TodayNewsCodes", "");
    }
    /*------------------------------------다음 뉴스 저장-----------------------------------*/
    public List<NewsData> getNextNews()
    {
        arrNextNews.Clear();    //다음 뉴스 스택 초기화
        string strRawNewsCodes = PlayerPrefs.GetString("NextNewsCodes", "");  //다음 뉴스 가져오기
        Debug.Log(strRawNewsCodes);
        string[] arrRawNewsCodes = strRawNewsCodes.Split('#');

        if (arrRawNewsCodes[0].Contains("News"))
        {
            foreach (string strNewsCode in arrRawNewsCodes)
            {
                NewsData data = new NewsData(strNewsCode);
                arrNextNews.Add(data);
                
            }
        }
        foreach(NewsData data in arrNextNews)
        {
           // Debug.Log(data.strCode);
        }
        
        return arrNextNews;
    }
    public void addNextNews(NewsData data)
    {
        string strRawNewsCodes = PlayerPrefs.GetString("NextNewsCodes", "");  //다음 뉴스 가져오기
        string[] arrRawNewsCodes = strRawNewsCodes.Split('#');
        strRawNewsCodes = "";
        foreach (string strNewsCode in arrRawNewsCodes)
        {
            if (!strNewsCode.Equals(""))
            {
                strRawNewsCodes += (strNewsCode + "#");
            }
        }

        strRawNewsCodes += data.strCode;
        Debug.Log(strRawNewsCodes);
        PlayerPrefs.SetString("NextNewsCodes", strRawNewsCodes);
    }

    public void clearNextNews()
    {
        Debug.Log("뉴스 스택 초기화");
        PlayerPrefs.SetString("NextNewsCodes", "");
    }

    /*-------------------------------------뉴스 데이터-------------------------------------*/
    private void initNewsData()
    {
        //뉴스 Xml가져오기
        m_arrAllNewsData = new List<NewsData>();
        XmlDocument NewsDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/News", typeof(TextAsset)));
        XmlNodeList m_newsNodeList = NewsDoc.SelectNodes("List/News");
        foreach (XmlNode node in m_newsNodeList)
        {
            NewsData data = new NewsData();
            //제목
            data.strTitle = node.FirstChild.InnerText;

            //내용
            data.strContents = node.SelectNodes("Detail")[0].InnerText;
            //data.strContents.Replace('#', '\n');
            //코드
            data.strCode = node.Attributes[0].InnerText;

            //타입
            switch (node.Attributes[1].InnerText)
            {
                case "환경":
                    data.eType = NewsData.E_TYPE.환경;
                    break;
                case "모험가":
                    data.eType = NewsData.E_TYPE.모험가;
                    break;
                case "학계":
                    data.eType = NewsData.E_TYPE.학계;
                    break;
            }

            //효과
            switch (node.Attributes[2].InnerText)
            {
                case "수요":
                    data.eEffect = NewsData.E_EFFECT.수요;
                    break;
                case "값":
                    data.eEffect = NewsData.E_EFFECT.값;
                    break;
                case "방문":
                    data.eEffect = NewsData.E_EFFECT.방문;
                    break;
                case "호감도":
                    data.eEffect = NewsData.E_EFFECT.호감도;
                    break;
                case "레벨":
                    data.eEffect = NewsData.E_EFFECT.레벨;
                    break;
                case "사망":
                    data.eEffect = NewsData.E_EFFECT.사망;
                    break;
                case "평판":
                    data.eEffect = NewsData.E_EFFECT.평판;
                    break;

            }

            //값
            data.nValue = Convert.ToInt32(node.Attributes[3].InnerText);

            //타겟 코드들
            List<string> arrTargetCode = new List<string>();
            string[] arrRawTargetCode = node.Attributes[4].InnerText.Split('#');
            foreach (string strRawCode in arrRawTargetCode)
            {
                arrTargetCode.Add(strRawCode);
            }
            data.arrTargetCode = arrTargetCode;

            m_arrAllNewsData.Add(data);
        }
    }
    //모든 뉴스 데이터 얻기
    public List<NewsData> getAllNewsData()
    {
        return m_arrAllNewsData;
    }

    //상황에 맞는 뉴스 얻기
    public NewsData getCustomNews(NewsData.E_TYPE type, NewsData.E_EFFECT effect, NewsData.E_VALUE value, string strTargetCode)
    {
        foreach(NewsData data in getAllNewsData())
        {
            if(data.eType == type && data.eEffect == effect && data.eValue == value)
            {
                foreach(string strCode in data.arrTargetCode)
                {
                    if (strCode.Equals(strTargetCode))
                    {
                        return data;
                    }
                }
            }
        }

        return null;
    }


    //뉴스 효과 적용
    public void setNewsEffectApply(List<NewsData> applyDataList)
    {
        foreach (NewsData data in applyDataList)
        {
            //타입
            switch (data.eType)
            {
                case NewsData.E_TYPE.환경:

                    break;
                case NewsData.E_TYPE.모험가:
                    switch (data.eEffect)
                    {
                        case NewsData.E_EFFECT.호감도:
                            foreach (string strAdventurerCode in data.arrTargetCode)
                            {
                                AdventurerData adventureData = new AdventurerData(strAdventurerCode);
                                adventureData.addLikeAbility(data.nValue);
                            }

                            break;
                        case NewsData.E_EFFECT.레벨:
                            break;
                    }
                    break;
                case NewsData.E_TYPE.학계:

                    break;
            }
        }
    }
}
