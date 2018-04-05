using UnityEngine;
using System;
using System.Xml;
using System.IO;

[ExecuteInEditMode]
[SerializeField]
public static class XmlCtrl {

    public static XmlDocument Load(TextAsset xmlFile)
    {
        MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
        XmlReader reader = XmlReader.Create(assetStream);
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(reader);
        }catch (Exception ex)
        {
            Debug.Log(ex.Message+" : 대사 못 불러왔음");
        }
        finally
        {
            Debug.Log("대사 불러옴");
        }

        return xmlDoc;
    }
	
}
