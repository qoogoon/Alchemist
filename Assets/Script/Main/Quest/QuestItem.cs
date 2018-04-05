using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestItem : MonoBehaviour {
    private QuestData m_QuestData;
    public Image CharacterImage;

    public Text Title;
    public Text Summary;
    public GameObject compliteBtn;
    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setItemData(string strCode)
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_QuestData = new QuestData(strCode);

        //캐릭터 이미지
        Sprite characterSprite = (Sprite)Resources.Load("Image/Main/Character/" + m_QuestData.strCharacterCode, typeof(Sprite));
        CharacterImage.sprite = characterSprite;

        //제목
        Title.text = m_QuestData.strTitle;

        //요약
        StartCoroutine(QuestValueListener());
        
    }

    IEnumerator QuestValueListener()
    {
        while (!compliteBtn.active)
        {
            Debug.Log("listener");
            switch (m_QuestData.eType)
            {
                case QuestData.QUEST_TYPE.평판:

                    if (m_ctrl.reputationCtrl.getPlayerReputation() >= m_QuestData.nCompliteValue)
                    {
                        setComplite();//퀘스트 충족
                    }
                    else
                    {
                        Summary.text = "평판 : " + m_ctrl.reputationCtrl.getPlayerReputation() + " / " + m_QuestData.nCompliteValue;
                    }

                    break;
                case QuestData.QUEST_TYPE.골드:
                    if (m_ctrl.gameData.getPlayerGold() >= m_QuestData.nCompliteValue)
                    {
                        setComplite();//퀘스트 충족
                    }
                    else
                    {
                        Summary.text = "골드 : " + m_ctrl.gameData.getPlayerGold() + " / " + m_QuestData.nCompliteValue;
                    }

                    break;
                case QuestData.QUEST_TYPE.물약:
                    PotionData potion = new PotionData(m_QuestData.strTargetCode);
                    if (potion.getPlayersPotionAmount() >= m_QuestData.nCompliteValue)       //퀘스트 충족
                    {
                        setComplite();
                    }
                    else
                    {
                        Summary.text = potion.strName + " : " + potion.getPlayersPotionAmount() + " / " + m_QuestData.nCompliteValue;
                    }

                    break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void setComplite()
    {
        compliteBtn.SetActive(true);
    }

    public QuestData getQuestData()
    {
        return m_QuestData;
    }

    //완료
    public void complite()
    {
        QuestCtrl qCtrl = GameObject.Find("QuestCtrl").GetComponent<QuestCtrl>();
        qCtrl.compliteProceedingQuest(m_QuestData);
        qCtrl.openWindow();

        if (m_QuestData.strCharacterCode.Equals("CHAR_000"))
        {
            foreach (QuestData data in qCtrl.getAllQuestData())      //플레이어 퀘스트는 이어서 진행
            {
                if (data.strCharacterCode.Equals("CHAR_000") && data.nLv == m_QuestData.nLv + 1)
                {
                    qCtrl.startQuest(data);
                    break;
                }
            }
        }else
        {
            //대사
            DialogCtrl dCtrl = GameObject.Find("DialogCtrl").GetComponent<DialogCtrl>();
            dCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.QUEST_END, m_QuestData.strCode);
        }

        Destroy(gameObject);
    }

    public void showDetailWindow()
    {
        m_ctrl.questCtrl.QuestDetailWindow.SetActive(true);
        m_ctrl.questCtrl.QuestDetailWindow.GetComponent<QuestDetailObj>().setQuestData(m_QuestData);
    }

    
}
