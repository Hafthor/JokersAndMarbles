namespace JokersAndMarbles;

public class Player {
    public List<Card> Hand { get; }
    public List<Marble> Marbles { get; } = new(5);

    public Player(int player, Deck deck, string marbleLetters, int cards = 3) {
        Hand = new(cards);
        for (int card = 0; card < cards; card++)
            Hand.Add(deck.Draw());
        foreach (char letter in marbleLetters)
            Marbles.Add(new Marble(letter, player));
    }

    public Card UseAndDraw(Deck deck, Card card) {
        Hand.Remove(card);
        card = deck.UseAndDraw(card);
        Hand.Add(card);
        return card;
    }

    public int Score() => Marbles.Select(m => m.Score()).Sum();
}