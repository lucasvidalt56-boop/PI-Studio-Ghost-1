using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Coletavel : MonoBehaviour
{
    [SerializeField] Item _item;

    [SerializeField] string _idSave;

    [Header("Objetivo opcional")]
    [Tooltip("Arraste o GatilhoObjetivo desta coleta, configurado como Apos Interacao Concluida. Se vazio, procura no mesmo objeto.")]
    [SerializeField] GatilhoObjetivo _objetivoColeta;

    bool _coletado;

    void Awake()
    {
        AtualizarSprite();
    }

    void Start()
    {
        if (!string.IsNullOrWhiteSpace(_idSave) &&
            SistemaSave.Coletados.Contains(_idSave))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void OnValidate()
    {
        AtualizarSprite();
    }

    void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void AtualizarSprite()
    {
        GetComponent<SpriteRenderer>().sprite =
            _item != null ? _item.Sprite : null;
    }

    public void Coletar(Inventario inventario)
    {
        if (_coletado || !isActiveAndEnabled || inventario == null) return;

        if (string.IsNullOrWhiteSpace(_idSave))
        {
            Debug.LogError("Preencha o Id Save deste coletavel.", this);
            return;
        }

        if (!inventario.AdicionarItem(_item)) return;

        _coletado = true;

        if (!SistemaSave.Coletados.Contains(_idSave))
            SistemaSave.Coletados.Add(_idSave);

        // So confirma depois da coleta bem-sucedida, antes de desativar o item.
        if (_objetivoColeta == null)
            _objetivoColeta = GetComponent<GatilhoObjetivo>();

        if (_objetivoColeta != null)
            _objetivoColeta.ConfirmarInteracao();

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
