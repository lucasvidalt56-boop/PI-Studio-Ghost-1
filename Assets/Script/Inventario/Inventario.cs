using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    [SerializeField, Min(1)] int _quantidadeSlots = 6;

    [SerializeField] SlotInventario _slotPrefab;

    [SerializeField] Sprite _layout;

    [SerializeField] Sprite _layoutSelecionado;

    [SerializeField] Item[] _catalogoItens;

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
        _slots[indice].SetItem(item != null ? item.IconeInventario : null, _quantidades[indice]);
    }

    public SlotSave[] ObterSave()
    {
        if (_itens == null) return null;

        SlotSave[] dados = new SlotSave[_itens.Length];

        for (int i = 0; i < _itens.Length; i++)
        {
            if (_itens[i] != null && string.IsNullOrWhiteSpace(_itens[i].Nome))
            {
                Debug.LogError("Preencha o nome de todos os itens.");
                return null;
            }

            dados[i] = new SlotSave
            {
                item = _itens[i] != null ? _itens[i].Nome : "",
                quantidade = _quantidades[i]
            };
        }

        return dados;
    }

    public bool CarregarSave(SlotSave[] dados, int selecionado)
    {
        if (_itens == null || dados == null ||
            dados.Length != _itens.Length || _catalogoItens == null)
        {
            Debug.LogError("Confira a quantidade de slots e o catalogo de itens.");
            return false;
        }

        Item[] encontrados = new Item[dados.Length];

        for (int i = 0; i < dados.Length; i++)
        {
            if (string.IsNullOrEmpty(dados[i].item)) continue;

            int correspondencias = 0;

            foreach (Item item in _catalogoItens)
            {
                if (item == null || item.Nome != dados[i].item) continue;

                encontrados[i] = item;
                correspondencias++;
            }

            if (correspondencias != 1 ||
                dados[i].quantidade > encontrados[i].QuantidadeMaxima)
            {
                Debug.LogError("Confira o item do save: " + dados[i].item);
                return false;
            }
        }

        for (int i = 0; i < dados.Length; i++)
        {
            SetItem(i, encontrados[i], dados[i].quantidade);
        }

        SetSelecionado(selecionado);
        return true;
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
