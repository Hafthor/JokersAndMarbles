namespace JokersAndMarbles;

public class Deck {
    private readonly List<Card> cards;

    public Deck(int decks = 3, int jokersPerDeck = 2) {
        cards = new(decks * (52 + jokersPerDeck));
        for (int deck = 0; deck < decks; deck++) {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                if (suit != Suit.None)
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                        if (rank != Rank.Joker)
                            cards.Add(new Card(suit, rank));
            for (int joker = 0; joker < jokersPerDeck; joker++)
                cards.Add(new Card(Suit.None, Rank.Joker));
        }
    }

    public Card Draw() {
        Card card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }

    public Card UseAndDraw(Card card) {
        cards.Insert(0, card);
        card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }

    public List<Card> SaveCards() => [..cards];

    public void RestoreCards(List<Card> saveCards) {
        cards.Clear();
        cards.AddRange(saveCards);
    }

    public Deck Shuffle(int seed) { // Fisher-Yates shuffle
        Random rnd = new(seed);
        for (int i = cards.Count - 1; i > 0; i--) {
            int j = rnd.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
        return this;
    }
}