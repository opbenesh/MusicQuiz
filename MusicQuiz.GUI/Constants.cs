using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicQuiz.GUI
{
    public static class Constants
    {
        public const int SUBJECTS_COUNT = 3;
        public const int TRACKS_COUNT = 100;
        public const int OPTIONS_COUNT = 4;
        public const int THRESHOLD = 500;
        public const int MINIMUM_SECONDS = 60;

        public static readonly Dictionary<Subject, string> QuestionsBySubject = new Dictionary<Subject, string>()
        {
            {Subject.Artist,"Which artist perform this track?"},
            {Subject.Album, "Which album contains this track?"},
            {Subject.Title,"What is the title of this track?"},
        };

        public static readonly Dictionary<Subject, Func<TagLib.Tag,string>> ProjectionBySubject = new Dictionary<Subject, Func<TagLib.Tag,string>>()
        {
            {Subject.Artist,t=>t.Performers[0]},
            {Subject.Album, t=>t.Album},
            {Subject.Title, t=>t.Title},
        };
    }
}
