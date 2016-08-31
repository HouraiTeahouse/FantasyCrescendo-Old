using System.Collections;
using UnityEngine;

public class clearStencilCamera : MonoBehaviour {

 

	// Use this for initialization
	void Start () {

        GetComponent<Camera>().clearStencilAfterLightingPass = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
