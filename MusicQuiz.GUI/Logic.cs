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
            var musicDir = GetMusicDir();
            var files = Directory.GetFiles(musicDir, "*.mp3",SearchOption.AllDirectories);
            for (int i = 0; i < Constants.TRACKS_COUNT; )
            {
                TagLib.File file;
                try
                {
                    //TODO: HashSet                    
                    file = TagLib.File.Create(files[_random.Next(files.Length)]);
                }
                catch { continue;}
                if (!CheckFileSanity(file))
                    continue;
                _files.Add(file);
                i++;
                Trace.WriteLine(string.Format("Scan progress: {0:p0} done.",(float)i/Constants.TRACKS_COUNT));
            }
        }

        private string GetMusicDir()
        {
            try
            {
                var lines = File.ReadAllLines("config.txt");
                if (Directory.Exists(lines[0]))
                    return lines[0];
            }
            catch
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        private bool CheckFileSanity(TagLib.File file)
        {
            var tag = file.Tag;
            return tag != null
                && file.Properties.Duration.TotalSeconds>Constants.MINIMUM_SECONDS
                && !string.IsNullOrWhiteSpace(tag.Album)
                && !string.IsNullOrWhiteSpace(tag.Title)
                && tag.Performers.Length > 0
                && !string.IsNullOrWhiteSpace(tag.Performers[0]);
        }

        private string Reencode(string source)
        {
            var ascii = Encoding.GetEncoding(1252);
            var hebrew = Encoding.GetEncoding(1255);
            var rawBytes = ascii.GetBytes(source);
            return hebrew.GetString(rawBytes);
        }

        public Question CreateNewQuestion()
        {
            var subject = (Subject)_random.Next(Constants.SUBJECTS_COUNT);
            Dictionary<string, TagLib.File> rawOptions = new Dictionary<string, TagLib.File>();
            while (rawOptions.Count < Constants.OPTIONS_COUNT)
            {
                var file = _files[_random.Next(_files.Count)];
                var option = Reencode(Constants.ProjectionBySubject[subject](file.Tag));
                if (rawOptions.ContainsKey(option))
                    continue;
                rawOptions[option] = file;
            }
            var options = rawOptions.Select(kvp => new Option() { Title = kvp.Key, File=kvp.Value, IsCorrect = false }).ToList();
            var title = Constants.QuestionsBySubject[subject];

            int answer = _random.Next(Constants.OPTIONS_COUNT);
            options[answer].IsCorrect = true;
            var musicFile = new MusicFile(options[answer].File);
            return new Question()
                {
                    Title=title,
                    Options=options,
                    File = musicFile
                };
        }
    }
}
