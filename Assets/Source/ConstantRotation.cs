using UnityEngine;

namespace HouraiTeahouse {

    public class ConstantRotation : MonoBehaviour {

        [SerializeField]
        Vector3 rotationPerSecond;

        public Vector3 RotationPerSecond {
            get { return rotationPerSecond; }
            set { rotationPerSecond = value; }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() { 
            transform.Rotate(RotationPerSecond * Time.deltaTime); 
        }

    }

}