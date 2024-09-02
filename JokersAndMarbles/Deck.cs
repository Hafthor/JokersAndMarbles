namespace JokersAndMarbles;

public class Deck {
    private readonly List<Card> _cards;

    public Deck(int decks = 3, int jokersPerDeck = 2) {
        _cards = new(decks * (52 + jokersPerDeck));
        for (int deck = 0; deck < decks; deck++) {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                if (suit != Suit.None)
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                        if (rank != Rank.Joker)
                            _cards.Add(new Card(suit, rank));
            for (int joker = 0; joker < jokersPerDeck; joker++)
                _cards.Add(new Card(Suit.None, Rank.Joker));
        }
    }

    public Card Draw() {
        Card card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    public Card UseAndDraw(Card card) {
        _cards.Insert(0, card);
        card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    public List<Card> SaveCards() => new(_cards);

    public void RestoreCards(List<Card> saveCards) {
        _cards.Clear();
        _cards.AddRange(saveCards);
    }

    public Deck Shuffle(Random rnd) { // Fisher-Yates shuffle
        for (int i = _cards.Count - 1; i > 0; i--) {
            int j = rnd.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
        return this;
    }
}