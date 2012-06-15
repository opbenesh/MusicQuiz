using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicQuiz.GUI
{
    public enum Subject
    {
        Artist = 0,
        Album,
        Title
    }
    public class QuizDB
    {

        private List<TagLib.File> _files;
        private Random _random;

        public QuizDB()
        {
            _random = new Random();
            _files = new List<TagLib.File>();
            var musicDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            var files = Directory.GetFiles(musicDir, "*.mp3",SearchOption.AllDirectories);
            for (int i = 0; i < Constants.TRACKS_COUNT; )
            {
                try
                {
                    //TODO: HashSet                    
                    _files.Add(TagLib.File.Create(files[_random.Next(files.Length)]));
                    i++;
                }
                catch { continue;}
                Trace.WriteLine(string.Format("Scan progress: {0:p0} done.",(float)i/Constants.TRACKS_COUNT));
            }
        }

        public Question CreateNewQuestion()
        {
            var files = new List<TagLib.File>();
            for (int i = 0; i < Constants.OPTIONS_COUNT; i++)
            {
                files.Add(_files[_random.Next(_files.Count)]);
            }
            var subject = (Subject)_random.Next(Constants.SUBJECTS_COUNT);
            var rawOptions = files.Select(f=>f.Tag).Select(Constants.ProjectionBySubject[subject]);
            var options = rawOptions.Select(s => new Option() { Title = s, IsCorrect = false }).ToList();
            var title = Constants.QuestionsBySubject[subject];
            options[_random.Next(Constants.OPTIONS_COUNT)].IsCorrect = true;
            return new Question()
                {
                    Title=title,
                    Options=options
                };
        }
    }
}
