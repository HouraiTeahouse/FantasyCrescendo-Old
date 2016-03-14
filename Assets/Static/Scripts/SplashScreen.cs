﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {
    [SerializeField] private AnimationCurve alphaOverTime;

    [SerializeField] private GameObject[] disableWhileLoading;

    [SerializeField] private Graphic[] splashGraphics;

    [SerializeField] private string targetSceneName;

    // Use this for initialization
    private void Start() {
        StartCoroutine(DisplaySplashScreen());
    }

    private IEnumerator DisplaySplashScreen() {
        foreach (var target in disableWhileLoading)
            target.SetActive(false);
        var logoDisplayDuration = alphaOverTime.keys[alphaOverTime.length - 1].time;
        foreach (var graphic in splashGraphics)
            graphic.enabled = false;
        foreach (var graphic in splashGraphics) {
            if (graphic == null)
                continue;
            graphic.enabled = true;
            float t = 0;
            var baseColor = graphic.color;
            var targetColor = baseColor;
            baseColor.a = 0f;
            while (t < logoDisplayDuration) {
                graphic.color = Color.Lerp(baseColor, targetColor, alphaOverTime.Evaluate(t));

                //Wait one frame
                yield return null;
                t += Time.deltaTime;
            }
            graphic.enabled = false;
            graphic.color = targetColor;
        }
        var operation = SceneManager.LoadSceneAsync(targetSceneName);
        if (operation != null && !operation.isDone) {
            foreach (var target in disableWhileLoading)
                target.SetActive(true);
            while (!operation.isDone)
                yield return null;
        }
        Destroy(gameObject);
    }
}