namespace JokersAndMarbles;

public static class ExtensionMethods {
    // used to rank cards in hand to use or discard
    public static int Worth(this Rank rank) =>
        rank switch {
            Rank.Joker => 11,
            Rank.Ten => 10,
            Rank.Nine => 9,
            Rank.Eight => 7,
            Rank.Seven => 5,
            Rank.Ace => 3,
            Rank.King => 3,
            Rank.Queen => 2,
            Rank.Jack => 2,
            Rank.Two => 1,
            Rank.Three => 1,
            _ => 0
        };
}