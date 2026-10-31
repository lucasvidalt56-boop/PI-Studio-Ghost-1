using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Coletavel : MonoBehaviour
{
    [SerializeField] Item _item;

    void Awake()
    {
        AtualizarSprite();
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
        GetComponent<SpriteRenderer>().sprite = _item != null ? _item.Sprite : null;
    }

    public void Coletar(Inventario inventario)
    {
        if (!isActiveAndEnabled || inventario == null) return;
        if (!inventario.AdicionarItem(_item)) return;

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
