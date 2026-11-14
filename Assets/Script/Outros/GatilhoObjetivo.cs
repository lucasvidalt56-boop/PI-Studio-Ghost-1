using UnityEngine;

[DisallowMultipleComponent]
public class GatilhoObjetivo : MonoBehaviour
{
    public enum Acao { Mostrar, Concluir }
    public enum ModoAtivacao { AoEntrarNaArea = 0, AposInteracaoConcluida = 1 }

    [Header("Quando ativar")]
    [Tooltip("Ao Entrar Na Area: basta passar pelo trigger. Apos Interacao Concluida: o script da acao precisa chamar ConfirmarInteracao() depois do sucesso.")]
    [SerializeField] private ModoAtivacao _modoAtivacao = ModoAtivacao.AoEntrarNaArea;

    [Header("Objetivo")]
    [Tooltip("Arraste a caixa aqui. Se vazio, procura uma caixa ativa na cena.")]
    [SerializeField] private CaixaObjetivos _caixa;
    [SerializeField] private Acao _acao = Acao.Concluir;
    [SerializeField] private string _idObjetivo = "entrar_basilica";

    [Header("Jogador (somente no modo de area)")]
    [Tooltip("Tag do objeto que possui o Rigidbody2D do jogador.")]
    [SerializeField] private string _tagJogador = "Player";

    private bool _executado;
    private float _proximaTentativa;

    private void Awake()
    {
        // Interacoes nao precisam de collider e nao alteram o collider do item/porta/pedra.
        // No modo de area, use este componente em um objeto separado com BoxCollider2D.
        if (_modoAtivacao != ModoAtivacao.AoEntrarNaArea) return;
        BoxCollider2D area = GetComponent<BoxCollider2D>();
        if (area != null)
            area.isTrigger = true;
        else
            Debug.LogWarning("GatilhoObjetivo: no modo Ao Entrar Na Area, adicione um BoxCollider2D com Is Trigger em um objeto separado.", this);
    }

    private void OnTriggerEnter2D(Collider2D outro) { TentarAtivarPorArea(outro); }
    private void OnTriggerStay2D(Collider2D outro) { TentarAtivarPorArea(outro); }

    private void TentarAtivarPorArea(Collider2D outro)
    {
        if (_modoAtivacao != ModoAtivacao.AoEntrarNaArea || _executado ||
            !isActiveAndEnabled || Time.unscaledTime < _proximaTentativa) return;
        Rigidbody2D corpo = outro.attachedRigidbody;
        if (corpo == null || !corpo.CompareTag(_tagJogador)) return;
        _proximaTentativa = Time.unscaledTime + 0.2f;
        ExecutarObjetivo();
    }

    // Pode ser selecionado em UnityEvents pelo Inspector.
    // Chame APOS a coleta/abertura/quebra ter sucesso, antes de desativar ou destruir
    // o objeto. Nao chame apenas porque E/X foi pressionado.
    public void ConfirmarInteracao()
    {
        if (_modoAtivacao != ModoAtivacao.AposInteracaoConcluida) return;
        if (!ExecutarObjetivo() && !_executado && isActiveAndEnabled)
            Debug.LogWarning("GatilhoObjetivo: a interacao ocorreu, mas o objetivo nao foi acionado. Confira a Caixa, o ID e se essa missao esta ativa.", this);
    }

    private bool ExecutarObjetivo()
    {
        if (_executado || !isActiveAndEnabled) return false;
        if (_caixa == null) _caixa = FindFirstObjectByType<CaixaObjetivos>();
        if (_caixa == null) return false;

        _executado = _acao == Acao.Mostrar
            ? _caixa.TentarMostrarObjetivo(_idObjetivo)
            : _caixa.TentarConcluirObjetivo(_idObjetivo);
        return _executado;
    }
}
