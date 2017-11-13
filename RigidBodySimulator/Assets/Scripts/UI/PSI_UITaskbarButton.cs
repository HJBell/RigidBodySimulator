using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PSI_UITaskbarButton : MonoBehaviour {

    [SerializeField]
    private Text Title;

    public string pTitle { get { return Title.text; } set { Title.text = value; } }


    //----------------------------------------Public Functions---------------------------------------

    public void Enable()
    {
        this.gameObject.SetActive(true);
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
