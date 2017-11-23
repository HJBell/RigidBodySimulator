using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum UIWindowType
{
    Settings, Transform, Rigidbody, Controls, Collider
}

[System.Serializable]
public struct WindowInputField { public string key; public InputField value; }
[System.Serializable]
public struct WindowSlider { public string key; public Slider value; }
[System.Serializable]
public struct WindowToggle { public string key; public Toggle value; }
[System.Serializable]
public struct WindowText { public string key; public Text value; }

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
    [SerializeField]
    private WindowSlider[] Sliders;
    [SerializeField]
    private WindowToggle[] Toggles;
    [SerializeField]
    private WindowText[] Text;

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
        foreach (var slider in Sliders)
            slider.value.value = slider.value.minValue;
    }

    public string GetSetContentValue(string key, string value = "", bool overrideValue = false)
    {
        // Input fields.
        foreach (var inputField in InputFields)
        {
            if (inputField.key == key)
            {
                if (inputField.value.isFocused)
                    return inputField.value.text;
                else
                {
                    inputField.value.text = value;
                    return value;
                }
            }
        }

        // Sliders.
        foreach (var slider in Sliders)
        {
            if (slider.key == key)
            {
                if (overrideValue)
                    slider.value.value = float.Parse(value);
                return slider.value.value.ToString();
            }
        }

        // Toggles.
        foreach (var toggle in Toggles)
        {
            if (toggle.key == key)
            {
                if (overrideValue)
                    toggle.value.isOn = bool.Parse(value);
                return toggle.value.isOn.ToString();
            }
        }

        // Text.
        foreach (var text in Text)
        {
            if (text.key == key)
            {
                if (overrideValue)
                    text.value.text = value;
                return text.value.text;
            }
        }

        return value;
    }

}