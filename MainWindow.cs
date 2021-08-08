using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mouse_Warp
{
    public partial class MainWindow : Form
    {
        private HashSet<Button> buttons = new HashSet<Button>();
        public MainWindow()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            SetWindowSize(500, 500);

            Array.ForEach(Screen.AllScreens, s => Debug.WriteLine(s));

            this.Shown += (s, e) => CreateButton("Monitor1", new Size(200, 100), new Point(50, 50));
            this.Shown += (s, e) => CreateButton("Monitor2", new Size(150, 100), new Point(80, 300));
            this.Shown += (s, e) => CreateButton("Monitor3", new Size(150, 100), new Point(100, 200));

            //foreach (var screen in Screen.AllScreens)
            //{
            //    // For each screen, add the screen properties to a list box.
            //    //System.Diagnostics.Debug.WriteLine("Device Name: " + screen);
            //    this.Shown += (s, e) => CreateButton("test", new Size(), new Point());
            //    //Debug.WriteLine("Device Name: " + screen.DeviceName);
            //    //Debug.WriteLine("Bounds: " + screen.Bounds.ToString());
            //    //Debug.WriteLine("Type: " + screen.GetType().ToString());
            //    //Debug.WriteLine("Working Area: " + screen.WorkingArea.ToString());
            //    //Debug.WriteLine("Primary Screen: " + screen.Primary.ToString());
            //}

            MouseWarp mouseWarp = new MouseWarp();
            mouseWarp.TestFunction("Yeoooo!");

        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            Debug.WriteLine("On Resize");

            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                trayIcon.Visible = true;

            }
        }

        public void SetWindowSize(int width, int height)
        {
            this.Size = new Size(width, height);
        }

        private void CreateButton(string name, Size size, Point location)
        {
            Button newButton = new Button();
            this.Controls.Add(newButton);
            newButton.Text = name;
            newButton.Name = name;
            newButton.BackColor = Color.Black;
            newButton.ForeColor = Color.White;
            newButton.FlatStyle = FlatStyle.Flat;
            newButton.FlatAppearance.BorderColor = Color.Gray;
            newButton.FlatAppearance.BorderSize = 1;
            newButton.Size = size;
            newButton.Location = location;
            newButton.MouseDown += new MouseEventHandler(this.MyButtonHandler);
        }

        private void MyButtonHandler(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(buttons.Count());
            //Debug.WriteLine((sender as Button).Name);
            if (buttons.Count() == 2)
            {
                buttons.ToList().ForEach(button => SetButtonColor(button, Color.Black));
                buttons.Clear();
            }
            buttons.Add(sender as Button);
            //buttons.Find(button => button.Name == "Monitor2").BackColor = Color.Pink;
            buttons.ToList().ForEach(button => SetButtonColor(button, Color.Blue));
            buttons.ToList().ForEach(button => Debug.WriteLine(button));
        }

        private void SetButtonColor(Button button, Color color)
        {
            button.BackColor = color;
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.WindowsShutDown)
            {
                Hide();
                Debug.WriteLine("Close override and minimize to tray");
                trayIcon.Visible = true;
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

    }
}
