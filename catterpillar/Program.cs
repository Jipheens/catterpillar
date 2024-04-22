using System;
using System.Collections.Generic;

public class CaterpillarControlSystem
{


    static void Main(string[] args)
    {
        CaterpillarControlSystem controlSystem = new CaterpillarControlSystem();

        
        controlSystem.ExecuteCommand('R', 4);
        controlSystem.ExecuteCommand('U', 4);
        controlSystem.ExecuteCommand('L', 3);
        controlSystem.ExecuteCommand('D', 1);
        controlSystem.ExecuteCommand('R', 4);
        controlSystem.ExecuteCommand('D', 1);
        controlSystem.ExecuteCommand('L', 5);
        controlSystem.ExecuteCommand('R', 2);

        controlSystem.DisplayPlanet();
        controlSystem.DisplayCaterpillar();
    }
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
    private const char spice = '$';
    private const char booster = 'B';
    private const char obstacle = '#';
    private const char head = 'H';
    private const char tail = 'T';

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
        // i am initialize planet grid with empty squares
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                planetGrid[i, j] = emptySquare;
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

    private void LogCommand(char direction, int steps)
    {
        // Log the executed command
        string command = $"{direction} {steps}";
        commandLog.Add(command);
        undoLog.Push(command); // Push onto undo log for potential undoing
        redoLog.Clear(); // Clear redo log as a new command was executed
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
                    ExecuteCommand('D', steps); // Move opposite direction
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

}
