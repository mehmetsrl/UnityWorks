using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileOperation : MonoBehaviour
{


    protected string xmlFile;
    private string _fileName;

    public string fileName
    {
        get
        {
            return _fileName;
        }

        set
        {
            _fileName = value;
        }
    }



    public FileOperation()
    {
       
    }

  

    public void print(string data)
    {
        string path = createFolder();
        path += fileName;
        StreamWriter file = File.AppendText(path);
        file.WriteLine(data);
        file.Close();
    }



  

    public virtual string createFolder()
    {
        return null; 
    }

 

  
    


}
