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
                rank = Enum.GetValues<Rank>().First(r => r != Rank.Joker && r.ToString()[0] == str[0]);
            char f = char.ToUpper(str[1]);
            suit = Enum.GetValues<Suit>().First(s => s != Suit.None && s.ToString()[0] == f);
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