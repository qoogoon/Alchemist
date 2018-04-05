using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewsItemObj : MonoBehaviour {
    public Text NewsTitle;
    public Image NewsIcon;
    public Sprite enviromentIcon;
    public Sprite adventorIcon;
    public Sprite alchemyIcon;

    private Text boardDetialText;
    private NewsData m_NewsData;
    private Controller m_ctrl;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setNewsData(NewsData data)
    {
        boardDetialText = GameObject.Find("NewsDetialText").GetComponent<Text>();
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_NewsData = data;
    }
    public void setNewsTitle(string strTitleText)
    {
        NewsTitle.text = strTitleText;
    }

    public void setNewsIcon(NewsData.E_TYPE eType)
    {
        switch (eType)
        {
            case NewsData.E_TYPE.환경:
                NewsIcon.sprite = enviromentIcon;
                break;
            case NewsData.E_TYPE.모험가:
                NewsIcon.sprite = adventorIcon;
                break;
            case NewsData.E_TYPE.학계:
                NewsIcon.sprite = alchemyIcon;
                break;

        }
    }

    //아이템 선택
    public void selectItem()
    {
        //선택한 항목으로 세부 정보창 글귀 변경
        
        boardDetialText.text = m_NewsData.strContents;
        
        boardDetialText.text = boardDetialText.text.Replace("\\n", "\n");
        boardDetialText.text = boardDetialText.text.Replace("%d", m_NewsData.nValue.ToString());
        //선택 타이틀 색 변경
        for (int i = 0; i < m_ctrl.newsCtrl.arrItems.Count; i++)
        {
            m_ctrl.newsCtrl.arrItems[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);   
        }

        gameObject.GetComponent<Image>().color = new Color32(255, 179, 179, 255);
    }
}
