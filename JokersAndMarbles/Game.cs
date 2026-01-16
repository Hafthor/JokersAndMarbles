namespace JokersAndMarbles;

public class Game(int playerCount, int seed) {
    private readonly Board board = new(playerCount, new Deck().Shuffle(seed));

    public void Run() {
        for (bool fullAuto = false;;) {
            board.Paint();
            string clr = Marble.ColorForPlayer(board.Turn, playerCount);
            Console.Write($"{clr}Player {board.Turn}[{board.Score()}]{Ansi.Reset} ({board.Hand()}): {Ansi.ClearRestOfLine}");
            string sCmd = fullAuto ? "auto" : Console.ReadLine() ?? "";
            if (sCmd is "exit" or "quit") break;
            if (sCmd is "seed") {
                Console.WriteLine($"seed={seed}");
                Console.ReadLine();
                continue;
            } else if (sCmd == "plays") {
                var plays = board.LegalPlays().OrderByDescending(p => p.score);
                Console.WriteLine(string.Join(", ", plays.Select(p => $"{p.play}={p.score}")));
                Console.ReadLine();
                continue;
            } else if (sCmd == "fullauto") {
                fullAuto = true;
                sCmd = "auto";
            }
            if (sCmd is "" or "auto") sCmd = board.LegalPlays().MaxBy(p => p.score).play;
            string s = board.Play(sCmd);
            if (s != null)
                board.Print(s);
            else if (board.Win) {
                board.Paint();
                break;
            }
        }
    }
}