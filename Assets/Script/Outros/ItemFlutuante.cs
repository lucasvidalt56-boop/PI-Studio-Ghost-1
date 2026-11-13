using UnityEngine;

[DisallowMultipleComponent]
public class ItemFlutuante : MonoBehaviour
{
    [SerializeField, Min(0f)] float _altura = 0.1f;
    [SerializeField, Min(0.1f)] float _duracaoCiclo = 3f;

    Vector3 _posicaoInicial;
    float _fase;

    void OnEnable()
    {
        _posicaoInicial = transform.localPosition;
        _fase = 0f;
    }

    void Update()
    {
        _fase = Mathf.Repeat(
            _fase + Time.deltaTime / Mathf.Max(0.1f, _duracaoCiclo), 1f
        );

        float deslocamento = Mathf.Sin(_fase * Mathf.PI * 2f) * _altura;
        transform.localPosition = _posicaoInicial + Vector3.up * deslocamento;
    }

    void OnDisable()
    {
        transform.localPosition = _posicaoInicial;
    }
}