using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
TMP_Text localFrame;
    [SerializeField]
 TMP_Text remoteFrame;
    [SerializeField]
 TMP_Text frameDifference;

    [SerializeField]
 TMP_Text debugText;

    [SerializeField]
    TMP_Text simFramerate;

    [SerializeField]
    TMP_Text renderFramerate;

    [SerializeField]
    TMP_Text rollbackCount;

    [SerializeField]
    TMP_Text ping;

    [SerializeField]
    TMP_Text simPing;

    [SerializeField]
    TMP_Text totalPing;

    [SerializeField]
    TMP_Text InputBuffer;

    [SerializeField]
    TMP_Text p1state;

    [SerializeField]
    TMP_Text p2state;

    [SerializeField]
    NetworkManager net;

    [SerializeField]
    NetworkConfig cfg;

    [SerializeField]
    TMP_InputField ipentry;



    float t = 0;
    int frameOneSecondAgo = 0;

    private void Update()
    {
        int l =  GameSimulation.LocalFrame;
        int r =  GameSimulation.LastRemoteFrame;


        localFrame.text = "Local Frame: " + l.ToString();
        remoteFrame.text = "Remote Frame: " + r.ToString();
        frameDifference.text = "Frame Advantage: " + (l - r).ToString();
        rollbackCount.text = $"Total Rollbacks: {GameSimulation.rollbackCount}";
        ping.text = "Network Ping: " + NetworkManager.pingTime;
        InputBuffer.text = "Input Buffer: " + PlayerInput.INPUT_DELAY;
        GameState g;
        if ((g = Transport.current) != null)
        {
            p1state.text = (NetworkManager.hosting ? "Local" : "Remote") + " State: " + g.players[0].state;
            p2state.text = (!NetworkManager.hosting ? "Local" : "Remote") + " State: " + g.players[1].state;
        }

       if((t+=Time.deltaTime) >= 1) //triggers every second
        {
            t = 0;
            RemoveLine(1);
            simFramerate.text = "Sim FPS: " + (l - frameOneSecondAgo);
            frameOneSecondAgo = l;
            renderFramerate.text = "Render FPS: " + Mathf.CeilToInt(Time.frameCount / Time.time);
        }     
    }

    private void RemoveLine(int i) =>  debugText.text = string.Join(Environment.NewLine,Regex.Split(debugText.text, "\n").Skip(i).ToArray());

    private void PrintLog(string log, string stack, LogType type)
    {
        debugText.text+= $"{type}: {log}\n";

    }

    private void OnEnable()
    {
        Application.logMessageReceived += PrintLog;
    }


}
