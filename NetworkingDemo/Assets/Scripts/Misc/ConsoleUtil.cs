using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ConsoleUtil 
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;
     
    static readonly IntPtr handle = GetConsoleWindow();

    public static void Show() => ShowWindow(handle, SW_SHOW);
    public static void Hide() => ShowWindow(handle, SW_HIDE);

}
