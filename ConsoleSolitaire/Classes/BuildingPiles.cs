using neXn.Lib.Playingcards.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ConsoleSolitaire.Classes.PreloadedResources;

namespace ConsoleSolitaire.Classes
{
    internal class BuildingPile
    {
        public bool IsEmpty
        {
            get
            {
                return !this.pile.Any();
            }
        }

        public bool IsFull
        {
            get
            {
                return this.pile.Count >= 13;
            }
        }

        public Card.Colors? PileColor
        {
            get
            {
                if (this.IsEmpty)
                {
                    return null;
                }
                return this.pile.First().Color;
            }
        }

        public Card.Suits? PileSuit
        {
            get
            {
                if (this.IsEmpty)
                {
                    return null;
                }
                return this.pile.First().Suit;
            }
        }

        private readonly Stack<Card> pile = new();

        public void Clear()
        {
            this.pile.Clear();
        }

        public Card Peek()
        {
            return this.pile.Any() ? this.pile.Peek() : null;
        }

        public bool PushCard(Card card)
        {
            if (this.IsEmpty)
            {
                this.pile.Push(card);
                return true;
            }

            if (card.Suit == this.PileSuit && card.Numbervalue == this.pile.Peek().Numbervalue + 1)
            {
                this.pile.Push(card);
                return true;
            }

            return false;
        }

        public string RenderPile()
        {
            StringBuilder cardColor = new();

            for (int i = 0; i < FULLCARD.Split('\n').Length; i++)
            {
                cardColor.Append((this.PileColor == null || this.PileColor == Card.Colors.Black ? "@W" : "@R") + (this.IsEmpty ? BLANKCARD.Split('\n')[i] : FULLCARD.Split('\n')[i]) + '\n');
            }

            if (!this.IsEmpty)
            {
                cardColor.Replace("{{VALUE}}", this.pile.Peek().Value.Length >= 2 ? this.pile.Peek().Value : this.pile.Peek().Value + " ").Replace("{{SUIT}}", Encoding.UTF8.GetString(this.pile.Peek().UtfSymbol).ToString()).Replace("{{VALUE2}}", this.pile.Peek().Value.Length >= 2 ? this.pile.Peek().Value : " " + this.pile.Peek().Value);
            }

            return cardColor.ToString();
        }
    }
}
