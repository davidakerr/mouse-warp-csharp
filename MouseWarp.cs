using Gma.System.MouseKeyHook;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;


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
    private IKeyboardMouseEvents m_Events;
    private List<string> monitorNames = null;

    public MouseWarp()
    {
        SubscribeGlobal();
    }

    public void setMonitorNames(List<string> _monitorNames)
    {
        monitorNames = _monitorNames;
    }

    private void SubscribeGlobal()
    {
        Unsubscribe();
        Subscribe(Hook.GlobalEvents());
    }
    private void Subscribe(IKeyboardMouseEvents events)
    {
        m_Events = events;
        m_Events.MouseMove += HookManager_MouseMove;

    }

    private void Unsubscribe()
    {
        if (m_Events == null) return;
        m_Events.MouseMove -= HookManager_MouseMove;
        m_Events.Dispose();
        m_Events = null;
    }

    private void HookManager_MouseMove(object sender, MouseEventArgs e)
    {
        if (monitorNames != null)
        {
            monitorNames.ForEach(montior => Debug.WriteLine(montior));

        }
        Debug.WriteLine(string.Format("x={0}; y={1}", e.X, e.Y));
    }

    private int translate(int value, int left_min, int left_max, int right_min, int right_max)
    {
        return ((value - left_min) * (right_max - right_min)) / (left_max - left_min) + right_min;
    }


}
