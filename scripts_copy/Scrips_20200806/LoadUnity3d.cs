using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadUnity3d : MonoBehaviour 
{      // Use this for initialization 
    public GameObject a;
    public GameObject b;
    public List<string> path;
    [TextArea]
    public string path1;
    public bool createperferb;
    void Start()     
    {
        //load1();
        for(int i = 0; i < path.Count; i++)
        {
             StartCoroutine(LoadScene(i));      

        }
    }      
    IEnumerator LoadScene(int i)     
    {         //文件路径，也就是我们打包的那个         
        WWW www = new WWW("file:///" + Application.dataPath + "/AB/" + path[i]+".unity3d");        
        yield return www;
        //Instantiate(www.assetBundle.mainAsset);
        for(int j = 0; j < www.assetBundle.GetAllAssetNames().Length; j++)
        {
            Debug.Log(www.assetBundle.GetAllAssetNames()[j]);
        }
        //a.transform.SetParent(b.transform);
        if (createperferb)
        {
            if (www.assetBundle.Contains(path1))
            {
                Instantiate(www.assetBundle.LoadAsset<GameObject>(path1));
                Debug.Log("SUCCESS!");
            }
            else
            {
                Debug.Log("fail!");
            }
        }
    }
    /*private IEnumerator LoadAsync()
    {
        string tempPathURL = "file:///" + Application.dataPath + " / AB / "+path1;

        bundleLoadRequest = AssetBundle.LoadFromFileAsync(tempPathURL);
        while (!bundleLoadRequest.isDone)
        {

            yield return bundleLoadRequest;
        }

        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            uiText.text += "Failed LoadAsync:" + tempPathURL;
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(loadGameObjName);
        yield return assetLoadRequest;

        GameObject prefab = assetLoadRequest.asset as GameObject;
        prefab.transform.position = Vector3.zero;
        loadObj = Instantiate(prefab);

        myLoadedAssetBundle.Unload(false);

    }*/
    public void load1()
    {
        AssetBundle ab = AssetBundle.LoadFromFile("AB/"+path1+".unity3d");
        //AssetBundleManifest manifest = manifesAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        Object[] obj = ab.LoadAllAssets<GameObject>();//加载出来放入数组中
        //创建出来
        foreach (Object o in obj)
        {
            Instantiate(o);
        }

    }
}