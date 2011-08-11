public class MazeDriver : ISimulationDriver
{
    private IFluidSimulation _simulation;
    private int _mazeSize = 64;

    public MazeDriver(IFluidSimulation simulation)
    {
        _simulation = simulation;

        for (int y = 0; y < _simulation.Height; ++y)
        {
            for (int x = 0; x < _simulation.Width; ++x)
            {
                _simulation.SetHeight(x, y, 1.0f);
                _simulation.SetSolid(x, y, true);
            }
        }

        int[,] maze = new int[_mazeSize, _mazeSize];
        for (int y = 0; y < _mazeSize; y += 2)
        {
            for (int x = 0; x < _mazeSize; x += 2)
            {
                maze[x,y] = 1;
            }
        }

        int numWalls = 2 * (_mazeSize/2) * ((_mazeSize/2) - 1);
        int[] walls = new int[numWalls];
        for (int i = 0; i < numWalls; ++i)
        {
            walls[i] = i;
        }
        System.Random rng = new System.Random();
        for (int i = 0; i < numWalls-1; ++i)
        {
            int swapWith = rng.Next(numWalls - i) + i;
            if (swapWith != i)
            {
                int old = walls[i];
                walls[i] = walls[swapWith];
                walls[swapWith] = old;
            }
        }

        maze[_mazeSize/4 * 2, _mazeSize/4 * 2] = 2;
        int count = (_mazeSize/2) * (_mazeSize/2) - 1;
        int pos = 0;
        while (count > 0)
        {
            ++pos;
            if (pos >= numWalls) pos = 0;

            int x, y, dx, dy;
            if (walls[pos] < numWalls / 2)
            {
                x = 2 * (walls[pos] % (_mazeSize/2));
                y = 1 + 2 * (walls[pos] / (_mazeSize/2));
                dx = 0;
                dy = 1;
            }
            else
            {
                y = 2 * ((walls[pos] - numWalls/2) % (_mazeSize/2));
                x = 1 + 2 * ((walls[pos] - numWalls/2) / (_mazeSize/2));
                dy = 0;
                dx = 1;
            }
            if (maze[x, y] != 0)
                continue;
            if (maze[x+dx, y+dy] != 2 && maze[x-dx, y-dy] != 2)
                continue;
            if (maze[x+dx, y+dy] == 2 && maze[x-dx, y-dy] == 2)
                continue;

            maze[x+dx, y+dy] = 2;
            maze[x-dx, y-dy] = 2;
            maze[x, y] = 2;
            --count;
        }

        for (int y = 0; y < _mazeSize; ++y)
        {
            for (int x = 0; x < _mazeSize; ++x)
            {
                if (maze[x, y] != 0)
                    ClearSquare(x, y);
            }
        }
    }

    public void Update()
    {
        _simulation.AddWater(CellToSim(_mazeSize/2 + 0.5f), CellToSim(_mazeSize/2 + 0.5f), 2.0f);
    }

    int CellToSim(float x)
    {
        return (int)(_simulation.Width/(_mazeSize+1)/2 + _simulation.Width * x / (_mazeSize+1));
    }

    void ClearSquare(int x, int y)
    {
        int sim_x = CellToSim(x);
        int sim_y = CellToSim(y);

        int sim_w = (CellToSim(x+1) - sim_x) * 12 / 10;

        for (int dx = 0; dx <= sim_w; ++dx)
        {
            for (int dy = 0; dy <= sim_w; ++dy)
            {
                _simulation.SetHeight(sim_x + dx, sim_y + dy, 0.0f);
                _simulation.SetSolid(sim_x + dx, sim_y + dy, false);
            }
        }
    }
}
