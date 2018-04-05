using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DateCtrl : MonoBehaviour {
    //public value
    public int m_nDate { get; private set; }

    private GameData m_gameData;
    
    private Animator m_aDayLabel;
    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_gameData = GameObject.Find("GameData").GetComponent<GameData>();
        m_nDate = m_gameData.getDay();
        GameObject.Find("weekLabelText").GetComponent<Text>().text = m_nDate + " Week";
        m_aDayLabel = GameObject.Find("weekLabel").GetComponent<Animator>();
        //애니메이션
        m_aDayLabel.SetTrigger("action");

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void nextDate()
    {
        m_nDate = m_gameData.getDay();

        //데이터 변경
        m_nDate++;
        m_gameData.setDay(m_nDate);
        GameObject.Find("weekLabelText").GetComponent<Text>().text = m_nDate + " Week";

        //애니메이션
        m_aDayLabel.SetBool("action", true);
        //StartCoroutine(openNews());

        //뉴스 갱신
        foreach(NewsData data in m_ctrl.newsCtrl.getNextNews())
        {
            Debug.Log(data.strCode);
        }
        m_ctrl.newsCtrl.setTodayNews(m_ctrl.newsCtrl.getNextNews());    //오늘 뉴스 데이터 갱신
        m_ctrl.newsCtrl.createNews(m_ctrl.newsCtrl.getTodayNews());      //뉴스 만들기

        //환경 뉴스 추가
        m_ctrl.newsCtrl.addTodayNews(m_ctrl.environmentCtrl.getEnvironmentNews());

        //뉴스 효과 적용
        m_ctrl.newsCtrl.setNewsEffectApply(m_ctrl.newsCtrl.getTodayNews());

        //다음 뉴스 초기화
        m_ctrl.newsCtrl.clearNextNews();    //다음날 뉴스 초기화
        
    }

    IEnumerator openNews()
    {
        Debug.Log("open");
        yield return new WaitForSeconds(2.5f);
        m_ctrl.newsCtrl.openNews();
    }
}
