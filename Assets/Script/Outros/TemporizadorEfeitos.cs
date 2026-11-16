using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class TemporizadorEfeitos : MonoBehaviour
{
    [Header("Sprites recortados de Tempo_Perfume")]
    [SerializeField] Sprite _caixa;
    [SerializeField] Sprite _relogio;
    [SerializeField] Sprite _fundoBarra;

    [Header("Visual")]
    [Tooltip("Use a mesma fonte TMP da caixa de objetivos. Vazio usa a fonte padrao do TMP.")]
    [SerializeField] TMP_FontAsset _fonte;
    [SerializeField] Color _corTexto = new Color32(244, 232, 198, 255);
    [SerializeField] Color _corBarra = new Color32(59, 185, 97, 255);
    [SerializeField, Min(122f)] float _largura = 366f;
    [Tooltip("Distancia a partir do canto superior esquerdo do Canvas. Y negativo desce.")]
    [SerializeField] Vector2 _posicao = new Vector2(24f, -100f);
    [SerializeField, Min(0f)] float _espacoEntreCaixas = 8f;

    const float LarguraModelo = 366f;
    const float AlturaModelo = 114f;

    class Indicador
    {
        public MonoBehaviour origem;
        public RectTransform raiz;
        public Image icone;
        public TMP_Text nome;
        public TMP_Text segundos;
        public RectTransform preenchimento;
        public float inicio;
        public float duracao;
        public int ultimoSegundo = -1;
    }

    readonly List<Indicador> _indicadores = new List<Indicador>();
    RectTransform _raiz;
    bool _avisouConfiguracao;

    void Awake()
    {
        _raiz = GetComponent<RectTransform>();
        Organizar();
    }

    public void Mostrar(MonoBehaviour origem, string nome, Sprite icone,
                        float inicio, float duracao)
    {
        if (!isActiveAndEnabled || origem == null || !origem.isActiveAndEnabled) return;
        if (duracao <= 0f || float.IsNaN(duracao) || float.IsInfinity(duracao) ||
            float.IsNaN(inicio) || float.IsInfinity(inicio) || Time.time - inicio >= duracao)
        {
            Ocultar(origem);
            return;
        }
        if (!ConfiguracaoValida()) return;
        if (_raiz == null) _raiz = GetComponent<RectTransform>();

        Indicador indicador = _indicadores.Find(i => i.origem == origem);
        if (indicador == null)
        {
            indicador = CriarIndicador(origem);
            _indicadores.Add(indicador);
        }

        indicador.inicio = inicio;
        indicador.duracao = duracao;
        if (indicador.nome.text != nome) indicador.nome.text = nome;
        indicador.icone.sprite = icone;
        indicador.icone.enabled = icone != null;
        AtualizarIndicador(indicador);
        Organizar();
    }

    public void Ocultar(MonoBehaviour origem)
    {
        for (int i = _indicadores.Count - 1; i >= 0; i--)
            if (_indicadores[i].origem == origem) Remover(i);
    }

    void LateUpdate()
    {
        for (int i = _indicadores.Count - 1; i >= 0; i--)
        {
            Indicador indicador = _indicadores[i];
            if (indicador.origem == null || !indicador.origem.isActiveAndEnabled ||
                Time.time - indicador.inicio >= indicador.duracao)
                Remover(i);
            else
                AtualizarIndicador(indicador);
        }
    }

    void AtualizarIndicador(Indicador indicador)
    {
        float restante = Mathf.Clamp(indicador.duracao - (Time.time - indicador.inicio),
                                    0f, indicador.duracao);
        int segundos = Mathf.CeilToInt(restante);
        if (segundos != indicador.ultimoSegundo)
        {
            indicador.segundos.text = segundos + "s";
            indicador.ultimoSegundo = segundos;
        }
        indicador.preenchimento.anchorMax = new Vector2(restante / indicador.duracao, 1f);
    }

    void Remover(int indice)
    {
        GameObject caixa = _indicadores[indice].raiz.gameObject;
        caixa.SetActive(false);
        Destroy(caixa);
        _indicadores.RemoveAt(indice);
        Organizar();
    }

    void Organizar()
    {
        if (_raiz == null) return;
        _raiz.anchorMin = _raiz.anchorMax = new Vector2(0f, 1f);
        _raiz.pivot = new Vector2(0f, 1f);
        _raiz.anchoredPosition = _posicao;
        float escala = _largura / LarguraModelo;
        float altura = AlturaModelo * escala;
        _raiz.sizeDelta = new Vector2(_largura,
            _indicadores.Count * altura + Mathf.Max(0, _indicadores.Count - 1) * _espacoEntreCaixas);
        for (int i = 0; i < _indicadores.Count; i++)
        {
            _indicadores[i].raiz.localScale = Vector3.one * escala;
            _indicadores[i].raiz.anchoredPosition = new Vector2(0f, -i * (altura + _espacoEntreCaixas));
        }
    }

    bool ConfiguracaoValida()
    {
        bool valida = _caixa != null && _relogio != null && _fundoBarra != null &&
                      GetComponentInParent<Canvas>() != null &&
                      (_fonte != null || TMP_Settings.defaultFontAsset != null);
        if (!valida && !_avisouConfiguracao)
        {
            Debug.LogWarning("TemporizadorEfeitos: coloque dentro de um Canvas e preencha Caixa, Relogio, Fundo Barra e uma fonte TMP.", this);
            _avisouConfiguracao = true;
        }
        if (valida) _avisouConfiguracao = false;
        return valida;
    }

    Indicador CriarIndicador(MonoBehaviour origem)
    {
        Image fundo = CriarImagem("Duracao_" + origem.GetType().Name, transform,
                                  _caixa, 0f, 0f, LarguraModelo, AlturaModelo);
        RectTransform raiz = fundo.rectTransform;
        Image icone = CriarImagem("IconeItem", raiz, null, 18f, 21f, 63f, 72f);
        icone.preserveAspect = true;
        TMP_Text nome = CriarTexto("NomeEfeito", raiz, 96f, 15f, 162f, 36f);
        nome.enableAutoSizing = true;
        nome.fontSizeMin = 16f;
        nome.fontSizeMax = 24f;
        Image relogio = CriarImagem("Relogio", raiz, _relogio, 264f, 21f, 24f, 30f);
        relogio.preserveAspect = true;
        TMP_Text segundos = CriarTexto("Segundos", raiz, 294f, 15f, 57f, 36f);
        segundos.enableAutoSizing = true;
        segundos.fontSizeMin = 16f;
        segundos.fontSizeMax = 24f;

        Image barra = CriarImagem("FundoBarra", raiz, _fundoBarra, 96f, 72f, 249f, 24f);
        RectTransform area = CriarRetangulo("AreaPreenchimento", barra.transform, 6f, 6f, 237f, 12f);
        Image preenchimento = CriarImagem("Preenchimento", area, null, 0f, 0f, 237f, 12f);
        preenchimento.color = _corBarra;
        RectTransform rt = preenchimento.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        return new Indicador
        {
            origem = origem,
            raiz = raiz,
            icone = icone,
            nome = nome,
            segundos = segundos,
            preenchimento = rt
        };
    }

    RectTransform CriarRetangulo(string nome, Transform pai, float x, float y, float largura, float altura)
    {
        RectTransform rt = new GameObject(nome, typeof(RectTransform)).GetComponent<RectTransform>();
        rt.SetParent(pai, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(largura, altura);
        return rt;
    }

    Image CriarImagem(string nome, Transform pai, Sprite sprite, float x, float y, float largura, float altura)
    {
        Image imagem = CriarRetangulo(nome, pai, x, y, largura, altura).gameObject.AddComponent<Image>();
        imagem.sprite = sprite;
        imagem.type = Image.Type.Simple;
        imagem.raycastTarget = false;
        return imagem;
    }

    TMP_Text CriarTexto(string nome, Transform pai, float x, float y, float largura, float altura)
    {
        TextMeshProUGUI texto = CriarRetangulo(nome, pai, x, y, largura, altura)
            .gameObject.AddComponent<TextMeshProUGUI>();
        texto.font = _fonte != null ? _fonte : TMP_Settings.defaultFontAsset;
        texto.fontSize = 24f;
        texto.color = _corTexto;
        texto.alignment = TextAlignmentOptions.MidlineLeft;
#pragma warning disable CS0618
        texto.enableWordWrapping = false;
#pragma warning restore CS0618
        texto.overflowMode = TextOverflowModes.Ellipsis;
        texto.raycastTarget = false;
        return texto;
    }
}
