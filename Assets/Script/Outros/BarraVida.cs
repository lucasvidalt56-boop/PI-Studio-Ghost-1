using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BarraVida : MonoBehaviour
{
    [SerializeField] Image _preenchimento;

    [SerializeField, Min(0.1f)] float _velocidade = 1f;

    [SerializeField] Color _verde =
        new Color32(100, 220, 100, 255);

    [SerializeField] Color _amarelo =
        new Color32(255, 205, 90, 255);

    [SerializeField] Color _vermelho =
        new Color32(240, 85, 85, 255);

    Slider _barra;
    bool _iniciou;

    void Awake()
    {
        _barra = GetComponent<Slider>();
    }

    void OnEnable()
    {
        _iniciou = false;
    }

    void LateUpdate()
    {
        if (_preenchimento == null) return;

        float vida = _barra.normalizedValue;

        if (!_iniciou)
        {
            _preenchimento.fillAmount = vida;
            _iniciou = true;
        }
        else
        {
            _preenchimento.fillAmount = Mathf.MoveTowards(
                _preenchimento.fillAmount,
                vida,
                _velocidade * Time.deltaTime
            );
        }

        _preenchimento.color = vida > 0.5f
            ? _verde
            : vida > 0.25f
                ? _amarelo
                : _vermelho;
    }
}