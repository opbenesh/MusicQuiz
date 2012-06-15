using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicQuiz.GUI
{
    public class QuizDB
    {
        enum Subject
        {
            Artist=0,
            Album,
            Title
        }
        private const int SUBJECTS_COUNT = 3;
        private const int TRACKS_COUNT = 100;
        private const int OPTIONS_COUNT = 4;

        private List<TagLib.File> _files;
        private Random _random;

        public QuizDB()
        {
            _files = new List<TagLib.File>();
            var musicDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            var files = Directory.GetFiles(musicDir, "*.mp3");
            for (int i = 0; i < TRACKS_COUNT;)
            {
                try
                {
                    _files.Add(TagLib.File.Create(files[TRACKS_COUNT]));
                    i++;
                }
                catch { }
            }
        }

        public Question CreateNewQuestion()
        {
            var files = new List<TagLib.File>();
            for (int i = 0; i < OPTIONS_COUNT; i++)
            {
                files.Add(_files[_random.Next(_files.Count)]);
            }
            var subject = (Subject)_random.Next(SUBJECTS_COUNT);
            List<string> rawOptions = null;
            string title= string.Empty;
            switch (subject)
            {
                case Subject.Artist:
                    rawOptions = files.Select(f => f.Tag.Performers[0]).ToList();
                    title = "Which artist perform this track?";
                    break;
                case Subject.Album:
                    rawOptions = files.Select(f => f.Tag.Album).ToList();
                    title = "Which album contains this track?";
                    break;
                case Subject.Title:
                    rawOptions = files.Select(f => f.Tag.Title).ToList();
                    title = "What is the title of this track?";
                    break;
            }
            var options = rawOptions.Select(s => new Option() { Title = s, IsCorrect = false }).ToList();
            options[_random.Next(OPTIONS_COUNT)].IsCorrect = true;
            return new Question()
                {
                    Title=title,
                    Options=options
                };
        }
    }
}
