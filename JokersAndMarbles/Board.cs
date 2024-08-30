namespace JokersAndMarbles;

public class Board {
    public List<Player> Players { get; } = new(4);
    public int Turn { get; set; } = 0;

    public int Teammate => (Turn + 2) % 4;
    public bool Win => Players[Turn].Marbles.All(m => m.IsSafe) && Players[Teammate].Marbles.All(m => m.IsSafe);

    private string[] info = new string[9];
    private readonly Deck deck;

    //29  3031323334353637383940414243444546   47
    //   o o o o o o o o o o o o o o o o o o o
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
    //   x o o o o o o o o o o o o o o o o o o
    //11  10 9 8 7 6 5 4 3 2 172717069686766   65
    private char[][] board = """
                             ..*.....*.....*....
                             .  j    p         .
                             .  i    q         *
                             .  hgf tsr     HIJ.
                             *              G  .
                             .   $          F  .
                             .   $             .
                             .   $          T  .
                             .   $          SQP*
                             .  M$          R  .
                             *KLN$             .
                             .  O$             .
                             .   $             .
                             .  A$             .
                             .  B              *
                             .EDC     mno abc  .
                             *         l    d  .
                             .         k    e  .
                             ....*.....*.....*..
                             """.Split('\n').Select(s => s.ToCharArray()).ToArray();

    private Dictionary<char, (int y, int x)> positionForChar = new(5 * 2 * 4 + 9);

