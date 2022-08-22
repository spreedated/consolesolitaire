using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.Threading;
using static ConsoleSolitaire.Classes.PreloadedResources;

namespace ConsoleSolitaire.Classes
{
    internal class Sound
    {
        internal static void Play(byte[] soundbytes)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name = $"SoundPlay";
                Log.Information($"[Sound] Playing");
                
                using (WaveOutEvent w = new())
                {
                    w.PlaybackStopped += (s, e) =>
                                                {
                                                    Log.Information($"[Sound] Playback finished");
                                                };
                    w.Init(new WaveFileReader(new MemoryStream(soundbytes)));
                    w.Volume = 0.5f;
                    w.Play();

                    while (w.PlaybackState != PlaybackState.Stopped)
                    {
                        Thread.Sleep(50);
                    }
                }
            });
            Thread.CurrentThread.Name = $"SoundPlay";
            Log.Information($"[Sound] Playing");
        }
    }
}
