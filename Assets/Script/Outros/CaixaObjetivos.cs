using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class CaixaObjetivos : MonoBehaviour
{
    [Serializable]
    public class Objetivo
    {
        [Tooltip("ID unico nesta caixa. Exemplo: pegar_picareta")]
        public string id;
        [TextArea(2, 4)] public string texto;
        [Tooltip("Executado quando o certo DESTA linha terminar de animar.")]
        public UnityEvent aoConcluir = new UnityEvent();
    }

    [Header("Modelos das linhas")]
    [Tooltip("Arraste DescricaoObjetivo. O script copia sua fonte e cor para cada linha. Nao use o titulo.")]
    [SerializeField] private TMP_Text _texto;
    [Tooltip("Arraste IconeConclusao. O script cria uma copia para cada objetivo.")]
    [SerializeField] private Image _icone;
    [SerializeField] private Sprite _spritePendente;
    [SerializeField] private Sprite _spriteConcluido;

    [Header("Objetivos simultaneos")]
    [SerializeField] private List<Objetivo> _objetivos = new List<Objetivo>();
    [Tooltip("Mostra TODAS as linhas da lista ao iniciar.")]
    [SerializeField] private bool _mostrarAoIniciar = true;
    [SerializeField] private bool _ocultarAoConcluirTodos = true;
    [Tooltip("Executado apos todos os certos e a saida da caixa (se ativada).")]
    [SerializeField] private UnityEvent _aoConcluirTodos = new UnityEvent();

    [Header("Organizacao automatica das linhas")]
    [SerializeField, Min(0f)] private float _espacoCabecalho = 50f;
    [SerializeField, Min(24f)] private float _alturaLinha = 40f;
    [SerializeField, Min(0f)] private float _espacoEntreLinhas = 6f;
    [SerializeField, Min(0f)] private float _margemEsquerda = 24f;
    [SerializeField, Min(0f)] private float _margemDireita = 16f;
    [SerializeField, Min(0f)] private float _margemInferior = 16f;
    [SerializeField, Min(8f)] private float _tamanhoMarcacao = 24f;
    [SerializeField, Min(0f)] private float _espacoTextoMarcacao = 12f;
    [Tooltip("Aumenta a altura para caberem as linhas. Configure o fundo como Image Sliced para preservar a borda.")]
    [SerializeField] private bool _ajustarAltura = true;

    [Header("Animacao (segundos)")]
    [SerializeField, Min(0.01f)] private float _duracaoEntrada = 0.35f;
    [SerializeField, Min(0.01f)] private float _duracaoCerto = 0.3f;
    [SerializeField, Min(0f)] private float _tempoExibindoCerto = 1f;
    [SerializeField, Min(0.01f)] private float _duracaoSaida = 0.45f;
    [SerializeField, Range(1f, 1.5f)] private float _escalaDestaque = 1.2f;
    [SerializeField, Min(0f)] private float _margemForaDaTela = 20f;

    private class Linha
    {
        public Objetivo objetivo;
        public Image icone;
        public bool concluida;
        public bool notificada;
        public float tempoAnimacao;
    }

    private enum Estado { Oculta, Entrando, Visivel, Esperando, Saindo, Finalizada }
    private readonly List<Linha> _linhas = new List<Linha>();
    private readonly Dictionary<string, Linha> _porId = new Dictionary<string, Linha>();
    private readonly Vector3[] _cantos = new Vector3[4];
    private RectTransform _retangulo;
    private CanvasGroup _grupo;
    private Vector2 _posicaoVisivel;
    private Estado _estado;
    private float _tempo;
    private bool _pronta;
    private bool _eventoTotalDisparado;

    // Compatibilidade: retorna a primeira linha ainda pendente.
    public string ObjetivoAtual
    {
        get
        {
            foreach (Linha linha in _linhas)
                if (!linha.concluida) return linha.objetivo.id;
            return "";
        }
    }

    private void Awake()
    {
        _retangulo = GetComponent<RectTransform>();
        _grupo = GetComponent<CanvasGroup>();
        _posicaoVisivel = _retangulo.anchoredPosition;
        _grupo.alpha = 0f;
        _grupo.interactable = false;
        _grupo.blocksRaycasts = false;

        if (_texto == null || _icone == null || _spritePendente == null || _spriteConcluido == null)
        {
            Debug.LogError("CaixaObjetivos: preencha Texto, Icone, Sprite Pendente e Sprite Concluido.", this);
            return;
        }
        if (_texto.transform.parent != transform || _icone.transform.parent != transform)
        {
            Debug.LogError("CaixaObjetivos: DescricaoObjetivo e IconeConclusao devem ser filhos diretos desta caixa.", this);
            return;
        }
        if (_objetivos.Count == 0) return;
        HashSet<string> ids = new HashSet<string>();
        foreach (Objetivo objetivo in _objetivos)
        {
            if (objetivo == null || string.IsNullOrWhiteSpace(objetivo.id) || !ids.Add(objetivo.id))
            {
                Debug.LogError("CaixaObjetivos: cada objetivo precisa de um ID preenchido e diferente.", this);
                return;
            }
        }

        // Os modelos ficam ocultos durante o jogo. O titulo permanece como voce configurou.
        _texto.gameObject.SetActive(false);
        _icone.gameObject.SetActive(false);
        float alturaLinha = Mathf.Max(24f, Mathf.Max(_alturaLinha, _tamanhoMarcacao));
        if (_ajustarAltura)
        {
            float altura = _espacoCabecalho + _margemInferior +
                _objetivos.Count * alturaLinha + (_objetivos.Count - 1) * _espacoEntreLinhas;
            _retangulo.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, altura);
        }
        for (int i = 0; i < _objetivos.Count; i++)
            CriarLinha(_objetivos[i], i, alturaLinha);
        _pronta = true;
    }

    private void CriarLinha(Objetivo objetivo, int indice, float altura)
    {
        float topo = _espacoCabecalho + indice * (altura + _espacoEntreLinhas);
        TMP_Text texto = Instantiate(_texto, transform);
        texto.name = "Texto_" + objetivo.id;
        texto.gameObject.SetActive(true);
        texto.enabled = true;
        texto.text = objetivo.texto;
        texto.raycastTarget = false;
        texto.alignment = TextAlignmentOptions.Left;
        RectTransform rt = texto.rectTransform;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        float direita = _margemDireita + _tamanhoMarcacao + _espacoTextoMarcacao;
        rt.sizeDelta = new Vector2(-(_margemEsquerda + direita), altura);
        rt.anchoredPosition3D = new Vector3(_margemEsquerda, -topo, 0f);

        Image icone = Instantiate(_icone, transform);
        icone.name = "Marcacao_" + objetivo.id;
        icone.gameObject.SetActive(true);
        icone.enabled = true;
        icone.sprite = _spritePendente;
        icone.color = Color.white;
        icone.type = Image.Type.Simple;
        icone.preserveAspect = true;
        icone.raycastTarget = false;
        rt = icone.rectTransform;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = Vector2.one * _tamanhoMarcacao;
        rt.anchoredPosition3D = new Vector3(-_margemDireita - _tamanhoMarcacao * 0.5f,
            -topo - altura * 0.5f, 0f);

        Linha linha = new Linha { objetivo = objetivo, icone = icone };
        _linhas.Add(linha);
        _porId.Add(objetivo.id, linha);
    }

    private void Start()
    {
        if (_mostrarAoIniciar) MostrarTodos();
    }

    public void MostrarTodos()
    {
        if (!_pronta || !isActiveAndEnabled || _estado != Estado.Oculta) return;
        Canvas.ForceUpdateCanvases();
        _retangulo.anchoredPosition = PosicaoForaDaTela();
        _grupo.alpha = 1f;
        MudarEstado(Estado.Entrando);
    }

    // Mantem os metodos usados pelo GatilhoObjetivo. Mostrar revela a lista inteira.
    public void MostrarObjetivo(string id) { TentarMostrarObjetivo(id); }
    public void ConcluirObjetivo(string id) { TentarConcluirObjetivo(id); }
    public void ConcluirAtual() { TentarConcluirObjetivo(ObjetivoAtual); }

    public bool TentarMostrarObjetivo(string id)
    {
        if (!_pronta || !isActiveAndEnabled || string.IsNullOrWhiteSpace(id) ||
            !_porId.ContainsKey(id) || _estado == Estado.Saindo || _estado == Estado.Finalizada)
            return false;
        MostrarTodos();
        return true;
    }

    public bool TentarConcluirObjetivo(string id)
    {
        if (!_pronta || !isActiveAndEnabled || string.IsNullOrWhiteSpace(id) ||
            !_porId.TryGetValue(id, out Linha linha) || linha.concluida) return false;
        MostrarTodos();
        linha.concluida = true;
        linha.tempoAnimacao = 0f;
        // Se a caixa ainda estiver entrando, espera ficar visivel para animar.
        return true;
    }

    private void Update()
    {
        if (!_pronta || _estado == Estado.Oculta || _estado == Estado.Finalizada) return;
        _tempo += Time.unscaledDeltaTime;
        if (_estado == Estado.Entrando)
        {
            float t = Mathf.Clamp01(_tempo / Mathf.Max(0.01f, _duracaoEntrada));
            _retangulo.anchoredPosition = Vector2.Lerp(PosicaoForaDaTela(), _posicaoVisivel,
                Mathf.SmoothStep(0f, 1f, t));
            if (t >= 1f) MudarEstado(Estado.Visivel);
            return;
        }

        // Cada linha tem seu proprio progresso: podem ser concluidas em qualquer ordem.
        foreach (Linha linha in _linhas)
        {
            if (!linha.concluida || linha.notificada) continue;
            linha.icone.sprite = _spriteConcluido;
            linha.tempoAnimacao += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(linha.tempoAnimacao / Mathf.Max(0.01f, _duracaoCerto));
            float escala = t < 0.65f
                ? Mathf.Lerp(0.15f, _escalaDestaque, Mathf.SmoothStep(0f, 1f, t / 0.65f))
                : Mathf.Lerp(_escalaDestaque, 1f, Mathf.SmoothStep(0f, 1f, (t - 0.65f) / 0.35f));
            linha.icone.rectTransform.localScale = Vector3.one * escala;
            if (t >= 1f)
            {
                linha.notificada = true;
                linha.objetivo.aoConcluir?.Invoke();
            }
        }

        if (_estado == Estado.Visivel)
        {
            foreach (Linha linha in _linhas)
                if (!linha.notificada) return;
            MudarEstado(Estado.Esperando);
        }
        else if (_estado == Estado.Esperando && _tempo >= _tempoExibindoCerto)
        {
            if (_ocultarAoConcluirTodos) MudarEstado(Estado.Saindo);
            else FinalizarLista();
        }
        else if (_estado == Estado.Saindo)
        {
            float t = Mathf.Clamp01(_tempo / Mathf.Max(0.01f, _duracaoSaida));
            _retangulo.anchoredPosition = Vector2.Lerp(_posicaoVisivel, PosicaoForaDaTela(),
                Mathf.SmoothStep(0f, 1f, t));
            if (t >= 1f)
            {
                _grupo.alpha = 0f;
                _retangulo.anchoredPosition = _posicaoVisivel;
                FinalizarLista();
            }
        }
    }

    private void FinalizarLista()
    {
        MudarEstado(Estado.Finalizada);
        if (_eventoTotalDisparado) return;
        _eventoTotalDisparado = true;
        _aoConcluirTodos?.Invoke();
    }

    private void MudarEstado(Estado estado)
    {
        _estado = estado;
        _tempo = 0f;
    }

    private Vector2 PosicaoForaDaTela()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform pai = _retangulo.parent as RectTransform;
        if (canvas == null || pai == null)
            return _posicaoVisivel + Vector2.right * (_retangulo.rect.width + 1000f);
        RectTransform tela = canvas.rootCanvas.transform as RectTransform;
        tela.GetWorldCorners(_cantos);
        float direita = Mathf.Max(pai.InverseTransformPoint(_cantos[2]).x,
                                  pai.InverseTransformPoint(_cantos[3]).x);
        float ancora = _retangulo.localPosition.x - _retangulo.anchoredPosition.x;
        float esquerda = ancora + _posicaoVisivel.x + _retangulo.rect.xMin * _retangulo.localScale.x;
        return _posicaoVisivel + Vector2.right * (Mathf.Max(0f, direita - esquerda) + _margemForaDaTela);
    }

    [ContextMenu("Testar/Mostrar lista (em Play)")]
    private void TestarMostrar() { if (Application.isPlaying) MostrarTodos(); }

    [ContextMenu("Testar/Concluir primeira pendente (em Play)")]
    private void TestarConcluir() { if (Application.isPlaying) ConcluirAtual(); }
}
