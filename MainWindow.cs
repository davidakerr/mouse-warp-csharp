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
        private MouseWarp mouseWarp = new MouseWarp();
        public MainWindow()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            Screen[] screens = Screen.AllScreens;

            SetWindowSize(
                // Dont ask about the 20 ? 
                totalRealEstateWidth(screens) / GlobalConstant.Scale + GlobalConstant.WindowPadding + 20,
                totalRealEstateHeight(screens) / GlobalConstant.Scale + GlobalConstant.MenuBarHeight + GlobalConstant.WindowPadding
               );

            foreach (Screen screen in Screen.AllScreens)
            {
                this.Shown += (s, e) => CreateButton(
                    screen.DeviceName,
                    new Size(
                        screen.Bounds.Width,
                        screen.Bounds.Height) / GlobalConstant.Scale,
                    new Point(
                        screen.Bounds.X / GlobalConstant.Scale + GlobalConstant.WindowPadding / 2,
                        (screen.Bounds.Y + yOffset(screens)) / GlobalConstant.Scale + GlobalConstant.WindowPadding / 2) 
                    );
            }
        }
        // If Y is a minus then give the inverse of that to add to values in the find max height function
        private int yOffset(Screen[] screens)
        {
            int lowestHeight = 0;
            foreach (Screen screen in screens)
            {

                if (screen.Bounds.Y < lowestHeight)
                {
                    lowestHeight = screen.Bounds.Y;
                }
            }
            return lowestHeight <= 0 ? lowestHeight * -1 : 0;
        }

        private int totalRealEstateWidth(Screen[] screens)
        {
            int maxWidth = 0;
            foreach (Screen screen in screens)
            {
                int totalWidth = screen.Bounds.X + screen.Bounds.Width;
                if (totalWidth > maxWidth)
                {
                    maxWidth = totalWidth;
                }
            }
            return maxWidth;
        }

        private int totalRealEstateHeight(Screen[] screens)
        {
            int maxHeight = 0;
            foreach (Screen screen in screens)
            {
                int theFuckIsThis = screen.Bounds.Y + yOffset(screens) + screen.Bounds.Height;
                if (theFuckIsThis > maxHeight)
                {
                    maxHeight = theFuckIsThis;
                }
            }
            return maxHeight;
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
            newButton.Text = name.Replace(@"\\.\", "");
            newButton.Name = name;
            newButton.BackColor = Color.Black;
            newButton.ForeColor = Color.White;
            newButton.FlatStyle = FlatStyle.Flat;
            newButton.FlatAppearance.BorderColor = Color.Gray;
            newButton.FlatAppearance.BorderSize = 1;
            newButton.Size = size;
            newButton.Location = location;
            newButton.MouseDown += new MouseEventHandler(this.OnScreenSelectHandler);
        }

        private void OnScreenSelectHandler(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(buttons.Count());
            //Debug.WriteLine((sender as Button).Name);
            if (buttons.Count() == 2)
            {
                buttons.ToList().ForEach(button => SetButtonColor(button, Color.Black));
                buttons.Clear();
            }
            buttons.Add(sender as Button);
            buttons.ToList().ForEach(button => { button.BackColor = Color.Blue; });
            mouseWarp.setMonitorNames(new List<string>(buttons.Select(button => button.Name)));
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
