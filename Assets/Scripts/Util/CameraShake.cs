using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    public CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin vCamNoise;

    private float elapsedTime = 0f;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than one instance of CameraShake present");
        }
        instance = this;
    }

    private void Start()
    {
        vCamNoise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera()
    {
        StartCoroutine(CameraShakeCoroutine(.1f, 1.5f, 1.5f));
    }

    public IEnumerator CameraShakeCoroutine(float shakeDuration, float shakeAmplitude, float shakeFrequency)
    {

        vCamNoise.m_AmplitudeGain = shakeAmplitude;
        vCamNoise.m_FrequencyGain = shakeFrequency;

        while (shakeDuration >= 0)
        {
            shakeDuration -= Time.deltaTime;
            yield return null;
        }

        vCamNoise.m_AmplitudeGain = 0f;
        vCamNoise.m_FrequencyGain = 0f;
    }

}
