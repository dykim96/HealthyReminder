using HealthyReminder.Utils;
using System;

namespace HealthyReminder.Models
{
    public class Schedule
    {
        private long LastNotifiedUnixTimeSeconds;

        private long NotifySeconds;

        private uint _notifyMinutes;

        public bool IsDisabled { get; private set; } = false;

        public string Title { get; set; }

        public string NotificationMessage { get; set; }

        public uint NotifyMinutes
        {
            get { return _notifyMinutes; }
            set
            {
                _notifyMinutes = value;
                NotifySeconds = CommonConstants.SECONDS_IN_MINUTE * (long)value;
            }
        }

        public Schedule(string title, string notificationMessage, uint notifyMinutes)
        {
            Title = title;
            NotificationMessage = notificationMessage;
            NotifyMinutes = notifyMinutes;
            WakeUp();
        }

        public bool ShouldNotify(long currentUnixTimeSeconds)
        {
            if (IsDisabled)
                return false;

            long differenceUnixTimeSeconds = currentUnixTimeSeconds - LastNotifiedUnixTimeSeconds;

            if (differenceUnixTimeSeconds < CommonConstants.SECONDS_IN_MINUTE)
                return false;

            return differenceUnixTimeSeconds >= (long)NotifySeconds;
        }

        public void Notify()
        {
            SystemTrayHelper.Notify(Title, NotificationMessage, 1000);
            LastNotifiedUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public void Disable(bool value)
        {
            IsDisabled = value;
            if (!value)
            {
                WakeUp();
            }
        }

        public void WakeUp()
        {
            LastNotifiedUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
