using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VarjoExample
{
    public class Locomotion : MonoBehaviour
    {
        Controller controller;
        public Transform xrRig;
        public Transform head;
        public Transform bodyTracker;
        [Header("Use controller.primaryButton to move")]
        public float moveSpeed;
        public Transform labStartPos;
        public float desiredHeight;
        private Vector3 curPos;
        [Header("Press T to teleport to this location")]
        public Transform startPos;
        private bool teleported;


        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<Controller>();
            curPos = labStartPos.position;
            curPos.y = Terrain.activeTerrain.SampleHeight(labStartPos.position);
            xrRig.position = curPos;
            teleported = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!teleported && Input.GetKeyDown(KeyCode.T))
            {
                Vector3 tempPos = startPos.position;
                tempPos.y = Terrain.activeTerrain.SampleHeight(startPos.position)+ desiredHeight;
                xrRig.position = tempPos;
                teleported = true;
            }

            if (teleported && controller.primaryButton)
            {
                // Head-based steering
                //xrRig.transform.Translate(VectorYToZero(head.forward) * moveSpeed * Time.deltaTime, Space.World);

                // Body-based steering (Body rotation is tracked by a Vive Tracker)
                //Debug.Log(bodyTracker.forward);
                xrRig.transform.Translate(ProjectToXZPlane(bodyTracker.forward) * moveSpeed * Time.deltaTime, Space.World);
                curPos = xrRig.position;
                curPos.y = Terrain.activeTerrain.SampleHeight(xrRig.position) + desiredHeight;
                xrRig.position = curPos;
            }

        }

        Vector3 ProjectToXZPlane(Vector3 v)
        {
            return new Vector3(v.x, 0.0f, v.z).normalized;
            // for the specific projection to x-z plane, simply setting y to 0 works.
        }
    }
}
