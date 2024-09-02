using System.Collections.Immutable;

namespace JokersAndMarbles;

public class Board {
    //28  2930313233343536373839404142434445   46
    //   o o o o o o o o o o o o o o o o o o o
    //27 x     o         o                   x 47
    //26 x     o         o                   x 48
    //25 x     o o o   o o o           x x x x 49
    //24 x                             x     x 50
    //23 x                             x     x 51
    //22 x                                   x 52
    //21 x                             x     x 53
    //20 x                             x x x x 54
    //19 x     x                       x     x 55
    //18 x x x x                             x 56
    //17 x     x                             x 57
    //16 x                                   x 58
    //15 x     x                             x 59
    //14 x     x          home -5 7271 safe  x 60
    //13 x x x x           o o o   o o o70   x 61
    //12 x                   o         o69   x 62
    //11 x                   o         o68   x 63
    //   x o o o o o o o o o o o o o o o o o o
    //10   9 8 7 6 5 4 3 2 1 0-1-2-3-4676665   64
    // ascii without . $ * and space
    //     home        safe
    // A   ! " # % &   ' ( ) + ,
    // B   0 1 2 3 4   5 6 7 8 9
    // C   : ; < = >   A B C D E
    // D   F G H I J   K L M N O
    // E   P Q R S T   U V W X Y
    // F   [ \ ] ^ _   a b c d e
    // G   f g h i j   k l m n o
    // H   p q r s t   u v w x y
    
    // legend by player, home/safe, and board position
    private const int LEGEND_HOME = 0, LEGEND_SAFE = 1;
    private static readonly ImmutableArray<ImmutableArray<ImmutableArray<char>>> legend =
        "!\"#%&.'()+,*01234.56789*:;<=>.ABCDE*FGHIJ.KLMNO*PQRST.UVWXY*[\\]^_.abcde*fghij.klmno*pqrst.uvwxy"
            .Split('*').Select(s => s.Split('.').Select(s => s.ToCharArray().ToImmutableArray()).ToImmutableArray())
            .ToImmutableArray();

    private static readonly ImmutableArray<ImmutableArray<char>> board4 = """
        ..*.....*.....*....
        .  A    :         .
        .  B    ;         *
        .  CDE >=<     MLK.
        *              N  .
        .   $          O  .
        .   $             .
        .   $          J  .
        .   $          IGF*
        .  2$          H  .
        *013$             .
        .  4$             .
        .   $             .
        .  9$             .
        .  8              *
        .567     #%& ,+)  .
        *         "    (  .
        .         !    '  .
        ....*.....*.....*..
        """
        .Split('\n').Select(s => s.ToCharArray().ToImmutableArray()).ToImmutableArray();
    
    private static readonly ImmutableArray<ImmutableArray<char>> board6 = """
        ..*.....*.....*......*.....*.....*....
        .  A    :             K    F         .
        .  B    ;             L    G         *
        .  CDE >=<            MNO JIH     WVU.
        *                                 X  .
        .   $                             Y  .
        .   $                                .
        .   $                             T  .
        .   $                             SQP*
        .  2$                             R  .
        *013$                                .
        .  4$                                .
        .   $                                .
        .  9$                                .
        .  8                                 *
        .567     #%& ,+)            ]^_ edc  .
        *         "    (             \    b  .
        .         !    '             [    a  .
        ....*.....*.....*......*.....*.....*..
        """
        .Split('\n').Select(s => s.ToCharArray().ToImmutableArray()).ToImmutableArray();
    
    private static readonly ImmutableArray<ImmutableArray<char>> board8 = """
        ..*.....*.....*.....*.....*.....*....
        .  K    F            U    P         .
        .  L    G            V    Q         *
        .  MNO JIH           WXY TSR     cba.
        *                                d  .
        .   $                            e  .
        .   $                               .
        .   $                            _  .
        .   $                            ^\[*
        .  <$                            ]  .
        *:;=$                               .
        .  >$                               .
        .   $                               .
        .  E$                               .
        .  D$                               *
        .ABC$                               .
        *   $                               .
        .   $                               .
        .   $                               .
        .   $                               .
        .   $                               *
        .   $                            mlk.
        *   $                            n  .
        .   $                            o  .
        .   $                               .
        .   $                            j  .
        .   $                            igf*
        .  2$                            h  .
        *013$                               .
        .  4$                               .
        .   $                               .
        .  9$                               .
        .  8                                *
        .567     #%& ,+)           rst yxw  .
        *         "    (            q    v  .
        .         !    '            p    u  .
        ....*.....*.....*.....*.....*.....*..
        """
        .Split('\n').Select(s => s.ToCharArray().ToImmutableArray()).ToImmutableArray();

