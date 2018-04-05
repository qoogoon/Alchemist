using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaterialObj : MonoBehaviour {
    //public Resource
    public Text TitleTxt;
    public Text GoldTxt;
    public Text NewMaterialTxt;
    public GameObject selectPointerImage;
    public GameObject m_objShapeViewParent;
    public GameObject m_objShapePhysicParent;
    public GameObject selectMaterialObj;
    

    //public value
    public string m_strCode; 
    public MaterialData m_MaterialData { get; private set; }

    //private
    private string m_strName;
    private int m_nGold;
    private GameObject[] m_objShapes;
    private GameObject[] m_objShapesImage;
    private bool[] m_objShapesMap;
    private int m_nAmount;
    
    
    private bool m_bShape = false;   //true shap, false Image
    private Controller m_ctrl;

    // Use this for initialization
    void Awake () {
        init();
        setShapeObj();
        selectPointerImage.SetActive(false);
        foreach(Transform obj in gameObject.GetComponentsInChildren<Transform>())
        {
            if (obj.name.Equals("selectMaterial"))
            {
                selectMaterialObj = obj.gameObject;
                selectMaterialObj.SetActive(false);
                break;
            }
        }
    }

    void init()
    {
        m_nAmount = 0;
        m_objShapes = new GameObject[9];
        m_objShapesImage = new GameObject[9];
        m_objShapesMap = new bool[9];
        for(int i =0;i< m_objShapesMap.Length; i++)
        {
            m_objShapesMap[i] = false;
        }
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        
    }

    //연금 모양 생성
    void setShapeObj()
    {
        if(m_objShapePhysicParent != null)
        {
            Transform[] arrShapes = m_objShapePhysicParent.GetComponentsInChildren<Transform>();
            for (int i = 0; i < arrShapes.Length-1; i++)
            {
                m_objShapes[i] = arrShapes[i + 1].gameObject;
            }
            
        }
        if (m_objShapeViewParent != null)
        {
            Transform[] arrShapes2x2 = m_objShapeViewParent.GetComponentsInChildren<Transform>();
            for (int i = 0; i < arrShapes2x2.Length - 1; i++)
            {
                m_objShapesImage[i] = arrShapes2x2[i + 1].gameObject;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    //셀 회전하기
    public void rotateCell()
    {
        GameObject.Find("DutorialCtrl").GetComponent<DutorialCtrl>().speach_bubble01.SetActive(false);
        m_objShapePhysicParent.transform.Rotate(new Vector3(0f, 0f, 90f));
        m_objShapeViewParent.transform.Rotate(new Vector3(0f, 0f, 90f));
    }
    public void setTitle(string strTitle)
    {
        m_strName = strTitle;

        if (m_nAmount != 0)
        {
            TitleTxt.text = strTitle + "x" + m_nAmount;
        }else
        {
            TitleTxt.text = strTitle;
        }
    }

    public void setGold(int nGold)
    {
        setGoldText(nGold + "G");
        m_nGold = nGold;

        /*
        Color32 imageColor = (Color32)gameObject.GetComponent<Image>().color;
        Color32 titleColor = (Color32)TitleTxt.GetComponent<Text>().color;
        Color32 goldColor = (Color32)GoldTxt.GetComponent<Text>().color;
        Color32 shapeColor;
        byte byteAcolor = 255;
        if (m_nGold <= m_ctrl.gameData.getPlayerGold())
        {
            byteAcolor = 255;
        }
        else
        {
            byteAcolor = 150;
        }
        gameObject.GetComponent<Image>().color = new Color32(imageColor.r, imageColor.g, imageColor.b, byteAcolor);
        TitleTxt.GetComponent<Text>().color = new Color32(titleColor.r, titleColor.g, titleColor.b, byteAcolor);
        GoldTxt.GetComponent<Text>().color = new Color32(goldColor.r, goldColor.g, goldColor.b, byteAcolor);
        foreach (GameObject shapeImageObj in m_objShapesImage)
        {
            shapeColor = (Color32)shapeImageObj.GetComponent<Image>().color;
            shapeImageObj.GetComponent<Image>().color = new Color32(shapeColor.r, shapeColor.g, shapeColor.b, byteAcolor);
        }
        */
    }

    public int getGold()
    {
        return m_nGold;
    }
    public void setGoldText(string strGoldText)
    {
        GoldTxt.text = strGoldText;


    }

    public void setSelectMaterial()
    {
        
        GameData gData = new GameData();
        foreach(GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
        {
            if(obj != gameObject)
            {
                obj.GetComponent<MaterialObj>().selectMaterialObj.SetActive(false);
            }
            else
            {
                selectMaterialObj.SetActive(true);
            }
        }
    }

    public void clearSelectMaterial()
    {
        selectMaterialObj.SetActive(false);
    }

    public void setMaterialActive(bool bActive)
    {
        Color32 imageColor;
        Color32 titleColor = (Color32)TitleTxt.GetComponent<Text>().color;
        
        byte byteAcolor = 255;
        if (bActive)
        {
            byteAcolor = 255;
        }
        else
        {
            byteAcolor = 150;
        }
        gameObject.GetComponent<BoxCollider>().enabled = bActive;
        foreach(GameObject obj in m_objShapesImage)
        {
            imageColor = obj.GetComponent<Image>().color;
            obj.GetComponent<Image>().color = new Color32(imageColor.r, imageColor.g, imageColor.b, byteAcolor);
        }
        
        TitleTxt.GetComponent<Text>().color = new Color32(titleColor.r, titleColor.g, titleColor.b, byteAcolor);
    }

    //갯수(실제 데이터가 변경 되는 것은 아니고 일종의 임시 저장임.)
    /*
    public void setMaterialAmount(int nAmount)
    {
        m_nAmount = nAmount;

        Color32 imageColor = (Color32)gameObject.GetComponent<Image>().color;
        Color32 titleColor = (Color32)TitleTxt.GetComponent<Text>().color;
        Color32 shapeColor;
        byte byteAcolor = 255;
        if (m_nAmount != 0)
        {
            TitleTxt.text = m_strName + "x" + m_nAmount;
            byteAcolor = 255;
        }
        else
        {
            TitleTxt.text = m_strName;
            byteAcolor = 150;
        }
        gameObject.GetComponent<Image>().color = new Color32(imageColor.r, imageColor.g, imageColor.b, byteAcolor);
        TitleTxt.GetComponent<Text>().color = new Color32(titleColor.r, titleColor.g, titleColor.b, byteAcolor);
        foreach (GameObject shapeImageObj in m_objShapesImage)
        {
            shapeColor = (Color32)shapeImageObj.GetComponent<Image>().color;
            shapeImageObj.GetComponent<Image>().color = new Color32(shapeColor.r, shapeColor.g, shapeColor.b, byteAcolor);
        }
    }
    */
    /*
    public int getMaterialAmount()
    {
        return m_nAmount;
    }
    */
    //이미지 설정
    public void setItemImage(string strCode)
    {
        gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Image/Main/Alchemy/Item/" + strCode,typeof(Sprite));
        m_strCode = strCode;
        //Debug.Log(m_strCode);
        //Debug.Log("Image/Main/Image/Item/" + strCode);
    }

    public string getCode()
    {
        
        return m_strCode;
    }

    public void setCode(string strCode)
    {
        m_strCode = strCode;
        m_MaterialData = new MaterialData(strCode);
    }

    public void setShape(bool[,] strShapeMap)
    {
        processSetShap(strShapeMap, m_objShapesMap, m_objShapesImage);
        processSetShap(strShapeMap, m_objShapesMap, m_objShapes);

        m_objShapePhysicParent.SetActive(false);
        setShapeImageVisiable(false);
        setItemImageVisiable(true);    //모양 안보이게 하기
    }

    private void processSetShap(bool[,] strShapeMap, bool[] arrShapeMap, GameObject[] arrGameObjs)
    {
        
        int nShapeCount = 0;
        for (int i = 0; i < strShapeMap.GetLength(0); i++)    //모양 데이터 정리
        {
            for (int j = 0; j < strShapeMap.GetLength(1); j++)
            {
                if (strShapeMap[i,j])
                {
                    arrShapeMap[nShapeCount] = true;
                }
                else
                {
                    arrShapeMap[nShapeCount] = false;
                }
                nShapeCount++;
            }
        }

        for(int i = 0; i< arrGameObjs.Length; i++)//모양 형성
        {
            arrGameObjs[i].SetActive(arrShapeMap[i]);
        }
  
    }
    //복제 되고 꼭 불리워야함
    public void setDuplicateImage()
    {
        m_objShapePhysicParent.SetActive(true);
        m_objShapeViewParent.SetActive(false);
    }

    //겉으로 보여지는 이미지
    public void setShapeImageVisiable(bool bVisible)
    {
        m_objShapeViewParent.SetActive(bVisible);

        //새 재료 텍스트가 있다면 숨기기
        if (NewMaterialTxt.enabled && bVisible)
        {
            NewMaterialTxt.enabled = false;
        }

    }

    public void setItemImageVisiable(bool bVisiable)
    {
        gameObject.GetComponent<Image>().enabled = bVisiable;
    }

    public void setVisiablePointer(bool visiable)
    {
        selectPointerImage.SetActive(visiable);
    }
}
