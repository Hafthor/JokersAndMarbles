namespace JokersAndMarbles;

public class Game {
    private readonly Board board;
    private readonly Deck deck;
    private readonly int playerCount;

    public Game(int playerCount, Random rnd) {
        this.playerCount = playerCount;
        board = new Board(playerCount, new Deck().Shuffle(rnd));
    }

    public void Run() {
        for (;;) {
            board.Paint();
            string clr = Marble.ColorForPlayer(board.Turn, playerCount);
            Console.Write($"{clr}Player {board.Turn}[{board.Score()}]{Ansi.Reset} ({board.Hand()}):");
            string sCmd = Console.ReadLine() ?? "";
            if (sCmd == "exit" || sCmd == "quit") break;
            if (sCmd == "plays") {
                var plays = board.LegalPlays().OrderByDescending(p => p.score);
                Console.WriteLine(string.Join(", ", plays.Select(p => $"{p.play}={p.score}")));
                Console.ReadLine();
                continue;
            }
            if (sCmd == "" || sCmd == "auto") sCmd = board.LegalPlays().MaxBy(p => p.score).play;
            string s = board.Play(sCmd);
            if (s != null) {
                board.Print(s);
            } else if (board.Win) {
                board.Paint();
                break;
            }
        }
    }
}