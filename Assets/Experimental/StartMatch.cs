using HouraiTeahouse;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMatch : MonoBehaviour {
    [SerializeField] private InputTarget button;

    [SerializeField, Scene] private string scene;

    // Update is called once per frame
    private void Update() {
        foreach (var device in InputManager.Devices)
            if (device.GetControl(button).WasPressed)
                SceneManager.LoadScene(scene);
    }
}