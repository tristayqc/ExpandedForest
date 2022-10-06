using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCanvas : MonoBehaviour
{
    public Canvas defaultDest;
    public Canvas backDest;
    public bool back;
    // Start is called before the first frame update
    void Start()
    {
        defaultDest.enabled = false;
        backDest.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (back)
            {
                backDest.enabled = true;
            } else
            {
                defaultDest.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (back)
            {
                backDest.enabled = false;
            }
            else
            {
                defaultDest.enabled = false;
            }
        }
    }
}
