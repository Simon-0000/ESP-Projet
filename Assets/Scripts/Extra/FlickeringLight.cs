using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sergio abreo alvarez

public class FlickeringLight : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float flickerSpeedMin = 1, flickerSpeedMax = 1, flickerDelayMin = 1, flickerDelayMax = 1, flickerDurationMin = 1, flickerDurationMax = 1, minIntensity = 0;



    private float flickerSpeed, flickerDelay, flickerDuration, maxIntensity, lastFlickerTime = 0;

    Light objLight;
    bool isCurrentlyFlickering = false;
    void Awake()
    {
        Debug.Assert(GetComponent<Light>() != null);
        objLight = GetComponent<Light>();
        maxIntensity = objLight.intensity;

        flickerDelay = Random.Range(flickerDelayMin, flickerDelayMax);
    }

    // Update is called once per frame
    void Update()
    {
        if(isCurrentlyFlickering == false && lastFlickerTime + flickerDelay < Time.time)
        {
            isCurrentlyFlickering = true;
            flickerDelay = Random.Range(flickerDelayMin, flickerDelayMax);
            flickerDuration = Random.Range(flickerDurationMin, flickerDurationMax);
            flickerSpeed = Random.Range(flickerSpeedMin, flickerSpeedMax);
        }
        nextFlickerFrame();

    }
    void nextFlickerFrame()
    {
        if(isCurrentlyFlickering)
        {
            //L'idée d'utiliser un Perlin Noise pour osciller l'intensité de manière fluide vient de ChatGPT
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            objLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
            flickerDuration -= Time.deltaTime;

            if (flickerDuration < 0)
            {
                lastFlickerTime = Time.time;
                isCurrentlyFlickering = false;
            }
        }
        else { isCurrentlyFlickering = false; }
    }
}
