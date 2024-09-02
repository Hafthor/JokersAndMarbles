namespace JokersAndMarbles;

public class Card {
    public readonly Suit Suit;
    public readonly Rank Rank;

    public bool IsAceOrFace => Rank is Rank.Ace or >= Rank.Jack;
    public bool IsJoker => Rank is Rank.Joker;
    public bool CanSplit => Rank is Rank.Seven or Rank.Nine or Rank.Ten;
    public bool MustSplit => Rank is Rank.Ten;

    public Card(Suit suit, Rank rank) {
        Suit = suit;
        Rank = rank;
    }

    public Card(string str) {
        if (str[0] == 'J' && str[1] == 'o') {
            Suit = Suit.None;
            Rank = Rank.Joker;
        } else {
            if (char.IsDigit(str[0]))
                Rank = (Rank)(str[0] - '0');
            else
                Rank = Enum.GetValues<Rank>().First(r => r != Rank.Joker && r.ToString()[0] == str[0]);
            char f = char.ToUpper(str[1]);
            Suit = Enum.GetValues<Suit>().First(s => s != Suit.None && s.ToString()[0] == f);
        }
    }

    public override string ToString() => !IsJoker
        ? $"{(IsAceOrFace || Rank == Rank.Ten ? Rank.ToString()[..1] : ((int)Rank).ToString())}{char.ToLower(Suit.ToString()[0])}"
        : "Jo";

    public int Value => Rank == Rank.Eight ? -8 : (int)Rank;

    public int Worth => Rank switch {
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