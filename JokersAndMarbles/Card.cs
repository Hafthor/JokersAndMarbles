namespace JokersAndMarbles;

public class Card {
    public Suit Suit { get; }
    public Rank Rank { get; }

    public bool IsAceOrFace => Rank is Rank.Ace or >= Rank.Jack;
    public bool IsJoker => Rank is Rank.Joker;
    public bool CanSplit => Rank is Rank.Seven or Rank.Nine or Rank.Ten;
    public bool MustSplit => Rank is Rank.Ten;

    public Card(Suit suit, Rank rank) {
        Suit = suit;
        Rank = rank;
    }

    public Card(string str) {
        Suit = Suit.None;
        Rank = Rank.Joker;
        if (str[0] != 'J' || str[1] != 'o') {
            if (char.IsDigit(str[0])) {
                Rank = (Rank)(str[1] - '0');
            } else {
                foreach (var v in Enum.GetValues<Rank>()) {
                    if (v != Rank.Joker && v.ToString()[0] == str[0]) {
                        Rank = v;
                        break;
                    }
                }
            }
            char f = char.ToUpper(str[1]);
            foreach(var v in Enum.GetValues<Suit>()) {
                if (v != Suit.None && v.ToString()[0] == f) {
                    Suit = v;
                    break;
                }
            }
            if (Rank == Rank.Joker || Suit == Suit.None) {
                throw new ArgumentException("Invalid card " + str);
            }
        }
    }

    public override string ToString() => !IsJoker
        ? (IsAceOrFace || Rank == Rank.Ten ? Rank.ToString()[..1] : ((int)Rank).ToString())
          + "" + char.ToLower(Suit.ToString()[0])
        : "Jo";

    public int Value => Rank == Rank.Eight ? -8 : (int)Rank;
}