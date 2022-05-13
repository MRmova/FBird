using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentMovement : MonoBehaviour
{
    #region PoolObject
    class PoolObject
    {
        //Variables publicas del objeto
        public Transform transform;
        public bool inUse;

        //Constructor: necesitamos transform para spawn
        public PoolObject(Transform t)
        {
            transform = t;
        }

        //Funcion para indicar que un objeto concreto esta en uso
        public void Use()
        {
            inUse = true;
        }

        //Funcion para liberar el objeto
        public void Dispose()
        {
            inUse = false;
        }
    }


    #endregion

    #region Variables
    //privadas
    public float ymin;
    public float ymax;

    //generales
    public GameObject prefab;
    public float speed;
    public float spawnRate;
    public float spawnPositionX;
    public int poolSize;
    PoolObject[] poolObjects;

    float timer;
    bool moverEnvironment = false;

    #endregion

    #region Funciones
    private void Awake()
    {
        //Suscripcion a los eventos
        GameManager.OnGameStart += OnGameStartPool;
        GameManager.OnGameReStart += OnGameReStartPool;
        GameManager.OnGameReStart += OnGameOverPool;
    }

    void OnGameStartPool()
    {
        moverEnvironment = true;
    }

    void OnGameReStartPool()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            //Liberamos todos los obejtos 
            poolObjects[i].Dispose();
            //Posicionamos todos los objetos en un lugar adecuado
            poolObjects[i].transform.position = Vector3.right * spawnPositionX;
        }
        moverEnvironment = false;
    }

    void OnGameOverPool()
    {
        //Paramos el movimiento de los objetos
        moverEnvironment = false;
    }

    private void Start()
    {
        //Inicializar la instancia de PoolObject
        poolObjects = new PoolObject[poolSize];

        //inicializar cada objeto a traves de la llamada al contructor
        for (int i = 0; i < poolObjects.Length; i++)
        {
            //instancio el prefab
            GameObject gameObjectPool = Instantiate(prefab);

            //ponerlo como hijo del padre
            gameObjectPool.transform.SetParent(this.transform);

            //transladarlo a una posicion lejana para que no se vea en la pantalla
            gameObjectPool.transform.position = Vector3.right * spawnPositionX;

            //Lo añado al pool
            poolObjects[i] = new PoolObject(gameObjectPool.transform);
        }
    }

    private void Update()
    {
        //no mover los objetos hasta que empieze el juego
        if (moverEnvironment)
        {
            //Gestionar el pool y mover los objetos
            for (int i = 0; i < poolObjects.Length; i++)
            {
                //si esta en uso lo muevo
                if (poolObjects[i].inUse)
                {
                    poolObjects[i].transform.position += Vector3.left * speed * Time.deltaTime;

                    if (poolObjects[i].transform.position.x < -spawnPositionX)
                    {
                        poolObjects[i].Dispose();
                        poolObjects[i].transform.position = new Vector3(spawnPositionX, 0.0f, 0.0f);

                    }
                }
            }

            //Gestion del pool ¿Cuando voy a utilizar los objetos del pool?
            //Cada vez que supere el speed Rate, saco un objeto de la pool
            timer += Time.deltaTime;
            if (timer > spawnRate)
            {
                timer = 0.0f;
                Transform transformObject = GetPoolObject();
                if (transformObject = null) return;
                transformObject.position = new Vector3();
            }

            //funcion para obtener un objeto libre de la pool
            Transform GetPoolObject()
            {
                //recorro el pool completo
                for (int i = 0; i < poolObjects.Length; i++)
                {
                    //voy preguntando a cada objeto si esta en uso
                    if (!poolObjects[i].inUse)
                    {
                        //si no esta en uso lo voy a utilizar
                        //- lo marco como en uso
                        poolObjects[i].Use();
                        //- Devuelvo el transform
                        return poolObjects[i].transform;
                    }
                }
                //Si recorro el pol completo y todos los objetos estan en uso 
                //Devuelve null
                return null;
            }
        }
    }
    #endregion
}
