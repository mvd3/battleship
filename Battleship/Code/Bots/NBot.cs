using Battleship.Platform.Helper;

namespace Battleship.Platform.Bot;

public class NBot(string name) : IBot
{
    private string _name = name;

    public string GetName()
    {
        return _name;
    }

    public Coordinate NextPosition(in Field[,] board, in Ship[] ships)
    {
        Coordinate coordinate = new()
        {
            X = 0,
            Y = 0
        };

        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
                if (board[i, j].IsAvailable)
                    coordinate = new Coordinate
                    {
                        X = i,
                        Y = j
                    };

        return coordinate;
    }
}