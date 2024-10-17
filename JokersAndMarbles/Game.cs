namespace JokersAndMarbles;

public class Game(int playerCount, int seed) {
    private readonly Board _board = new(playerCount, new Deck().Shuffle(seed));

    public void Run() {
        for (bool fullAuto = false;;) {
            _board.Paint();
            string clr = Marble.ColorForPlayer(_board.Turn, playerCount);
            Console.Write($"{clr}Player {_board.Turn}[{_board.Score()}]{Ansi.Reset} ({_board.Hand()}): {Ansi.ClearRestOfLine}");
            string sCmd = fullAuto ? "auto" : Console.ReadLine() ?? "";
            if (sCmd is "exit" or "quit") break;
            if (sCmd is "seed") {
                Console.WriteLine($"seed={seed}");
                Console.ReadLine();
                continue;
            } else if (sCmd == "plays") {
                var plays = _board.LegalPlays().OrderByDescending(p => p.score);
                Console.WriteLine(string.Join(", ", plays.Select(p => $"{p.play}={p.score}")));
                Console.ReadLine();
                continue;
            } else if (sCmd == "fullauto") {
                fullAuto = true;
                sCmd = "auto";
            }
            if (sCmd is "" or "auto") sCmd = _board.LegalPlays().MaxBy(p => p.score).play;
            string s = _board.Play(sCmd);
            if (s != null)
                _board.Print(s);
            else if (_board.Win) {
                _board.Paint();
                break;
            }
        }
    }
}