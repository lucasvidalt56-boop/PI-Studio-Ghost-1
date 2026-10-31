using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    [SerializeField, Min(1)] int _quantidadeSlots = 6;

    [SerializeField] SlotInventario _slotPrefab;

    [SerializeField] Sprite _layout;

    [SerializeField] Sprite _layoutSelecionado;

    int _slotSelecionado;

    SlotInventario[] _slots;

    Item[] _itens;

    int[] _quantidades;

    public int SlotSelecionado => _slotSelecionado;

    public Item ItemSelecionado => _itens == null ? null : _itens[_slotSelecionado];

    public int QuantidadeSelecionada => _quantidades == null ? 0 : _quantidades[_slotSelecionado];

    void Start()
    {
        _slots = new SlotInventario[_quantidadeSlots];
        _itens = new Item[_quantidadeSlots];
        _quantidades = new int[_quantidadeSlots];

        for (int i = 0; i < _slots.Length; i++)
        {
            int indice = i;
            _slots[i] = Instantiate(_slotPrefab, transform);
            _slots[i].GetComponent<Button>().onClick.AddListener(() => SetSelecionado(indice));
        }

        SetLayout(_layout, _layoutSelecionado);
        SetSelecionado(0);
    }

    public void SetLayout(Sprite layout, Sprite selecionado)
    {
        _layout = layout;
        _layoutSelecionado = selecionado;

        if (_slots == null) return;

        foreach (SlotInventario slot in _slots)
        {
            slot.SetLayout(_layout, _layoutSelecionado);
        }
    }

    public void SetSelecionado(int indice)
    {
        if (_slots == null || indice < 0 || indice >= _slots.Length) return;

        _slots[_slotSelecionado].SetSelecionado(false);
        _slotSelecionado = indice;
        _slots[_slotSelecionado].SetSelecionado(true);
    }

    public bool AdicionarItem(Item item)
    {
        if (_itens == null || item == null || item.Sprite == null) return false;

        for (int i = 0; i < _itens.Length; i++)
        {
            if (_itens[i] != item) continue;
            if (_quantidades[i] >= item.QuantidadeMaxima) continue;

            SetItem(i, item, _quantidades[i] + 1);
            return true;
        }

        for (int i = 0; i < _itens.Length; i++)
        {
            if (_itens[i] != null) continue;

            SetItem(i, item);
            return true;
        }

        return false;
    }

    public void SetItem(int indice, Item item, int quantidade = 1)
    {
        if (_itens == null || indice < 0 || indice >= _itens.Length) return;

        if (quantidade <= 0) item = null;

        _itens[indice] = item;
        _quantidades[indice] = item != null ? Mathf.Clamp(quantidade, 1, item.QuantidadeMaxima) : 0;
        _slots[indice].SetItem(item != null ? item.Sprite : null, _quantidades[indice]);
    }

    void Update()
    {
        if (_slots == null) return;

        if (Keyboard.current != null)
        {
            for (int i = 0; i < Mathf.Min(_slots.Length, 9); i++)
            {
                if (Keyboard.current[(Key)((int)Key.Digit1 + i)].wasPressedThisFrame)
                {
                    SetSelecionado(i);
                }
            }
        }

        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            int direcao = scroll > 0 ? -1 : 1;
            SetSelecionado((_slotSelecionado + direcao + _slots.Length) % _slots.Length);
        }
    }
}
