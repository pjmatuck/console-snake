using System.Text;

namespace SnakeRootGame;

class Program
{
    //GAME CONFIG
    static int GAME_RESOLUTION = 16;
    static int FRAME_DELTA_TIME_MILLI = 500;
    static bool IS_GAME_OVER = false;
    static bool RESTART_GAME = false;
    static int FRUIT_SPAWN_TIME = 5000;
    static int ELAPSED_TIME = 0;
    //GAME DATA
    static int PLAYER_SCORE = 0;
    //PLAYER CONFIG
    static List<Point> SNAKE_BODY = new List<Point>();
    static Point SNAKE_HEAD = new Point(1, 1);
    static Point COLLISION_POINT;
    //FRUITS
    static char FRUIT_CHAR = '⬤';
    static int FRUIT_AMOUNT = 10;
    static int[,] FRUITS;
     

    static void Main()
    {
        do
        {
            InputData inputData = new();

            InitialSetup();
            PlayStartSound();          

            InitializeFruits();
            ExpandBody(0,0);

            do
            {
                Draw();
                Thread.Sleep(FRAME_DELTA_TIME_MILLI - PLAYER_SCORE);
                Input(inputData);
                Update(inputData);
            } while (!IS_GAME_OVER);

            Console.ForegroundColor = ConsoleColor.Magenta;
            
            GameOver();
        }

        while(RESTART_GAME);
    }

