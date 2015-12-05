using UnityEngine;
using System.Collections;

public class RandomSparks : MonoBehaviour
{

    public ParticleSystem Sparks;
    public Light FlashLight;
    public float MinFlashTime = 3f;
    public float MaxFlashTime = 8f;
    public float MinFlashLength = 0.25f;
    public float MaxFlashLength = 1f;
    public float LightChangeTime = 0.1f;
    public float MinBurstSize = 5f;
    public float MaxButstSize = 100;

    private float NextFlashStart;
    private float FlashTimer;
    private bool Flashing;
    private float LightChangeTimer;

    // Use this for initialization
    void Start()
    {
        Sparks.enableEmission = false;
        FlashLight.intensity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        NextFlashStart -= Time.deltaTime;
        if (NextFlashStart < 0)
        {
            if (!Flashing)
            {
                FlashTimer = Random.value * (MaxFlashLength - MinFlashLength) + MinFlashLength;
                FlashLight.intensity = 8f;
                LightChangeTimer = LightChangeTime;
                Flashing = true;
                Sparks.enableEmission = true;
            }
            LightChangeTimer -= Time.deltaTime;
            if (LightChangeTimer < 0f)
            {
                LightChangeTimer = Random.value * LightChangeTime;
                FlashLight.intensity = Random.value * 5f + 3f;
                int sparkCount = (int)(Random.value * (MaxButstSize - MinBurstSize) + MinBurstSize);
                Sparks.Emit(sparkCount);
            }
            FlashTimer -= Time.deltaTime;
            if (FlashTimer < 0)
            {
                Flashing = false;
                NextFlashStart = Random.value * (MaxFlashTime - MinFlashTime) + MinFlashTime;
                Sparks.enableEmission = false;
                FlashLight.intensity = 0f;
            }
        }
    }
}
