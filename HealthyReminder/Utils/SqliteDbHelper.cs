using Dapper;
using HealthyReminder.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyReminder.Utils
{
    class SqliteDbHelper
    {
        private const string SCHEDULE_FETCH = "SELECT Id, Title, Msg NotificationMessage, AlertInvlMnt NotifyMinutes, IsDisa IsDisabled, CanDel CanDelete"
            + " FROM Reminder ORDER BY Id";

        private const string SCHEDULE_INSERT = "INSERT INTO Reminder (Title, Msg, AlertInvlMnt, IsDisa, CanDel)"
            + " values (@Title, @NotificationMessage, @NotifyMinutes, @IsDisabled, @CanDelete)";

        private const string SCHEDULE_UPDATE = "UPDATE Reminder SET Title = @Title, Msg = @NotificationMessage,"
            + " AlertInvlMnt = @NotifyMinutes, IsDisa = @IsDisabled, CanDel = @CanDelete"
            + " WHERE Id = @Id";

        private const string SCHEDULE_DELETE = "DELETE FROM Reminder WHERE Id = ";

        private const string FETCH_LAST_INSERT_ID = "; SELECT last_insert_rowid();";

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static List<Schedule> LoadSchedules()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Schedule>(SCHEDULE_FETCH, new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveSchedule(Schedule schedule)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                if (schedule.Id <= 0)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        string sql = SCHEDULE_INSERT + FETCH_LAST_INSERT_ID;

                        cmd.Connection = cnn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@Title", schedule.Title);
                        cmd.Parameters.AddWithValue("@NotificationMessage", schedule.NotificationMessage);
                        cmd.Parameters.AddWithValue("@NotifyMinutes", schedule.NotifyMinutes);
                        cmd.Parameters.AddWithValue("@IsDisabled", schedule.IsDisabled);
                        cmd.Parameters.AddWithValue("@CanDelete", schedule.CanDelete);

                        cnn.Open();
                        var output = cmd.ExecuteScalar();
                        schedule.Id = (int)(long)output;
                    }
                }
                else
                {
                    cnn.Execute(SCHEDULE_UPDATE, schedule);
                }
            }
        }

        public static void DeleteSchedule(int id)
        {
            if (id <= 0)
                return;

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute(SCHEDULE_DELETE + id, new DynamicParameters());
            }
        }
    }
}
