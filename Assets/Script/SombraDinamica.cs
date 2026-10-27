using UnityEngine;

public class SombraDinamica : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform personagem;
    [SerializeField] private LayerMask camadaChao;

    [Header("Tamanho")]
    [SerializeField] private float largura = 1f;
    [SerializeField] private float altura = 1f;
    [SerializeField] private float distanciaMaxima = 5f;

    [Header("Encolhimento")]
    [SerializeField, Range(0f, 1f)]
    private float shrinkFactor = 0.5f;

    [Header("Cores")]
    [SerializeField] private Color darkestShade = Color.black;
    [SerializeField] private Color lightestShade = Color.gray;

    [Header("Transparencia")]
    [SerializeField, Range(0f, 1f)]
    private float opacidadeChao = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float opacidadeAr = 0.12f;

    [Header("Terreno")]
    [SerializeField] private bool adaptarRampas = true;
    [SerializeField] private bool adaptarBordas = true;

    private SpriteRenderer spriteSombra;
    private const int quantidadeRaios = 9;
    private readonly RaycastHit2D[] contatos =
        new RaycastHit2D[quantidadeRaios];

    void Awake()
    {
        spriteSombra = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        AtualizarSombra();
    }

    void AtualizarSombra()
    {
        if (personagem == null || spriteSombra == null ||
            spriteSombra.sprite == null) return;

        RaycastHit2D centro = DetectarChao(personagem.position);

        float distancia = centro.collider
            ? centro.distance : distanciaMaxima;

        float proporcao = Mathf.Clamp01(
            distancia / Mathf.Max(0.01f, distanciaMaxima));

        float larguraFinal = largura * (1f - shrinkFactor * proporcao);
        float larguraMundo = spriteSombra.sprite.bounds.size.x * larguraFinal;

        int melhor = -1;
        float menorDistancia = float.MaxValue;

        for (int i = 0; i < quantidadeRaios; i++)
        {
            float deslocamento = larguraMundo *
                (i / (float)(quantidadeRaios - 1) - 0.5f);

            contatos[i] = DetectarChao(
                personagem.position + Vector3.right * deslocamento);

            if (!contatos[i].collider) continue;

            float distanciaCentro = Mathf.Abs(i - 4);

            if (distanciaCentro < menorDistancia)
            {
                menorDistancia = distanciaCentro;
                melhor = i;
            }
        }

        spriteSombra.enabled = melhor != -1;
        if (melhor == -1) return;

        int esquerda = melhor;
        int direita = melhor;

        if (adaptarBordas)
        {
            while (esquerda > 0 && contatos[esquerda - 1].collider)
                esquerda--;

            while (direita < 8 && contatos[direita + 1].collider)
                direita++;
        }
        else
        {
            esquerda = 0;
            direita = 8;
        }

        float fracao = adaptarBordas
            ? (direita - esquerda + 1f) / quantidadeRaios : 1f;

        float deslocamentoCentro = adaptarBordas
            ? ((esquerda + direita) / 2f - 4f) / 8f * larguraMundo
            : 0f;

        RaycastHit2D hit = contatos[melhor];

        transform.position = new Vector3(
            personagem.position.x + deslocamentoCentro,
            hit.point.y + 0.02f,
            transform.position.z);

        transform.localScale = new Vector3(
            larguraFinal * fracao, altura, 1f);

        float angulo = adaptarRampas
            ? Mathf.Atan2(hit.normal.y, hit.normal.x) *
              Mathf.Rad2Deg - 90f
            : 0f;

        transform.rotation = Quaternion.Euler(0, 0, angulo);

        float distanciaCor = Mathf.Clamp01(
            hit.distance / Mathf.Max(0.01f, distanciaMaxima));

        Color cor = Color.Lerp(
            darkestShade, lightestShade, distanciaCor);

        cor.a = Mathf.Lerp(
            opacidadeChao, opacidadeAr, distanciaCor);

        spriteSombra.color = cor;
    }

    RaycastHit2D DetectarChao(Vector2 origem)
    {
        return Physics2D.Raycast(
            origem, Vector2.down, distanciaMaxima, camadaChao);
    }

    void OnValidate()
    {
        transform.localScale = new Vector3(largura, altura, 1f);

        if (personagem == null) return;

        RaycastHit2D hit = DetectarChao(personagem.position);

        if (hit.collider)
        {
            transform.position = new Vector3(
                personagem.position.x,
                hit.point.y + 0.02f,
                transform.position.z);
        }
    }
}

