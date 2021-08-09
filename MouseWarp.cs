using Gma.System.MouseKeyHook;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

using System.Runtime.InteropServices;
using System;
using System.Timers;

//Screen[Bounds ={ X = 196,Y = -1440,Width = 2560,Height = 1440}
//WorkingArea ={ X = 196,Y = -1440,Width = 2560,Height = 1440}
//Primary = False DeviceName =\\.\DISPLAY2
//Screen[Bounds ={X = 3272, Y = -1050, Width = 1680, Height = 1050} WorkingArea ={ X = 3272,Y = -1050,Width = 1680,Height = 1050}
//Primary = False DeviceName =\\.\DISPLAY1
//Screen[Bounds ={X = 0, Y = 0, Width = 5120, Height = 1440} WorkingArea ={ X = 0,Y = 0,Width = 5120,Height = 1400}
//Primary = True DeviceName =\\.\DISPLAY3
public class MouseWarp
{

    private int warpDistance = 100;
    private static List<string> monitorNames = null;
    private static System.Timers.Timer aTimer;


    public MouseWarp()
    {
        SetTimer();
    }

    private static void SetTimer()
    {
        // Create a timer with a two second interval.
        aTimer = new System.Timers.Timer(25);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        if (monitorNames != null && monitorNames.Count == 2)
        {
            //monitorNames.ForEach(montior => Debug.WriteLine(montior));
            var monitor1 = Array.Find(Screen.AllScreens, s => s.DeviceName == monitorNames[0]);
            var monitor2 = Array.Find(Screen.AllScreens, s => s.DeviceName == monitorNames[1]);


            if (Cursor.Position.X == monitor1.Bounds.X + monitor1.Bounds.Width - 1)
            {
                Cursor.Position = new Point(monitor2.Bounds.X + 2, Cursor.Position.Y);

            }
            if (Cursor.Position.X == monitor2.Bounds.X)
            {
                Cursor.Position = new Point(monitor1.Bounds.X + monitor1.Bounds.Width - 2, Cursor.Position.Y);

            }
        }
    }

    public void setMonitorNames(List<string> _monitorNames)
    {
        monitorNames = _monitorNames;
    }

    private void HookManager_MouseMove(object sender, MouseEventArgs e)
    {

      

        
    }


    private int translate(int value, int left_min, int left_max, int right_min, int right_max)
    {
        return ((value - left_min) * (right_max - right_min)) / (left_max - left_min) + right_min;
    }


}
