using UnityEngine;
using System.Collections;

public class ReputationRankData {
    public string strCode;
    public string strTitle;
    public string strContents;
    public int nLv;
    public int nConditionValue;

    public ReputationRankData()
    {

    }

    public ReputationRankData(string strReputationCode)
    {
        ReputationCtrl rCtrl = GameObject.Find("ReputationCtrl").GetComponent<ReputationCtrl>();
        foreach(ReputationRankData data in rCtrl.getAllReputationRankData())
        {
            if (data.strCode.Equals(strReputationCode))
            {
                strCode = data.strCode;
                strTitle = data.strTitle;
                strContents = data.strContents;
                nLv = data.nLv;
                nConditionValue = data.nConditionValue;
            }
        }
    }
}
