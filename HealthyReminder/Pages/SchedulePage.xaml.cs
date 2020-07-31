using HealthyReminder.Models;
using HealthyReminder.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HealthyReminder.Pages
{
    /// <summary>
    /// Interaction logic for SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page
    {
        private const int MIN_NUMBER = 1;
        private const int MAX_NUMBER = 999;

        private Schedule _schedule;

        private Action _showHomePageFunc;

        public void RefreshPage(Schedule schedule)
        {
            if (schedule != null)
            {

                _schedule = new Schedule(schedule.Title, schedule.NotificationMessage, schedule.NotifyMinutes);
                _schedule.Id = schedule.Id;
                _schedule.IsDisabled = schedule.IsDisabled;
                _schedule.CanDelete = schedule.CanDelete;
                if (_schedule.CanDelete)
                {
                    DeleteBtn.Visibility = Visibility.Visible;
                    DefaultBtn.Visibility = Visibility.Hidden;
                }
                else
                {
                    DeleteBtn.Visibility = Visibility.Hidden;
                    DefaultBtn.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _schedule = new Schedule("", "", 5);
                DeleteBtn.Visibility = Visibility.Visible;
                DefaultBtn.Visibility = Visibility.Hidden;
            }
            TitleTextBox.Text = _schedule.Title + "";
            NotificationTextBox.Text = _schedule.NotificationMessage + "";
            NumericTextBox.Text = _schedule.NotifyMinutes.ToString();
        }

        public SchedulePage(Action showHomePageFunc)
        {
            InitializeComponent();
            _showHomePageFunc = showHomePageFunc;
            RefreshPage(null);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            _showHomePageFunc();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            ScheduleHelper.DeleteSchedule(_schedule);
            _showHomePageFunc();
        }

        private void DefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            Schedule schedule = null;
            switch(_schedule.Id)
            {
                case 1:
                    schedule = ScheduleHelper.WaterReminder;
                    break;
                case 2:
                    schedule = ScheduleHelper.EyeRestReminder;
                    break;
                case 3:
                    schedule = ScheduleHelper.StandUpReminder;
                    break;
                default:
                    break;
            }
            if (schedule != null)
            {
                TitleTextBox.Text = schedule.Title;
                NotificationTextBox.Text = schedule.NotificationMessage;
                NumericTextBox.Text = schedule.NotifyMinutes.ToString();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Reminder Title Can't Be Empty!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NotificationTextBox.Text))
            {
                MessageBox.Show("Notification Message Can't Be Empty!");
                return;
            }
            if (IsNumericTextBoxEmptyOrZero())
            {
                MessageBox.Show("Notification Time Can't Be Zero or Empty!");
                return;
            }

            _schedule.Title = TitleTextBox.Text.Trim();
            _schedule.NotificationMessage = NotificationTextBox.Text.Trim();
            _schedule.NotifyMinutes = uint.Parse(NumericTextBox.Text);
            if (_schedule.Id > 0)
            {
                ScheduleHelper.UpdateSchedule(_schedule);
            }
            else
            {
                ScheduleHelper.AddSchedule(_schedule);
            }
            _showHomePageFunc();
        }

        private void NumericUpBtn_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (!int.TryParse(NumericTextBox.Text, out number) || number == MAX_NUMBER)
            {
                return;
            }

            if (number > MAX_NUMBER)
            {
                number = MAX_NUMBER;
            }
            else if (number < MAX_NUMBER)
            {
                number++;
            }
            NumericTextBox.Text = number.ToString();
        }

        private void NumericDownBtn_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (!int.TryParse(NumericTextBox.Text, out number) || number == MIN_NUMBER)
            {
                return;
            }

            if (number < MIN_NUMBER)
            {
                number = MIN_NUMBER;
            }
            else if (number > MIN_NUMBER)
            {
                number--;
            }
            NumericTextBox.Text = number.ToString();
        }

        private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Key input = e.Key;
            if (Key.Up == input)
            {
                NumericUpBtn_Click(sender, null);
                e.Handled = true;
                NumericTextBox.CaretIndex = NumericTextBox.Text.Length;
            }
            else if (Key.Down == input)
            {
                NumericDownBtn_Click(sender, null);
                e.Handled = true;
                NumericTextBox.CaretIndex = NumericTextBox.Text.Length;
            }
            else if (Key.D0 == input || Key.NumPad0 == input)
            {
                if (IsNumericTextBoxEmptyOrZero())
                {
                    e.Handled = true;
                }
            }
            else if (Key.D1 <= input && input <= Key.D9
                || Key.NumPad1 <= input && input <= Key.NumPad9
                || Key.Back == input || Key.Left == input || Key.Right == input)
            {
                // Do nothing
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                && (input == Key.A || input == Key.C))
            {
                // Do nothing
            }
            else
            {
                e.Handled = true;
            }
        }

        private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsNumericTextBoxEmptyOrZero())
            {
                NumericTextBox.Text = MIN_NUMBER.ToString();
                NumericTextBox.CaretIndex = NumericTextBox.Text.Length;
            }
        }

        private bool IsNumericTextBoxEmptyOrZero()
        {
            return NumericTextBox.Text.Length == 0 || NumericTextBox.Text == "0";
        }
    }
}
