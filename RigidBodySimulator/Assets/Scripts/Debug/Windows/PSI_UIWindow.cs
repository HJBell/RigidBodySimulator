using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum UIWindowType
{
    Transform, Rigidbody
}

[System.Serializable]
public struct WindowInputField { public string key; public InputField value; }

public class PSI_UIWindow : PSI_UIDraggable {

    [SerializeField]
    private UIWindowType Type;
    [SerializeField]
    private List<EventTrigger> FocusEventTriggers = new List<EventTrigger>();
    [SerializeField]
    private Button MinimiseButton;
    [SerializeField]
    private Text TitleTextObj;
    [SerializeField]
    private WindowInputField[] InputFields;

    public UIWindowType pType { get { return Type; } }


    //----------------------------------------Unity Functions----------------------------------------

    protected override void Start()
    {
        base.Start();

        foreach (var eventTrigger in FocusEventTriggers)
        {
            EventTrigger.Entry mouseDownEntry = new EventTrigger.Entry();
            mouseDownEntry.eventID = EventTriggerType.PointerDown;
            mouseDownEntry.callback.AddListener((eventData) => { HasFocus(); });
            eventTrigger.triggers.Add(mouseDownEntry);
        }

        MinimiseButton.onClick.AddListener(delegate { Minimise(); });
    }


    //----------------------------------------Public Functions---------------------------------------

    public virtual void Init(Transform parent, Vector2 pos)
    {
        TitleTextObj.text = pType.ToString();
        this.transform.SetParent(parent);
        this.transform.position = pos;
    }

    public void Minimise()
    {
        FindObjectOfType<PSI_UITaskbar>().WindowMinimised(pType);
        this.gameObject.SetActive(false);
    }

    public void Maximise()
    {
        this.gameObject.SetActive(true);
    }

    public void HasFocus()
    {
        FindObjectOfType<PSI_UITaskbar>().WindowHasFocus(pType);
    }

    public void ResetContent()
    {
        foreach (var inputField in InputFields)
            inputField.value.text = "";
    }

    public string GetSetContentValue(string key, string value)
    {
        foreach (var inputField in InputFields)
        {
            if (inputField.key == key)
            {
                if (inputField.value.isFocused)
                    return inputField.value.text;
                else
                    inputField.value.text = value;
            }
        }
        return value;
    }

}