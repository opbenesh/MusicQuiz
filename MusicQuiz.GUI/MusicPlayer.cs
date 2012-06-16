using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass;
using System.IO;

namespace MusicQuiz.GUI
{
    public class MusicPlayer:IDisposable
    {
        private bool _isDisposed = false;
        private Random _random = new Random();
        private int _currentStream;

        public MusicPlayer()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        public void Play(TagLib.File file)
        {
            if (_currentStream != 0)
                Stop();
            _currentStream = Bass.BASS_StreamCreateFile(file.Name, 0, 0, BASSFlag.BASS_DEFAULT);
            if (_currentStream == 0)
                throw new ApplicationException(string.Format("Could not play file {0}.", file.Name));
            long position = _random.NextLong(Bass.BASS_ChannelGetLength(_currentStream)*3/4);
            Bass.BASS_ChannelSetPosition(_currentStream, position);
            Bass.BASS_ChannelPlay(_currentStream, false);
        }

        public void Pause()
        {
        }

        public void Stop()
        {
            if(_currentStream==0)
                return;
            Bass.BASS_StreamFree(_currentStream);
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if(disposing)
                {
                }
                Bass.BASS_Free();
                _isDisposed = true;

            }
        }

        ~MusicPlayer()
        {
            Dispose(false);
        }
    }
}
