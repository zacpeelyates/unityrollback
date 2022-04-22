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
    NetworkManager net;

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
        simPing.text = "Simulated Ping: " + net.simulatedPing;
        totalPing.text = "Total Delay: " + (NetworkManager.pingTime + net.simulatedPing);
        InputBuffer.text = "Input Buffer: " + PlayerInput.INPUT_DELAY;


       if((t+=Time.deltaTime) >= 1) //triggers every second
        {
            t = 0;
            RemoveLine(1);
            simFramerate.text = "Sim FPS: " + ((float)(l - frameOneSecondAgo)).ToString();
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
