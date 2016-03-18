using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SubtitlesApp
{
    class SubtitleLoader : ISubtitleLoader
    {
        public List<Caption> LoadSubtitles(string path)
        {
            string extension = Path.GetExtension(path);
            return GetFormatMethod(extension)(readLines(path));
        }

        private string[] readLines(string path)
        {
            return File.ReadAllLines(path,Encoding.UTF8);
        }

        private Func<string[], List<Caption>> GetFormatMethod(string extension)
        {
            switch (extension)
            {
                case SubtitleFormats.Mpl2:
                    return LoadMpl2Subtitles;
                case SubtitleFormats.SubRip:
                    return LoadSrtSubtitles;
                default:
                    throw new UnsupportedFormatException();
            }
        }

        private List<Caption> LoadSrtSubtitles(string[] lines)
        {
            List<Caption> output = new List<Caption>();
            while (lines.Count() >= 3)
            {
                int count = Array.FindIndex(lines, s => s.Equals(""));
                var ls = lines.Take(count).ToArray();
                if (ls.Count() == 0) break;
                lines = lines.Skip(count+1).ToArray();
                output.Add(new Caption(ls));
            }
            foreach (var c in output)
            {
                Console.WriteLine("{0} - {1}: {2}", c.From, c.To, c.Content);
            }
            return output;
        }

        private List<Caption> LoadMpl2Subtitles(string[] lines)
        {
            List<Caption> output = new List<Caption>();
            lines = lines.Where(l => !l.Equals("")).ToArray();
            foreach (var l in lines)
            {
                output.Add(new Caption(l));
            }
            foreach (var c in output)
            {
                Console.WriteLine("{0} - {1}: {2}", c.From, c.To, c.Content);
            }
            return output;
        }

        private class SubtitleFormats
        {
            public const string Mpl2 = ".txt";
            public const string SubRip = ".srt";
        }
    }
    

    class UnsupportedFormatException : Exception
    {
        public UnsupportedFormatException() : base("This subtitle format is not supported.")
        {

        }
    }
}
