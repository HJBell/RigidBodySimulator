using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PSI_UIDraggable : MonoBehaviour {

    [SerializeField]
    private List<EventTrigger> DragEventTriggers = new List<EventTrigger>();

    private Vector2 mDragOffset = Vector2.zero;


    //----------------------------------------Unity Functions----------------------------------------

    protected virtual void Start()
    {
        foreach(var eventTrigger in DragEventTriggers)
        {
            EventTrigger.Entry dragBeginEntry = new EventTrigger.Entry();
            dragBeginEntry.eventID = EventTriggerType.BeginDrag;
            dragBeginEntry.callback.AddListener((eventData) => { OnBeginDrag(); });
            eventTrigger.triggers.Add(dragBeginEntry);

            EventTrigger.Entry dragEntry = new EventTrigger.Entry();
            dragEntry.eventID = EventTriggerType.Drag;
            dragEntry.callback.AddListener((eventData) => { OnDrag(); });
            eventTrigger.triggers.Add(dragEntry);
        }        
    }


    //----------------------------------------Public Functions---------------------------------------

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
}
