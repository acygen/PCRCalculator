using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemWindowMessage : MonoBehaviour
{
    public Text messageText;
    public delegate void configDelegate();
    public configDelegate config;
    public configDelegate cancel;
    public void SetWindowMassage(string word,configDelegate config,configDelegate cancel = null)
    {
        messageText.text = word;
        this.config = config;
        this.cancel = cancel;
    }
    public void ConfigButton()
    {
        config?.Invoke();
        Destroy(gameObject);
    }
    public void CancalButton()
    {
        cancel?.Invoke();
        Destroy(gameObject);
    }

}
