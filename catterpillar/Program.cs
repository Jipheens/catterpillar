using System;
using System.Collections.Generic;
using System.IO;

public class CaterpillarControlSystem
{
    private char[,] planetGrid;
    private List<string> commandLog;
    private Stack<string> undoLog;
    private Stack<string> redoLog;
    private int[] headPosition;
    private int[] tailPosition;
    private int caterpillarLength;
    private const int gridWidth = 30;
    private const int gridHeight = 30;
    private const char emptySquare = '.';
    private const char head = 'H';
    private const char tail = 'T';
    private const string logFileName = "command_log.txt";  

    public CaterpillarControlSystem()
    {
        planetGrid = new char[gridWidth, gridHeight];
        commandLog = new List<string>();
        undoLog = new Stack<string>();
        redoLog = new Stack<string>();
        headPosition = new int[2]; // [x, y]
        tailPosition = new int[2]; // [x, y]
        caterpillarLength = 2; // Initial caterpillar length
        InitializePlanet();
        InitializeCaterpillar();
    }

    private void InitializePlanet()
    {
        string[] mapLayout = new string[]
        {
            "$*********$**********$********",
            "***$*******B*************#****",
            "************************#*****",
            "***#**************************",
            "**$*************************#*",
            "$$***#************************",
            "**************$***************",
            "**********$*********$*****#***",
            "********************$*******$*",
            "*********#****$***************",
            "**B*********$*****************",
            "*************$$****B**********",
            "****$************************B",
            "**********************#*******",
            "***********************$***B**",
            "********$***$*****************",
            "************$*****************",
            "*********$********************",
            "*********************#********",
            "*******$**********************",
            "*#***$****************#*******",
            "****#****$****$********B******",
            "***#**$********************$**",
            "***************#**************",
            "***********$******************",
            "****B****#******B*************",
            "***$***************$*****B****",
            "**********$*********#*$*******",
            "**************#********B******",
            "s**********$*********#*B******"
        };

        // lets do a loop through each row of the map layout and populate the planet grid
        for (int i = 0; i < mapLayout.Length; i++)
        {
            string row = mapLayout[i];
            for (int j = 0; j < row.Length; j++)
            {
                planetGrid[j, i] = row[j];
            }
        }
    }

    private void InitializeCaterpillar()
    {
        // Initialize caterpillar at a starting position (as provided in the mission brief)
        headPosition[0] = 14; // Initial x-coordinate for head
        headPosition[1] = 14; // Initial y-coordinate for head
        tailPosition[0] = 15; // Initial x-coordinate for tail
        tailPosition[1] = 14; // Initial y-coordinate for tail

        // Update planet grid with caterpillar positions
        planetGrid[headPosition[0], headPosition[1]] = head;
        planetGrid[tailPosition[0], tailPosition[1]] = tail;
    }

    public void ExecuteCommand(char direction, int steps)
    {
        // Execute rider's command and update caterpillar position
        int[] newPosition = new int[] { headPosition[0], headPosition[1] };

        switch (direction)
        {
            case 'U':
                newPosition[1] -= steps;
                break;
            case 'D':
                newPosition[1] += steps;
                break;
            case 'L':
                newPosition[0] -= steps;
                break;
            case 'R':
                newPosition[0] += steps;
                break;
            default:
                Console.WriteLine("Invalid direction command.");
                return;
        }

        MoveCaterpillar(newPosition);
        LogCommand(direction, steps);
    }

    private void MoveCaterpillar(int[] newPosition)
    {
        // Move caterpillar to the new position and update planet grid
        // Implementation of caterpillar movement logic from mission brief
        // Update head position
        planetGrid[headPosition[0], headPosition[1]] = emptySquare;
        headPosition = newPosition;
        planetGrid[headPosition[0], headPosition[1]] = head;

        // Update tail position if necessary
        int[] tailNewPosition = new int[] { tailPosition[0], tailPosition[1] };
        if (CalculateDistance(headPosition, tailPosition) > 1)
        {
            int deltaX = headPosition[0] - tailPosition[0];
            int deltaY = headPosition[1] - tailPosition[1];
            if (deltaX != 0)
            {
                tailNewPosition[0] += Math.Sign(deltaX);
            }
            else if (deltaY != 0)
            {
                tailNewPosition[1] += Math.Sign(deltaY);
            }
        }
        tailPosition = tailNewPosition;
        planetGrid[tailPosition[0], tailPosition[1]] = tail;
    }

