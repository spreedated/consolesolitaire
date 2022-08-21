using neXn.Lib.Playingcards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleSolitaire.Classes
{
    internal class TableuPile
    {
        public bool IsEmpty
        {
            get
            {
                return !this.pile.Any();
            }
        }

        internal readonly Stack<Card> pile = new();

        public void Clear()
        {
            this.pile.Clear();
        }

        public void PushCard(Card card)
        {
            this.pile.Push(card);
        }

        public Card Peek()
        {
            var p = this.pile.FirstOrDefault(x => !x.Hidden);
            return p;
        }

        public Card PeekHighStack()
        {
            var p = this.pile.LastOrDefault(x => !x.Hidden);
            return p;
        }

        public IEnumerable<Card> PopCards()
        {
            Card[] x = this.pile.Where(x => !x.Hidden).ToArray();

            for (int i = 0; i < x.Length; i++)
            {
                this.pile.Pop();
            }

            if (this.pile.Any())
            {
                Card c = this.pile.Pop();
                c.Hidden = false;
                this.pile.Push(c);
            }

            return x;
        }

        public Card PopCard()
        {
            Card c = this.pile.Pop();

            if (this.pile.Any())
            {
                Card cc = this.pile.Pop();
                cc.Hidden = false;
                this.pile.Push(cc);
            }

            return c;
        }

        public string RenderPile()
        {
            Card[] p = this.pile.Reverse().ToArray();
            StringBuilder pileString = new();

            foreach (Card card in p)
            {
                if (p.ToList().IndexOf(card) + 1 == p.Length)
                {
                    pileString.Append(Frame.FrameFull(card, card.Color));
                    continue;
                }
                if (card.Hidden)
                {
                    pileString.Append(Frame.FramePartialHidden());
                }
                else
                {
                    pileString.Append(Frame.FramePartial(card, card.Color));
                }
            }

            return pileString.ToString();
        }
    }
}