    private readonly string[] info;
    private readonly Deck deck;
    private readonly Dictionary<char, (int y, int x)> positionForChar = new();
    private readonly Player[] players;
    private readonly ImmutableArray<ImmutableArray<char>> board;

    public int Turn { get; private set; }
    public int Teammate => (Turn + players.Length / 2) % players.Length;
    public bool Win => players[Turn].marbles.All(m => m.IsSafe) && players[Teammate].marbles.All(m => m.IsSafe);

    public Board(int playerCount, Deck deck) {
        players = new Player[playerCount];
        board = playerCount switch {
            4 => board4,
            6 => board6,
            8 => board8,
            _ => throw new ArgumentException("Invalid player count")
        };
        int infoLines = board.Count(b => b.Contains('$'));
        info = new string[infoLines];
        this.deck = deck;
        for (int p = 0, m = 0, l = playerCount / 2, t = l; p < l;) {
            players[p] = new Player(p++, deck, Marble.MarbleLettersByTeams[m++], players.Length);
            players[t] = new Player(t++, deck, Marble.MarbleLettersByTeams[m++], players.Length);
        }
        Array.Fill(info, "");
        //     1234567890123456789
        Print("Jokers and Marbles");

        for (int r = 0; r < board.Length; r++) {
            var row = board[r];
            for (int c = 0; c < row.Length; c++)
                if (row[c] is not ' ' and not '.' and not '*' and not '$')
                    positionForChar[row[c]] = (y: r, x: c);
        }
    }

    public void Print(string s) {
        int w = players.Length > 4 ? 19 + 18 + 18 : 19;
        while (s.Length > 0) {
            Array.Copy(info, 1, info, 0, info.Length - 1);
            info[^1] = s.PadRight(w)[..w];
            s = s.Length > w ? s[w..] : "";
        }
    }

    public void NextTurn() => Turn = (Turn + 1) % players.Length;

    public List<Marble> AllMarbles() => players.SelectMany(p => p.marbles).ToList();

    public List<Marble> TeamMarbles() => players[Turn].marbles.Concat(players[Teammate].marbles).ToList();


