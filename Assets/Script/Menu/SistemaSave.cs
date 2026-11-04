using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SlotSave
{
    public string item;
    public int quantidade;
}

[Serializable]
public class DadosSave
{
    public int versao = 1;
    public string cena;
    public Vector3 posicao;
    public int slotSelecionado;
    public SlotSave[] slots;
    public List<string> coletados = new List<string>();
}

public static class SistemaSave
{
    public static DadosSave Carregamento;

    public static List<string> Coletados = new List<string>();

    static string Caminho =>
        Path.Combine(Application.persistentDataPath, "save.json");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        NovoJogo();
    }

    public static void NovoJogo()
    {
        Carregamento = null;
        Coletados = new List<string>();
    }

    public static void PrepararCarregamento(DadosSave dados)
    {
        Carregamento = dados;
        Coletados = new List<string>(dados.coletados);
    }

    public static DadosSave Ler()
    {
        if (!File.Exists(Caminho)) return null;

        try
        {
            DadosSave dados =
                JsonUtility.FromJson<DadosSave>(File.ReadAllText(Caminho));

            if (dados == null || dados.versao != 1 ||
                string.IsNullOrEmpty(dados.cena) ||
                dados.slots == null || dados.slots.Length == 0 ||
                dados.coletados == null ||
                dados.slotSelecionado < 0 ||
                dados.slotSelecionado >= dados.slots.Length)
                return null;

            if (!Application.CanStreamedLevelBeLoaded(dados.cena))
                return null;

            if (!PosicaoValida(dados.posicao))
                return null;

            foreach (SlotSave slot in dados.slots)
            {
                if (slot == null || slot.quantidade < 0)
                    return null;

                if (!string.IsNullOrEmpty(slot.item) && slot.quantidade == 0)
                    return null;
            }

            return dados;
        }
        catch (Exception erro)
        {
            Debug.LogWarning("Não foi possível ler o save: " + erro.Message);
            return null;
        }
    }

    public static bool Salvar(DadosSave dados)
    {
        string temporario = Caminho + ".tmp";

        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);

            File.WriteAllText(temporario, JsonUtility.ToJson(dados, true));

            if (File.Exists(Caminho))
                File.Replace(temporario, Caminho, null);
            else
                File.Move(temporario, Caminho);

            Debug.Log("Jogo salvo automaticamente.");
            return true;
        }
        catch (Exception erro)
        {
            Debug.LogError("Não foi possível salvar: " + erro.Message);
            return false;
        }
    }

    static bool PosicaoValida(Vector3 posicao)
    {
        return !float.IsNaN(posicao.x) && !float.IsInfinity(posicao.x) &&
               !float.IsNaN(posicao.y) && !float.IsInfinity(posicao.y) &&
               !float.IsNaN(posicao.z) && !float.IsInfinity(posicao.z);
    }
}