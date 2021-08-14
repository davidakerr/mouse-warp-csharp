using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

//Screen[Bounds ={ X = 196,Y = -1440,Width = 2560,Height = 1440}
//WorkingArea ={ X = 196,Y = -1440,Width = 2560,Height = 1440}
//Primary = False DeviceName =\\.\DISPLAY2
//Screen[Bounds ={X = 3272, Y = -1050, Width = 1680, Height = 1050} WorkingArea ={ X = 3272,Y = -1050,Width = 1680,Height = 1050}
//Primary = False DeviceName =\\.\DISPLAY1
//Screen[Bounds ={X = 0, Y = 0, Width = 5120, Height = 1440} WorkingArea ={ X = 0,Y = 0,Width = 5120,Height = 1400}
//Primary = True DeviceName =\\.\DISPLAY3
namespace Mouse_Warp
{
    public class MouseWarp
    {
        private static List<string> _monitorNames;
        private static Timer _aTimer;


        public MouseWarp()
        {
            SetTimer();
        }

        private static void SetTimer()
        {
            _aTimer = new Timer(25);
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (_monitorNames is not {Count: 2}) return;
            var selection1 = Array.Find(Screen.AllScreens, s => s.DeviceName == _monitorNames[0]);
            var selection2 = Array.Find(Screen.AllScreens, s => s.DeviceName == _monitorNames[1]);

            ref var monitor1 = ref selection1;
            ref var monitor2 = ref selection2;

            if (!(selection2.Bounds.X > selection1.Bounds.X + selection1.Bounds.Width))
            {
                monitor1 = ref selection2;
                monitor2 = ref selection1;
            }


            // If left to right then :

            if (Cursor.Position.X == monitor1.Bounds.X + monitor1.Bounds.Width - 1)
                Cursor.Position = new Point(monitor2.Bounds.X + 2, Cursor.Position.Y);
            if (Cursor.Position.X == monitor2.Bounds.X)
                Cursor.Position = new Point(monitor1.Bounds.X + monitor1.Bounds.Width - 2, Cursor.Position.Y);

            // TODO: If top to bottom
        }

        public void SetMonitorNames(List<string> monitorNames)
        {
            _monitorNames = monitorNames;
        }


        private int Translate(int value, int leftMin, int leftMax, int rightMin, int rightMax)
        {
            return (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin) + rightMin;
        }
    }
}
