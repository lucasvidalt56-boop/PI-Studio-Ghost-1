using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAutomatico : MonoBehaviour
{
    [SerializeField] Transform _personagem;

    [SerializeField] Inventario _inventario;

    [SerializeField, Min(1)] float _intervaloSave = 300f;

    IEnumerator Start()
    {
        // Aguarda o Start do inventário criar os slots.
        yield return null;

        if (_personagem == null || _inventario == null)
        {
            Debug.LogError("Preencha Personagem e Inventario no SaveAutomatico.");
            yield break;
        }

        DadosSave dados = SistemaSave.Carregamento;

        if (dados != null)
        {
            if (!_inventario.CarregarSave(dados.slots, dados.slotSelecionado))
            {
                Debug.LogError("Carregamento interrompido. O save não será sobrescrito.");
                yield break;
            }

            Rigidbody2D rig = _personagem.GetComponent<Rigidbody2D>();

            _personagem.position = dados.posicao;

            if (rig != null)
            {
                rig.position = new Vector2(dados.posicao.x, dados.posicao.y);
                rig.linearVelocity = Vector2.zero;
                rig.angularVelocity = 0;
            }

            SistemaSave.Carregamento = null;
        }

        while (true)
        {
            yield return new WaitForSeconds(_intervaloSave);
            Salvar();
        }
    }

    void Salvar()
    {
        SlotSave[] slots = _inventario.ObterSave();

        if (slots == null) return;

        DadosSave dados = new DadosSave
        {
            cena = SceneManager.GetActiveScene().path,
            posicao = _personagem.position,
            slotSelecionado = _inventario.SlotSelecionado,
            slots = slots,
            coletados = new List<string>(SistemaSave.Coletados)
        };

        SistemaSave.Salvar(dados);
    }
}