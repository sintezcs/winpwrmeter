using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

namespace WinPowerMeter
{
    internal class PowerMeter : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenuStrip _trayMenu;

        private DateTime _offlineStartTime;

        private PowerLineStatus _currentStatus;

        public PowerMeter()
        {
            _trayMenu = new ContextMenuStrip()
            {
                Items =
                {
                    new ToolStripMenuItem("A&bout", null, OnAboutClick),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("E&xit", null, OnExitClick)
                }
            };

            _trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.BatteryIcon,
                Visible = true,
                ContextMenuStrip = _trayMenu,
                Text = "WinPowerMeter"                
            };

            _trayIcon.MouseClick += _trayIcon_MouseClick;

            UpdatePowerStatus();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        void _trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _trayIcon.ShowBalloonTip(3000, "PowerMeter", GetBatteryTime(), ToolTipIcon.Info);    
            }            
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.StatusChange)
            {
                UpdatePowerStatus();
            }
        }

        private void UpdatePowerStatus()
        {
            _currentStatus = SystemInformation.PowerStatus.PowerLineStatus;
            if (_currentStatus == PowerLineStatus.Online)
            {
                TogglePowerMode();
            }
            else
            {
                ToggleBatteryMode();
            }  
        }

        private void OnExitClick(object target, EventArgs args)
        {
            Application.Exit();            
        }

        private void OnAboutClick(object target, EventArgs args)
        {
            MessageBox.Show("WindowsPowerMeter v1.0\n© breakpoints.ru 2015", "About");
        }

        private void ToggleBatteryMode()
        {
            _offlineStartTime = DateTime.Now;
        }

        private void TogglePowerMode()
        {
            
        }

        private const string BatteryisCharging = "DC power mode";
        private const string BatteryTime = "Offline time: {0}";

        private string GetBatteryTime()
        {
            if (_currentStatus == PowerLineStatus.Online)
            {
                return BatteryisCharging;
            }
            else
            {
                string total;
                var delta = DateTime.Now.Subtract(_offlineStartTime).TotalMinutes;
                if (delta > 60)
                {
                    var hours = Math.Ceiling(delta/60);
                    var minutes = (int) delta%60;
                    total = String.Format("{0}:{1}", hours, minutes);
                    
                }
                else
                {
                    total = Math.Ceiling(delta).ToString();
                }
                return string.Format(BatteryTime, total);
            }
        }

        protected override void ExitThreadCore()
        {
            base.ExitThreadCore();
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }
    }
}
