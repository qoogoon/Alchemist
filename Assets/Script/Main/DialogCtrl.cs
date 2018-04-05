using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;

public class DialogCtrl : MonoBehaviour {
    public GameObject DialogTextObj;
    public GameObject DialogImgLeftObj;
    public GameObject DialogImgRightObj;
    public GameObject DialogObj;
    public Text CharacterNameText;

    private GameData m_gData;

    private Text m_tDialogContents; //대사
    private Image m_DialogCharLeftImage;    //캐릭터 이미지    
    private Image m_DialogCharRightImage;    //캐릭터 이미지    
    private ArrayList m_arrDialogText;
    private ArrayList m_arrDialogCharacter;  //대사 치는 캐릭터
    private ArrayList m_arrDialogImgSide;    //캐릭터 이미지 위치
    private int m_nDialogCounter = 0;
    public bool m_bDialoging;              //다이어로그 중
    public bool m_bTyping;
    private float m_fClickTimeCount = 0;      //클릭 허용 시간 카운트
    private Controller m_ctrl;

    enum E_ImagePosition
    {
        RIGHT, LEFT
    }

    public enum E_DIALOG_TYPE
    {
        TUTORIAL, QUEST_START, QUEST_END, EVENT
    }
    void init()
    {
        m_gData = new GameData();
        m_tDialogContents = DialogTextObj.GetComponent<Text>();
        m_DialogCharLeftImage = DialogImgLeftObj.GetComponent<Image>();
        m_DialogCharRightImage = DialogImgRightObj.GetComponent<Image>();
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();

        m_arrDialogText = new ArrayList();
        m_arrDialogCharacter = new ArrayList();
        m_arrDialogImgSide = new ArrayList();
        m_bDialoging = false;
        m_bTyping = false;
    }

    // Use this for initialization
    void Awake () {
        init();
    }
	
	// Update is called once per frame
	void Update () {
        if (m_bDialoging)
        {
            m_fClickTimeCount = m_fClickTimeCount + Time.deltaTime;
        }else
        {
            m_fClickTimeCount = 0;
        }
        
        //Debug.Log(m_fClickTimeCount);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name.Equals(DialogObj.name) && m_fClickTimeCount > 0.25)    //카드겜
                {
                    if (m_bTyping)
                    {
                        m_bTyping = false;
                    }else if(m_arrDialogText.Count> m_nDialogCounter)
                    {
                        setDialog(m_nDialogCounter++);
                        m_fClickTimeCount = 0;
                    }
                    else
                    {
                        GameObject.Find("QuestCtrl").GetComponent<QuestCtrl>().bListening = true;
                        destroyDialog();
                        m_fClickTimeCount = 0;
                    }
                    
                }
            }
        }
    }

    public void createDialog(E_DIALOG_TYPE type, string strCode) 
    {
        DialogObj.transform.position = Vector3.zero;
        string strType = "";
        switch (type)
        {
            case E_DIALOG_TYPE.TUTORIAL:
                strType = "Tutorial";
                break;

            case E_DIALOG_TYPE.QUEST_START:
                strType = "Quest_Start";
                break;

            case E_DIALOG_TYPE.QUEST_END:
                strType = "Quest_End";
                break;

            case E_DIALOG_TYPE.EVENT:
                strType = "Event";
                break;
        }
        Debug.Log(strType + ":" + strCode);
        XmlDocument dialogXml = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Dialog/"+ strType, typeof(TextAsset)));
        m_bDialoging = true;
        
        foreach (XmlNode parentNode in dialogXml.SelectNodes("List/Scene")) //대사 정보 배열에 넣기
        {
            if (parentNode.Attributes[0].InnerText.Equals(strCode)){
                foreach(XmlNode node in parentNode)
                {
                    m_arrDialogText.Add(node.InnerText.Replace("\n","").Replace("\\n","\n").Replace("\t",""));  //데이터 값 수정
                    m_arrDialogCharacter.Add(node.Attributes[0].Value);
                    m_arrDialogImgSide.Add(node.Attributes[1].Value);
                }
                break;
            }
        }
        
        setDialog(m_nDialogCounter++);
    }

    public void destroyDialog() //다이얼로그 제거
    {
        DialogObj.transform.localPosition = Vector3.right * 1500f;
        m_bDialoging = false;

        //초기화
        m_arrDialogText.Clear();
        m_arrDialogCharacter.Clear();
        m_arrDialogImgSide.Clear();
        m_nDialogCounter = 0;
    }

    void setDialog(int nDialogNo)
    {
        StartCoroutine(typingCoroutine(m_arrDialogText[nDialogNo].ToString()));
        Debug.Log((string)m_arrDialogCharacter[nDialogNo]);
        CharacterData cData = new CharacterData((string)m_arrDialogCharacter[nDialogNo]);
        CharacterNameText.text = cData.strName;
        if (m_arrDialogImgSide[nDialogNo].Equals("left"))
        {
            m_DialogCharLeftImage.sprite = (Sprite)Resources.Load("Image/Main/Character/" + m_arrDialogCharacter[nDialogNo].ToString(), typeof(Sprite));
            DialogImgLeftObj.SetActive(true);
            DialogImgRightObj.SetActive(false);
        }
        else
        {
            m_DialogCharRightImage.sprite = (Sprite)Resources.Load("Image/Main/Character/" + m_arrDialogCharacter[nDialogNo].ToString(), typeof(Sprite));
            DialogImgLeftObj.SetActive(false);
            DialogImgRightObj.SetActive(true);
        }
    }

    IEnumerator typingCoroutine(string strDialog)
    {
        m_bTyping = true;
        for (int i = 0; i < strDialog.Length; i++)
        {
            if(strDialog.Substring(i,1).Equals(" "))
            {
                i++;
            }
            m_tDialogContents.text = strDialog.Remove(i);
            if (!m_bTyping)
            {
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
        m_bTyping = false;
        m_tDialogContents.text = strDialog;
    }

    public bool isDialoging()
    {
        return m_bDialoging;
    }
}
