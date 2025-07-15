using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform finishLine;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private float speedOnWin;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject particleEffect;

    private Player stickman;
    private Vector3 initPos;

    private bool won;

    private void Start()
    {
        stickman = player.GetComponent<Player>();
        initPos = player.transform.position;

    }

    private void Update()
    {
        if(stickman.GetSticked() == false)
        {
            if(player.transform.position.x < -5)
            {
                ResetGame();
            }
            if(player.transform.position.y < -6)
            {
                ResetGame();
            }
        }
        if(finishLine.position.x < player.transform.position.x && !won)
        {
            won = true;
            Win();
        }
    }

    private void ResetGame()
    {
        stickman.ResetGame(initPos);
    }

    private void Win()
    {
        stickman.Win(speedOnWin);

        particleEffect.SetActive(true);
        particleEffect.transform.parent = null;

        cameraFollow.Win();

        StartCoroutine(FinishLevel());
    }

    IEnumerator FinishLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
