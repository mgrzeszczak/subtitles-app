using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesApp
{
    class Caption
    {
        public TimeSpan From { get; }
        public TimeSpan To { get; }
        public string Content { get; }

        public Caption(string mpl2)
        {
            string test = mpl2.Substring(1, mpl2.IndexOf("]")-1);
            int dsFrom = int.Parse(mpl2.Substring(1, mpl2.IndexOf("]")-1));
            mpl2 = mpl2.Substring(mpl2.IndexOf("[", 1) + 1);
            int dsTo = int.Parse(mpl2.Substring(0, mpl2.IndexOf("]")));
            Content = mpl2.Substring(mpl2.IndexOf("]") + 1).Replace("|", "\n");

            From = TimeSpan.FromMilliseconds(100 * dsFrom);
            To = TimeSpan.FromMilliseconds(100 * dsTo);
        }

        public Caption(string[] subrip)
        {
            Content = string.Join("\n", subrip.Skip(2));
            string[] times = subrip[1].Split(new string[] { " --> " },StringSplitOptions.None);
            From = SubripStringToTime(times[0]);
            To = SubripStringToTime(times[1]);
        }

        private TimeSpan SubripStringToTime(string s)
        {
            int hours = int.Parse(s.Substring(0, s.IndexOf(":")));
            s = s.Substring(s.IndexOf(":") + 1);
            int minutes = int.Parse(s.Substring(0, s.IndexOf(":")));
            s = s.Substring(s.IndexOf(":") + 1);
            int seconds = int.Parse(s.Substring(0, s.IndexOf(",")));
            s = s.Substring(s.IndexOf(",") + 1);
            int milliseconds = int.Parse(s);
            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }
    }
}
