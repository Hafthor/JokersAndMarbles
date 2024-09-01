namespace JokersAndMarbles;

public class Game {
    private readonly Board board;
    private readonly Deck deck;

    public Game(Random rnd) => board = new Board(new Deck().Shuffle(rnd));

    public void Run() {
        for (;;) {
            Console.Clear();
            board.Paint();
            Console.Write($"Player {board.Turn}[{board.Score()}] ({board.Hand()}):");
            string sCmd = Console.ReadLine() ?? "";
            if (sCmd == "exit") break;
            if (sCmd == "") {
                sCmd = board.AutoPlay();
                board.Print($"Auto{board.Turn}: {sCmd}");
            }
            string s = board.Play(sCmd);
            if (s != null) {
                board.Print(s);
            } else {
                if (board.Win) {
                    board.Paint();
                    Console.WriteLine($"Players {board.Turn} and {board.Teammate} win!");
                    break;
                }
                board.NextTurn();
            }
        }
    }
}