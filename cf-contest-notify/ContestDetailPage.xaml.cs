using cf_contest_notify.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace cf_contest_notify
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class ContestDetailPage : Page
    {
        Contest contest = new Contest();
        public ContestDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null) return;

            contest = e.Parameter as Contest;

            title.Text = contest.name;
            Dateo.Text = "Start Date";
            Date.Text = contest.startDate.ToString();

            int hours = contest.durationSeconds / 3600;
            int min = (contest.durationSeconds- hours*3600)/60;

            Durationo.Text = "Duration Time";
            Duration.Text = hours.ToString() + " hour " + min.ToString() + " min ";

            RemainDateo.Text = "Remain Date";
            TimeSpan diff = (DateTime.Now - contest.startDate).Negate();
            RemainDate.Text = string.Format("{0:%d} days {0:%h} hours {0:%m} minutes ", diff) + (diff<TimeSpan.Zero?"Passed":"Left");

            AppBarButton buttonForRegist = new AppBarButton();
            buttonForRegist.Icon = new SymbolIcon(Symbol.Flag);
            buttonForRegist.Click += Regist_click;
            buttonForRegist.Label = "Alarm Me!";

            panel.Children.Add(buttonForRegist);
            RelativePanel.SetAlignBottomWithPanel(buttonForRegist, true);
            RelativePanel.SetAlignRightWithPanel(buttonForRegist, true);
        }

        private async void Regist_click(object sender, RoutedEventArgs e )
        {
            var appointment = new Appointment();
            appointment.OnlineMeetingLink = "www.codeforces.com";
            appointment.Subject = contest.name;
            appointment.StartTime = contest.startDate;
            appointment.Reminder = TimeSpan.FromHours(1);

            var rect = MainPage.GetElementRect(sender as FrameworkElement);
            await AppointmentManager.ShowAddAppointmentAsync(appointment, rect, Windows.UI.Popups.Placement.Default);

        }
    }
}
