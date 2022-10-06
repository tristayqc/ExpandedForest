using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableScript : MonoBehaviour
{
    public GameObject landmark;
    private Outline outlineScript;

    // Start is called before the first frame update
    void Start()
    {
        outlineScript = landmark.GetComponent<Outline>();
        outlineScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            outlineScript.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            outlineScript.enabled = false;
        }
    }
}
