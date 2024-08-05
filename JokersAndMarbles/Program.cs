namespace JokersAndMarbles;

class Program {
    static void Main(string[] args) {
        new Game(new Random(0)).Run();
    }
}

enum Suit {
    None = 0,
    Spades,
    Hearts,
    Clubs,
    Diamonds,
}

enum Rank {
    Joker = 0,
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
}

class Card {
    public Suit Suit { get; }
    public Rank Rank { get; }
    public bool IsAceOrFace => Rank is Rank.Ace or Rank.Jack or Rank.Queen or Rank.King;
    public bool IsJoker => Rank is Rank.Joker;
    public bool CanSplit => Rank is Rank.Seven or Rank.Nine or Rank.Ten;
    public bool MustSplit => Rank is Rank.Ten;

    public Card(Suit suit, Rank rank) {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString() => IsJoker
        ? "Jo"
        : (IsAceOrFace || Rank == Rank.Ten ? Rank.ToString()[..1] : ((int)Rank).ToString()) + "" +
          char.ToLower(Suit.ToString()[0]);

    public int Value => Rank == Rank.Eight ? -8 : (int)Rank;
}

class Deck {
    private List<Card> cards = [];

    public Deck(int decks = 3) {
        foreach (Suit suit in Enum.GetValues(typeof(Suit))) {
            if (suit != Suit.None) {
                foreach (Rank rank in Enum.GetValues(typeof(Rank))) {
                    if (rank != Rank.Joker) {
                        cards.Add(new Card(suit, rank));
                    }
                }
            }
        }
        for (int i = 0; i < 2; i++) {
            cards.Add(new Card(Suit.None, Rank.Joker));
        }
    }

    public Card Draw() {
        var card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }
    
    public void Return(Card card) {
        cards.Insert(0, card);
    }
    
