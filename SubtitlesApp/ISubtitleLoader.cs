using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesApp
{
    interface ISubtitleLoader
    {
        List<Caption> LoadSubtitles(string path);
    }
}
