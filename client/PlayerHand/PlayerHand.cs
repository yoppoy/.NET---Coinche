using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using client.Card;

namespace client.PlayerHand
{
    public class PlayerHand
    {
        private readonly List<ACard> _hand;

        public PlayerHand()
        {
            _hand = new List<ACard>();
        }
        
        public void AddCard(ACard card)
        {
           _hand.Add(card);
        }

        public void Dump()
        {
            foreach (var card in _hand)
            {
                card.Dump();
            }
        }

        public int Size()
        {
            return _hand.Capacity;
        }

        public void DeleteHand()
        {
            _hand.Clear();
        }

        public ACard FindCard(int value, string color)
        {
            foreach (var card in _hand)
            {
                if (card.Color.Equals(color) && card.Value.Equals(value))
                    return card;
            }
            return null;
        }

        public bool DeleteCard(ACard cardToFind)
        {
            var index = 0;
            foreach (var card in _hand)
            {
                if (card.Color.Equals(cardToFind.Color) &&
                    card.Value.Equals(cardToFind.Value))
                {
                    _hand.RemoveAt(index);
                    return true;
                }
                index++;
            }
            return false;
        }

        public int GetSize()
        {
            return _hand.Count;
        }

        public bool DeleteCard(int nb)
        {
            _hand.RemoveAt(nb - 1);
            return true;
        }

        public ACard FindCard(int nb)
        {
            return _hand.ElementAt(nb - 1);
        }
    }
}