# Опис имплементације

## Увод

Овде можете прочитати детаљниј опис за *MainWindow.axaml* и *MainWindow.axaml.cs*. Све остале датотеке су аутоматски генерисане и нису измењене или битне за функционисање ове апликације.

## MainWindow.axaml

Овде нема ништа нарочито да се напомене, изузев тога да дугмад за поља нису дефинисана експлицитно, него динамички током покретања програма.

## MainWindow.axaml.cs

Конструктор `MainWindow` иницијализује основне компоненте (лева и десна табла), иницијализује платформу и поставља режим игре.

`InitializeComponent` је део радног оквира.

`DrawBoards` исцтрава све потребне елементе и поставља текст за имена ботова, да се зна који је леви а који десни.

`AddFieldsToBoard` генерише дугмад који служе за приказивање поља на табли.

`StartGame` притиском било којег дугмета за покретање игре, покреће се ова метода која припрема терен за игру и поставља конкретан режим.

`ClearBoard` враћа таблу у првобитно стање.

`SetShipsOnField` исцртава бродове на табели.

`BotVersusBotMatch` регулише партију између два бота. Асинхрона метода, завршава тек када се заврши партија.

`ChangeFieldState` мења стање поља, зависно од тога да ли је погодак или промашај.

`MarkShipAsDestroyed` дохвата од платформе коордниате брода и обележава та поља као уништен брод.

`BotMove` асинхорина метода која извршава један потез бота. Има уграђено закашњење како би се партија могла пратити.

`SetBoardClass` поставља класу поља (дугмади) на табели, зависно од тога да ли игра бот или играч на њој.

`SelectFieldHandler` када играч одабере поље приликом свог потеза, обрађује тај потез, а затим чека потез бота да би и њега приказао.

`GenerateClickHandlers` поставља горе поменуту методу као одзив за десну табелу, када игра играч.

`EndPlayerVersusBotGame` завршно чишћење када се партија заврши.