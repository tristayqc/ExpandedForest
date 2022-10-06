using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;
using VarjoExample;

/// <summary>
///  Attach this script to the controller
/// </summary>
public class ControllerTriggerFunc : MonoBehaviour
{
    public TeleportInPointingTask task;
    public ControllerRay controllerRay;
    [SerializeField] DataManager dataManager;
    //[SerializeField] AudioSource audioData;

    bool buttonDown;
    Controller controller;
    int TriggerNum = 0; 
    int TrialNum = 0;
    int landmarkNum = 6;

    string Path;
    string FileName;

    Vector3 estDirection, groundtruthDirection;
    string referencelandmark, displaylandmark;

    void Start()
    {
        controller = GetComponent<Controller>();
        //Record Data -- First Line
        Path = dataManager.folderPath;
        FileName = dataManager.fileName;
        RecordData.SaveData(Path, FileName,
              "Time" + ";"
            + "TrialNum" + ";"
            + "TriggerNum" + ","
            + "Referencelandmark" + ";"
            + "Displaylandmark" + ", "
            + "GroundTruthDirection"+ "; "
            + "EstDirection" + "; "
            + "Angle" + '\n');
        //Record the task starting time
        RecordData.SaveData(Path, FileName,
              DateTime.Now.ToString() + ";"
                        + ";"
                        + ";"
                        + ";"
                        + ";"
                        + ";"
                        + ";"
                        + '\n');
    }

    void Update()
    {
        
        if (controller.triggerButton)
        {
            if (!buttonDown)
            {
                // Button is pressed
                buttonDown = true;
                // Button is released
                Debug.Log("Button is pressed");
            }
            else
            {
                // Button is held down
            }
        }
        else if (!controller.triggerButton && buttonDown)
        {
            // Button is released
            Debug.Log("tRIGGER");
            //audioData.Play(0);

            /// 
            /// Order cannot be changed here 
            /// 
            if (!task.taskFinish)
            {
                //Log estimated direction for the previous trial
                if (TriggerNum >= 1 && TriggerNum % landmarkNum != 0)
                {
                    controllerRay.SetEstimatedDirection();
                    estDirection = controllerRay.EstimatedDirection;
                    estDirection = Vector3.ProjectOnPlane(estDirection, new Vector3(0, 1, 0));//xz plane

                    Debug.Log("groundtruthDirectionRead: " + groundtruthDirection.ToString("f3"));
                    Debug.Log("estimatedDirectionRead: " + estDirection.ToString("f3"));

                    //Calculate Angle between "groundtruthDirection" and "estDirection"
                    float angle = Vector3.Angle(estDirection, groundtruthDirection);
                    Debug.Log(angle.ToString("f3"));

                    //Record Data
                    RecordData.SaveData(Path, FileName,
                          DateTime.Now.ToString() + ";"
                        + TrialNum.ToString() + ";"
                        + TriggerNum.ToString() + ";"
                        + referencelandmark + ";"
                        + displaylandmark + ";"
                        + groundtruthDirection.ToString("f3") + ";"
                        + estDirection.ToString("f3") + ";"
                        + angle.ToString("f3") + '\n');

                    //Trial Num
                    TrialNum++;
                }

                //Trigger Num
                Debug.Log("TriggerNum: " + TriggerNum);

                //Get landmark name & ground truth direction
                task.CallPointingTask();

                referencelandmark = task.referenceLandmark_name;
                displaylandmark = task.displayLandmark_name;

                groundtruthDirection = task.GroundTruthDirection;
                groundtruthDirection = Vector3.ProjectOnPlane(groundtruthDirection, new Vector3(0, 1, 0));//xz plane

                //Update Trigger Num
                TriggerNum++;
            }

            else
            {
                //StartCoroutine(Quit.WaitQuit(6));
            }
            buttonDown = false;
        }
    }
}
