using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
[RequireComponent(typeof(Light2D))]
public class LuzOlhosMatinta : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField, Min(0f)] float _intensidadeMinima = 0.3f;
    [SerializeField, Min(0f)] float _intensidadeMaxima = 0.6f;
    [SerializeField, Min(0.1f)] float _duracaoCiclo = 3f;

    Light2D _luz;
    Vector3 _posicaoInicial;
    float _anguloInicial;
    float _fase;

    void Awake()
    {
        _luz = GetComponent<Light2D>();
        _posicaoInicial = transform.localPosition;
        _anguloInicial = transform.localEulerAngles.z;
    }

    void LateUpdate()
    {
        _fase = Mathf.Repeat(
            _fase + Time.deltaTime / Mathf.Max(0.1f, _duracaoCiclo), 1f
        );

        float pulso = (1f - Mathf.Cos(_fase * Mathf.PI * 2f)) * 0.5f;
        _luz.intensity = Mathf.Lerp(_intensidadeMinima, _intensidadeMaxima, pulso);

        if (_sprite == null) return;

        Vector3 posicao = _posicaoInicial;
        if (_sprite.flipX) posicao.x = -posicao.x;
        transform.localPosition = posicao;
        transform.localRotation = Quaternion.Euler(
            0f, 0f, _sprite.flipX ? -_anguloInicial : _anguloInicial
        );
    }
}