    private static void InitialSetup()
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;
        RESTART_GAME = false;
        PLAYER_SCORE = 0;
        SNAKE_HEAD = new Point(1, 1);
        SNAKE_BODY.Clear();
        COLLISION_POINT = new Point(0, 0);
        IS_GAME_OVER = false;
    }

    private static void GameOver()
    {
        PlayEndSound();
        Console.SetCursorPosition(0, 0);
        for(int y = 0; y <= GAME_RESOLUTION + 1; y++)
        {
            for(int x = 0; x <= GAME_RESOLUTION + 1; x++)
            {
                Console.Write("  ");
                Thread.Sleep(1);
            }
            Console.Write("\n");
        }

        Console.Clear();
        Console.Write(@"


         GG    A   MM MM EEEE
        G     A A  M M M E
        G GG A   A M M M EEE
        G  G AAAAA M   M E
        GG   A   A M   M EEEE

         OO  V   V EEEEE RRRR
        O  O V   V E     R  R
        O  O V   V EEE   RRRR
        O  O  V V  E     R R
         OO    V   EEEEE R  R");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"\n\n\tSCORE: {PLAYER_SCORE}\n\n");
        Console.ResetColor();
        Console.WriteLine("  Press HOME to restart the game.");
        
        CheckForGameRestart();
    }

    private static void CheckForGameRestart()
    {
        ConsoleKeyInfo consoleInput = Console.ReadKey(true);

        if(consoleInput.Key == ConsoleKey.Home)
        {
            RESTART_GAME = true;
        }
    }

    private static void InitializeFruits()
    {
        FRUITS = new int[GAME_RESOLUTION+1, GAME_RESOLUTION+1];

        for(int i = 0; i < FRUIT_AMOUNT; i++)
        {
            GenerateFruit();
        }
    }

    static void GenerateFruit()
    {
        Random random = new Random();

        int x = random.Next(1, GAME_RESOLUTION);
        int y = random.Next(1, GAME_RESOLUTION);

        if(FRUITS[x,y] == 1 || CheckIfThereIsBody(x,y))
            GenerateFruit();
        else 
            FRUITS[x,y] = 1;
    }


    private static void Draw()
    {
        Console.Clear();
        for(int y = 0; y < GAME_RESOLUTION + 2; y++)
        {
            for(int x = 0; x < GAME_RESOLUTION + 2; x++)
            {
                if(COLLISION_POINT.INITIALIZED 
                && COLLISION_POINT.X == x 
                && COLLISION_POINT.Y == y)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('X');
                    Console.ResetColor();
                    IS_GAME_OVER = true;
                }
                if(y == 0 && x == 0)
                    Console.Write(" ╔");
                else if(y == 0 && x == GAME_RESOLUTION + 1)
                    Console.Write("╗ ");
                else if(y == GAME_RESOLUTION + 1 && x == 0)
                    Console.Write(" ╚");
                else if (y == GAME_RESOLUTION + 1 && x == GAME_RESOLUTION + 1)
                    Console.Write("╝ ");
                else if(y == 0 && x != 0)
                    Console.Write("══");
                else if(y == GAME_RESOLUTION + 1 && x != 0)
                    Console.Write("══");
                else if(x == 0)
                    Console.Write(" ║");
                else if(x == GAME_RESOLUTION + 1)
                    Console.Write("║ ");

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
                else if(SNAKE_HEAD.X == x && SNAKE_HEAD.Y == y)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("  ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("  ");
                }
                
            }
            Console.Write("\n");
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  SCORE: {PLAYER_SCORE}");
        Console.ResetColor();
    }

    private static bool CheckIfThereIsBody(int x, int y)
    {
        for(int i = 1; i < SNAKE_BODY.Count; i++)
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
        if(IS_GAME_OVER) return;

        if(inputData.lastKeyInfo != inputData.newKeyInfo)
            inputData.direction =  UpdateDirection(inputData.lastKeyInfo, inputData.newKeyInfo);
                    
        Point updatedPosition = UpdatePosition(inputData.direction);

        if(CheckWallCollision() || CheckIfThereIsBody(updatedPosition.X, updatedPosition.Y))
        {
            COLLISION_POINT = updatedPosition;
            return;
        }

        CheckFruitCollision();

        ELAPSED_TIME += FRAME_DELTA_TIME_MILLI - PLAYER_SCORE;
        if(ELAPSED_TIME > FRUIT_SPAWN_TIME)
        {
            GenerateFruit();
            ELAPSED_TIME = 0;
        }
    }

    private static bool CheckWallCollision()
    {
        if(SNAKE_HEAD.X == 0 || SNAKE_HEAD.X == GAME_RESOLUTION + 1
        || SNAKE_HEAD.Y == 0 || SNAKE_HEAD.Y == GAME_RESOLUTION + 1)
        {
            return true;
        }

        return false;
    }

    private static void CheckFruitCollision()
    {
        if(FRUITS[SNAKE_HEAD.X,SNAKE_HEAD.Y] == 1)
        {
            PLAYER_SCORE += 10;
            FRUITS[SNAKE_HEAD.X,SNAKE_HEAD.Y] = 0;
            ExpandBody(SNAKE_HEAD.X,SNAKE_HEAD.Y);
            Console.Beep(400, 200);
        }
    }

    private static void ExpandBody(int x, int y)
    {
        SNAKE_BODY.Add(new Point(x, y));
    }

    private static Direction UpdateDirection(ConsoleKeyInfo oldKeyInfo, ConsoleKeyInfo newKeyInfo)
    {
        Direction direction = Direction.RIGHT;

        switch(newKeyInfo.Key)
        {
            case ConsoleKey.W:
                if (oldKeyInfo.Key != ConsoleKey.S)
                    direction = Direction.UP;
            break;
            case ConsoleKey.A:
                if (oldKeyInfo.Key != ConsoleKey.D)
                    direction = Direction.LEFT;
            break;
            case ConsoleKey.S:
                if (oldKeyInfo.Key != ConsoleKey.W)
                    direction = Direction.DOWN;
            break;
            case ConsoleKey.D:
                if (oldKeyInfo.Key != ConsoleKey.A)
                    direction = Direction.RIGHT;
            break;
        }

        return direction;
    }

    private static Point UpdatePosition(Direction direction)
    {
        switch(direction)
        {
            case Direction.UP:
                //if(SNAKE_HEAD.Y > 0)
                    SNAKE_HEAD.Y--;
            break;
            case Direction.RIGHT:
                //if(SNAKE_HEAD.X < (GAME_RESOLUTION - 1))
                    SNAKE_HEAD.X++;
            break;
            case Direction.DOWN:
                //if(SNAKE_HEAD.Y < (GAME_RESOLUTION - 1))
                    SNAKE_HEAD.Y++;
            break;
            case Direction.LEFT:
                //if(SNAKE_HEAD.X > 0)
                    SNAKE_HEAD.X--;
            break;
        }
        UpdateBody();

        return new Point(SNAKE_HEAD.X, SNAKE_HEAD.Y);
    }

    private static void UpdateBody()
    {
        for(int i = SNAKE_BODY.Count-1; i > 0; i--)
        {
            SNAKE_BODY[i] = SNAKE_BODY[i-1];
        }

        SNAKE_BODY[0] = new Point(SNAKE_HEAD.X, SNAKE_HEAD.Y);
    }

    static void PlayStartSound()
    {
        Console.Beep(1000, 100); // Nota aguda
        Console.Beep(1200, 100); // Nota mais aguda
        Console.Beep(1400, 150); // Nota final
        Console.Beep(1000, 100); // Nota aguda
        Console.Beep(1200, 100); // Nota mais aguda
        Console.Beep(1400, 150); // Nota final
    }

    static void PlayEndSound()
    {
        Console.Beep(800, 150); // Nota média
        Console.Beep(600, 150); // Nota grave
        Console.Beep(400, 200); // Nota mais grave
        Console.Beep(800, 150); // Nota média
        Console.Beep(600, 150); // Nota grave
        Console.Beep(400, 200); // Nota mais grave
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
    public bool INITIALIZED => X + Y > 0 ? true : false;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
