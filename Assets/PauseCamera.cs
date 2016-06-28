using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew
{
    public class PauseCamera : MonoBehaviour
    {

   
        [SerializeField]
        float cameraSpeed = 5f;
        [SerializeField]
        float rotateSpeed = 45f;
        [SerializeField]
        GameObject cameraControl;

        private Vector3 OriginalPos;
        private Quaternion OriginalRot;

        //[SerializeField]
        //Vector2 _rotateRange;


        // Use this for initialization
        void Start()
        {
            OriginalPos = cameraControl.transform.position;
            OriginalRot = cameraControl.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
                cameraControl.transform.Translate(cameraSpeed * Input.GetAxis("joystick 1 analog 0") * Time.unscaledDeltaTime,
                                    cameraSpeed * -Input.GetAxis("joystick 1 analog 1") * Time.unscaledDeltaTime,
                                    0f,
                                    Space.World);

                cameraControl.transform.Rotate(rotateSpeed * Input.GetAxis("joystick 1 analog 4") * Time.unscaledDeltaTime,
                                    rotateSpeed * Input.GetAxis("joystick 1 analog 3") * Time.unscaledDeltaTime,
                                    0f);

               //force Z axis rotation to stay 0
               // cameraControl.transform.rotation.eulerAngles = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
                if (transform.up != Vector3.up)
                {
                    transform.LookAt(transform.position + transform.forward, Vector3.up);
                }

                //stop moving it after release controls
                //cameraControl.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //cameraControl.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //lel doesn't work, no camera rigidbody

            

            //reset position and rotation
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                cameraControl.transform.Translate(OriginalPos);
                cameraControl.transform.rotation = OriginalRot;
            }
        }
    }
}