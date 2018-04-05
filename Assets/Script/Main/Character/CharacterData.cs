using UnityEngine;
using System.Collections;

public class CharacterData {
    public string strCode;
    public string strName;
    public CharacterData()
    {

    }

    public CharacterData(string strCharacterCode)
    {
        CharacterCtrl cCtrl = GameObject.Find("CharacterCtrl").GetComponent<CharacterCtrl>();
        foreach(CharacterData data in cCtrl.getAllCharacterData())
        {
            if (data.strCode.Equals(strCharacterCode))
            {
                strCode = data.strCode;
                strName = data.strName;
                break;
            }
            
        }
    }
}
