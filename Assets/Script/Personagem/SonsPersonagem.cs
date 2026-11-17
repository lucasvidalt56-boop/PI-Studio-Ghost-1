using UnityEngine;

public class SonsPersonagem : MonoBehaviour
{
    [SerializeField] AudioSource _efeitos;

    [SerializeField] AudioSource _voz;

    [SerializeField] AudioSource _audioCorrida;

    [SerializeField] AudioClip[] _passos;

    [SerializeField] AudioClip[] _corrida;

    [SerializeField] AudioClip[] _pulo;

    [SerializeField] AudioClip[] _vozPulo;

    [SerializeField] AudioClip[] _espada;

    [SerializeField] AudioClip[] _vozEspada;

    [SerializeField] AudioClip[] _picareta;

    [SerializeField] AudioClip[] _vozPicareta;

    [SerializeField] AudioClip[] _impactoEspada;

    [SerializeField] AudioClip[] _impactoPicareta;

    bool _noChao;

    public void SetNoChao(bool valor)
    {
        _noChao = valor;
    }

    public void Passo()
    {
        if (_audioCorrida != null && _audioCorrida.isPlaying) return;
        if (_noChao) Tocar(_efeitos, _passos);
    }

    public void SetCorrendo(bool correndo)
    {
        if (_audioCorrida == null) return;

        if (!correndo)
        {
            if (_audioCorrida.isPlaying) _audioCorrida.Stop();
            return;
        }

        if (_audioCorrida.isPlaying || _corrida == null || _corrida.Length == 0) return;

        AudioClip som = _corrida[Random.Range(0, _corrida.Length)];
        if (som == null) return;

        _audioCorrida.clip = som;
        _audioCorrida.loop = true;
        _audioCorrida.Play();
    }

    void OnDisable()
    {
        SetCorrendo(false);
    }

    public void Pulo()
    {
        _noChao = false;
        SetCorrendo(false);
        Tocar(_efeitos, _pulo);
        Tocar(_voz, _vozPulo, true);
    }

    public void Espada()
    {
        SetCorrendo(false);
        Tocar(_efeitos, _espada);
        Tocar(_voz, _vozEspada, true);
    }

    public void Picareta()
    {
        SetCorrendo(false);
        Tocar(_efeitos, _picareta);
        Tocar(_voz, _vozPicareta, true);
    }

    public void ImpactoEspada()
    {
        Tocar(_efeitos, _impactoEspada);
    }

    public void ImpactoPicareta()
    {
        Tocar(_efeitos, _impactoPicareta);
    }

    void Tocar(AudioSource fonte, AudioClip[] sons, bool voz = false)
    {
        if (fonte == null || sons == null || sons.Length == 0) return;

        AudioClip som = sons[Random.Range(0, sons.Length)];
        if (som == null) return;

        if (voz)
        {
            fonte.clip = som;
            fonte.Play();
        }
        else
        {
            fonte.PlayOneShot(som);
        }
    }
}
