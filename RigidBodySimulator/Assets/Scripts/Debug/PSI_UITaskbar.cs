using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PSI_UITaskbar : MonoBehaviour {

    [SerializeField]
    private List<PSI_UIWindow> WindowPrefabs = new List<PSI_UIWindow>();
    [SerializeField]
    private Transform WindowParent;
    [SerializeField]
    private PSI_UITaskbarButton TaskbarButtonPrefab;
    [SerializeField]
    private int TaskbarSpacing = 100;

    private Dictionary<UIWindowType, PSI_UIWindow> mWindows = new Dictionary<UIWindowType, PSI_UIWindow>();
    private Dictionary<UIWindowType, PSI_UITaskbarButton> mTaskbarButtons = new Dictionary<UIWindowType, PSI_UITaskbarButton>();
    private List<Transform> mActiveButtonTransforms = new List<Transform>();


    //----------------------------------------Unity Functions----------------------------------------

    private void Start()
    {
        if(!TaskbarButtonPrefab | !WindowParent)
            Debug.LogError("Taskbar variables missing!");

        int index = 0;
        foreach(var windowPrefab in WindowPrefabs)
        {
            var type = windowPrefab.pType;

            // Creating the window.
            mWindows[type] = Instantiate(windowPrefab);
            var windowPos = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) * (0.5f + (float)index * 0.03f);
            mWindows[type].Init(WindowParent, windowPos);

            // Creating the taskbar button.
            var taskbarButton = Instantiate(TaskbarButtonPrefab);
            var taskbarButtonObj = taskbarButton.gameObject;
            taskbarButtonObj.transform.SetParent(this.transform);
            mActiveButtonTransforms.Add(taskbarButtonObj.transform);
            taskbarButtonObj.transform.GetChild(0).GetComponent<Text>().text = type.ToString();
            taskbarButton.GetComponent<Button>().onClick.AddListener(delegate { TaskbarButtonClicked(type); });
            mTaskbarButtons[type] = taskbarButton;

            index++;
        }

        MinimiseAllWindows();
    }


    //----------------------------------------Public Functions---------------------------------------

    public void TaskbarButtonClicked(UIWindowType type)
    {
        if (!mTaskbarButtons.ContainsKey(type)) return;
        mTaskbarButtons[type].Disable();
        if (mActiveButtonTransforms.Contains(mTaskbarButtons[type].transform))
            mActiveButtonTransforms.Remove(mTaskbarButtons[type].transform);
        mWindows[type].Maximise();
        UpdateTaskbar();
    }

    public void WindowMinimised(UIWindowType type)
    {
        if (!mTaskbarButtons.ContainsKey(type)) return;
        mTaskbarButtons[type].Enable();
        if (!mActiveButtonTransforms.Contains(mTaskbarButtons[type].transform))
            mActiveButtonTransforms.Add(mTaskbarButtons[type].transform);
        UpdateTaskbar();
    }

    public void WindowHasFocus(UIWindowType type)
    {
        if (!mTaskbarButtons.ContainsKey(type)) return;
        mWindows[type].transform.SetAsLastSibling();
    }

    public PSI_UIWindow GetWindow(UIWindowType type)
    {
        if (!mWindows.ContainsKey(type)) return null;
        return mWindows[type];
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
