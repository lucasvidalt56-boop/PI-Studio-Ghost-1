using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private GameObject botaoInicial;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(botaoInicial);
    }
}