    public string Play(string sCmd, bool quiet = false) {
        // examples
        // Kd e - means play King of Diamonds, move marble e out of home or ahead 13 steps if not in home
        // Kd e13 - means play King of Diamonds, move marble e 13 steps forward
        // Kd e1 - means play King of Diamonds, move marble e out of home
        // Ts b5 C-5 - means play Ten of Spades, move marble b 5 steps forward, move marble C 5 steps back
        // Jo a - means play Joker, move marble a to opposite start
        // Jo aB - means play Joker, move marble a and click marble B
        // 6s x - means discard Six of Spades, draw new card
        if (!quiet) Print($"P{Turn}: {sCmd}");
        string[] ss = sCmd.Split(' ');
        if (ss.Length < 2 || ss.Length > 3)
            return $"Bad command {sCmd}";
        string[] moves = ss[1..];
        bool splitMove = moves.Length > 1;
        Card card = players[Turn].hand.FirstOrDefault(c => c.ToString() == ss[0], default);
        if (card == null) return $"Bad card {ss[0]}";
        if (!splitMove && moves[0] == "x") { // discard
            Card newCard = players[Turn].UseAndDraw(deck, card);
            if (!quiet) Print($"Discard {card}, draw {newCard}");
            NextTurn();
            return null;
        }
        List<Marble> allMarbles = AllMarbles(),
            teamMarbles = TeamMarbles(),
            marbles = card.rank == Rank.Ten ? allMarbles : teamMarbles;
        bool mustSplit = card.MustSplit || (card.rank == Rank.Nine && teamMarbles.Count(m => m.IsSafe) < 9);
        if (!splitMove && mustSplit) return "Requires 2 moves";
        if (splitMove && !card.CanSplit) return "Only 1 move";

        // save current state
        Marble[][] saveMarbles = SaveMarbles();
        List<string> messages = new();
        try {
            int prevMove = 0;
            Marble prevMarble = null;
            for (int moveIndex = 0, curMove; moveIndex < moves.Length; moveIndex++) {
                string move = moves[moveIndex];
                Marble marble = marbles.FirstOrDefault(m => m.Letter == move[0], default);
                if (marble == null) return $"Bad marble {move[0]}";
                bool hostile = card.rank == Rank.Ten && teamMarbles.All(m => m.Letter != move[0]);
                if (move.Length == 1) { // infer move
                    if (card.rank is Rank.Ten || card.rank is Rank.Seven or Rank.Nine && splitMove && moveIndex == 0)
                        return $"Give distances for {card.rank}";

                    curMove = card.Value;
                    if (marble.IsHome)
                        if (card.IsAceOrFace)
                            curMove = Marble.START - Marble.HOME;
                        else if (card.IsJoker)
                            curMove = Marble.SIDE * players.Length / 2 - Marble.HOME; // opposite start
                        else
                            return $"Can't exit home w/ {card.rank}";
                } else { // move specified
                    Marble click = card.rank != Rank.Joker
                        ? null
                        : players.SelectMany(p => p.marbles).FirstOrDefault(m => m.Letter == move[1], default);
                    if (click != null) {
                        if (click.IsHome) return "Can't click home marble";
                        if (click.IsSafe) return "Can't click safe marble";
                        if (click.player == marble.player) return "Can't click same color";
                        if (card.rank == Rank.Joker && marble.IsHome)
                            marble.Position = Marble.START; // move marble out of home
                        curMove = (marble.MAX + click.AbsPosition - marble.AbsPosition) % marble.MAX;
                    } else {
                        if (!int.TryParse(move[1..], out curMove) || curMove == 0) return $"Bad move {move}";
                        if (marble.IsHome && (curMove != Marble.START - Marble.HOME || !card.IsAceOrFace) &&
                            !card.IsJoker)
                            return "Can't move marble out of home";
                        if (marble.IsSafe)
                            if (Turn != marble.player && Teammate != marble.player)
                                return "Can't move safe marble";
                            else if (curMove < 0)
                                return "Can't move safe marble backwards";
                            else if (marble.Position + curMove > marble.MAX)
                                return "Can't move marble past home";
                    }
                }

                if (marble.InLimbo && card.rank != Rank.Ten && curMove >= 0) return "Marble in limbo";
                if (marble.IsHome && curMove != Marble.START - Marble.HOME && !card.IsJoker)
                    return "Can't move marble out of home";
                if (!marble.IsHome && curMove != card.Value && !card.CanSplit && !card.IsJoker)
                    return $"Bad move {move} w/ {card}";

                if (moveIndex > 0) {
                    bool isCorrectValue = Math.Abs(prevMove) + Math.Abs(curMove) == card.Value;
                    if (card.rank == Rank.Ten && !isCorrectValue) return "Must be 10 steps";
                    if (card.rank == Rank.Ten && prevMarble.player == marble.player) return "Must be 2 colors";
                    if (card.rank == Rank.Seven && (curMove < 0 || prevMove < 0 || !isCorrectValue))
                        return "Must be 7 steps";
                    if (card.rank == Rank.Nine && (curMove < 0 == prevMove < 0 || !isCorrectValue))
                        return "Must be 9 steps +/-";
                }

                string s = marble.Move(curMove, hostile,
                    marble.IsHome || card.IsJoker ? null : players[marble.player].marbles);
                if (s != null) return s;

                // chain of clicks
                int pos = marble.AbsPosition;
                var clicked = allMarbles.FirstOrDefault(m => m.AbsPosition == pos && m != marble && !m.IsSafe);
                if (clicked != null) {
                    if (clicked.player == marble.player) return "Can't click same color";
                    if (clicked.player != marble.Teammate) {
                        clicked.Position = Marble.HOME; // send marble home
                        messages.Add($"Clicked {clicked.Letter}, sent home");
                    } else {
                        clicked.Position = clicked.ENTRY; // send marble to safe entry
                        messages.Add($"Clicked {clicked.Letter}, sent to entry");
                        pos = clicked.AbsPosition;
                        var clicked2 =
                            allMarbles.FirstOrDefault(m => m.AbsPosition == pos && m != clicked && !m.IsSafe);
                        if (clicked2 != null) {
                            if (clicked2.player == clicked.player)
                                return $"Can't click same color after clicking {clicked.Letter}";
                            if (clicked2.player != clicked.Teammate) {
                                clicked2.Position = Marble.HOME; // send marble home
                                messages.Add($"Clicked {clicked2.Letter}, sent home");
                            } else {
                                clicked2.Position = clicked2.ENTRY; // send marble to safe entry
                                messages.Add($"Clicked {clicked2.Letter}, sent to entry");
                                var clicked3 = allMarbles.FirstOrDefault(m =>
                                    m.AbsPosition == clicked2.AbsPosition && m != clicked2 && !m.IsSafe);
                                if (clicked3 != null) {
                                    clicked3.Position = Marble.HOME; // send marble home - can only be hostile
                                    messages.Add($"Clicked {clicked3.Letter}, sent home");
                                }
                            }
                        }
                    }
                }

                prevMove = curMove;
                prevMarble = marble;
            }
            Card newCard = players[Turn].UseAndDraw(deck, card);
            messages.Add($"{card} used, draw {newCard}");
            saveMarbles = null; // prevent rollback
        } finally {
            if (saveMarbles != null)
                RestoreMarbles(saveMarbles);
            else if (!quiet)
                foreach (var m in messages)
                    Print(m);
            if (Win) Print($"Game over - Players {Turn} and {Teammate} win!");
            else if (saveMarbles == null) NextTurn();
        }
        return null;
    }

