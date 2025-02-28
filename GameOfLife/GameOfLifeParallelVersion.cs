namespace GameOfLife;

/// <summary>
/// Represents Conway's Game of Life in a parallel version.
/// The class provides methods to simulate the game's evolution based on simple rules.
/// </summary>
public sealed class GameOfLifeParallelVersion
{
    private readonly bool[,] initialGrid;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameOfLifeParallelVersion"/> class with the specified number of rows and columns of the grid. The initial state of the grid is randomly set with alive or dead cells.
    /// </summary>
    /// <param name="rows">The number of rows in the grid.</param>
    /// <param name="columns">The number of columns in the grid.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the number of rows or columns is less than or equal to 0.</exception>
    public GameOfLifeParallelVersion(int rows, int columns)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(rows, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(columns, 0);

        this.GridGame = new bool[rows, columns];
        Random random = new Random();
        object locker = new object();

        _ = Parallel.For(0, this.GridGame.GetLength(0), (i) =>
        {
            _ = Parallel.For(0, this.GridGame.GetLength(1), (j) =>
            {
                lock (locker)
                {
                    this.GridGame[i, j] = random.Next(2) == 0;
                }
            });
        });

        this.initialGrid = (bool[,])this.GridGame.Clone();
        this.Generation = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameOfLifeParallelVersion"/> class with the given grid.
    /// </summary>
    /// <param name="grid">The 2D array representing the initial state of the grid.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <param name="grid"/> is null.</exception>
    public GameOfLifeParallelVersion(bool[,] grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        this.GridGame = (bool[,])grid.Clone();
        this.initialGrid = (bool[,])grid.Clone();

        this.Generation = 0;
    }

    /// <summary>
    /// Gets the current generation grid as a separate copy.
    /// </summary>
    public bool[,] CurrentGeneration
    {
        get
        {
            return this.GridGame;
        }
    }

    /// <summary>
    /// Gets the current generation number.
    /// </summary>
    public int Generation { get; private set; }

    private bool[,] GridGame { get; set; }

    /// <summary>
    /// Restarts the game by resetting the current grid to the initial state.
    /// </summary>
    public void Restart()
    {
        this.GridGame = (bool[,])this.initialGrid.Clone();
        this.Generation = 0;
    }

    /// <summary>
    /// Advances the game to the next generation based on the rules of Conway's Game of Life.
    /// </summary>
    public void NextGeneration()
    {
        int rows = this.GridGame.GetLength(0);
        int columns = this.GridGame.GetLength(1);
        bool[,] newGrid = new bool[rows, columns];

        _ = Parallel.For(0, rows, i =>
        {
            for (int j = 0; j < columns; j++)
            {
                int aliveNeighbors = this.CountAliveNeighbors(i, j);
                if (this.GridGame[i, j])
                {
                    newGrid[i, j] = aliveNeighbors == 2 || aliveNeighbors == 3;
                }
                else
                {
                    newGrid[i, j] = aliveNeighbors == 3;
                }
            }
        });

        this.GridGame = newGrid;
        this.Generation++;
    }

    /// <summary>
    /// Counts the number of alive neighbors for a given cell in the grid.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="column">The column index of the cell.</param>
    /// <returns>The number of alive neighbors for the specified cell.</returns>
    private int CountAliveNeighbors(int row, int column)
    {
        int count = 0;
        int rows = this.GridGame.GetLength(0);
        int columns = this.GridGame.GetLength(1);
        object lockObject = new object();

        _ = Parallel.For(-1, 2, (i) =>
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int newRow = row + i;
                int newColumn = column + j;

                if (newRow >= 0 && newRow < rows && newColumn >= 0 && newColumn < columns && this.GridGame[newRow, newColumn])
                {
                    lock (lockObject)
                    {
                        count++;
                    }
                }
            }
        });

        return count;
    }
}
