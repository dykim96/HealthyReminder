using HealthyReminder.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToggleSwitch;

namespace HealthyReminder.Models
{
    class ButtonTile
    {
        /*
        <Button HorizontalAlignment = "Stretch" HorizontalContentAlignment="Stretch" Margin="17,10,0,0" VerticalAlignment="Top" Height="50" BorderBrush="LightGray" BorderThickness="1px" Background="White">
            <Button.Resources>
                <Style TargetType = "Border" >
                    < Setter Property="CornerRadius" Value="10" />
                </Style>
            </Button.Resources>
            <Grid>
                < Grid.ColumnDefinitions >
                    < ColumnDefinition Width="*" />
                    <ColumnDefinition Width = "150" />
                    < ColumnDefinition Width="100" />
                </Grid.ColumnDefinitions>
                <Label Content = "Hello World" FontSize="26" VerticalAlignment="Center" />
                <Label Grid.Column="1" Content= "Every 999 min" FontSize= "20" VerticalAlignment= "Center" />
                < ToggleSwitch:HorizontalToggleSwitch Grid.Column= "2" IsChecked= "False" VerticalAlignment= "Center" />
            </ Grid >
        </ Button >
        */

        private const string REMINDER_FORMAT = "Every {0} min";

        private Label TitleLabel, ReminderTimeLabel;

        private ToggleSwitchBase ToggleSwitch;
        
        public Button TopButton { get; set; }

        public Schedule Schedule { get; private set; }

        public ButtonTile(Schedule schedule, double height, Thickness margin)
        {
            Schedule = schedule;

            TopButton = new Button();
            TopButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            TopButton.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            TopButton.VerticalAlignment = VerticalAlignment.Top;
            TopButton.Height = height;
            TopButton.Margin = margin;
            TopButton.Background = new SolidColorBrush(Colors.White);
            TopButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
            TopButton.BorderThickness = new Thickness(1);

            Grid grid = new Grid();
            grid.Margin = new Thickness(10, 0, 10, 0);
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(150);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(100);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);
            grid.ColumnDefinitions.Add(c3);

            TopButton.Content = grid;

            TitleLabel = new Label();
            TitleLabel.FontSize = 26;
            TitleLabel.Content = Schedule.Title;
            TitleLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(TitleLabel, 0);
            Grid.SetColumn(TitleLabel, 0);
            grid.Children.Add(TitleLabel);

            ReminderTimeLabel = new Label();
            ReminderTimeLabel.FontSize = 20;
            ReminderTimeLabel.Content = GetReminderMessage(Schedule);
            ReminderTimeLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(ReminderTimeLabel, 0);
            Grid.SetColumn(ReminderTimeLabel, 1);
            grid.Children.Add(ReminderTimeLabel);

            ToggleSwitch = new HorizontalToggleSwitch();
            ToggleSwitch.IsChecked = !schedule.IsDisabled;
            ToggleSwitch.VerticalAlignment = VerticalAlignment.Center;
            ToggleSwitch.Checked += ToggleSwitch_Check;
            ToggleSwitch.Unchecked += ToggleSwitch_Check;
            Grid.SetRow(ToggleSwitch, 0);
            Grid.SetColumn(ToggleSwitch, 2);
            grid.Children.Add(ToggleSwitch);
        }

        private string GetReminderMessage(Schedule schedule)
        {
            return string.Format(REMINDER_FORMAT, schedule.NotifyMinutes);
        }

        private void ToggleSwitch_Check(object sender, RoutedEventArgs e)
        {
            Schedule.Disable(!ToggleSwitch.IsChecked);
        }

    }
}
