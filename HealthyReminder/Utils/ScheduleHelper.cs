using HealthyReminder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HealthyReminder.Utils
{
    public class ScheduleHelper
    {
        private static List<Schedule> _schedules = Initialize();

        public static long ActiveStartUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public static bool IsUserActive { get; private set; } = true;

        public static List<Schedule> Schedules
        {
            get { return _schedules; }
        }

        private ScheduleHelper() { }

        private static List<Schedule> Initialize()
        {
            var schedules = new List<Schedule>();

            // TODO: try to connect to SQLite and read user settings
            // ReadConfigurations()

            if (schedules.Count <= 0)
            {
                Schedule waterReminder = new Schedule("Water Reminder",
                    "Are you staying hydrated?\nA general rule of thumb is to drink 2 to 3 cups of water per hour.",
                    120);
                schedules.Add(waterReminder);
                Schedule eyeRestReminder = new Schedule("Eye Rest Reminder",
                    "How about giving your eyes a little break?\nLook into the distance for 20 seconds to allow your eyes a chance to refocus.",
                    30);
                schedules.Add(eyeRestReminder);
                Schedule standUpReminder = new Schedule("Stand Up Reminder",
                    "Sitting too much?\nHow about a stretch, a little walk, or working while standing up?",
                    60);
                schedules.Add(standUpReminder);
            }

            return schedules;
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
