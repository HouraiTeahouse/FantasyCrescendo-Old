using UnityEngine;
using System.Collections;
using HouraiTeahouse.HouraiInput;
using HouraiTeahouse.SmashBrew;

public class PauseControl : MonoBehaviour {

    [SerializeField]
    float cameraSpeed = 5f;
    [SerializeField]
    float rotateSpeed = 45f;
    [SerializeField]
    GameObject cameraControl;
    [SerializeField]
    GameObject pausePivot;
    [SerializeField]
    GameObject matchTargetScript;

    private Vector3 OriginalPos;
    private Vector3 PausePos;
    private Quaternion OriginalRot;

    // Use this for initialization
    void Start () {
        OriginalPos = cameraControl.transform.position;
        OriginalRot = cameraControl.transform.rotation;
        PausePos = pausePivot.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        PausePos = pausePivot.transform.position;
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            //matchTargetScript.SetActive(false);
            cameraControl.GetComponent<CameraController>().enabled = false;
            lerpToCharacter();
        }

    }

    void lerpToCharacter() {
        cameraControl.transform.position = Vector3.Lerp(cameraControl.transform.position,
            PausePos,
            Time.unscaledDeltaTime);
    }
}
