using cf_contest_notify.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace cf_contest_notify
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ContestList list;
        private Stack<Contest> mys;
        private HashSet<string> is_ok;

        public MainPage()
        {
            this.InitializeComponent();
            mys = new Stack<Contest>();
            list = new ContestList();
            is_ok = new HashSet<string>();
            CreateStartupTask();
            Query();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Query();
        }

        private async void CreateStartupTask()
        {
            StartupTask startupTask = await StartupTask.GetAsync("01050265099"); // Pass the task ID you specified in the appxmanifest file
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    // Task is disabled but can be enabled.
                    StartupTaskState newState = await startupTask.RequestEnableAsync(); // ensure that you are on a UI thread when you call RequestEnableAsync()
                    Debug.WriteLine("Request to enable startup, result = {0}", newState);
                    break;
                case StartupTaskState.DisabledByUser:
                    // Task is disabled and user must enable it manually.
                    MessageDialog dialog = new MessageDialog(
                        "You have disabled this app's ability to run " +
                        "as soon as you sign in, but if you change your mind, " +
                        "you can enable this in the Startup tab in Task Manager.",
                        "TestStartup");
                    await dialog.ShowAsync();
                    break;
                case StartupTaskState.DisabledByPolicy:
                    Debug.WriteLine("Startup disabled by group policy, or not supported on this device");
                    break;
                case StartupTaskState.Enabled:
                    Debug.WriteLine("Startup is enabled.");
                    break;
            }
        }

        private async Task Query()
        {
            ContentDialog msg = new ContentDialog();

            msg.Title = "Request to codeforces...";
            msg.Content = new ProgressRing()
            {
                IsActive = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            msg.ShowAsync();

            DateTime now = DateTime.UtcNow;
            DateTime old = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            list.Clear();
            HttpClient httpClient = new HttpClient();
            Uri request;
            try
            {
                 request = new Uri("https://codeforces.com/api/contest.list?");
            }
            catch(Exception ex) {
                msg.Hide();
                await new MessageDialog(string.Format("{0}", ex.Message)).ShowAsync();
                return;
            }

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                httpResponse = await httpClient.GetAsync(request);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                msg.Hide();
                await new MessageDialog(string.Format("{0}", ex.Message)).ShowAsync();
                return; 
            }
            JsonObject jobject = new JsonObject();
            JsonArray result = new JsonArray();
            try
            {
                jobject = JsonObject.Parse(httpResponseBody);
                result = jobject["result"].GetArray();
            }
            catch(Exception ex)
            {
                msg.Hide();
                await new MessageDialog(string.Format("{0}", ex.Message)).ShowAsync();
                return;
            }

            foreach (JsonValue i in result)
            {
                Contest contest = JsonConvert.DeserializeObject<Contest>(i.ToString());
                if (now <= old.AddSeconds(contest.startTimeSeconds + contest.durationSeconds))
                {
                    contest.startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(contest.startTimeSeconds);
                    contest.startDate = contest.startDate.ToLocalTime();
                    contest.startDate_str = contest.startDate.ToString("MM dd HH:mm");
                    mys.Push(contest);
                }
                else break;
            }
            while (mys.Count != 0)
            {
                Contest contest = mys.Pop();
                list.Add(contest);
            }
            msg.Hide();

            if (list.Count==0)
            {
                await new MessageDialog("No Contest Available")
                {
                    Title = "-_-...."
                }.ShowAsync();
            }

        }

        private async void CreateBackgroundTask()
        {
            /*
            var builder = new BackgroundTaskBuilder();
            builder.Name = "My Background Trigger";
            builder.SetTrigger(new TimeTrigger(15, true));
            // Do not set builder.TaskEntryPoint for in-process background tasks
            // Here we register the task and work will start based on the time trigger.
            BackgroundTaskRegistration task = builder.Register();
            */
        }

        private void ViewContestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (FrameworkElement)sender;
            int idx = (item as ListView).SelectedIndex;
            if (idx >= 0)
            {
                ContentDetail.Navigate(typeof(ContestDetailPage), list[idx]);
            }
        }

        public static Rect GetElementRect(Windows.UI.Xaml.FrameworkElement element)
        {
            Windows.UI.Xaml.Media.GeneralTransform transform = element.TransformToVisual(null);
            Point point = transform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"http://www.codeforces.com");

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }
    }
}
