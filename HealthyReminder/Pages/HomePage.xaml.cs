using HealthyReminder.Models;
using HealthyReminder.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace HealthyReminder.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private const string AWAY_MESSAGE = "You've been inactive for long time.";

        private static bool _wasUserActive = true;

        private static long _lastDifferenceSeconds = 0;

        private static Dictionary<Button, ButtonTile> _ButtonTileDic;

        private static Action<Schedule> _showSchedulePageFunc;

        public void RefreshPage()
        {
            ScheduleGrid.Children.Clear();
            _ButtonTileDic = new Dictionary<Button, ButtonTile>();
            if (ScheduleHelper.Schedules.Count > 0)
            {
                int height = 60;
                foreach (Schedule s in ScheduleHelper.Schedules)
                {
                    int topMargin = 10 + ((height + 10) * _ButtonTileDic.Count);
                    var tile = new ButtonTile(s, height, new Thickness(17, topMargin, 0, 0));
                    tile.TopButton.Click += EditSchedule;
                    ScheduleGrid.Children.Add(tile.TopButton);
                    _ButtonTileDic.Add(tile.TopButton, tile);
                }
            }
        }

        private void EditSchedule(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var tile = _ButtonTileDic[btn];
            _showSchedulePageFunc(tile.Schedule);
        }

        private void AddSchedule(object sender, EventArgs e)
        {
            _showSchedulePageFunc(null);
        }

        public HomePage(Action<Schedule> showSchedulePageFunc)
        {
            InitializeComponent();
            _showSchedulePageFunc = showSchedulePageFunc;
            RefreshPage();
        }

        public void Tick()
        {
            if (!ScheduleHelper.IsUserActive)
            {
                if (_wasUserActive)
                {
                    Debug.WriteLine(AWAY_MESSAGE);
                    StatusTextBlock.Text = AWAY_MESSAGE;
                    _wasUserActive = false;
                }
                return;
            }
            else
            {
                _wasUserActive = true;
            }

            long differenceSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - ScheduleHelper.ActiveStartUnixTimeSeconds;
            if (_lastDifferenceSeconds == differenceSeconds)
            {
                return;
            }

            string activeMessage = "You've been active for\n" + SecondsToTime(differenceSeconds);
            Debug.WriteLine(activeMessage);
            StatusTextBlock.Text = activeMessage;
            _lastDifferenceSeconds = differenceSeconds;
        }

        private string SecondsToTime(long seconds)
        {
            string timeStr = "";
            if (seconds >= 3600)
            {
                timeStr += (seconds / 3600) + " Hr";
                seconds %= 3600;
                if (seconds > 0)
                {
                    timeStr += " ";
                }
            }
            if (seconds >= 60)
            {
                timeStr += (seconds / 60) + " Min";
                seconds %= 60;
                if (seconds > 0)
                {
                    timeStr += " ";
                }
            }
            if (seconds >= 1 || timeStr.Length == 0)
            {
                timeStr += seconds + " Sec";
            }
            return timeStr;
        }
    }
}
