using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] protected GameObject pauseMenuUi;
    [SerializeField] protected GameObject successMenuUi;
    [SerializeField] protected GameObject failureMenuUi;
    [SerializeField] protected GameObject junkCounterUi;
    [SerializeField] protected GameObject timerUi;

    protected PlayerStateMachineBase[] players;
    protected bool isPaused;
    protected bool levelFailed;
    protected bool timerEnabled;

    protected int totalJunk;
    protected int[] junkCollected;
    protected int totalJunkCollected;
    protected float elapsedTime;
    protected string elapsedTimeString;

    //public int TotalJunk { get { return totalJunk; } set { totalJunk = value; } }
    //public int[] JunkCollected { get { return junkCollected; } set { junkCollected = value; } }

    void Start() {
        // Define any variables
        isPaused = false;
        levelFailed = false;
        timerEnabled = true;

        // Get UserID from UserInfo object
        //UserInfo userInfo = FindObjectOfType<UserInfo>();
        Debug.Log("HERE");
        Debug.Log(UserInfo.uid);
        /*
        if (userInfo != null)
        {
            Debug.Log(UserInfo.uid);
        }
        else
        {
            Debug.Log("NO USER CREATED. DID YOU LOGIN / REFGISTER?");
        }
        */

        // Define all timing variables to be 0
        elapsedTime = 0f;
        elapsedTimeString = null;

        // Find all movement scripts in sceneand store in array. Keep indexes the same as their assigned index
        players = FindObjectsOfType<PlayerStateMachineBase>();
        SortPlayers(players);

        junkCollected = new int[players.Length];
        for (int i = 0; i < junkCollected.Length; i++) {
            junkCollected[i] = 0;
        }

        // Calculate total junk in the scene
        totalJunk = FindTotalJunk();
        //Debug.Log("Total Junk in Level: " + totalJunk);
        totalJunkCollected = 0;

    }

    void Update() {

        if (timerEnabled) {
            UpdateTimer();
        }

        if (!levelFailed) {
            for (int i = 0; i < players.Length; i++) {
                if (players[i].EPlayerState == EPlayerState.Dead) {
                    levelFailed = true;
                    Failure();
                }
            }
        }
    }

    private void SortPlayers(PlayerStateMachineBase[] players) {
        bool playersSorted = false;
        while (!playersSorted) {
            PlayerStateMachineBase tempPlayer = null;
            for (int i = 0; i < players.Length; i++) {
                if (players[i].PlayerIndex != i) {
                    tempPlayer = players[players[i].PlayerIndex];
                    players[players[i].PlayerIndex] = players[i];
                    players[i] = tempPlayer;
                }
            }
            if (tempPlayer == null) {
                playersSorted = true;
            }
        }
    }

    protected void UpdateTimer() {
        // update timer
        elapsedTime += Time.deltaTime;
        int currentMinutes = (int)elapsedTime / 60;
        int currentSeconds = (int)elapsedTime % 60;
        int currentMilliseconds = (int)(1000 * elapsedTime % 1000);

        string currentMinutesString = currentMinutes.ToString();
        string currentSecondsString = currentSeconds.ToString();
        string currentMillisecondsString = currentMilliseconds.ToString();

        if (currentMinutes > 99) {
            // Don't wanna count hours, and don't want three digit minutes
            elapsedTimeString = "Why are you still here?";
            elapsedTime -= Time.deltaTime;
            timerEnabled = false;
        }
        else {
            // print the current time
            if (currentMinutes < 10) {
                currentMinutesString = '0' + currentMinutesString;
            }

            if (currentSeconds < 10) {
                currentSecondsString = '0' + currentSecondsString;
            }
            if (currentMilliseconds < 100) {
                if (currentMilliseconds < 10) {
                    currentMillisecondsString = "00" + currentMillisecondsString;
                }
                currentMillisecondsString = '0' + currentMillisecondsString;
                if (currentMillisecondsString.Length > 3) {
                    currentMillisecondsString = currentMillisecondsString.Substring(1, currentMillisecondsString.Length - 1);
                }
            }

            elapsedTimeString = currentMinutesString + ":" + currentSecondsString + "." + currentMillisecondsString;
        }

        if (timerUi != null) {
            if (timerUi.GetComponent<TMP_Text>() != null) {
                timerUi.GetComponent<TMP_Text>().text = elapsedTimeString;
            }
        }
    }

    protected int FindTotalJunk() {
        Collectible[] collectiblesInScene = FindObjectsOfType<Collectible>();
        return collectiblesInScene.Length;
    }

    public void PausePressed() {
        if (isPaused) {
            // unpause
            Time.timeScale = 1.0f;

            // allow players to accept movement input again
            for (int i = 0; i < players.Length; i++) {
                players[i].EnableMovementInput();
            }
            
            // bring up the pause screen
            if (pauseMenuUi != null) {
                pauseMenuUi.SetActive(false);
            }

        }
        else {
            // pause
            Time.timeScale = 0.0f;

            // disable movement input from players while paused
            for (int i = 0; i < players.Length; i++) {
                players[i].DisableMovementInput();
            }

            // remove the pause screen
            if (pauseMenuUi != null) {
                pauseMenuUi.SetActive(true);
            }
        }

        // toggle the isPaused boolean
        isPaused = !isPaused;
    }

    public void CollectJunk(int playerIndex, int value) {
        // increment junk collected for the player who collected it
        junkCollected[playerIndex] += 1; // Can potentially change to value if we implement that

        // calculate our total junk collected
        totalJunkCollected = 0;
        for (int i = 0; i < junkCollected.Length; i++) {
            totalJunkCollected += junkCollected[i];
        }

        // update our counter ui to reflect the current junk collected
        if (junkCounterUi != null) {
            if (junkCounterUi.GetComponent<TMP_Text>() != null) {
                junkCounterUi.GetComponent<TMP_Text>().text = "Total Junk Collected: " + totalJunkCollected;
            }
        }

        //Debug.Log(players[playerIndex].gameObject.name + " has collected " + junkCollected[playerIndex] + " junk!");
        //Debug.Log("Total Junk Collected: " + totalJunkCollected);

        // Check to see if we have collected all the garbage in the level
        if (totalJunkCollected >= totalJunk) {
            // All junk collected
            Success();
        }
    }

    public void Success() {
        if (successMenuUi != null) { 
            successMenuUi.SetActive(true);
        }

        EndLevel();
    }

    public void Failure() {
        if (failureMenuUi != null) {
            failureMenuUi.SetActive(true);
        }

        EndLevel();
    }

    public void EndLevel() {
        // Stop the timer
        timerEnabled = false;

        // Disable all movement input
        for (int i = 0; i < players.Length; i++) {
            players[i].DisableMovementInput();
        }

        // Do firebase stuff with timeElapsed, junkCollected, inc deaths if levelfailed, etc.
    }
}
