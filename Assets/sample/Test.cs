using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Test : MonoBehaviour {


    public GameObject mask;

	// Use this for initialization
	void Start () {

        if (mask == null)
        {
            return;
        }

        UnityAction<BaseEventData> onDrag = new UnityAction<BaseEventData>(MaskOnDrag);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener(onDrag);

        EventTrigger trigger = mask.AddComponent<EventTrigger>();
        trigger.triggers.Add(entry);

	}


    void MaskOnDrag(BaseEventData eventData)
    {
        mask.transform.localPosition += (Vector3)((PointerEventData)eventData).delta;
    }


}
