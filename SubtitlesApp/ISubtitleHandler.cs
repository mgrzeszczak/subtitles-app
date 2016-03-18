using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesApp
{
    interface ISubtitleHandler
    {
        void Start();
        void Stop();
        void Load(string path);
        void RegisterToEvents(Action<CaptionEvent> handler);
        void UpdateProgress(double progress);
    }
}
