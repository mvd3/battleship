using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Platform.Helper;

namespace Battleship.Platform.Bot;

public class NBot(string name) : IBot
{
    private string _name = name;
    private Random _randomNumberGenerator = new();

    public string GetName()
    {
        return _name;
    }

    public Coordinate NextPosition(in FieldState[,] board, in Ship[] ships)
    {
        Coordinate coordinate = new()
        {
            X = 0,
            Y = 0
        };

        List<Coordinate> freeFields = [];

        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
                if (board[i, j] == FieldState.Empty)
                    freeFields.Add(new Coordinate
                    {
                        X = i,
                        Y = j
                    });

        int nextField = _randomNumberGenerator.Next(freeFields.Count);

        coordinate = freeFields.ElementAt(nextField);
        freeFields.RemoveAt(nextField);

        return coordinate;
    }
}