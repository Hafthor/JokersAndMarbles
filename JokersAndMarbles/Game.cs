namespace JokersAndMarbles;

public class Game(int playerCount, Random rnd) {
    private readonly Board _board = new(playerCount, new Deck().Shuffle(rnd));

    public void Run() {
        for (;;) {
            _board.Paint();
            string clr = Marble.ColorForPlayer(_board.Turn, playerCount);
            Console.Write($"{clr}Player {_board.Turn}[{_board.Score()}]{Ansi.Reset} ({_board.Hand()}):");
            string sCmd = Console.ReadLine() ?? "";
            if (sCmd == "exit" || sCmd == "quit") break;
            if (sCmd == "plays") {
                var plays = _board.LegalPlays().OrderByDescending(p => p.score);
                Console.WriteLine(string.Join(", ", plays.Select(p => $"{p.play}={p.score}")));
                Console.ReadLine();
                continue;
            }
            if (sCmd == "" || sCmd == "auto") sCmd = _board.LegalPlays().MaxBy(p => p.score).play;
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