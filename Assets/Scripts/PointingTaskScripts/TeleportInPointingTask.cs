using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Varjo.XR;

/// <summary>
///  Attach this script to the experiment manager (an empty gameobject)
/// </summary>
public class TeleportInPointingTask : MonoBehaviour
{
    [Tooltip("This field sets the reference landmarks' position")]
    [SerializeField] List<Transform> ReferenceLandmarkTransforms;

    [Tooltip("This field sets the reference landmarks' teleporting position \n Participants are teleported to the positions near by the reference landmarks")]
    [SerializeField] List<Transform> ReferenceLandmarkTeleportTransforms;

    [Tooltip("This field sets the target landmark images that participants are asked to point to")]
    [SerializeField] List<GameObject> LandmarkImages;

    [Tooltip("This field sets a TMP_Text")]
    [SerializeField] TMP_Text m_TextComponent;

    public Transform xrRig;                                 // What should we move when teleporting
    public Transform mainCamera;                            // Where is our head  

    public Vector3 GroundTruthDirection { get; set; }
    public string referenceLandmark_name { get; set; }
    public string displayLandmark_name { get; set; }
    public bool taskFinish { get; set; }

    int referencelandmark_index = 0;      // control the reference landmark 
    int displaylandmark_index = 0;        // control the landmark to display 
    int previous_displaylandmark_index = 0;
    bool buttonDown;
    Transform LandmarkImage_position;

    List<int> CurrentLandmark_IndexList;
    List<int> PointToLandmark_IndexList;
    List<int> Random_IndexList;
    Dictionary<int, List<int>> TaskOrder; // Save Task Order - (key: Teleproting Landmarks; value: Pointing-to landmarks)

    // Start is called before the first frame update
    void Start()
    {

        //Create Index List from Landmarks
        CurrentLandmark_IndexList = new List<int>();
        PointToLandmark_IndexList = new List<int>();
        Random_IndexList = new List<int>();
        for (int i = 0; i < ReferenceLandmarkTeleportTransforms.Count; i++)
        {
            CurrentLandmark_IndexList.Add(i);
            PointToLandmark_IndexList.Add(i);

            Random_IndexList.Add(i);
        }

        //Create a dictionary of reference & pointing landmark pairs
        TaskOrder = OrderLandmarkDisplay(CurrentLandmark_IndexList, PointToLandmark_IndexList);

        //Shuffle the Random_IndexList
        IListExtensions.Shuffle<int>(Random_IndexList);

        //Inactivate all landmark images
        for (int i = 0; i < LandmarkImages.Count; ++i)
        {
            LandmarkImages[i].SetActive(false);
        }
    }


    Dictionary<int, List<int>> OrderLandmarkDisplay(List<int> CurrentLandmark_IndexList, List<int> PointToLandmark_IndexList)
    {
        Dictionary<int, List<int>> OrderDict = new Dictionary<int, List<int>>();

        for (int i = 0; i < CurrentLandmark_IndexList.Count; i++)
        {
            List<int> listValue = new List<int>();
            for (int j = 0; j < PointToLandmark_IndexList.Count; j++)
            {
                if (i != j)
                {
                    listValue.Add(j);
                }
            }
            OrderDict.Add(i, listValue);
        }
        return OrderDict;
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public void CallPointingTask()
    {
        if (referencelandmark_index < ReferenceLandmarkTeleportTransforms.Count)
        {
            taskFinish = false;

            var key = TaskOrder.Keys.ElementAt(Random_IndexList[referencelandmark_index]); // Key: save the landmarks to teleport
            referenceLandmark_name = ReferenceLandmarkTeleportTransforms[key].gameObject.name;
            TeleportToLandmark(ReferenceLandmarkTeleportTransforms[key]);

            var values = TaskOrder.Values.ElementAt(Random_IndexList[referencelandmark_index]); // Values: save the landmark to display

            // if not go though all displaylandmarks, show the next displaylandmarks
            if (displaylandmark_index < values.Count)
            {
                m_TextComponent.text = "Point to...";

                var displayLandmarkID_inValues = values[displaylandmark_index];
                displayLandmark_name = LandmarkImages[displayLandmarkID_inValues].name;
                ChooseDisplayLandmark(previous_displaylandmark_index, displayLandmarkID_inValues);

                for (int ind = 0; ind < ReferenceLandmarkTransforms.Count; ind++)
                    {
                        //set the names under displayLandmark_name correspond to the names under ReferenceLandmarkTransforms
                        if (displayLandmark_name == ReferenceLandmarkTransforms[ind].gameObject.name)
                        {
                            Debug.Log("ReferenceLandmarkTransforms[ind]" + ReferenceLandmarkTransforms[ind].position);
                            LandmarkImage_position = ReferenceLandmarkTransforms[ind];
                            break;
                        }
                    }
                Debug.Log("mainCamera"+ mainCamera.position); 
                Debug.Log("LandmarkImage_position"+LandmarkImage_position.position);
                GroundTruthDirection = CalculateGroundTruthDirection(mainCamera, LandmarkImage_position);

                //Update index for next trial
                previous_displaylandmark_index = displayLandmarkID_inValues;
                displaylandmark_index++;
            }
            // if already gone though all displaylandmarks, prepare to go to the next reference landmark
            else
            {
                    referencelandmark_index++;
                    displaylandmark_index = 0;

                    //Inactive the previous landmark
                    if (LandmarkImages[previous_displaylandmark_index].activeSelf == true)
                        LandmarkImages[previous_displaylandmark_index].SetActive(false);

                    //Ask participant to press trigger to continue
                    Debug.Log("Go to Next landmark");
                    m_TextComponent.text = "Please press \"TriggerButton\" to continue.";
            }
        }
        else
        {
                Debug.Log("Task Finish!!!!!!!!!!!!!");
                taskFinish = true;
                m_TextComponent.text = "You have finished the task! \n Let's go to the next task. ";
        }
    }

    public void TeleportToLandmark(Transform TargetLandmarkPosition)
    {
        Vector3 userOffsetFromTrackingOrigin = xrRig.position - mainCamera.position;
        userOffsetFromTrackingOrigin.y = 0.5f; // Add 0.5m in y (consistent height with exploration task)

        Vector3 targetPosition = new Vector3(TargetLandmarkPosition.position.x, TargetLandmarkPosition.position.y, TargetLandmarkPosition.position.z);
        xrRig.position = targetPosition + userOffsetFromTrackingOrigin;
    }

    public void ChooseDisplayLandmark(int previous_index, int current_index)
    {
        if(LandmarkImages[previous_index].activeSelf == true)
            LandmarkImages[previous_index].SetActive(false);
        if (LandmarkImages[previous_index].activeSelf == false)
            LandmarkImages[current_index].SetActive(true);
    }

    public Vector3 CalculateGroundTruthDirection(Transform pos1, Transform pos2)
    {
        Vector3 GroundTruthDirection = (pos2.position - pos1.position).normalized;

        return GroundTruthDirection;
    }

}
