using ConsoleSolitaire.Classes;
using System.IO;

namespace ConsoleSolitaire.Models
{
    internal class SaveStateResponse
    {
        public string Filename { get; set; }
        public string Filepath
        {
            get
            {
                return Path.Combine(SaveGame.saveDirectory, $"{this.Filename}.{SaveGame.SAVESUFFIX}");
            }
        }
        public FileInfo FileInfo
        {
            get
            {
                return new(this.Filepath);
            }
        }
    }
}
