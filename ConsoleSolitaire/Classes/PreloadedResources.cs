#pragma warning disable S2223

using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ConsoleSolitaire.EventArgs;

namespace ConsoleSolitaire.Classes
{
    internal static class PreloadedResources
    {
        public static string BLANKCARD = null;
        public static string FULLCARD = null;
        public static string HIDDENFULLCARD = null;
        public static string HIDDENPARTIALCARD = null;
        public static string PARTIALCARD = null;
        public static byte[] SND_LOADED = null;
        public static byte[] SND_BEEP = null;
        public static byte[] SND_LONGBEEP = null;
        public static byte[] SND_DENY = null;

        internal readonly static Dictionary<string,string> resources = new()
                                                                        {
                                                                            {"BLANKCARD", "blankcard.txt" },
                                                                            {"FULLCARD", "fullcard.txt" },
                                                                            {"HIDDENFULLCARD", "hiddenfullcard.txt" },
                                                                            {"HIDDENPARTIALCARD", "hiddenpartialcard.txt" },
                                                                            {"PARTIALCARD", "partialcard.txt" },
                                                                            {"SND_LOADED", "loaded.wav" },
                                                                            {"SND_BEEP", "beep.wav" },
                                                                            {"SND_LONGBEEP", "longbeep.wav" },
                                                                            {"SND_DENY", "deny.wav" }
                                                                        };

        internal static event EventHandler PreloadCompleted;
        internal static event EventHandler<ResourcePreloadedEventArgs> ResourcePreloaded;

        internal static void Preload()
        {
            Log.Information($"[Preload] Preloading {resources.Count} resources ...");

            Stopwatch sw = new();
            sw.Start();

            Assembly asm = Assembly.GetExecutingAssembly();

            foreach(KeyValuePair<string,string> p in resources)
            {
                FieldInfo fi = typeof(PreloadedResources).GetFields(BindingFlags.Static | BindingFlags.Public).First(x => x.Name == p.Key);
                if (fi == null)
                {
                    Log.Error($"[Preload] Field {p.Key} not found!");
                    continue;
                }

                using (Stream s = asm.GetManifestResourceStream(asm.GetName().Name + ".Resources." + p.Value))
                {
                    if (s == null)
                    {
                        Log.Error($"[Preload] Resource {p.Value} not found!");
                        continue;
                    }
                    using (StreamReader r = new(s))
                    {
                        switch (Path.GetExtension(p.Value).Substring(1))
                        {
                            case "txt":
                                fi.SetValue(fi, r.ReadToEnd().Replace("\r\n", "\n"));
                                break;
                            case "wav":
                            case "mp3":
                                byte[] buffer = new byte[r.BaseStream.Length];
                                r.BaseStream.Read(buffer, 0, (int)r.BaseStream.Length);
                                fi.SetValue(fi, buffer);
                                break;
                            default:
                                break;
                        }
                    }
                }

                int indexofitem = resources.ToList().IndexOf(p);

                ResourcePreloaded?.Invoke(null, new(p.Key));
                Log.Information($"[Preload][{indexofitem + 1}/{resources.Count}][{((indexofitem + 1) * 100) / (float)resources.Count:#}%] Resource \"{p.Value}\" successfully loaded");
            }
            sw.Stop();

            PreloadCompleted?.Invoke(null, System.EventArgs.Empty);

            Log.Information($"[Preload] All resources processed, {sw.Elapsed:mm\\:ss\\:ffffff}");
        }
    }
}
