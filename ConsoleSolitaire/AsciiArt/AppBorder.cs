using ConsoleSolitaire.Extensions;
using System.Text;

namespace ConsoleSolitaire.AsciiArt
{
    internal static class AppBorder
    {
        public static string Draw(int width = 144, int height = 80)
        {
            StringBuilder s = new();

            s.Append("@B.");

            int count = 0;

            for (int i = 0; i < width - 2; i++)
            {
                s.Append(count.IsEven() ? "-" : "=");
                count++;
            }

            s.Append('.');

            return s.ToString();
        }
    }
}