    public Board(Deck deck) {
        this.deck = deck;
        var marbleLetters = new[] { "ABCDE", "abcde", "FGHIJ", "fghij" };
        for (var p = 0; p < marbleLetters.Length; p++) {
            Players.Add(new Player(p, deck, marbleLetters[p]));
        }
        for (int i = 0; i < 8; i++) {
            info[i] = "";
        }
        //     1234567890123456789
        Print("Jokers and Marbles");

        for (int r = 0; r < board.Length; r++) {
            var row = board[r];
            for (int c = 0; c < row.Length; c++) {
                if (row[c] is >= 'A' and <= 'T' or >= 'a' and <= 't') {
                    positionForChar[row[c]] = (y: r, x: c);
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

    public List<Marble> AllMarbles() => Players.SelectMany(p => p.Marbles).ToList();

    public List<Marble> TeamMarbles() => Players[Turn].Marbles.Concat(Players[Teammate].Marbles).ToList();


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
            return $"Bad command {sCmd}";
        }
        string[] moves = ss[1..];
        bool splitMove = moves.Length > 1;
        Card card = Players[Turn].Hand.FirstOrDefault(c => c.ToString() == ss[0], default);
        List<List<Marble>> saveMarbles = null;
        if (card == null) {
            return $"Bad card {ss[0]}";
        } else if (!splitMove && moves[0] == "x") { // discard
            Players[Turn].UseAndDraw(deck, card);
            return null;
        } else if (!splitMove && card.MustSplit) {
            return "Requires 2 moves";
        } else if (splitMove && !card.CanSplit) {
            return "Only 1 move";
        }
        // save current state
        saveMarbles = SaveMarbles();
        var allMarbles = AllMarbles();
        var teamMarbles = TeamMarbles();

        try {
            int prevMove = 0;
            Marble prevMarble = null;
            for (int moveIndex = 0; moveIndex < moves.Length; moveIndex++) {
                string move = moves[moveIndex];
                int curMove;
                List<Marble> marbles = card.Rank == Rank.Ten ? allMarbles : teamMarbles;
                Marble marble = marbles.FirstOrDefault(m => m.Letter == move[0], default);
                if (marble == null) {
                    return $"Bad marble ${move[0]}";
                }
                bool hostile = card.Rank == Rank.Ten && teamMarbles.All(m => m.Letter != move[0]);
                if (move.Length == 1) { // infer move
                    if (card.Rank is Rank.Ten || card.Rank is Rank.Seven or Rank.Nine && splitMove && moveIndex == 0) {
                        return $"Give distances for {card.Rank}";
                    } else {
                        curMove = card.Value;
                        if (marble.IsHome) {
                            if (card.IsAceOrFace) {
                                curMove = 1;
                            } else if (card.IsJoker) {
                                curMove = 18 * 2 + 1; // opposite start
                            } else {
                                return $"Can't exit home w/ {card.Rank}";
                            }
                        }
                    }
                } else { // move specified
                    Marble click = card.Rank != Rank.Joker
                        ? null
                        : Players.SelectMany(p => p.Marbles).FirstOrDefault(m => m.Letter == move[1], default);
                    if (click != null) {
                        if (click.IsHome) {
                            return "Can't click home marble";
                        } else if (click.IsSafe) {
                            return "Can't click safe marble";
                        } else if (click.Player == marble.Player) {
                            return "Can't click same color";
                        }
                        if (card.Rank == Rank.Joker && marble.Position == 0) {
                            marble.Position = 1; // move marble out of home
                        }
                        curMove = (72 + click.AbsPosition - marble.AbsPosition - 1) % 72 + 1;
                    } else {
                        if (!int.TryParse(move[1..], out curMove) || curMove == 0) {
                            return $"Bad move ${move}";
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
                    return $"Bad move {move} w/ {card}";
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
                var clicked = allMarbles.FirstOrDefault(m => m.AbsPosition == pos && m != marble && !m.IsSafe);
                if (clicked != null) {
                    if (clicked.Player == marble.Player) {
                        return "Can't click same color";
                    } else if (clicked.Player != marble.Teammate) {
                        clicked.Position = 0; // send marble home
                    } else {
                        clicked.Position = 68; // send marble to safe entry
                        pos = clicked.AbsPosition;
                        var clicked2 =
                            allMarbles.FirstOrDefault(m => m.AbsPosition == pos && m != clicked && !m.IsSafe);
                        if (clicked2 != null) {
                            if (clicked2.Player == clicked.Player) {
                                return "Can't click same color (2)";
                            } else if (clicked2.Player != clicked.Teammate) {
                                clicked2.Position = 0; // send marble home
                            } else {
                                clicked2.Position = 68; // send marble to safe entry
                                var clicked3 = allMarbles.FirstOrDefault(m =>
                                    m.AbsPosition == clicked2.AbsPosition && m != clicked2 && !m.IsSafe);
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
            Players[Turn].UseAndDraw(deck, card);
            saveMarbles = null; // prevent rollback
        } finally {
            if (saveMarbles != null) {
                RestoreMarbles(saveMarbles);
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
        foreach (var e in positionForChar) {
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

        int il = 0;
        foreach (var line in board) {
            for (var j = 0; j < line.Length; j++) {
                var c = line[j];
                if (c == '$') {
                    j += 9;
                    Console.Write(' ');
                    Console.Write(info[il++].PadRight(19)[..19]);
                } else {
                    Console.Write(c);
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }

        bool FindAndSet(char[][] board, char find, char set) {
            if (positionForChar.TryGetValue(find, out var xy) && board[xy.y][xy.x] == '.') {
                board[xy.y][xy.x] = set;
                return true;
            }
            return false;
        }
    }

    public int Score(int player) =>
        Players[player].Score() + Players[(player + 2) % 4].Score() + HomeImbalance(player)
        - Players[(player + 1) % 4].Score() - Players[(player + 3) % 4].Score();

    public int HomeImbalance(int player) {
        int c1 = Players[player].Marbles.Count(m => m.IsHome);
        int c2 = Players[(player + 2) % 4].Marbles.Count(m => m.IsHome);
        int c3 = Players[(player + 1) % 4].Marbles.Count(m => m.IsHome);
        int c4 = Players[(player + 3) % 4].Marbles.Count(m => m.IsHome);
        return -Math.Abs(c1 - c2) + Math.Abs(c3 - c4);
    }

    public int Score() => Score(Turn);

    public string Hand() => string.Join(',', Players[Turn].Hand);

    public List<List<Marble>> SaveMarbles() =>
        Players.Select((p, i) => p.Marbles.Select(m => new Marble(m.Letter, i) { Position = m.Position }).ToList())
            .ToList();

    public void RestoreMarbles(List<List<Marble>> saveMarbles) {
        for (int p = 0; p < Players.Count; p++) {
            var player = Players[p].Marbles;
            var savePlayer = saveMarbles[p];
            for (int i = 0; i < player.Count; i++) {
                player[i].Position = savePlayer[i].Position;
            }
        }
    }

    public IEnumerable<string> PossiblePlays() { // not including discard
        var allMarbles = AllMarbles();
        var teamMarbles = TeamMarbles();
        foreach (var card in Players[Turn].Hand) {
            if (card.CanSplit) {
                if (card.Rank == Rank.Ten) { // must split
                    int mi1 = 0;
                    foreach (var m1 in allMarbles) {
                        foreach (var m2 in allMarbles.Skip(++mi1)) {
                            if (m1.Player == m2.Player) continue;
                            for (int i = 1; i <= 9; i++) {
                                yield return $"{card} {m1.Letter}{i} {m2.Letter}{10 - i}";
                                yield return $"{card} {m1.Letter}{-i} {m2.Letter}{10 - i}";
                                yield return $"{card} {m1.Letter}{i} {m2.Letter}{-(10 - i)}";
                                yield return $"{card} {m1.Letter}{-i} {m2.Letter}{-(10 - i)}";
                            }
                        }
                    }
                } else {
                    int mi1 = 0;
                    foreach (var m1 in teamMarbles) {
                        foreach (var m2 in teamMarbles.Skip(++mi1)) {
                            for (int i = 1; i < (int)card.Rank; i++) {
                                string prefix = $"{card} {m1.Letter}{i} {m2.Letter}";
                                if (card.Rank == Rank.Seven) {
                                    yield return $"{prefix}{7 - i}";
                                } else {
                                    yield return $"{prefix}{-(9 - i)}";
                                }
                            }
                        }
                    }
                }
            }
            if (card.IsJoker) {
                foreach (var m1 in allMarbles) {
                    foreach (var m2 in allMarbles) {
                        if (m1.Player != m2.Player) continue;
                        yield return $"{card} {m1.Letter}{m2.Letter}";
                    }
                }
            }
            if (!card.MustSplit && !card.IsJoker) {
                foreach (var m in teamMarbles) {
                    yield return $"{card} {m.Letter}";
                }
            }
        }
    }

    public string AutoPlay() {
        int maxScore = int.MinValue;
        string maxPlay = null;
        var saveState = SaveMarbles();
        var saveCards = new List<Card>(Players[Turn].Hand);
        var saveDeck = deck.SaveCards();
        int saveTurn = Turn;
        foreach (var play in PossiblePlays().ToList()) {
            string err = Play(play);
            if (err == null) {
                deck.RestoreCards(saveDeck);
                int score = Score();
                var card = new Card(play[..2]);
                // subtract card worth to avoid using high value cards on low scoring moves
                score -= card.Rank.Worth();
                if (score > maxScore) {
                    maxScore = score;
                    maxPlay = play;
                }
                RestoreMarbles(saveState);
                Turn = saveTurn;
                Players[Turn].Hand.Clear();
                Players[Turn].Hand.AddRange(saveCards);
            }
        }
        maxPlay ??= $"{saveCards.OrderBy(c => c.Rank.Worth()).First()} x"; // discard
        return maxPlay;
    }
}