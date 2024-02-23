using System.Threading.Tasks;
using Battleship.Platform.Helper;

namespace Battleship.Platform.Bot;

public interface IBot
{
    public string GetName();
    public Coordinate NextPosition(in FieldState[,] board, in Ship[] ships);
}