using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowArrows : MonoBehaviour
{
    public GameObject cue;
    //public GameObject cueNS;
    //public bool salient;
    public bool back;

    // Start is called before the first frame update
    void Start()
    {
        cue.SetActive(false);
        //cueNS.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (back && CompareTag("reversedTrigger"))
            {
                cue.SetActive(true);
                /*if (salient)
                {
                    cue.SetActive(true);
                }
                else
                {
                    cueNS.SetActive(true);
                }*/
            }
            else if (!back && CompareTag("defaultTrigger"))
            {
                cue.SetActive(true);
                /*if (salient)
                {
                    cue.SetActive(true);
                }
                else
                {
                    cueNS.SetActive(true);
                }*/
            }
            else
            {
                cue.SetActive(false);
                //cueNS.SetActive(false);
            }
        }
    }
}
