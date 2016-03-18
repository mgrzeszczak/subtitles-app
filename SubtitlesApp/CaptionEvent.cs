using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesApp
{
    class CaptionEvent
    {
        public CaptionEvent(Caption caption, TimeSpan time, double progress)
        {
            Caption = caption;
            Time = time;
            Progress = progress;
        }
        public double Progress { get; }
        public Caption Caption { get; }
        public TimeSpan Time { get; }
    }
}
