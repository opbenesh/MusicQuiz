using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicQuiz.GUI
{
    public class MusicFile
    {
        public TagLib.File File { get; set; }

        public double CueIn { get; set; }

        public double CueOut { get; set; }

        public double StartPosition { get; set; }

        public MusicFile(TagLib.File file)
        {
            this.File = file;
            MusicPlayer.SetStartPosition(this);
        }
    }
}