    public void Shuffle(Random rnd) {
        for (int i = cards.Count - 1; i > 0; i--) {
            int j = rnd.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
}

class Marble(char letter, int player) {
    //    3031323334353637383940414243444546
    //29 o o o o o o o o o o o o o o o o o o o 47
    //28 x     o         o                   x 48
    //27 x     o         o                   x 49
    //26 x     o o o   o o o           x x x x 50
    //25 x                             x     x 51
    //24 x                             x     x 52
    //23 x                                   x 53
    //22 x                             x     x 54
    //21 x                             x x x x 55
    //20 x     x                       x     x 56
    //19 x x x x                             x 57
    //18 x     x                             x 58
    //17 x                                   x 59
    //16 x     x                             x 60
    //15 x     x          home 0  -1-2 safe  x 61
    //14 x x x x           o o o   o o o-3   x 62
    //13 x                   o         o-4   x 63
    //12 x                   o         o-5   x 64
    //11 x o o o o o o o o o o o o o o o o o o 65
    //    10 9 8 7 6 5 4 3 2 172717069686766
    public char Letter { get; } = letter;
    public int Player { get; } = player;
    public int Teammate => (Player + 2) % 4;
    public int Offset => Player * 18;
    public int Position { get; set; } // 0=home, 1-72=relative board position, -1 to -5=safe
    public int AbsPosition => Position > 0 ? (Position + Offset - 1) % 72 + 1 : Position;
    public bool IsHome => Position == 0;
    public bool IsSafe => Position < 0;
    public bool InLimbo => Position > 68; // will be 18*players - 4

    public string Move(int steps, bool hostile, List<Marble> sameColorMarbles) {
        if (hostile) {
            if (IsHome) {
                return "Can't move marble in home";
            } else if (IsSafe) {
                return "Cannot move safe marble";
            }
        } else if (steps < 0) {
            if (IsHome) {
                return "Can't back up marble in home";
            } else if (IsSafe) {
                return "Can't back up safe marble";
            }
        } else if (Position + steps > 73) {
            return "Can't move marble past home";
        }
        
        // teleport to location (Joker)
        if (sameColorMarbles == null) {
            Position = (Position + steps + 71) % 72 + 1;
            if (Position > 68) {
                Position -= 74;
            }
            return null;
        }

        int pos = Position, sign = Math.Sign(steps), abs = Math.Abs(steps);
        for (int i = 0; i < abs; i++) {
            if (sign > 0 && pos < 0) {
                pos += sign;
            } else {
                pos = (pos + 71 + sign) % 72 + 1;
            }
            if (sign > 0 && pos > 68) {
                pos -= 74;
            }
            if (sameColorMarbles.Any(m => m != this && m.Position == pos)) {
                return "Can't hop over same color marble";
            }
        }
        Position = pos;
        return null;
    }
}

class Player {
    public List<Card> Hand { get; } = new(3);
    public List<Marble> Marbles { get; } = new(5);

    public Player(int player, Deck deck, string letters) {
        for (int card = 0; card < 3; card++) {
            Hand.Add(deck.Draw());
        }
        foreach (char letter in letters) {
            Marbles.Add(new Marble(letter, player));
        }
    }
    
    public void Draw(Deck deck) {
        Hand.Add(deck.Draw());
    }

    public void Exchange(Deck deck, Card card) {
        Hand.Remove(card);
        deck.Return(card);
        Draw(deck);
    }
}

class Board {
    public List<Player> Players { get; } = new(4);
    public int Turn { get; set; } = 0;
    public int Teammate => (Turn + 2) % 4;
    public bool Win => Players[Turn].Marbles.All(m => m.IsSafe) && Players[Teammate].Marbles.All(m => m.IsSafe);
    public string[] info = new string[9];
    private readonly Deck deck;
    private char[][] board = """
                             ...................
                             .  j    t         .
                             .  i    s         .
                             .  hgf rqp     HIJ.
                             .              G  .
                             .   0          F  .
                             .   1             .
                             .   2          R  .
                             .   3          QST.
                             .  K4          P  .
                             .ONL5             .
                             .  M6             .
                             .   7             .
                             .  A8             .
                             .  B              .
                             .EDC     klm abc  .
                             .         n    d  .
                             .         o    e  .
                             ...................
                             """.Split('\n').Select(s => s.ToCharArray()).ToArray();
    private Dictionary<char, (int y, int x)> positionForChar = new();

    public Board(Deck deck) {
        this.deck = deck;
        var letters = new[] { "ABCDE", "abcde", "LMNOP", "lmnop" };
        for (var p = 0; p < letters.Length; p++) {
            Players.Add(new Player(p, deck, letters[p]));
        }
        for (int i = 0; i < 8; i++) {
            info[i] = "";
        }
        //         1234567890123456789
        info[8] = "Jokers and Marbles™";
        
        for (int r = 0; r < board.Length; r++) {
            var row = board[r];
            for (int c = 0; c < row.Length; c++) {
                if (row[c] is >='A' and <= 'z') {
                    positionForChar[row[c]] = (y:r, x:c);
                }
            }
        }
    }

    public void Print(string s) {
        while (s.Length > 0) {
            Array.Copy(info, 1, info, 0, 8);
            info[8] = s.PadRight(19)[..19];
            s = s.Length > 19 ? s[19..] : "";
        }
    }

    public void NextTurn() => Turn = (Turn + 1) % 4;

    public string Play(string sCmd) {
        // examples
        // Kd e - means play King of Diamonds, move marble e out of home or ahead 13 steps if not in home
        // Kd e13 - means play King of Diamonds, move marble e 13 steps forward
        // Kd e1 - means play King of Diamonds, move marble e out of home
        // Ts b5 C-5 - means play Ten of Spades, move marble b 5 steps forward, move marble C 5 steps back
        // Jo a - means play Joker, move marble a to opposite start
        // Jo aB - means play Joker, move marble a and click marble B
        // 6s x - means discard Six of Spades, draw new card
        string[] ss = sCmd.Split(' ');
        if (ss.Length < 2 || ss.Length > 3) {
            return "Bad command " + sCmd;
        }
        string[] moves = ss[1..];
        bool splitMove = moves.Length > 1;
        Card card = Players[Turn].Hand.Find(c => c.ToString() == ss[0]);
        List<List<Marble>> saveMarbles = null;
        if (card == null) {
            return "Bad card " + ss[0];
        } else if (!splitMove && moves[0] == "x") { // discard
            Players[Turn].Exchange(deck, card);
            return null;
        } else if (!splitMove && card.MustSplit) {
            return "Must give 2 moves";
        } else if (splitMove && !card.CanSplit) {
            return "Must give 1 move";
        }
        // save current state
        saveMarbles = Players
            .Select((p, i) => p.Marbles.Select(m => new Marble(m.Letter, i) { Position = m.Position }).ToList())
            .ToList();
        var allMarbles = Players.SelectMany(p => p.Marbles).ToList();
        var teamMarbles = Players[Turn].Marbles.Concat(Players[Teammate].Marbles).ToList();

        try {
            int prevMove = 0;
            Marble prevMarble = null;
            for (int moveIndex = 0; moveIndex < moves.Length; moveIndex++) {
                string move = moves[moveIndex];
                int curMove;
                List<Marble> marbles = card.Rank == Rank.Ten ? allMarbles : teamMarbles;
                Marble marble = marbles.FirstOrDefault(m => m.Letter == move[0], default);
                if (marble == null) {
                    return "Bad marble " + move[0];
                }
                bool hostile = card.Rank == Rank.Ten && teamMarbles.All(m => m.Letter != move[0]);
                if (move.Length == 1) { // infer move
                    if (card.Rank is Rank.Ten || card.Rank is Rank.Seven or Rank.Nine && splitMove && moveIndex == 0) {
                        return "Give distances for " + card.Rank;
                    } else {
                        curMove = card.Value;
                        if (marble.IsHome) {
                            if (card.IsAceOrFace) {
                                curMove = 1;
                            } else if (card.IsJoker) {
                                curMove = 18 * 2 + 1; // opposite start
                            } else {
                                return "Can't exit home w/ " + card.Rank;
                            }
                        }
                    }
                } else { // move specified
                    Marble click = card.Rank == Rank.Joker
                        ? Players.SelectMany(p => p.Marbles).FirstOrDefault(m => m.Letter == move[1])
                        : null;
                    if (click != null) {
                        if (click.IsHome) {
                            return "Can't click home marble";
                        } else if (click.IsSafe) {
                            return "Can't click safe marble";
                        } else if (click.Player == marble.Player) {
                            return "Can't click same color";
                        }
                        if (card.Rank == Rank.Joker && marble.Position == 0) marble.Position = 1; // move marble out of home
                        curMove = (72 + click.AbsPosition - marble.AbsPosition - 1) % 72 + 1;
                    } else {
                        if (!int.TryParse(move[1..], out curMove) || curMove == 0) {
                            return "Bad move " + move;
                        } else if (marble.IsHome && (curMove != 1 || !card.IsAceOrFace) && !card.IsJoker) {
                            return "Can't move marble out of home";
                        } else if (marble.IsSafe) {
                            if (Turn != marble.Player && Teammate != marble.Player) {
                                return "Can't move safe marble";
                            } else if (curMove < 0) {
                                return "Can't move safe marble backwards";
                            } else if (marble.Position + curMove > 0) {
                                return "Can't move marble past home";
                            }
                        }
                    }
                }

                if (marble.InLimbo && card.Rank != Rank.Ten && curMove >= 0) {
                    return "Marble in limbo";
                } else if (marble.IsHome && curMove != 1 && !card.IsJoker) {
                    return "Can't move marble out of home";
                } else if (!marble.IsHome && curMove != card.Value && !card.CanSplit && !card.IsJoker) {
                    return "Bad move " + move + " w/ " + card;
                }

                if (moveIndex > 0) {
                    bool isCorrectValue = Math.Abs(prevMove) + Math.Abs(curMove) == card.Value;
                    if (card.Rank == Rank.Ten && !isCorrectValue) {
                        return "Must be 10 steps";
                    } else if (card.Rank == Rank.Ten && prevMarble.Player == marble.Player) {
                        return "Must be 2 colors";
                    } else if (card.Rank == Rank.Seven && (curMove < 0 || prevMove < 0 || !isCorrectValue)) {
                        return "Must be 7 steps";
                    } else if (card.Rank == Rank.Nine && (curMove < 0 == prevMove < 0 || !isCorrectValue)) {
                        return "Must be 9 steps +/-";
                    }
                }

                string s = marble.Move(curMove, hostile, card.IsJoker ? null : Players[marble.Player].Marbles);
                if (s != null) {
                    return s;
                }

                // chain of clicks
                int pos = marble.AbsPosition;
                var clicked = allMarbles.FirstOrDefault(m => m.AbsPosition == pos && m != marble);
                if (clicked != null) {
                    if (clicked.Player == marble.Player) {
                        return "Can't click same color";
                    } else if (clicked.Player != marble.Teammate) {
                        clicked.Position = 0; // send marble home
                    } else {
                        clicked.Position = 68; // send marble to safe entry
                        pos = clicked.AbsPosition;
                        var clicked2 = allMarbles.FirstOrDefault(m => m.AbsPosition == pos && m != clicked);
                        if (clicked2 != null) {
                            if (clicked2.Player == clicked.Player) {
                                return "Can't click same color (2)";
                            } else if (clicked2.Player != clicked.Teammate) {
                                clicked2.Position = 0; // send marble home
                            } else {
                                clicked2.Position = 68; // send marble to safe entry
                                var clicked3 = allMarbles.FirstOrDefault(m =>
                                    m.AbsPosition == clicked2.AbsPosition && m != clicked2);
                                if (clicked3 != null) {
                                    clicked3.Position = 0; // send marble home - can only be hostile
                                }
                            }
                        }
                    }
                }

                prevMove = curMove;
                prevMarble = marble;
            }
            Players[Turn].Exchange(deck, card);
            saveMarbles = null; // prevent rollback
        } finally {
            if (saveMarbles != null) {
                for (int p = 0; p < Players.Count; p++) {
                    var player = Players[p].Marbles;
                    var savePlayer = saveMarbles[p];
                    for (int i = 0; i < player.Count; i++) {
                        player[i].Position = savePlayer[i].Position;
                    }
                }
            }
        }
        return null;
    }

    public void Paint() {
        for (int i = 0; i < 9; i++) {
            info[i] = info[i].PadRight(19)[..19];
        }
        string[] safes = ["abcde", "ABCDE", "fghij", "FGHIJ"];
        string[] homes = ["klmno", "KLMNO", "pqrst", "PQRST"];
        var board = this.board.Select(row => row.ToArray()).ToArray();
        foreach(var e in positionForChar) {
            var (y, x) = e.Value;
            board[y][x] = '.';
        }
        
        for (int i = 0; i < Players.Count; i++) {
            int offset = i * 18;
            var player = Players[i].Marbles;
            for (int j = 0; j < player.Count; j++) {
                int p = player[j].Position;
                char c = player[j].Letter;
                if (p == 0) { // Home
                    foreach (var s in homes[i]) {
                        if (FindAndSet(board, s, c)) {
                            break;
                        }
                    }
                } else if (p < 0) { // Safe
                    char s = safes[i][-p - 1];
                    FindAndSet(board, s, c);
                } else {
                    p = (p + offset + 61) % 72;
                    if (p < 18) {
                        board[18 - p][0] = c;
                    } else if (p < 36) {
                        board[0][p - 18] = c;
                    } else if (p < 54) {
                        board[p - 36][^1] = c;
                    } else {
                        board[^1][72 - p] = c;
                    }
                }
            }
        }

        foreach (var line in board) {
            for (var j = 0; j < line.Length; j++) {
                var c = line[j];
                if (c is >= '0' and <= '9') {
                    j += 9;
                    Console.Write(' ');
                    Console.Write(info[c - '0'].PadRight(19)[..19]);
                } else {
                    Console.Write(c);
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }

        bool FindAndSet(char[][] board, char find, char set) {
            if (!positionForChar.TryGetValue(find, out var xy) || board[xy.y][xy.x] != '.') {
                return false;
            }
            board[xy.y][xy.x] = set;
            return true;
        }
    }
}

class Game {
    private Board board;
    private Deck deck;

    public Game(Random rnd) {
        deck = new Deck();
        deck.Shuffle(rnd);
        board = new Board(deck);
    }

    public void Run() {
        while (true) {
            Console.Clear();
            board.Paint();
            Console.Write("Player " + board.Turn + " (" + string.Join(',', board.Players[board.Turn].Hand) + "):");
            string sCmd = Console.ReadLine();
            if (sCmd == "exit") {
                break;
            }
            string s = board.Play(sCmd);
            if (s != null) {
                board.Print(s);
            } else {
                if (board.Win) {
                    Console.WriteLine("Players " + board.Turn + " and " + board.Teammate + " win!");
                    break;
                }
                board.NextTurn();
            }
        }
    }
}