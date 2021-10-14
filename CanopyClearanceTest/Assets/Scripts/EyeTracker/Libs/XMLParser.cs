using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XMLParser { 

    //Singleton
    public static XMLParser Instance => _instance ?? (_instance = new XMLParser());

    private static XMLParser _instance;
    private XmlDocument _xmlDocument;
    private string _baseLocation;

    /// <summary>
    /// Constructor
    /// </summary>
    private XMLParser()
    {
        _xmlDocument = new XmlDocument();
        
    }

   

    public string baseLocation
    {
        get
        {
            return Application.streamingAssetsPath+ _baseLocation;
        }

        set
        {
            _baseLocation = value;
        }
    }


    public string getValue(string xmlFileName, string selectSingleNode, string attributeName = null, string checkValue = null)
    {
        _xmlDocument.Load(baseLocation + xmlFileName);
        XmlNode xmlNode = _xmlDocument.DocumentElement.SelectSingleNode(selectSingleNode);
        if (xmlNode != null)
        {
            if (attributeName != null)
            {
                for (int i = 0; i < xmlNode.Attributes.Count; i++)
                {
                    if (attributeName == xmlNode.Attributes[i].Name && xmlNode.Attributes[i].Value == checkValue)
                        return xmlNode.InnerText;

                }

            }
            else
            {
                return xmlNode.InnerText;
            }


        }

        return "There is no node!";

    }
    /// <summary>
    /// set value in XML file
    /// </summary>
    /// <param name="xmlFileName">xml file name</param>
    /// <param name="selectSingleNode">select single node. Ex: /book/author </param>
    /// <param name="value">value to set</param>
    public void setValue(string xmlFileName, string selectSingleNode, string value)
    {
        _xmlDocument.Load(baseLocation + xmlFileName);
        XmlNode xmlNode = _xmlDocument.DocumentElement.SelectSingleNode(selectSingleNode);
        if (xmlNode != null)
        {
            xmlNode.InnerText = value;
            _xmlDocument.Save(baseLocation + xmlFileName);
        }

    }

    /// <summary>
    /// this function getting value and automatically increment by 1 then save it.
    /// </summary>
    /// <param name="xmlFileName">xml file name</param>
    /// <param name="selectSingleNode">select single node. Ex: /book/author </param>
    public void automaticIncrementValue(string xmlFileName, string selectSingleNode)
    {
        _xmlDocument.Load(baseLocation + xmlFileName);
        XmlNode xmlNode = _xmlDocument.DocumentElement.SelectSingleNode(selectSingleNode);
        if (xmlNode != null)
        {
            string value = xmlNode.InnerText;
            int intValue = Convert.ToInt32(value);
            intValue++;
            xmlNode.InnerText = intValue.ToString();
            _xmlDocument.Save(baseLocation + xmlFileName);
        }

    }

    public string getAttributeValue(string xmlFileName, string selectSingleNode, string attributeName = null)
    {
        _xmlDocument.Load(baseLocation + xmlFileName);
        XmlNode xmlNode = _xmlDocument.DocumentElement.SelectSingleNode(selectSingleNode);
        if (xmlNode != null)
        {
            if (attributeName != null)
                for (int i = 0; i < xmlNode.Attributes.Count; i++)
                {
                    if (attributeName == xmlNode.Attributes[i].Name)
                        return xmlNode.Attributes[i].Value;
                }
        }

        return "There is no node!";
    }

    public string getValueWrtMultipleNodes(string xmlFileName, string selectSingleNode, string attributeName = null, string checkValue = null, bool isMultipleNodes = false)
    {
        _xmlDocument.Load(baseLocation + xmlFileName);
        XmlNode xmlNode = _xmlDocument.DocumentElement.SelectSingleNode(selectSingleNode);
        XmlNode parentNode = xmlNode.ParentNode;
        if (isMultipleNodes && parentNode.ChildNodes.Count > 1)
        {
            //has siblings
            for (int j = 0; j < parentNode.ChildNodes.Count; j++)
            {
                if (xmlNode != null)
                {
                    if (attributeName != null)
                    {
                        for (int i = 0; i < xmlNode.Attributes.Count; i++)
                        {
                            if (attributeName == xmlNode.Attributes[i].Name && xmlNode.Attributes[i].Value == checkValue)
                                return xmlNode.InnerText;

                        }

                    }

                }

                xmlNode = xmlNode.NextSibling;
            }
        }


        return "There is no node!";

    }


}
