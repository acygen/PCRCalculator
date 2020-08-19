using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemWindowMessage : MonoBehaviour
{
    public Text messageText;
    public delegate void configDelegate();
    public configDelegate config;
    public void SetWindowMassage(string word,configDelegate config)
    {
        messageText.text = word;
        this.config = config;
    }
    public void ConfigButton()
    {
        config();
        Destroy(gameObject);
    }
    public void CancalButton()
    {
        Destroy(gameObject);
    }

}