    private int CalculateDistance(int[] position1, int[] position2)
    {
        // Calculate Manhattan distance between two positions
        return Math.Abs(position1[0] - position2[0]) + Math.Abs(position1[1] - position2[1]);
    }

    // New method to save logs to a file
    private void SaveLogsToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(logFileName))
            {
                foreach (string command in commandLog)
                {
                    writer.WriteLine(command);
                }
            }
            Console.WriteLine("Command logs saved to file: " + logFileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving command logs to file: " + ex.Message);
        }
    }

    // Updated method to log commands
    private void LogCommand(char direction, int steps)
    {
        string command = $"{direction} {steps}";
        commandLog.Add(command);
        undoLog.Push(command);
        redoLog.Clear();
        SaveLogsToFile(); // Call the new method to save logs to file
    }

    public void UndoLastCommand()
    {
        // Undo the last command
        if (commandLog.Count > 0)
        {
            string lastCommand = commandLog[commandLog.Count - 1];
            commandLog.RemoveAt(commandLog.Count - 1);
            undoLog.Pop(); // Pop the last command from the undo log
            redoLog.Push(lastCommand); // Push onto redo log for potential redoing

            // Reverse the last command's effect
            char direction = lastCommand[0];
            int steps = int.Parse(lastCommand.Substring(2));
            switch (direction)
            {
                case 'U':
                    ExecuteCommand('D', steps); // Move to the opposite dir
                    break;
                case 'D':
                    ExecuteCommand('U', steps);
                    break;
                case 'L':
                    ExecuteCommand('R', steps);
                    break;
                case 'R':
                    ExecuteCommand('L', steps);
                    break;
            }
        }
        else
        {
            Console.WriteLine("No commands to undo.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public void RedoLastCommand()
    {
        // Redo the last undone command
        if (redoLog.Count > 0)
        {
            string lastUndoneCommand = redoLog.Pop();
            commandLog.Add(lastUndoneCommand);
            undoLog.Push(lastUndoneCommand); // Push onto undo log
            ExecuteCommand(lastUndoneCommand[0], int.Parse(lastUndoneCommand.Substring(2))); // Execute the command
        }
        else
        {
            Console.WriteLine("No commands to redo.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public void DisplayPlanet()
    {
        // Display planet grid with radar image
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Console.Write(planetGrid[j, i]); // Swap indices to correctly display grid
            }
            Console.WriteLine();
        }
    }

    public void DisplayCaterpillar()
    {
        // Display caterpillar's position on the planet's grid
        DisplayPlanet(); // Reuse DisplayPlanet to also show caterpillar
    }

    //a simple user interface.... just something basic...
    public void Run()
    {
        while (true)
        {
            Console.Clear(); // We call the Clear function to clear the console for a cleaner display
            DisplayPlanet();
            DisplayCaterpillar();

            Console.WriteLine("\nCommands:");
            Console.WriteLine("1. Move Right (R)");
            Console.WriteLine("2. Move Left (L)");
            Console.WriteLine("3. Move Up (U)");
            Console.WriteLine("4. Move Down (D)");
            Console.WriteLine("5. Undo Last Move (Z)");
            Console.WriteLine("6. Redo Last Move (Y)");
            Console.WriteLine("7. Exit (X)");

            Console.Write("\nEnter your choice: ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine(); // We Move to the next line after user input

            switch (choice)
            {
                case 'R':
                case 'L':
                case 'U':
                case 'D':
                    Console.Write("Enter number of steps: ");
                    if (int.TryParse(Console.ReadLine(), out int steps))
                    {
                        ExecuteCommand(choice, steps);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input for steps.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    break;
                case 'Z':
                    UndoLastCommand();
                    break;
                case 'Y':
                    RedoLastCommand();
                    break;
                case 'X':
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        CaterpillarControlSystem controlSystem = new CaterpillarControlSystem();
        controlSystem.Run();
    }
}
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome
// terminate done by jipheens wahome