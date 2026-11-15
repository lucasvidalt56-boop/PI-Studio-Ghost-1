using UnityEngine;

[DisallowMultipleComponent]
public class GatilhoObjetivo : MonoBehaviour
{
    public enum Acao { Mostrar, Concluir }
    // Os dois primeiros valores preservam os componentes que voce ja configurou.
    public enum ModoAtivacao
    {
        AoEntrarNaArea = 0,
        AposInteracaoConcluida = 1,
        AoColetarItem = 2,
        AoAbrirBau = 3,
        AoEntrarNaPorta = 4,
        AoDerrotarInimigo = 5,
        AoQuebrarPedra = 6
    }

    [Header("Quando ativar")]
    [Tooltip("Escolha a acao real deste objeto. O modo de area continua funcionando ao passar pelo trigger.")]
    [SerializeField] private ModoAtivacao _modoAtivacao = ModoAtivacao.AoEntrarNaArea;

    [Header("Objetivo")]
    [Tooltip("No prefab, pode ficar vazio: procura a unica caixa ativa da cena. Com varias caixas, arraste a correta.")]
    [SerializeField] private CaixaObjetivos _caixa;
    [Tooltip("Concluir soma 1 na meta. Mostrar apenas exibe a caixa.")]
    [SerializeField] private Acao _acao = Acao.Concluir;
    [SerializeField] private string _idObjetivo = "entrar_basilica";

    [Header("Jogador (somente no modo de area)")]
    [SerializeField] private string _tagJogador = "Player";

    private bool _executado;
    private float _proximaTentativa;

    // Usado pela porta para dar tempo de mostrar a marcacao antes de trocar a cena.
    public bool ObjetivoAcionado => _executado;

    private void Awake()
    {
        if (_modoAtivacao != ModoAtivacao.AoEntrarNaArea) return;
        BoxCollider2D area = GetComponent<BoxCollider2D>();
        if (area != null) area.isTrigger = true;
        else Debug.LogWarning("GatilhoObjetivo: para entrar numa area, use um objeto separado com BoxCollider2D e Is Trigger.", this);
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

    // Podem ser chamados por codigo ou selecionados em um UnityEvent.
    public void ConfirmarInteracao() { Confirmar(ModoAtivacao.AposInteracaoConcluida); }
    public void ConfirmarColeta() { Confirmar(ModoAtivacao.AoColetarItem); }
    public void ConfirmarAberturaBau() { Confirmar(ModoAtivacao.AoAbrirBau); }
    public void ConfirmarEntradaPorta() { Confirmar(ModoAtivacao.AoEntrarNaPorta); }
    public void ConfirmarDerrotaInimigo() { Confirmar(ModoAtivacao.AoDerrotarInimigo); }
    public void ConfirmarQuebraPedra() { Confirmar(ModoAtivacao.AoQuebrarPedra); }

    private void Confirmar(ModoAtivacao acaoRealizada)
    {
        if (_executado || !isActiveAndEnabled) return;
        if (_modoAtivacao != acaoRealizada &&
            _modoAtivacao != ModoAtivacao.AposInteracaoConcluida) return;
        if (!ExecutarObjetivo())
            Debug.LogWarning("GatilhoObjetivo: nao registrou a acao. Confira a Caixa ativa e o Id Objetivo: " + _idObjetivo, this);
    }

    private bool ExecutarObjetivo()
    {
        if (_executado || !isActiveAndEnabled) return false;
        if (_caixa == null)
        {
            CaixaObjetivos[] caixas = FindObjectsByType<CaixaObjetivos>(FindObjectsSortMode.None);
            foreach (CaixaObjetivos caixa in caixas)
            {
                if (!caixa.isActiveAndEnabled) continue;
                if (_caixa != null)
                {
                    _caixa = null;
                    return false; // Nunca escolha uma caixa arbitraria.
                }
                _caixa = caixa;
            }
        }
        if (_caixa == null) return false;

        _executado = _acao == Acao.Mostrar
            ? _caixa.TentarMostrarObjetivo(_idObjetivo)
            : _caixa.TentarAdicionarProgresso(_idObjetivo);
        return _executado;
    }
}
