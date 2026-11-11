using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(VidaPlayer), typeof(Rigidbody2D))]
public class CombatePlayer : MonoBehaviour
{
    [Header("Dano das armas")]
    [SerializeField, Min(1)] int _danoEspada = 20;
    [SerializeField, Min(1)] int _danoPicareta = 20;

    [Header("Area de ataque")]
    [SerializeField] Vector2 _centroAtaque = new Vector2(0.8f, 0f);
    [SerializeField] Vector2 _tamanhoAtaque = new Vector2(1.2f, 1.2f);
    [SerializeField] LayerMask _camadaInimigos;
    [SerializeField] LayerMask _camadaMinerios;
    [SerializeField] LayerMask _camadaObstaculos;
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] bool _spriteOriginalOlhaDireita = true;

    [Header("Morte")]
    [SerializeField] UnityEvent _aoMorrer = new UnityEvent();

    VidaPlayer _vida;
    Rigidbody2D _rig2d;
    bool _morto;
    bool _golpeDisponivel;
    readonly List<Collider2D> _resultados = new List<Collider2D>();
    readonly HashSet<AtaqueInimigo> _atingidas = new HashSet<AtaqueInimigo>();
    readonly HashSet<Minerio> _mineriosAtingidos = new HashSet<Minerio>();

    public bool EstaMorto => _morto || (_vida != null && _vida.VidaAtual <= 0);

    void Awake()
    {
        _vida = GetComponent<VidaPlayer>();
        _rig2d = GetComponent<Rigidbody2D>();
        if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (!_morto && _vida.VidaAtual <= 0) Morrer();
    }

    public void ReceberDano(int quantidade)
    {
        if (EstaMorto || quantidade <= 0) return;
        _vida.ReceberDano(quantidade);
        if (_vida.VidaAtual <= 0) Morrer();
    }

    // Eventos no inicio de cada animacao.
    public void IniciarGolpeEspada()
    {
        _golpeDisponivel = !EstaMorto;
    }

    public void IniciarGolpePicareta()
    {
        _golpeDisponivel = !EstaMorto;
    }

    // Eventos no quadro de impacto.
    public void AcertarEspada()
    {
        AcertarGolpe(_danoEspada, false);
    }

    public void AcertarPicareta()
    {
        AcertarGolpe(_danoPicareta, true);
    }

    void AcertarGolpe(int dano, bool minerar)
    {
        if (EstaMorto || !_golpeDisponivel) return;
        _golpeDisponivel = false;

        ContactFilter2D filtro = new ContactFilter2D();
        filtro.SetLayerMask(_camadaInimigos.value | (minerar ? _camadaMinerios.value : 0));
        filtro.useTriggers = false;
        _atingidas.Clear();
        _mineriosAtingidos.Clear();

        Physics2D.OverlapBox(CentroAtaque(), TamanhoAtaque(),
            transform.eulerAngles.z, filtro, _resultados);

        foreach (Collider2D resultado in _resultados)
        {
            if (Physics2D.Linecast(transform.position, resultado.bounds.center,
                _camadaObstaculos).collider != null) continue;

            if (minerar)
            {
                Minerio minerio = resultado.GetComponentInParent<Minerio>();
                if (minerio != null && minerio.isActiveAndEnabled && _mineriosAtingidos.Add(minerio))
                    minerio.ReceberGolpe();
            }

            AtaqueInimigo inimigo = resultado.GetComponentInParent<AtaqueInimigo>();
            if (inimigo != null && inimigo.isActiveAndEnabled && _atingidas.Add(inimigo))
                inimigo.ReceberDano(dano);
        }
    }

    Vector2 CentroAtaque()
    {
        SpriteRenderer desenho = _sprite != null ? _sprite : GetComponent<SpriteRenderer>();
        float sentido = _spriteOriginalOlhaDireita ? 1f : -1f;
        if (desenho != null && desenho.flipX) sentido *= -1f;
        return transform.TransformPoint(new Vector3(_centroAtaque.x * sentido, _centroAtaque.y, 0f));
    }

    Vector2 TamanhoAtaque()
    {
        Vector3 escala = transform.lossyScale;
        return new Vector2(Mathf.Abs(_tamanhoAtaque.x * escala.x),
            Mathf.Abs(_tamanhoAtaque.y * escala.y));
    }

    void Morrer()
    {
        if (_morto) return;
        _morto = true;
        _golpeDisponivel = false;

        if (TryGetComponent(out PlayerInput entrada)) entrada.enabled = false;
        if (TryGetComponent(out Move movimento)) movimento.enabled = false;
        if (TryGetComponent(out ColetarPlayer coleta)) coleta.enabled = false;
        if (TryGetComponent(out Animator animator)) animator.enabled = false;

        _rig2d.linearVelocity = Vector2.zero;
        _rig2d.angularVelocity = 0f;
        _rig2d.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        _aoMorrer.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        Matrix4x4 anterior = Gizmos.matrix;
        Vector2 centro = CentroAtaque();
        Gizmos.matrix = Matrix4x4.TRS(new Vector3(centro.x, centro.y, transform.position.z),
            Quaternion.Euler(0f, 0f, transform.eulerAngles.z), Vector3.one);
        Gizmos.color = Color.red;
        Vector2 tamanho = TamanhoAtaque();
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(tamanho.x, tamanho.y, 0f));
        Gizmos.matrix = anterior;
    }
}