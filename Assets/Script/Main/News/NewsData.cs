using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewsData {
    public string strTitle;
    public string strContents;
    public string strCode;
    public E_TYPE eType;
    public E_EFFECT eEffect;
    public E_VALUE eValue;
    public int nValue;
    public List<string> arrTargetCode;

    private Controller m_ctrl;

    public enum E_TYPE
    {
        환경, 모험가, 학계
    }
    public enum E_EFFECT
    {
        수요, 값, 방문, 호감도, 레벨, 사망, 평판
    }
    public enum E_VALUE
    {
        증가, 감소
    }
	public NewsData()
    {

    }

    public NewsData(string strNewsCode)
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        foreach (NewsData data in m_ctrl.newsCtrl.getAllNewsData())
        {
            if (data.strCode.Equals(strNewsCode))
            {
                strTitle = data.strTitle;
                strContents = data.strContents;
                strCode = data.strCode;
                eType = data.eType;
                eEffect = data.eEffect;
                nValue = data.nValue;
                if (nValue >= 0)
                {
                    eValue = E_VALUE.증가;
                }else
                {
                    eValue = E_VALUE.감소;
                }
                arrTargetCode = data.arrTargetCode;
                break;
            }
        }
    }
}
