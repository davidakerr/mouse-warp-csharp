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
        private readonly HashSet<Button> _buttons = new();
        private readonly MouseWarp _mouseWarp = new();
        private bool _isQuitting;

        public MainWindow()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            var screens = Screen.AllScreens;

            SetWindowSize(
                // Don't ask about the 20 ? 
                TotalRealEstateWidth(screens) / GlobalConstant.Scale + GlobalConstant.WindowPadding + 20,
                TotalRealEstateHeight(screens) / GlobalConstant.Scale + GlobalConstant.MenuBarHeight +
                GlobalConstant.WindowPadding
            );

            foreach (var screen in Screen.AllScreens)
                Shown += (s, e) => CreateButton(
                    screen.DeviceName,
                    new Size(
                        screen.Bounds.Width,
                        screen.Bounds.Height) / GlobalConstant.Scale,
                    new Point(
                        screen.Bounds.X / GlobalConstant.Scale + GlobalConstant.WindowPadding / 2,
                        (screen.Bounds.Y + YOffset(screens)) / GlobalConstant.Scale + GlobalConstant.WindowPadding / 2)
                );
        }

        // If Y is a minus then give the inverse of that to add to values in the find max height function
        private static int YOffset(IEnumerable<Screen> screens)
        {
            var lowestHeight = screens.Select(screen => screen.Bounds.Y).Prepend(0).Min();
            return lowestHeight <= 0 ? lowestHeight * -1 : 0;
        }

        private static int TotalRealEstateWidth(IEnumerable<Screen> screens)
        {
            return screens.Select(screen => screen.Bounds.X + screen.Bounds.Width).Prepend(0).Max();
        }

        private static int TotalRealEstateHeight(Screen[] screens)
        {
            return screens.Select(screen => screen.Bounds.Y + YOffset(screens) + screen.Bounds.Height).Prepend(0).Max();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            Debug.WriteLine("On Resize");

            if (WindowState != FormWindowState.Minimized) return;
            Hide();
            trayIcon.Visible = true;
        }

        public void SetWindowSize(int width, int height)
        {
            Size = new Size(width, height);
        }

        private void CreateButton(string name, Size size, Point location)
        {
            var newButton = new Button();
            Controls.Add(newButton);
            newButton.Text = name.Replace(@"\\.\", "");
            newButton.Name = name;
            newButton.BackColor = Color.Black;
            newButton.ForeColor = Color.White;
            newButton.FlatStyle = FlatStyle.Flat;
            newButton.FlatAppearance.BorderColor = Color.Gray;
            newButton.FlatAppearance.BorderSize = 1;
            newButton.Size = size;
            newButton.Location = location;
            newButton.MouseDown += OnScreenSelectHandler;
        }

        private void OnScreenSelectHandler(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(_buttons.Count());
            //Debug.WriteLine((sender as Button).Name);
            if (_buttons.Count() == 2)
            {
                _buttons.ToList().ForEach(button => SetButtonColor(button, Color.Black));
                _buttons.Clear();
            }

            _buttons.Add(sender as Button);
            _buttons.ToList().ForEach(button => { button.BackColor = Color.Blue; });
            _mouseWarp.SetMonitorNames(new List<string>(_buttons.Select(button => button.Name)));
        }

        private static void SetButtonColor(Control button, Color color)
        {
            button.BackColor = color;
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.WindowsShutDown && !_isQuitting)
            {
                Hide();
                Debug.WriteLine("Close override and minimize to tray");
                trayIcon.Visible = true;
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            _isQuitting = true;
            Close();
        }
    }
}