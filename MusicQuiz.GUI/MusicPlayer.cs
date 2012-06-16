using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using System.IO;
using System.Diagnostics;

namespace MusicQuiz.GUI
{
    public class MusicPlayer:IDisposable
    {
        private bool _isDisposed = false;
        private static Random _random = new Random();
        private int _currentStream;

        public MusicPlayer()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        public void Play(MusicFile file)
        {
            if (_currentStream != 0)
                Stop();
            _currentStream = Bass.BASS_StreamCreateFile(file.File.Name, 0, 0, BASSFlag.BASS_DEFAULT);
            if (_currentStream == 0)
                throw new ApplicationException(string.Format("Could not play file {0}.", file.File.Name));
            Bass.BASS_ChannelSetPosition(_currentStream, file.StartPosition);
            Bass.BASS_ChannelPlay(_currentStream, false);
        }

        internal static void SetStartPosition(MusicFile file)
        {
            double cueIn = 0, cueOut = 0;
            int handle = Bass.BASS_StreamCreateFile(file.File.Name, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            if (handle == 0)
                throw new ApplicationException(string.Format("Could not open stream for file {0}.", file.File.Name));
            try
            {
                Utils.DetectCuePoints(handle, 1, ref cueIn, ref cueOut, -24, -24, 0);
                file.CueIn = cueIn;
                file.CueOut = cueOut;
            }
            catch
            {
                Debug.WriteLine(string.Format("Warning: Could not find cues for file {0}.", file.File.Name));
                file.CueIn = 0;
                file.CueOut = Un4seen.Bass.Bass.BASS_ChannelBytes2Seconds(handle, Bass.BASS_ChannelGetLength(handle));
            }
            double position = _random.NextDouble() * (cueOut - cueIn);
            file.StartPosition = position;
            Bass.BASS_StreamFree(handle);
        }

        public void Pause()
        {
            Bass.BASS_ChannelPause(_currentStream);
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
