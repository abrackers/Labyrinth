using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    private TextMeshProUGUI instinst, instDiff, inst, credits;
    private bool playing = false, finished = false;
    private int difficulty;

    public GameObject instGO, instinstGo, instDiffGo, creditsGo;
    public Sprite[] tileSprites;
    public Sprite playerSprite;


    void Start()
    {
        inst = instGO.GetComponent<TextMeshProUGUI>();
        instDiff = instDiffGo.GetComponent<TextMeshProUGUI>();
        instinst = instinstGo.GetComponent<TextMeshProUGUI>();
        credits = creditsGo.GetComponent<TextMeshProUGUI>();
        LabyCreator.PlayerSprite = playerSprite;
        LabyCreator.Main = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing)
        {
            if (Input.GetKeyDown("space"))
                Play();
            if (Input.GetKeyDown("a"))
                ChangeDifficulty(-1);
            if (Input.GetKeyDown("d"))
                ChangeDifficulty(1);
            if (Input.GetKeyDown("l"))
                Liscence(true);
            if (Input.GetKeyUp("l"))
                Liscence(false);
        }
        else
        {
            if (!finished)
            {
                if (Input.GetKeyDown("w"))
                    AnalyseResult(LabyCreator.TryMove(Vector2Int.up));
                if (Input.GetKeyDown("a"))
                    AnalyseResult(LabyCreator.TryMove(Vector2Int.left));
                if (Input.GetKeyDown("s"))
                    AnalyseResult(LabyCreator.TryMove(Vector2Int.down));
                if (Input.GetKeyDown("d"))
                    AnalyseResult(LabyCreator.TryMove(Vector2Int.right));
            }
            else
            {
                if (Input.GetKeyDown("space"))
                {
                    Debug.Log("Reset");
                    LabyCreator.resetPlayer();
                    playing = true;
                    finished = false;
                    instDiff.text = "Hold C to show answer" + System.Environment.NewLine + "W,S,D,A to move N,S,E,W";
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeDifficulty(0);
                Ret();
            }
            if (Input.GetKeyDown("h"))
            {
                if (LabyCreator.FlipHideOnKill())
                    instinst.text = "Press H to hide" + System.Environment.NewLine + "found traps";
                else
                    instinst.text = "Press H to show" + System.Environment.NewLine + "found traps";
            }
            if (Input.GetKeyDown("c"))
                LabyCreator.Colour();
            if (Input.GetKeyUp("c"))
                LabyCreator.ResetColour();
        }
    }


    public void DestroyGameObject(GameObject toDestroy)
    {
        Destroy(toDestroy, 0.1f);
    }

    private void AnalyseResult(int res)
    {
        if (res == 2)
        {
            LabyCreator.Colour();
            finished = true;
            Debug.Log("finished");
            LabyCreator.DestroyPlayer();
            difficulty += 1;
            instDiff.text = "Press Esc to return" + System.Environment.NewLine + "to menu";
        }
        if (res == 0)
        {
            instDiff.text = "Press Space to reset";
            finished = true;
            Debug.Log("Died");
        }
    }

    private void Ret()
    {
        LabyCreator.Disable();
        playing = false;
        finished = false;
        instinst.text = "Press space to start" + System.Environment.NewLine + "Hold L for liscencing" + System.Environment.NewLine + "and credits";
        inst.enabled = true;
    }

    private void Liscence(bool enable)
    {
        credits.enabled = enable;
        inst.enabled = !enable;
    }

    private void ChangeDifficulty(int delta)
    {
        difficulty += delta;
        if (difficulty < 0)
            difficulty = 0;
        if (difficulty > 9)
            difficulty = 9;
        instDiff.text = "Difficulty " + (difficulty + 1) + System.Environment.NewLine + "Press A to decrease" + System.Environment.NewLine + "Press D to increase";
    }


    private void Play()
    {
        playing = true;
        inst.enabled = false;
        credits.enabled = false;
        instDiff.text = "Hold C to show answer" + System.Environment.NewLine + "W,S,D,A to move N,S,E,W";
        instinst.text = "Press H to hide" + System.Environment.NewLine + "found traps";
        LabyCreator.CreateLabyrinth(5 + difficulty * 2, tileSprites);
    }
}
