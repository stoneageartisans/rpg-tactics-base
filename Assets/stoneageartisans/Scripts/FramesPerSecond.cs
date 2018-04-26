using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    float deltaTime = 0.0f;

    int currentFps;
    int previousFps;

    Text fpsText;

    void Start()
    {
        currentFps = 0;
        previousFps = currentFps;
        fpsText = GetComponent<Text>();
    }
	
	void Update()
    {
        deltaTime += ((Time.unscaledDeltaTime - deltaTime) * 0.1f);

        currentFps = ((int) Mathf.Round(1.0f / deltaTime));

        if(previousFps != currentFps)
        {
            fpsText.text = string.Format("{0}", currentFps);
        }
	}
}