    public void Paint() {
        int infoWidth = players.Length > 4 ? 19 + 18 + 18 : 19;
        for (int i = 0; i < info.Length; i++)
            info[i] = (info[i] ?? "").PadRight(infoWidth)[..infoWidth];
        var board = this.board.Select(row => row.ToArray()).ToArray();
        foreach (var e in positionForChar)
            board[e.Value.y][e.Value.x] = '.';

        for (int i = 0; i < players.Length; i++)
            foreach (var m in players[i].marbles)
                if (m.IsSafe)
                    FindAndSet(board, legend[i][LEGEND_SAFE][m.Position - m.ENTRY - 1], m.Letter);
                else if (m.IsHome)
                    _ = legend[i][LEGEND_HOME].FirstOrDefault(s => FindAndSet(board, s, m.Letter));
                else {
                    char c = m.Letter;
                    int p = (m.AbsPosition + m.MAX - 10) % m.MAX;
                    _ = players.Length switch {
                        4 => _ = (p / Marble.SIDE) switch {
                            0 => board[Marble.SIDE - p][0] = c, // left
                            1 => board[0][p - Marble.SIDE] = c, // top
                            2 => board[p - Marble.SIDE * 2][^1] = c, // right
                            _ => board[^1][m.MAX - p] = c // bottom
                        },
                        6 => _ = (p / Marble.SIDE) switch { // double wide
                            0 => board[Marble.SIDE - p][0] = c, // left
                            1 or 2 => board[0][p - Marble.SIDE] = c, // top
                            3 => board[p - Marble.SIDE * 3][^1] = c, // right
                            _ => board[^1][m.MAX - p] = c // bottom
                        },
                        8 => _ = (p / Marble.SIDE / 2) switch {
                            0 => board[Marble.SIDE * 2 - p][0] = c, // left
                            1 => board[0][p - Marble.SIDE * 2] = c, // top
                            2 => board[p - Marble.SIDE * 4][^1] = c, // right
                            _ => board[^1][m.MAX - p] = c // bottom
                        }
                    };
                }

        Console.Clear();
        int il = 0;
        int jAdd = players.Length > 4 ? 9 + 18 : 9;
        foreach (var line in board) {
            for (var j = 0; j < line.Length; j++) {
                var c = line[j];
                if (c == '$') {
                    j += jAdd;
                    Console.Write(' ');
                    Console.Write(info[il++].PadRight(infoWidth)[..infoWidth]);
                } else {
                    if (char.IsLetter(c)) Console.Write(Marble.ColorForLetter(c) + c + Ansi.Reset);
                    else Console.Write(c);
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

    public int Score(int player) {
        int teammate = (player + players.Length / 2) % players.Length, score = 0;
        for (int i = 0; i < players.Length; i++)
            score += i == player || i == teammate ? players[i].Score() : -players[i].Score();
        for (int i = 0; i < players.Length / 2; i++)
            score += i == player || i == teammate ? -HomeImbalance(i) : HomeImbalance(i);
        return score;
    }

    private int HomeImbalance(int player) {
        int teammate = (player + players.Length / 2) % players.Length;
        return Math.Abs(players[player].marbles.Count(m => m.IsHome) - players[teammate].marbles.Count(m => m.IsHome));
    }

    public int Score() => Score(Turn);

    public string Hand() => string.Join(',', players[Turn].hand.ToList());

    private Marble[][] SaveMarbles() => players.Select((p, i) =>
        p.marbles.Select(m => new Marble(m.Letter, i, players.Length) { Position = m.Position }).ToArray()).ToArray();

    private void RestoreMarbles(Marble[][] saveMarbles) {
        for (int p = 0; p < players.Length; p++) {
            Marble[] player = players[p].marbles, savePlayer = saveMarbles[p];
            for (int i = 0; i < player.Length; i++)
                player[i].Position = savePlayer[i].Position;
        }
    }

    private IEnumerable<string> PossiblePlays() { // not including discard
        List<Marble> allMarbles = AllMarbles(), teamMarbles = TeamMarbles();
        foreach (var card in players[Turn].hand.ToArray()) {
            int mi1 = 0;
            if (card.CanSplit)
                if (!card.MustSplit)
                    foreach (var m1 in teamMarbles)
                        foreach (var m2 in teamMarbles)
                            for (int i = 1; m1 != m2 && i < (int)card.rank; i++)
                                yield return $"{card} {m1.Letter}{i} {m2.Letter}" +
                                             (card.rank == Rank.Seven ? 7 - i : -(9 - i));
                else
                    foreach (var m1 in allMarbles)
                        foreach (var m2 in allMarbles.Skip(++mi1))
                            if (m1.player != m2.player)
                                for (int i = 1; i < (int)card.rank; i++)
                                    for (int mul1 = -1; mul1 <= 1; mul1 += 2)
                                        for (int mul2 = -1; mul2 <= 1; mul2 += 2)
                                            yield return $"{card} {m1.Letter}{i * mul1} {m2.Letter}{(10 - i) * mul2}";
            if (card.IsJoker)
                foreach (var m1 in allMarbles) {
                    foreach (var m2 in allMarbles)
                        if (m1.player == m2.player)
                            yield return $"{card} {m1.Letter}{m2.Letter}";
                    if (m1.IsHome)
                        yield return $"{card} {m1.Letter}";
                }
            else if (!card.MustSplit)
                if (card.rank != Rank.Nine || teamMarbles.Count(m => m.IsSafe) < 9)
                    foreach (var m in teamMarbles)
                        yield return $"{card} {m.Letter}";
        }
    }

    public List<(string play, int score)> LegalPlays() {
        Marble[][] saveState = SaveMarbles();
        List<Card> saveCards = new(players[Turn].hand), saveDeck = deck.SaveCards();
        int maxScore = int.MinValue, saveTurn = Turn;
        string maxPlay = null;
        List<(string play, int score)> plays = new();
        foreach (string play in PossiblePlays()) {
            string err = Play(play, true);
            if (err == null) {
                Card card = new(play[..2]);
                // subtract card worth to avoid using high value cards on low scoring moves
                Turn = saveTurn;
                plays.Add((play, Score() - card.Worth));
                // restore state
                deck.RestoreCards(saveDeck);
                RestoreMarbles(saveState);
                for (int i = 0; i < saveCards.Count; i++)
                    players[Turn].hand[i] = saveCards[i];
            }
        }
        int discardScore = Score() - 1000;
        foreach (var c in saveCards)
            plays.Add(($"{c} x", discardScore - c.Worth)); // discard
        return plays;
    }
}