using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Eventos que generara el player
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDie;
    public static event PlayerDelegate OnPlayerScore;


    [SerializeField] float force;
    Rigidbody2D rigidbody2;
    bool playerMove = false;

    [SerializeField] Vector3 upRotation;
    [SerializeField] Quaternion downRotation;
    [SerializeField] public float smooth;

    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip dieClip;
    [SerializeField] AudioClip scoreClip;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        //Obtener la referencia al componente Rigidbody2D del player
        rigidbody2 = GetComponent<Rigidbody2D>();
        //Obetner la referencia del componente audioSource
        audioSource = GetComponent<AudioSource>();
        //Desactivamos las fisicas para que no caiga mientras empieza el juego
        rigidbody2.simulated = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Comprobamos si puede empezar a moverse
        if (playerMove)
        {
            //compruebo si el usuario toco la pantalla
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    //Elimino toda velocidad que estubiera aplicandosele
                    rigidbody2.velocity = Vector2.zero;
                    //Le añado la fuerza hacia arriba para su movimiento
                    rigidbody2.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                    //Rotar la cabeza hacia arriba
                    //transform.rotation = upRotation;
                    transform.eulerAngles = upRotation;
                    //Play audio
                    audioSource.clip = jumpClip;
                    audioSource.Play();
                }
            }
            //funcion de movimiento entre mirar arriba y mirar abajo
            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, Time.deltaTime * smooth);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //lo mismo pero difrente escrito = if (collision.gameObject.tag == "DeadZone")
        if (collision.CompareTag("DeadZone"))
        {
            //Audio de que se mata
            audioSource.clip = dieClip;
            audioSource.Play();

            //Para la gravedad al morir
            rigidbody2.simulated = false;

            //Activo el evento OnPlayerDie si no es nulo
            if (OnPlayerDie != null) OnPlayerDie();
        }
    }

    private void Awake()
    {
        //Altavox -> GameManager.OnGameStart
        //Micro -> OnGameStartPlayer
        //Suscripcion a un evento
        GameManager.OnGameStart += OnGameStartPlayer;
        GameManager.OnGameOver += OnGameOverPlayer;
        GameManager.OnGameReStart += OnGameReStartPlayer;
    }

    void OnGameStartPlayer()
    {
        rigidbody2.simulated = true;
        playerMove = true;
    }

    void OnGameOverPlayer()
    {
        playerMove = false;
    }

    void OnGameReStartPlayer()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        playerMove = false;
        rigidbody2.velocity = Vector2.zero;
    }
}
