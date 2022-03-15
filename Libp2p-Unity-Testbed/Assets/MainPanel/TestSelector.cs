using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSelector : MonoBehaviour
{

    public GameObject theCamera;

    public GameObject startupPanel;

    public float smoothTime = 0.25F;

    protected Vector3 targetCamPos;
    private Vector3 curVel = Vector3.zero;

    public bool IsMoving {get; private set;}= false;

    // Start is called before the first frame update
    void Start()
    {
        SetCameraToPanel(startupPanel);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoving)
        {
            theCamera.transform.position = Vector3.SmoothDamp(theCamera.transform.position , targetCamPos, ref curVel, smoothTime);
            if (curVel.magnitude < 3f)
            {
                theCamera.transform.position = targetCamPos;
                IsMoving = false;
            }
        }
    }
    public void SetCameraToPanel(GameObject targetPanel)
    {
        targetCamPos = targetPanel.transform.Find("CameraPos").position;
        theCamera.transform.position = targetCamPos;
        IsMoving = false;
    }

    public void MoveCameraToPanel(GameObject targetPanel)
    {
        targetCamPos = targetPanel.transform.Find("CameraPos").position;
        IsMoving = true;
    }

}
