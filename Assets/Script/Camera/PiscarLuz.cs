using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PiscarLuz : MonoBehaviour
{
    public Light2D luz;
    public float intensidadeBase = 0.75f;
    public float variacaoIntensidade = 0.07f;

    public float raioBase = 2.8f;
    public float variacaoRaio = 0.12f;

    public float velocidade = 6f;

    void Update()
    {
        float ruido = Mathf.PerlinNoise(Time.time * velocidade, 0f);

        luz.intensity = intensidadeBase + (ruido - 0.5f) * 2f * variacaoIntensidade;
        luz.pointLightOuterRadius = raioBase + (ruido - 0.5f) * 2f * variacaoRaio;
    }
}
