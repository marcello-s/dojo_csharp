#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSudoku;

public class SudokuSolver
{
    private readonly IList<IEnumerable<int>> solvedPuzzles = new List<IEnumerable<int>>();

    public IEnumerable<IEnumerable<int>> Solve(IEnumerable<int> puzzle)
    {
        /*
         * 1) Start at puzzle index -1
         * 2) Get next free index
         * 3) Try candidates [1..9], if match proceed else return to try other match
         * 4) Recurse with step 2, until index is 80
         */

        // 1) Start at puzzle index -1
        var puzzleClone = puzzle.ToList();
        Solve(puzzleClone, -1);
        return solvedPuzzles;
    }

    private bool Solve(IList<int> puzzle, int index)
    {
        if (index == 80)
        {
            return true;
        }

        // 2) Get next free index
        var nextFree = GetNextFreeIndex(puzzle, index);

        // 3) Try candidates [1..9], if match proceed else return to try other candidate
        //for (int candidate = 1; candidate <= 9; candidate++ )
        var candidate = 1;
        while ((candidate = LookForCandidate(puzzle, nextFree, candidate)) <= 9)
        {
            //if(AcceptCandidate(puzzle, nextFree, candidate))
            //{
            // 4) Recurse with step 2, until index is 80
            puzzle[nextFree] = candidate;
            //Console.WriteLine(string.Format("index: {0} nextFree: {1} candidate: {2}", index, nextFree, candidate));
            if (Solve(puzzle, nextFree))
            {
                solvedPuzzles.Add(puzzle.ToList());
            }
            //}
        }
        //Console.WriteLine(string.Format("DEADEND index: {0} nextFree: {1}", index, nextFree));
        puzzle[nextFree] = 0;
        return false;
    }

    public static bool AcceptCandidate(IEnumerable<int> puzzle, int index, int candidate)
    {
        if (ColumnContainsNumber(puzzle, GetColumnFromIndex(index), candidate))
        {
            return false;
        }

        if (RowContainsNumber(puzzle, GetRowFromIndex(index), candidate))
        {
            return false;
        }

        if (SquareContainsNumber(puzzle, GetSquareFromIndex(index), candidate))
        {
            return false;
        }

        return true;
    }

    public static int LookForCandidate(IEnumerable<int> puzzle, int index, int proposedCandidate)
    {
        var candidate = proposedCandidate;
        while (
            ColumnContainsNumber(puzzle, GetColumnFromIndex(index), candidate)
            || RowContainsNumber(puzzle, GetRowFromIndex(index), candidate)
            || SquareContainsNumber(puzzle, GetSquareFromIndex(index), candidate) && candidate <= 9
        )
        {
            candidate++;
        }

        return candidate;
    }

    public static int GetRowFromIndex(int index)
    {
        return index / 9;
    }

    public static int GetColumnFromIndex(int index)
    {
        return index % 9;
    }

    public static int GetSquareFromIndex(int index)
    {
        return (GetRowFromIndex(index) / 3 % 3) * 3 + (GetColumnFromIndex(index) / 3 % 3);
    }

    public static bool ColumnContainsNumber(IEnumerable<int> puzzle, int column, int number)
    {
        for (var i = 0; i < 9; i++)
        {
            if (puzzle.ElementAt(i * 9 + column).Equals(number))
            {
                return true;
            }
        }

        return false;
    }

    public static bool RowContainsNumber(IEnumerable<int> puzzle, int row, int number)
    {
        for (var i = 0; i < 9; i++)
        {
            if (puzzle.ElementAt(row * 9 + i).Equals(number))
            {
                return true;
            }
        }

        return false;
    }

    public static bool SquareContainsNumber(IEnumerable<int> puzzle, int square, int number)
    {
        var squareOffset = ((square / 3) * 27) + ((square % 3) * 3);
        for (var j = 0; j < 3; j++)
        {
            for (var i = 0; i < 3; i++)
            {
                if (puzzle.ElementAt(j * 9 + i + squareOffset).Equals(number))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static int GetNextFreeIndex(IEnumerable<int> puzzle, int currentIndex)
    {
        var counter = 1;
        while (
            currentIndex + counter < 9 * 9 && !puzzle.ElementAt(currentIndex + counter++).Equals(0)
        ) { }

        return currentIndex + counter - 1;
    }
}
