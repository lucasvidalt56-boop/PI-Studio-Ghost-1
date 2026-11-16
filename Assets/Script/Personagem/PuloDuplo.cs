using UnityEngine;

[DisallowMultipleComponent]
public class PuloDuplo : MonoBehaviour
{
    [SerializeField] Inventario _inventario;
    [SerializeField] Item _pocao;
    [SerializeField, Min(0.1f)] float _duracao = 30f;

    [Header("Indicador de duracao")]
    [Tooltip("Opcional. Se vazio, procura o TemporizadorEfeitos ativo na cena.")]
    [SerializeField] TemporizadorEfeitos _temporizador;
    [SerializeField] string _nomeEfeito = "Salto +";
    [Tooltip("Se vazio, usa o Sprite do Item configurado em Pocao.")]
    [SerializeField] Sprite _iconeEfeito;

    float _inicio = float.NegativeInfinity;
    float _proximaBusca;

    public float TempoRestante => isActiveAndEnabled
        ? Mathf.Max(0f, Mathf.Max(0.1f, _duracao) - (Time.time - _inicio)) : 0f;
    public bool Ativa => TempoRestante > 0f;

    public bool Consumir()
    {
        if (!isActiveAndEnabled || Time.timeScale == 0f) return false;
        if (_inventario == null || _pocao == null) return false;
        if (_inventario.ItemSelecionado != _pocao) return false;
        if (_inventario.QuantidadeSelecionada <= 0) return false;

        _inventario.SetItem(_inventario.SlotSelecionado, _pocao,
                            _inventario.QuantidadeSelecionada - 1);

        _inicio = Time.time;
        _proximaBusca = 0f;
        AtualizarIndicador();
        return true;
    }

    void LateUpdate()
    {
        if (Ativa) AtualizarIndicador();
        else if (_temporizador != null) _temporizador.Ocultar(this);
    }

    void AtualizarIndicador()
    {
        if (_temporizador == null && Time.unscaledTime >= _proximaBusca)
        {
            _proximaBusca = Time.unscaledTime + 0.5f;
            _temporizador = FindFirstObjectByType<TemporizadorEfeitos>();
        }
        if (_temporizador == null) return;
        Sprite icone = _iconeEfeito != null ? _iconeEfeito : (_pocao != null ? _pocao.Sprite : null);
        _temporizador.Mostrar(this, _nomeEfeito, icone, _inicio, Mathf.Max(0.1f, _duracao));
    }

    void OnDisable()
    {
        _inicio = float.NegativeInfinity;
        if (_temporizador != null) _temporizador.Ocultar(this);
    }
}
