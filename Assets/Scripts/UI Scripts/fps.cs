using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fps : MonoBehaviour
{
    private Text fpsDisplay;
    private float refreshRate;

    private float timer;

    private bool visible;

    void Awake()
    {
        fpsDisplay = GetComponent<Text>();
        refreshRate = 0.5f;
        visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.unscaledTime > timer) {
            fpsDisplay.text = (1f / Time.unscaledDeltaTime).ToString("#.00") + " FPS";
            timer = Time.unscaledTime + refreshRate;
        }

    }

    public void ToggleVisible() {
        if (visible) {
            GetComponent<Text>().enabled = false;
            visible = false;
        } else {
            GetComponent<Text>().enabled = true;
            visible = true;
        }
    }
}
