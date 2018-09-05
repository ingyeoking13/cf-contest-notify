using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace cf_contest_notify.Model
{
    class Contest
    {
        public int id { get; set; }
        public string name { get; set; }
        public enum type { CF, IOI, ICPC };
        public enum phase {  BEFORE, CODING, PENDING_SYSTEM_TEST, SYSTEM_TEST, FINISHED };
        public bool frozen { get; set; }
        public int durationSeconds { get; set; }
        public int startTimeSeconds { get; set; }
        public int relativeTimeSeconds { get; set; }
        public string preparedBy;
        public string websiteUrl;
        public string description;
        public DateTime startDate;
        public string startDate_str;
        public TimeSpan timeSpan;

        public void Click(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (Contest)e.ClickedItem;
        }
    }
}
