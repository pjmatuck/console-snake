namespace SnakeRootGame;

class Program
{
    //GAME CONFIG
    static int GAME_RESOLUTION = 16;
    static int FRAME_DELTA_TIME_MILLI = 500;
    //GAME DATA
    static int PLAYER_POINTS = 0;
    //PLAYER CONFIG
    //static readonly char PLAYER_CHAR = 'S';
    static List<Point> SNAKE_BODY = new List<Point>();
    static Point SNAKE_HEAD = new Point(1, 1);
    //FRUITS
    static char FRUIT_CHAR = 'o';
    static int FRUIT_AMOUNT = 10;
    static int[,] FRUITS = new int[GAME_RESOLUTION+1, GAME_RESOLUTION+1];
     

    static void Main(string[] args)
    {
        InputData inputData = new();

        InitializeFruits();
        ExpandBody(0,0);

        while(true)
        {
            Draw();
            Thread.Sleep(FRAME_DELTA_TIME_MILLI);
            Input(inputData);
            Update(inputData);
        }
    }

    private static void InitializeFruits()
    {
        Random random = new Random();

        for(int i = 0; i < FRUIT_AMOUNT; i++)
        {
            FRUITS[
                random.Next(1, GAME_RESOLUTION),
                random.Next(1, GAME_RESOLUTION)] = 1;
        }
    }


    private static void Draw()
    {
        Console.Write("║");

        Console.Clear();
        for(int y = 0; y < GAME_RESOLUTION + 2; y++)
        {
            for(int x = 0; x < GAME_RESOLUTION + 2; x++)
            {
                if(y == 0 && x == 0)
                    Console.Write(" ╔");
                else if(y == 0 || y == GAME_RESOLUTION + 1)
                    Console.Write("══");
                else if(y == 0 && x == GAME_RESOLUTION + 1)
                    Console.Write("╗ ");
                else if(x == 0)
                    Console.Write(" ║");
                else if(x == GAME_RESOLUTION + 1)
                    Console.Write("║ ");
                else if(y == GAME_RESOLUTION + 1 && x == 0)
                    Console.Write("╚");
                else if (y == GAME_RESOLUTION + 1 && x == GAME_RESOLUTION + 1)
                    Console.Write("╝");

                else if(x > GAME_RESOLUTION || y > GAME_RESOLUTION)
                    continue;

                else if(FRUITS[x,y] == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(FRUIT_CHAR + " ");
                    Console.ResetColor();
                }
                else if(CheckIfThereIsBody(x, y))
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("  ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(". ");
                }
                
            }
            Console.WriteLine("\n");
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"POINTS: {PLAYER_POINTS}");
        Console.ResetColor();
    }

    private static bool CheckIfThereIsBody(int x, int y)
    {
        for(int i = 0; i < SNAKE_BODY.Count; i++)
        {
            if (SNAKE_BODY[i].X == x && SNAKE_BODY[i].Y == y)
                return true;
        }
        return false;
    }

    static void Input(InputData inputData)
    {
        List<ConsoleKeyInfo> availableKeys = new();

        while(Console.KeyAvailable)
        {
            availableKeys.Add(Console.ReadKey(true));    
        }
        
        if(availableKeys.Count > 0)
        {
            inputData.lastKeyInfo = inputData.newKeyInfo;
            inputData.newKeyInfo = availableKeys[availableKeys.Count - 1];
            availableKeys.Clear();
        }
    }

    private static void Update(InputData inputData)
    {
        if(inputData.lastKeyInfo != inputData.newKeyInfo)
            inputData.direction =  UpdateDirection(inputData.newKeyInfo);
                    
        UpdatePosition(inputData.direction);

        CheckFruitColision();
    }

    private static void CheckFruitColision()
    {
        if(FRUITS[SNAKE_HEAD.X,SNAKE_HEAD.Y] == 1)
        {
            PLAYER_POINTS += 10;
            FRUITS[SNAKE_HEAD.X,SNAKE_HEAD.Y] = 0;
            ExpandBody(SNAKE_HEAD.X,SNAKE_HEAD.Y);
        }
    }

    private static void ExpandBody(int x, int y)
    {
        SNAKE_BODY.Add(new Point(x, y));
    }

    private static Direction UpdateDirection(ConsoleKeyInfo newKeyInfo)
    {
        Direction direction = Direction.RIGHT;

        switch(newKeyInfo.Key)
        {
            case ConsoleKey.W:
                direction = Direction.UP;
            break;
            case ConsoleKey.A:
                direction = Direction.LEFT;
            break;
            case ConsoleKey.S:
                direction = Direction.DOWN;
            break;
            case ConsoleKey.D:
                direction = Direction.RIGHT;
            break;
        }

        return direction;
    }

    private static void UpdatePosition(Direction direction)
    {
        switch(direction)
        {
            case Direction.UP:
                if(SNAKE_HEAD.Y > 0)
                    SNAKE_HEAD.Y--;
            break;
            case Direction.RIGHT:
                if(SNAKE_HEAD.X < (GAME_RESOLUTION - 1))
                    SNAKE_HEAD.X++;
            break;
            case Direction.DOWN:
                if(SNAKE_HEAD.Y < (GAME_RESOLUTION - 1))
                    SNAKE_HEAD.Y++;
            break;
            case Direction.LEFT:
                if(SNAKE_HEAD.X > 0)
                    SNAKE_HEAD.X--;
            break;
        }
        UpdateBody();
    }

    private static void UpdateBody()
    {
        for(int i = SNAKE_BODY.Count-1; i > 0; i--)
        {
            SNAKE_BODY[i] = SNAKE_BODY[i-1];
        }

        SNAKE_BODY[0] = new Point(SNAKE_HEAD.X, SNAKE_HEAD.Y);
    }
}

public class InputData
{
    public ConsoleKeyInfo lastKeyInfo;
    public ConsoleKeyInfo newKeyInfo;
    public Direction direction;
}

public enum Direction 
{
    RIGHT,
    DOWN,
    LEFT,
    UP,
}

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
