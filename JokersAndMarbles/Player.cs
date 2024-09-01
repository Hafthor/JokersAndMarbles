namespace JokersAndMarbles;

public class Player {
    public readonly Card[] hand;
    public readonly Marble[] marbles;

    public Player(int player, Deck deck, string marbleLetters, int cards = 3) {
        hand = new Card[cards];
        for (int card = 0; card < cards; card++)
            hand[card] = deck.Draw();
        marbles = new Marble[marbleLetters.Length];
        for (var i = 0; i < marbleLetters.Length; i++)
            marbles[i] = new Marble(marbleLetters[i], player);
    }

    public Card UseAndDraw(Deck deck, Card card) {
        for (int i = 0; i < hand.Length; i++)
            if (hand[i] == card)
                return hand[i] = deck.UseAndDraw(card);
        throw new ArgumentException($"Card {card} not found in hand");
    }

    public int Score() => marbles.Select(m => m.Score()).Sum();
}