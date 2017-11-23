using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_ButtonHelper : MonoBehaviour {

    [SerializeField]
    private string ObjectName = "";
    [SerializeField]
    private string MessageName = "";


    //----------------------------------------Public Functions---------------------------------------

    public void OnClick()
    {
        GameObject.Find(ObjectName).SendMessage(MessageName);
    }
}
