using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SubtitlesApp
{
    class SubtitleHandler : ISubtitleHandler
    {
        private ISubtitleLoader loader = new SubtitleLoader();
        private List<Caption> captions;
        private event Action<CaptionEvent> eventDelegate = delegate { };
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        
        private Caption current;
        private DateTime beginTime;
        private TimeSpan subtitleTotalLength;
        private TimeSpan elapsed;
        
        public SubtitleHandler()
        {
            timer.Interval = 100;
            timer.Tick += TimerTick;
        }

        private void TimerTick(object o, EventArgs e)
        {
            TimeSpan diff = DateTime.Now - beginTime;
            elapsed = diff;
            
            foreach (var c in captions)
            {
                if (current != null && current.To < diff)
                {
                    current = null;
                }
                if (diff < c.From) break;
                if (diff>=c.From && diff <= c.To)
                {
                    current = c;                   
                }
            }
            eventDelegate(new CaptionEvent(current, diff,diff.TotalMilliseconds/subtitleTotalLength.TotalMilliseconds));
        }

        public void Load(string path)
        {
            if (timer.Enabled) timer.Stop();
            captions = loader.LoadSubtitles(path);
            elapsed = TimeSpan.FromMilliseconds(0);
            subtitleTotalLength = captions[captions.Count - 1].To;
            Start();
        }

        public void RegisterToEvents(Action<CaptionEvent> handler)
        {
            eventDelegate += handler;
        }

        public void Start()
        {
            if (captions == null) return;
            beginTime = (DateTime.Now - elapsed);
            timer.Start();
        }

        public void Stop()
        {
            if (timer.Enabled)
            timer.Stop();
        }

        public void UpdateProgress(double progress)
        {
            if (captions == null) return;
            elapsed = TimeSpan.FromMilliseconds(progress * subtitleTotalLength.TotalMilliseconds);
            beginTime = DateTime.Now - elapsed;
            current = null;
            TimerTick(null, null);
        }
    }
}
