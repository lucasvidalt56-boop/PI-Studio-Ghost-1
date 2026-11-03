using UnityEngine;

[CreateAssetMenu(fileName = "NovoItem", menuName = "Inventario/Item")]
public class Item : ScriptableObject
{
    [SerializeField] string _nome;

    [SerializeField] Sprite _sprite;

    [SerializeField] Sprite _iconeInventario;

    [SerializeField, Min(1)] int _quantidadeMaxima = 1;

    public string Nome => _nome;

    public Sprite Sprite => _sprite;

    public Sprite IconeInventario => _iconeInventario != null ? _iconeInventario : _sprite;

    public int QuantidadeMaxima => Mathf.Max(1, _quantidadeMaxima);
}