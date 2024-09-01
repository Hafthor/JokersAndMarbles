namespace JokersAndMarbles;

public class Card {
    public readonly Suit suit;
    public readonly Rank rank;

    public bool IsAceOrFace => rank is Rank.Ace or >= Rank.Jack;
    public bool IsJoker => rank is Rank.Joker;
    public bool CanSplit => rank is Rank.Seven or Rank.Nine or Rank.Ten;
    public bool MustSplit => rank is Rank.Ten;

    public Card(Suit suit, Rank rank) {
        this.suit = suit;
        this.rank = rank;
    }

    public Card(string str) {
        if (str[0] == 'J' && str[1] == 'o') {
            suit = Suit.None;
            rank = Rank.Joker;
        } else {
            if (char.IsDigit(str[0]))
                rank = (Rank)(str[0] - '0');
            else
                foreach (var v in Enum.GetValues<Rank>())
                    if (v != Rank.Joker && v.ToString()[0] == str[0]) {
                        rank = v;
                        break;
                    }
            char f = char.ToUpper(str[1]);
            foreach (var v in Enum.GetValues<Suit>())
                if (v != Suit.None && v.ToString()[0] == f) {
                    suit = v;
                    break;
                }
            if (rank == Rank.Joker || suit == Suit.None)
                throw new ArgumentException($"Invalid card {str}");
        }
    }

    public override string ToString() => !IsJoker
        ? $"{(IsAceOrFace || rank == Rank.Ten ? rank.ToString()[..1] : ((int)rank).ToString())}{char.ToLower(suit.ToString()[0])}"
        : "Jo";

    public int Value => rank == Rank.Eight ? -8 : (int)rank;

    public int Worth => rank switch {
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