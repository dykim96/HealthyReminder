using HealthyReminder.Utils;
using System;

namespace HealthyReminder.Models
{
    public class Schedule
    {
        private long LastNotifiedUnixTimeSeconds;

        private long NotifySeconds;

        private uint _notifyMinutes;

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

        public int Id { get; set; } = 0;

        public bool IsDisabled { get; set; } = false;

        public bool CanDelete { get; set; } = true;

        public Schedule(string title, string notificationMessage, uint notifyMinutes)
        {
            Title = title;
            NotificationMessage = notificationMessage;
            NotifyMinutes = notifyMinutes;
            WakeUp();
        }

        public Schedule(Int64 id, string title, string notificationMessage, Int64 notifyMinutes, Int64 isDisabled, Int64 canDelete)
        {
            Id = (int)id;
            Title = title;
            NotificationMessage = notificationMessage;
            NotifyMinutes = (uint)notifyMinutes;
            IsDisabled = isDisabled == 0 ? false : true;
            CanDelete = canDelete == 0 ? false : true;
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
            SqliteDbHelper.SaveSchedule(this);
        }

        public void WakeUp()
        {
            LastNotifiedUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
