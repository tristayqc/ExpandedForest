using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExprManager : MonoBehaviour
{
    [Tooltip("This field controls all the arrow triggers")]
    [SerializeField] List<GameObject> ArrowTriggers;
    [Tooltip("This field controls canvas triggers shown at destination")]
    [SerializeField] List<GameObject> CanvasTriggers;
    [Tooltip("check if reversed")]
    public bool reversed;
    [Header("Where xrRig jumps to")]
    public Transform startPoint;
    [SerializeField] GameObject xrRig;
    

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ArrowTriggers.Count; ++i)
        {
            ArrowTriggers[i].GetComponent<ShowArrows>().back = reversed;
        }
        for (int i = 0; i < CanvasTriggers.Count; ++i)
        {
            CanvasTriggers[i].GetComponent<ShowCanvas>().back = reversed;
        }
        xrRig.GetComponent<TeleportCamLoc>().startPos = startPoint;
        GameObject controllerR = xrRig.transform.Find("Controller Right").gameObject;
        controllerR.GetComponent<VarjoExample.Locomotion>().startPos = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
