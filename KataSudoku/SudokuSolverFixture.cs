#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2020 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSudoku;

[TestFixture]
public class SudokuSolverFixture
{
    private SudokuSolver solver = null!;

    [SetUp]
    public void Setup()
    {
        solver = new SudokuSolver();
    }

    [Test]
    public void SolveTest()
    {
        var puzzle = CreatePuzzle1();
        PrintPuzzle(puzzle);
        var solvedPuzzles = solver.Solve(puzzle);
        //PrintPuzzle(CreateSolution1());
        foreach (var solvedPuzzle in solvedPuzzles)
        {
            PrintPuzzle(solvedPuzzle);
        }

        Assert.That(solvedPuzzles.FirstOrDefault(), Is.EqualTo(CreateSolution1()));
    }

    [Test]
    public void GetRowFromIndexTest()
    {
        Assert.That(SudokuSolver.GetRowFromIndex(10), Is.EqualTo(1));
        Assert.That(SudokuSolver.GetRowFromIndex(78), Is.EqualTo(8));
    }

    [Test]
    public void GetColumnFromIndexTest()
    {
        Assert.That(SudokuSolver.GetColumnFromIndex(1), Is.EqualTo(1));
        Assert.That(SudokuSolver.GetColumnFromIndex(70), Is.EqualTo(7));
    }

    [Test]
    public void GetSquareFromIndexTest()
    {
        Assert.That(SudokuSolver.GetSquareFromIndex(18), Is.EqualTo(0));
        Assert.That(SudokuSolver.GetSquareFromIndex(41), Is.EqualTo(4));
    }

    [Test]
    public void ColumnContainsNumberTest()
    {
        var puzzle = CreatePuzzle1();
        Assert.That(SudokuSolver.ColumnContainsNumber(puzzle, 4, 2), Is.True);
        Assert.That(SudokuSolver.ColumnContainsNumber(puzzle, 7, 7), Is.True);
        Assert.That(SudokuSolver.ColumnContainsNumber(puzzle, 3, 5), Is.False);
    }

    [Test]
    public void RowContainsNumberTest()
    {
        var puzzle = CreatePuzzle1();
        Assert.That(SudokuSolver.RowContainsNumber(puzzle, 3, 6), Is.True);
        Assert.That(SudokuSolver.RowContainsNumber(puzzle, 6, 2), Is.True);
        Assert.That(SudokuSolver.RowContainsNumber(puzzle, 2, 1), Is.False);
    }

    [Test]
    public void SquareContainsNumberTest()
    {
        var puzzle = CreatePuzzle1();
        Assert.That(SudokuSolver.SquareContainsNumber(puzzle, 0, 9), Is.True);
        Assert.That(SudokuSolver.SquareContainsNumber(puzzle, 5, 1), Is.True);
        Assert.That(SudokuSolver.SquareContainsNumber(puzzle, 8, 3), Is.False);
    }

    [Test]
    public void GetNextFreeIndexTest()
    {
        //var current = 0;
        //while (current < (current = GetNextFreeIndex(puzzle, current)))
        //    Console.WriteLine(current + " ");

        var puzzle = CreatePuzzle1();
        Assert.That(SudokuSolver.GetNextFreeIndex(puzzle, 11), Is.EqualTo(15));
        Assert.That(SudokuSolver.GetNextFreeIndex(puzzle, -1), Is.EqualTo(0)); // edge case 0
        Assert.That(SudokuSolver.GetNextFreeIndex(puzzle, 80), Is.EqualTo(80)); // edge case 80
    }

    private static void PrintPuzzle(IEnumerable<int> puzzle)
    {
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                Console.Write(puzzle.ElementAt(i * 9 + j) + " ");
            }

            Console.Write("\r\n");
        }

        Console.WriteLine();
    }

    private static IEnumerable<int> CreatePuzzle1()
    {
        var puzzle = new List<int>(9 * 9);
        for (var i = 0; i < 9 * 9; i++)
        {
            puzzle.Add(0);
        }

        puzzle[1] = 3;
        puzzle[12] = 1;
        puzzle[13] = 9;
        puzzle[14] = 5;
        puzzle[19] = 9;
        puzzle[20] = 8;
        puzzle[25] = 6;
        puzzle[27] = 8;
        puzzle[31] = 6;
        puzzle[36] = 4;
        puzzle[41] = 3;
        puzzle[44] = 1;
        puzzle[49] = 2;
        puzzle[55] = 6;
        puzzle[60] = 2;
        puzzle[61] = 8;
        puzzle[66] = 4;
        puzzle[67] = 1;
        puzzle[68] = 9;
        puzzle[71] = 5;
        puzzle[79] = 7;
        return puzzle;
    }

    private static IEnumerable<int> CreateSolution1()
    {
        var puzzle = new List<int>(9 * 9);
        for (var i = 0; i < 9 * 9; i++)
        {
            puzzle.Add(0);
        }

        puzzle[0] = 5;
        puzzle[1] = 3;
        puzzle[2] = 4;
        puzzle[3] = 6;
        puzzle[4] = 7;
        puzzle[5] = 8;
        puzzle[6] = 9;
        puzzle[7] = 1;
        puzzle[8] = 2;

        puzzle[9] = 6;
        puzzle[10] = 7;
        puzzle[11] = 2;
        puzzle[12] = 1;
        puzzle[13] = 9;
        puzzle[14] = 5;
        puzzle[15] = 3;
        puzzle[16] = 4;
        puzzle[17] = 8;

        puzzle[18] = 1;
        puzzle[19] = 9;
        puzzle[20] = 8;
        puzzle[21] = 3;
        puzzle[22] = 4;
        puzzle[23] = 2;
        puzzle[24] = 5;
        puzzle[25] = 6;
        puzzle[26] = 7;

        puzzle[27] = 8;
        puzzle[28] = 5;
        puzzle[29] = 9;
        puzzle[30] = 7;
        puzzle[31] = 6;
        puzzle[32] = 1;
        puzzle[33] = 4;
        puzzle[34] = 2;
        puzzle[35] = 3;

        puzzle[36] = 4;
        puzzle[37] = 2;
        puzzle[38] = 6;
        puzzle[39] = 8;
        puzzle[40] = 5;
        puzzle[41] = 3;
        puzzle[42] = 7;
        puzzle[43] = 9;
        puzzle[44] = 1;

        puzzle[45] = 7;
        puzzle[46] = 1;
        puzzle[47] = 3;
        puzzle[48] = 9;
        puzzle[49] = 2;
        puzzle[50] = 4;
        puzzle[51] = 8;
        puzzle[52] = 5;
        puzzle[53] = 6;

        puzzle[54] = 9;
        puzzle[55] = 6;
        puzzle[56] = 1;
        puzzle[57] = 5;
        puzzle[58] = 3;
        puzzle[59] = 7;
        puzzle[60] = 2;
        puzzle[61] = 8;
        puzzle[62] = 4;

        puzzle[63] = 2;
        puzzle[64] = 8;
        puzzle[65] = 7;
        puzzle[66] = 4;
        puzzle[67] = 1;
        puzzle[68] = 9;
        puzzle[69] = 6;
        puzzle[70] = 3;
        puzzle[71] = 5;

        puzzle[72] = 3;
        puzzle[73] = 4;
        puzzle[74] = 5;
        puzzle[75] = 2;
        puzzle[76] = 8;
        puzzle[77] = 6;
        puzzle[78] = 1;
        puzzle[79] = 7;
        puzzle[80] = 9;
        return puzzle;
    }
}
