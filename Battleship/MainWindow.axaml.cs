using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Battleship.Platform;
using Battleship.Platform.Helper;

namespace Battleship;

public partial class MainWindow : Window
{
    private Platform.Platform _platform;
    private Grid? _leftBoardGrid, _rightBoardGrid;
    private TextBlock? _leftPlayerInfo, _rightPlayerInfo;
    private Button[,] _leftBoardButtons, _rightBoardButtons;
    private Button? _versusLeftBot, _versusRightBot, _botVersusBot;
    
    public MainWindow()
    {
        InitializeComponent();

        _leftBoardButtons = new Button[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _rightBoardButtons = new Button[Data.TABLE_SIZE, Data.TABLE_SIZE];

        _platform = new();

        DrawBoards();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DrawBoards()
    {
        _leftBoardGrid = this.FindControl<Grid>("LeftBoard");
        _rightBoardGrid = this.FindControl<Grid>("RightBoard");
        _leftPlayerInfo = this.FindControl<TextBlock>("LeftPlayerInfo");
        _rightPlayerInfo = this.FindControl<TextBlock>("RightPlayerInfo");
        _versusLeftBot = this.FindControl<Button>("VersusLeftBot");
        _versusRightBot = this.FindControl<Button>("VersusRightBot");
        _botVersusBot = this.FindControl<Button>("BotVersusBot");

        if (_leftBoardGrid == null 
            || _rightBoardGrid == null
            || _leftPlayerInfo == null
            || _rightPlayerInfo == null)
            return;

        AddFieldsToBoard(_leftBoardGrid, _leftBoardButtons);
        AddFieldsToBoard(_rightBoardGrid, _rightBoardButtons, false);

        _leftPlayerInfo.Text = _platform.GetLeftBotName();
        _rightPlayerInfo.Text = _platform.GetRightBotName();

        _leftBoardGrid.IsEnabled = false;
        _rightBoardGrid.IsEnabled = false;
    }

    private static void AddFieldsToBoard(Grid grid, Button[,] buttons, bool isLeftBoard = true)
    {
        for (int i = 0; i < Data.TABLE_SIZE; ++i)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        for (int i = 0; i < Data.TABLE_SIZE; ++i)
        {
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
            {
                var button = new Button
                {
                    Width = 50,
                    Height = 50,
                    Background = new SolidColorBrush(Color.Parse(Data.REGULAR_FIELD_BACKGROUND_COLOR_HEX)),
                    BorderBrush = new SolidColorBrush(Color.Parse(Data.BORDER_COLOR_HEX)),
                    BorderThickness = new(1)
                };

                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
                grid.Children.Add(button);

                buttons[i, j] = button;
            }
        }
    }
 
    public async void StartGame(object sender, RoutedEventArgs args)
    {
        if (_leftBoardGrid == null
            || _rightBoardGrid == null
            || _leftPlayerInfo == null
            || _rightPlayerInfo == null
            || _versusLeftBot == null
            || _versusRightBot == null
            || _botVersusBot == null
            || args.Source == null)
            return;
        
        Button button = (Button) args.Source;
        GameType mode = (GameType) int.Parse(button.Tag?.ToString() ?? "-1");

        _platform.StartGame(mode);
        ClearBoard(_leftBoardButtons);
        ClearBoard(_rightBoardButtons);

        _versusLeftBot.IsEnabled = false;
        _versusRightBot.IsEnabled = false;
        _botVersusBot.IsEnabled = false;

        SetShipsOnField(_leftBoardButtons, _platform.GetAllShipPositions());

        switch (mode)
        {
            case GameType.PlayerVersusLeftBot:
            case GameType.PlayerVersusRightBot:
            {
                _rightBoardGrid.IsEnabled = true;
                _leftPlayerInfo.Text = Data.HUMAN_PLAYER;
                _rightPlayerInfo.Text = mode == GameType.PlayerVersusLeftBot ? _platform.GetLeftBotName() : _platform.GetRightBotName();
                break;
            }
            case GameType.BotVersusBot:
            {
                SetBoardClass(_leftBoardButtons, Data.NO_HOVER_CLICK_CLASS_NAME);
                SetBoardClass(_rightBoardButtons, Data.NO_HOVER_CLICK_CLASS_NAME);
                _leftBoardGrid.IsEnabled = true;
                _rightBoardGrid.IsEnabled = true;
                _leftPlayerInfo.Text = _platform.GetLeftBotName();
                _rightPlayerInfo.Text = _platform.GetRightBotName();

                SetShipsOnField(_rightBoardButtons, _platform.GetAllShipPositions(false));
                await BotVersusBotMatch();
                break;
            }
            default: break;
        };

        _versusLeftBot.IsEnabled = true;
        _versusRightBot.IsEnabled = true;
        _botVersusBot.IsEnabled = true;
    }

    private static void ClearBoard(Button[,] buttons)
    {
        foreach (Button button in buttons)
        {
            button.Classes.Clear();
            button.Background = new SolidColorBrush(Color.Parse(Data.REGULAR_FIELD_BACKGROUND_COLOR_HEX));
            button.IsEnabled = true;
        }
    }

    private static void SetShipsOnField(Button[,] board, Coordinate[] shipPositions)
    {
        foreach (Coordinate coordinate in shipPositions)
        {
            board[coordinate.X, coordinate.Y].Classes.Add(Data.NO_HOVER_CLICK_CLASS_NAME);
            board[coordinate.X, coordinate.Y].Background = new SolidColorBrush(Color.Parse(Data.SHIP_BACKGROUND_COLOR_HEX));
        }
    }
    
    private async Task BotVersusBotMatch()
    {
        if (_leftPlayerInfo == null
            || _rightPlayerInfo == null)
            return;

        await Task.Delay(Data.DELAY_BEFORE_GAME_START);
        
        for (int i = 0; i < Data.MAX_NUMBER_OF_TURNS; ++i)
        {
            await BotMove(_rightBoardButtons, true, true);

            if (_platform.GameFinished())
            {
                _leftPlayerInfo.Text += Data.WINNER;
                break;
            }

            await BotMove(_leftBoardButtons, false, true);

            if (_platform.GameFinished())
            {
                _rightPlayerInfo.Text += Data.WINNER;
                break;
            }
        }
    }

    private static void ChangeFieldState(Button[,] board, Coordinate coordinate, FieldState fieldState)
    {
        board[coordinate.X, coordinate.Y].Classes.Add(Data.NO_HOVER_CLICK_CLASS_NAME);
        board[coordinate.X, coordinate.Y].IsEnabled = false;

        IBrush color = new SolidColorBrush(Color.Parse(Data.MISS_BACKGROUND_COLOR_HEX));

        switch(fieldState)
        {
            case FieldState.Damaged:
            {
                color = new SolidColorBrush(Color.Parse(Data.DAMAGED_SHIP_BACKGROUND_COLOR_HEX));
                break;
            }
            case FieldState.Destroyed:
            {
                color = new SolidColorBrush(Color.Parse(Data.DESTROYED_SHIP_BACKGROUND_COLOR_HEX));
                break;
            }
        }

        board[coordinate.X, coordinate.Y].Background = color;
    }

    private void MarkShipAsDestroyed(Button[,] board, Coordinate hitCoordinate, bool defaultLeft = true)
    {
        Coordinate[] coordinates = _platform.GetPositionOfDestroyedShip(hitCoordinate, defaultLeft);

        foreach(Coordinate coordinate in coordinates)
        {
            ChangeFieldState(board, coordinate, FieldState.Destroyed);
        }
    }

    private async Task BotMove(Button[,] board, bool isLeftBot = true, bool botVersusBot = false)
    {
        Coordinate coordinate;
        FieldState fieldState;

        (coordinate, fieldState) = _platform.BotNextMove(isLeftBot, botVersusBot);
        
        if (fieldState == FieldState.Destroyed)
            MarkShipAsDestroyed(board, coordinate, isLeftBot);
        else
            ChangeFieldState(board, coordinate, fieldState);
        
        ChangeFieldState(board, coordinate, fieldState);

        await Task.Delay(Data.BOT_DELAY_TIME_MS);
    }

    private static void SetBoardClass(Button[,] board, string className)
    {
        foreach(Button button in board)
            button.Classes.Add(className);
    }
}