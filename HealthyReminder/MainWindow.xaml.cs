using HealthyReminder.Utils;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using HealthyReminder.Pages;
using HealthyReminder.Models;

namespace HealthyReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static DispatcherTimer _timer;
        private static HomePage _homePage;
        private static SchedulePage _schedulePage;

        public MainWindow()
        {
            InitializeComponent();

            this.Content = new HealthyReminder.Pages.SplashScreen();
            SystemTrayHelper.SetShowEvent(ShowApp);

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _timer.Start();

            _homePage = new HomePage(NavigateToSchedulePage);
            _schedulePage = new SchedulePage(NavigateToHomePage);
            // Change to homePage 3 seconds later
            Task.Factory.StartNew(() => Thread.Sleep(2 * 1000))
            .ContinueWith((t) =>
            {
                this.Content = _homePage;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region Control Methods

        private void OnClose(object sender, CancelEventArgs args)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            args.Cancel = true;

            Hide();
            SystemTrayHelper.NotifyHide();
        }

        private void ShowApp(object sender, EventArgs e)
        {
            Show();
        }

        private void NavigateToHomePage()
        {
            _homePage.RefreshPage();
            this.Content = _homePage;
        }

        private void NavigateToSchedulePage(Schedule schedule)
        {
            _schedulePage.RefreshPage(schedule);
            this.Content = _schedulePage;
        }

        #endregion Control Methods

        private void Timer_Tick(object sender, object e)
        {
            uint idleTimeSeconds = WindowsIdleTimer.GetLastInputTime();
            _homePage.Tick();
            ScheduleHelper.Tick(idleTimeSeconds);
        }
    }
}
