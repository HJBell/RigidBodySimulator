using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PSI_UIWindow : MonoBehaviour {

    [SerializeField]
    private Text Title;
    [SerializeField]
    private Text Content;

    private Vector2 mDragOffset = Vector2.zero;
    private PSI_UITaskbar mTaskbar;

    public string pTitle { get { return Title.text; } set { Title.text = value; } }


    //----------------------------------------Unity Functions----------------------------------------

    private void OnEnable()
    {
        if (!Title || !Content)
            Debug.LogError("Window variables missing!");

        mTaskbar = FindObjectOfType<PSI_UITaskbar>();
        if (!mTaskbar)
            Debug.LogError("Window could not find a taskbar object");
    }


    //----------------------------------------Public Functions---------------------------------------

    public void SetContent(string content)
    {
        Content.text = content;
    }

    public void AddContentLine(string content)
    {
        SetContent(Content.text + "\n" + content);
    }

    public void ClearContent()
    {
        SetContent("");
    }

    public void Minimise()
    {
        mTaskbar.WindowMinimised(pTitle);
        this.gameObject.SetActive(false);
    }

    public void Maximise()
    {
        this.gameObject.SetActive(true);
    }

    public void OnBeginDrag()
    {
        mDragOffset.x = this.transform.position.x - Input.mousePosition.x;
        mDragOffset.y = this.transform.position.y - Input.mousePosition.y;
    }

    public void OnDrag()
    {
        var currentPos = this.transform.position;
        currentPos.x = Input.mousePosition.x + mDragOffset.x;
        currentPos.y = Input.mousePosition.y + mDragOffset.y;

        float width = this.GetComponent<RectTransform>().rect.width;
        float height = this.GetComponent<RectTransform>().rect.height;

        currentPos.x = Mathf.Clamp(currentPos.x, width / 2f, Camera.main.pixelWidth - width / 2f);
        currentPos.y = Mathf.Clamp(currentPos.y, height / 2f, Camera.main.pixelHeight - height / 2f);
        this.transform.position = currentPos;
    }

    public void HasFocus()
    {
        mTaskbar.WindowHasFocus(pTitle);
    }

    public void LostFocus()
    {

    }
}