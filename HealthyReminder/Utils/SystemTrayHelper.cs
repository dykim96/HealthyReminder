using System;
using System.Windows.Forms;

namespace HealthyReminder.Utils
{
    public class SystemTrayHelper
    {
        private static NotifyIcon _notifyIcon = Initialize();

        private static NotifyIcon Initialize()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Text = "Healthy Reminder";
            UpdateStatus(true);

            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

            return _notifyIcon;
        }

        public static void SetShowEvent(EventHandler showEventHandler)
        {
            _notifyIcon.DoubleClick += showEventHandler;
            if (_notifyIcon.ContextMenuStrip.Items.ContainsKey("Open"))
            {
                _notifyIcon.ContextMenuStrip.Items.RemoveByKey("Open");
            }
            var menuStrip = new ContextMenuStrip();
            menuStrip.Items.Add("Open", null, showEventHandler);
            _notifyIcon.ContextMenuStrip.Items.Insert(0, menuStrip.Items[0]);
        }

        private static void Exit(object sender, EventArgs args)
        {
            _notifyIcon.Dispose();
            _notifyIcon = null;
            System.Windows.Application.Current.Shutdown();
        }

        public static void UpdateStatus(bool isActive)
        {
            if (isActive)
            {
                _notifyIcon.Icon = Resource.Status_Active;
            }
            else
            {
                _notifyIcon.Icon = Resource.Status_Away;
            }
        }

        public static void NotifyHide()
        {
            Notify("Healthy Reminder", "The app has been minimized. Double click the tray icon to show.", 1000);
        }

        public static void Notify(string title, string message, int timeout)
        {
            if (_notifyIcon == null)
                return;

            if (timeout <= 0)
                timeout = 1000;

            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.ShowBalloonTip(timeout);
        }
    }
}
