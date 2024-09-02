namespace JokersAndMarbles;

public class Player {
    public readonly Card[] Hand;
    public readonly Marble[] Marbles;

    public Player(int player, Deck deck, string marbleLetters, int playerCount, int cards = 3) {
        Hand = new Card[cards];
        for (int card = 0; card < cards; card++)
            Hand[card] = deck.Draw();
        Marbles = new Marble[marbleLetters.Length];
        for (var i = 0; i < marbleLetters.Length; i++)
            Marbles[i] = new Marble(marbleLetters[i], player, playerCount);
    }

    public Card UseAndDraw(Deck deck, Card card) {
        for (int i = 0; i < Hand.Length; i++)
            if (Hand[i] == card)
                return Hand[i] = deck.UseAndDraw(card);
        throw new ArgumentException($"Card {card} not found in hand");
    }

    public int Score() => Marbles.Select(m => m.Score()).Sum();
}