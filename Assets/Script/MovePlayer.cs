using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] float _velocidadeX = 5;

    [SerializeField] Vector2 _posicao;

    [SerializeField] Rigidbody2D _rig2d;

    [SerializeField] bool _checkGround;

    [SerializeField] bool _isface;

    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _rig2d.angularVelocity = 0;
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _posicao = value.ReadValue<Vector2>();
    }

    public void SetJump(InputAction.CallbackContext value)
    {
        if (_checkGround)
        {
            _rig2d.AddForceY(150);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _checkGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _checkGround = false;
        }
    }

    private void Flip()
    {
        _isface = !_isface;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

    }

    void Update()
    {
        _rig2d.linearVelocityX = _posicao.x * _velocidadeX;
        if (_posicao.x < 0 && _isface == false)
        {
            Flip();
        }
        else if (_posicao.x > 0 && _isface == true)
        {
            Flip();
        }
    }
}

