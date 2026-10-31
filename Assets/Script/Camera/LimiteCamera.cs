using UnityEngine;

[DefaultExecutionOrder(10000)]
[RequireComponent(typeof(Camera))]
public class LimiteCameraHorizontal : MonoBehaviour
{
    public float bordaEsquerda;
    public float bordaDireita;

    private Camera cameraAtual;

    private void Awake()
    {
        cameraAtual = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        float metadeDaLargura = 5.625f*cameraAtual.aspect;

        float minimo = bordaEsquerda + metadeDaLargura;
        float maximo = bordaDireita - metadeDaLargura;

        Vector3 posicao = transform.position;

        posicao.x = minimo <= maximo
            ? Mathf.Clamp(posicao.x, minimo, maximo)
            : (bordaEsquerda + bordaDireita) / 2f;

        transform.position = posicao;
    }
}