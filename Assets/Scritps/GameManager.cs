using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Variables
    //Eventos
    public delegate void GameDelegate();
    public static event GameDelegate OnGameOver;
    public static event GameDelegate OnGameStart;
    public static event GameDelegate OnCounDtownStarted;
    public static event GameDelegate OnGameReStart;

    //Referencias a paneles y elementos del Canvas
    [SerializeField] Text scoreText;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject countDownPanel;
    [SerializeField] Text highScoreText;

    //Puntuacion
    int score = 0;

    //Funciones
    public void StartGame()
    {
        //Desactivar el panel de start
        startPanel.SetActive(false);
        //Activamos panel de cuenta atras
        countDownPanel.SetActive(true);
        //Lanzar la cuenta atras
        StartCoroutine(CountDown());


    }

    //Corutina para gestionar la cuenta atras
    IEnumerator CountDown()
    {
        int count = 3;
        for (int i = count; i >= 0; i--)
        {
            //Actualizar el valor de la cuenta atras en la UI
            Text countDownText = countDownPanel.GetComponentInChildren<Text>();
            countDownText.text = i.ToString();
            //Esperamos un segundo
            yield return new WaitForSeconds(1);
        }
        //Cuando pasan count segundos, ejecutamos el siguiente codigo
        //Desactivo el panel de cuenta atras
        countDownPanel.SetActive(false);
        //Generamos evento de que el juego empieza
        if (OnGameStart != null) OnGameStart();
    }
}