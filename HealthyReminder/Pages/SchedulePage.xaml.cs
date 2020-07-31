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

        private Schedule _destSchedule;

        private Action _showHomePageFunc;

        public void RefreshPage(Schedule schedule)
        {
            _destSchedule = schedule;
            if (schedule != null)
            {
                TitleTextBox.Text = _destSchedule.Title;
                NotificationTextBox.Text = _destSchedule.NotificationMessage;
                NumericTextBox.Text = _destSchedule.NotifyMinutes.ToString();
            }
            else
            {
                TitleTextBox.Text = "";
                NotificationTextBox.Text = "";
                NumericTextBox.Text = "5";
            }
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
            if (_destSchedule != null)
            {
                ScheduleHelper.Schedules.Remove(_destSchedule);
            }
            _showHomePageFunc();
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
            if (_destSchedule != null)
            {
                _destSchedule.Title = TitleTextBox.Text.Trim();
                _destSchedule.NotificationMessage = NotificationTextBox.Text.Trim();
                _destSchedule.NotifyMinutes = uint.Parse(NumericTextBox.Text);
            }
            else
            {
                _destSchedule = new Schedule(TitleTextBox.Text.Trim(),
                    NotificationTextBox.Text.Trim(), uint.Parse(NumericTextBox.Text));
                ScheduleHelper.Schedules.Add(_destSchedule);
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
