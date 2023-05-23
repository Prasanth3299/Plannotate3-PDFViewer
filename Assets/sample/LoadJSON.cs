using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Data
{
    public string firstName = "";
}

public class LoadJSON : MonoBehaviour
{
    void Start()
    {
        string jsonPath = "F:/GitHub/Plannotate3-PDFViewer/Assets/sample/sample2.json";
        string jsonStr = File.ReadAllText(jsonPath); // using System;
        var jsonObject = (JObject)JsonConvert.DeserializeObject(jsonStr);
        string str = jsonObject["lastName"].Value<string>();
        print(str);

    }

  
}
