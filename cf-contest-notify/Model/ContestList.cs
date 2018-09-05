using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cf_contest_notify.Model
{
    class ContestList : ObservableCollection<Contest>
    {
        public void erase_overTime()
        {

            DateTime now = DateTime.UtcNow;
            for(int i = this.Count-1; i>=0; i--)
            {
                DateTime old = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                if (now > old.AddSeconds(this[i].startTimeSeconds + this[i].durationSeconds))
                {
                    this.RemoveAt(i);
                }
            }
        }
    }
}
