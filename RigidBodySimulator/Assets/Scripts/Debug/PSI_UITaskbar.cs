﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PSI_UITaskbar : MonoBehaviour {

    [SerializeField]
    private PSI_UIWindow WindowPrefab;
    [SerializeField]
    private PSI_UITaskbarButton TaskbarButtonPrefab;
    [SerializeField]
    private Transform WindowParent;
    [SerializeField]
    private int TaskbarSpacing = 100;
    [SerializeField]
    private List<string> WindowsToCreate = new List<string>();

    private Dictionary<string, PSI_UIWindow> mWindows = new Dictionary<string, PSI_UIWindow>();
    private Dictionary<string, PSI_UITaskbarButton> mTaskbarButtons = new Dictionary<string, PSI_UITaskbarButton>();
    private List<Transform> mActiveButtonTransforms = new List<Transform>();


    //----------------------------------------Unity Functions----------------------------------------

    private void Start()
    {
        if(!WindowPrefab || !TaskbarButtonPrefab | !WindowParent)
            Debug.LogError("Taskbar variables missing!");

        for(int i = 0; i < WindowsToCreate.Count; i++)
        {
            var windowTitle = WindowsToCreate[i];
            if (mWindows.ContainsKey(windowTitle) && mTaskbarButtons.ContainsKey(windowTitle)) continue;

            // Creating the window.
            var window = Instantiate(WindowPrefab);
            var windowObj = window.gameObject;
            windowObj.transform.SetParent(WindowParent);
            windowObj.transform.position = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) * (0.5f + (float)i * 0.03f);
            window.pTitle = windowTitle;
            window.SetContent("");
            mWindows[windowTitle] = window;

            // Creating the taskbar button.
            var taskbarButton = Instantiate(TaskbarButtonPrefab);
            var taskbarButtonObj = taskbarButton.gameObject;
            taskbarButtonObj.transform.SetParent(this.transform);
            mActiveButtonTransforms.Add(taskbarButtonObj.transform);
            taskbarButtonObj.transform.GetChild(0).GetComponent<Text>().text = windowTitle;
            taskbarButton.GetComponent<Button>().onClick.AddListener(delegate { TaskbarButtonClicked(windowTitle); });
            mTaskbarButtons[windowTitle] = taskbarButton;
        }

        MinimiseAllWindows();
    }


    //----------------------------------------Public Functions---------------------------------------
    
    public void TaskbarButtonClicked(string title)
    {
        if (!mTaskbarButtons.ContainsKey(title)) return;
        mTaskbarButtons[title].Disable();
        if (mActiveButtonTransforms.Contains(mTaskbarButtons[title].transform))
            mActiveButtonTransforms.Remove(mTaskbarButtons[title].transform);
        mWindows[title].Maximise();
        UpdateTaskbar();
    }

    public void WindowMinimised(string title)
    {
        if (!mTaskbarButtons.ContainsKey(title)) return;
        mTaskbarButtons[title].Enable();
        if(!mActiveButtonTransforms.Contains(mTaskbarButtons[title].transform))
            mActiveButtonTransforms.Add(mTaskbarButtons[title].transform);
        UpdateTaskbar();
    }

    public void WindowHasFocus(string title)
    {
        if (!mTaskbarButtons.ContainsKey(title)) return;
        mWindows[title].transform.SetAsLastSibling();
    }

    public PSI_UIWindow GetWindow(string title)
    {
        if (!mWindows.ContainsKey(title)) return null;
        return mWindows[title];
    }


    //----------------------------------------Private Functions--------------------------------------

    private void MinimiseAllWindows()
    {
        foreach (var window in mWindows)
            window.Value.Minimise();
    }

    private void UpdateTaskbar()
    {
        int activeButtonIndex = 0;
        foreach(var buttonTrans in mActiveButtonTransforms)
        {
            var buttonPos = buttonTrans.position;
            buttonPos.y = this.transform.position.y;
            buttonPos.x = (TaskbarSpacing * 0.6f) + TaskbarSpacing * activeButtonIndex;
            buttonTrans.position = buttonPos;
            activeButtonIndex++;
        }
    }

    
}
