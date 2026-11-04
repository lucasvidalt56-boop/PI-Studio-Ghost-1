using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Coletavel : MonoBehaviour
{
    [SerializeField] Item _item;

    [SerializeField] string _idSave;

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
        if (!isActiveAndEnabled || inventario == null) return;

        if (string.IsNullOrWhiteSpace(_idSave))
        {
            Debug.LogError("Preencha o Id Save deste coletável.", this);
            return;
        }

        if (!inventario.AdicionarItem(_item)) return;

        if (!SistemaSave.Coletados.Contains(_idSave))
            SistemaSave.Coletados.Add(_idSave);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}