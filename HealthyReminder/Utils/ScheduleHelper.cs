using HealthyReminder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HealthyReminder.Utils
{
    public class ScheduleHelper
    {
        public static Schedule WaterReminder { get; private set; } = new Schedule(1, "Water Reminder",
                    "Are you staying hydrated?\nA general rule of thumb is to drink 2 to 3 cups of water per hour.",
                    120, 0, 0);
        public static Schedule EyeRestReminder { get; private set; } = new Schedule(2, "Eye Rest Reminder",
                    "How about giving your eyes a little break?\nLook into the distance for 20 seconds to allow your eyes a chance to refocus.",
                    30, 0, 0);
        public static Schedule StandUpReminder { get; private set; } = new Schedule(3, "Stand Up Reminder",
                    "Sitting too much?\nHow about a stretch, a little walk, or working while standing up?",
                    60, 0, 0);

        private static List<Schedule> _schedules;

        public static long ActiveStartUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public static bool IsUserActive { get; private set; } = true;

        public static List<Schedule> Schedules
        {
            get
            {
                if (_schedules == null)
                {
                    LoadSchedules();
                }
                return _schedules;
            }
            private set { _schedules = value; }
        }

        private static void LoadSchedules()
        {
            _schedules = SqliteDbHelper.LoadSchedules();
            if (_schedules == null)
            {
                _schedules = new List<Schedule>();
            }

            if (_schedules.Count <= 0)
            {
                AddSchedule(WaterReminder);
                AddSchedule(EyeRestReminder);
                AddSchedule(StandUpReminder);
            }
        }

        public static void AddSchedule(Schedule schedule)
        {
            if (schedule == null)
                return;

            SqliteDbHelper.SaveSchedule(schedule);
            _schedules.Add(schedule);
        }

        public static void UpdateSchedule(Schedule schedule)
        {
            if (schedule == null)
                return;

            if (schedule.Id <= 0)
            {
                AddSchedule(schedule);
            }
            Schedule scheduleToChange = _schedules.First(s => s.Id == schedule.Id);
            if (scheduleToChange != null)
            {
                SqliteDbHelper.SaveSchedule(schedule);
                scheduleToChange.Title = schedule.Title;
                scheduleToChange.NotificationMessage = schedule.NotificationMessage;
                scheduleToChange.NotifyMinutes = schedule.NotifyMinutes;
                scheduleToChange.CanDelete = schedule.CanDelete;
            }
            else
            {
                AddSchedule(schedule);
            }
        }

        public static void DeleteSchedule(Schedule schedule)
        {
            if (schedule == null)
                return;

            SqliteDbHelper.DeleteSchedule(schedule.Id);
            _schedules.Remove(_schedules.FirstOrDefault(s => s.Id == schedule.Id));
        }

        public static void Tick(uint idleTimeSeconds)
        {
            Debug.WriteLine(string.Format("You've been idle for {0} seconds", idleTimeSeconds));

            if (idleTimeSeconds > CommonConstants.AWAY_TIME_SECONDS)
            {
                if (IsUserActive)
                {
                    Mute(true);
                }
            }
            else if (!IsUserActive)
            {
                Mute(false);
            }

            if (IsUserActive)
            {
                ProcessSchedules();
            }
        }

        private static void Mute(bool value)
        {
            IsUserActive = !value;
            SystemTrayHelper.UpdateStatus(!value);

            if (!value)
            {
                ActiveStartUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (_schedules != null)
                {
                    foreach (var schedule in _schedules)
                    {
                        if (schedule == null)
                            continue;

                        schedule.WakeUp();
                    }
                }
            }
        }

        private static void ProcessSchedules()
        {
            if (_schedules == null)
                return;

            long currentUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            foreach (var schedule in _schedules)
            {
                if (schedule == null)
                    continue;

                if (schedule.ShouldNotify(currentUnixTimeSeconds))
                {
                    schedule.Notify();
                }
            }
        }
    }
}
