using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mouse_Warp.Properties;

namespace Mouse_Warp
{
    public partial class MainWindow : Form
    {
        private readonly List<Button> _buttons = new();
        private readonly MouseWarp _mouseWarp = new();
        private bool _isQuitting;
        private HashSet<string> _selectedMonitors = new();

        public MainWindow()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;


            LoadSettings();


            var screens = Screen.AllScreens;

            SetWindowSize(
                // Don't ask about the 20 ? 
                TotalRealEstateWidth(screens) / GlobalConstant.Scale + GlobalConstant.WindowPadding + 20,
                TotalRealEstateHeight(screens) / GlobalConstant.Scale + GlobalConstant.MenuBarHeight +
                GlobalConstant.WindowPadding);

            foreach (var screen in Screen.AllScreens)
                CreateButton(screen.DeviceName,
                    new Size(screen.Bounds.Width, screen.Bounds.Height) / GlobalConstant.Scale,
                    new Point(screen.Bounds.X / GlobalConstant.Scale + GlobalConstant.WindowPadding / 2,
                        (screen.Bounds.Y + YOffset(screens)) / GlobalConstant.Scale +
                        GlobalConstant.WindowPadding / 2));
            SetButtonSelected();
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
            _buttons.Add(newButton);
        }

        private void OnScreenSelectHandler(object sender, MouseEventArgs e)
        {
            if (_selectedMonitors.Count() == 2)
            {
                _buttons.ForEach(button => SetButtonColor(button, Color.Black));
                _selectedMonitors.Clear();
            }

            _selectedMonitors.Add((sender as Button).Name);
            SetButtonSelected();
            SaveSettings();
            _mouseWarp.SetMonitorNames(_selectedMonitors.ToList());
        }

        private void SetButtonSelected()
        {
            _buttons.ForEach(button =>
            {
                if (!_selectedMonitors.Any(button.Name.Contains)) return;
                SetButtonColor(button, Color.Blue);
            });
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

        private void LoadSettings()
        {
            if (Settings.Default.monitorNames == null) return;
            _selectedMonitors = new HashSet<string>(Settings.Default.monitorNames.Cast<string>().ToList());
            _mouseWarp.SetMonitorNames(_selectedMonitors.ToList());
        }

        private void SaveSettings()
        {
            StringCollection collection = new();
            collection.AddRange(_selectedMonitors.ToArray());
            Settings.Default["monitorNames"] = collection;
            Settings.Default.Save();
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