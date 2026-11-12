using UnityEngine;

public class PocaoPuloDuplo : MonoBehaviour
{
    [SerializeField] Inventario _inventario;
    [SerializeField] Item _pocao;
    [SerializeField, Min(0.1f)] float _duracao = 20f;

    float _inicio = float.NegativeInfinity;

    public bool Ativa => isActiveAndEnabled &&
                         Time.time - _inicio < _duracao;

    public bool Consumir()
    {
        if (!isActiveAndEnabled || Time.timeScale == 0f) return false;
        if (_inventario == null || _pocao == null) return false;
        if (_inventario.ItemSelecionado != _pocao) return false;
        if (_inventario.QuantidadeSelecionada <= 0) return false;

        _inventario.SetItem(
            _inventario.SlotSelecionado,
            _pocao,
            _inventario.QuantidadeSelecionada - 1
        );

        _inicio = Time.time;
        return true;
    }

    void OnDisable()
    {
        _inicio = float.NegativeInfinity;
    }
}