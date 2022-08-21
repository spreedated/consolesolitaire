using neXn.Lib.Playingcards.Models;
using System.Text;
using static ConsoleSolitaire.Classes.PreloadedResources;

namespace ConsoleSolitaire.Classes
{
    internal static class Frame
    {
        public static string FramePartialHidden()
        {
            StringBuilder cardString = new();

            foreach (string line in HIDDENPARTIALCARD.Split('\n'))
            {
                if (line.Length <= 3)
                {
                    continue;
                }
                cardString.AppendLine("@W" + line.Trim('\r').Trim('\n'));
            }

            return cardString.ToString();
        }

        public static string FramePartial(Card c, Card.Colors color = Card.Colors.Black)
        {
            string card;
            StringBuilder cardString = new();

            foreach (string line in PARTIALCARD.Split('\n'))
            {
                if (line.Length <= 3)
                {
                    continue;
                }
                cardString.AppendLine((color == Card.Colors.Red ? "@R" : "@W") + line.Trim('\r').Trim('\n'));
            }

            card = cardString.ToString();

            card = card.Replace("{{VALUE}}", c.Value + " ");
            card = card.Replace("{{SUIT}}", c.Value.Length >= 2 ? Encoding.UTF8.GetString(c.UtfSymbol).ToString() : Encoding.UTF8.GetString(c.UtfSymbol).ToString() + " ");

            return card;
        }

        public static string FrameFull(Card c, Card.Colors color = Card.Colors.Black)
        {
            string card;

            StringBuilder cardString = new();

            foreach (string line in FULLCARD.Split('\n'))
            {
                if (line.Length <= 3)
                {
                    continue;
                }
                cardString.AppendLine((color == Card.Colors.Red ? "@R" : "@W") + line.Trim('\r').Trim('\n'));
            }

            card = cardString.ToString();

            card = card.Replace("{{VALUE}}", c.Value.Length >= 2 ? c.Value : c.Value + " ");
            card = card.Replace("{{VALUE2}}", c.Value.Length >= 2 ? c.Value : " " + c.Value);
            card = card.Replace("{{SUIT}}", Encoding.UTF8.GetString(c.UtfSymbol).ToString());

            return card;
        }
    }
}
