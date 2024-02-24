# *Battleship* - платформа за играње

## Опис

Ово је реализација платформе за играње игре "Потапање бродова" (*Battleship* на енглеском). 

## Покретање

0. Требате имати [*Microsoft SDK*](https://dotnet.microsoft.com/en-us/download) на вашем рачунару инсталиран.
1. Преузмите репозиторијум: `git clone https://github.com/mvd3/battleship.git`
2. Уђите у фолдер са пројектом: `cd battleship\Battleship`
3. Инсталирајте *Avalonia* радни оквир који апликација користи: `dotnet add package Avalonia -s https://api.nuget.org/v3/index.json`
4. Покрените апликацију са: `dotnet run`

## Спецификација

### Правила игре и њихова реализација

Званична правила игре могу се наћи на сајту [сајту](https://www.hasbro.com/common/instruct/battleship.pdf) фирме *Hasbro*.
У реализацији ове апликације, платофрма распоређује бродове сваком играчу.
Постоје 2 могућности играња, играч против бота или бот против бота.