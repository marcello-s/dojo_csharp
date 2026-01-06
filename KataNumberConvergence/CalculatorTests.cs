#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataNumberConvergence;

[TestFixture]
public class CalculatorTests
{
    private Calculator calculator = null!;
    private const double PiSquared = Math.PI * Math.PI;
    private IList<string> results = null!;

    [SetUp]
    public void Setup()
    {
        calculator = new Calculator();
        results = new List<string>();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(calculator, Is.InstanceOf<Calculator>());
    }

    [Test]
    public void Sqrt_ValueIs9_Returns3()
    {
        var sqrt = Calculator.Sqrt(9.0);
        Console.Out.WriteLine("sqrt of 9 = {0}", sqrt);
        Assert.That(sqrt, Is.EqualTo(3.0));
    }

    [Test]
    public void Sqrt_ValuePiSquared_ReturnsPi()
    {
        var sqrt = Calculator.Sqrt(PiSquared);
        Console.Out.WriteLine("sqrt of PI^2 = {0}", sqrt);
    }

    private static void PrintResultList(IEnumerable<string> results)
    {
        foreach (var result in results)
        {
            Console.Out.WriteLine(result);
        }
    }

    [Test]
    public void SqrtWriteToConsoleLockOnWriteLine_WriteResult()
    {
        var results = new List<string>();
        calculator.SqrtToListLockOnList(PiSquared, results);
        PrintResultList(results);
    }

    private void CalculateSqrt()
    {
        CalculateSqrtForValue(calculator, results, 10, PiSquared);
    }

    private void CalculateSqrtForValue(
        Calculator calculator,
        IList<string> results,
        int times,
        double value
    )
    {
        for (var i = 0; i < times; i++)
        {
            calculator.SqrtToListLockOnList(value, results);
        }
    }

    [Test]
    public void SqrtWriteToConsoleLockOnWriteLine_On10Tasks_WriteResult()
    {
        const int numberOfTimes = 10;
        const int numberOfTasks = 10;
        var tasks = new Task[numberOfTasks];
        var results = new List<string>();
        for (var i = 0; i < numberOfTasks; i++)
        {
            tasks[i] = Task.Factory.StartNew(() =>
                CalculateSqrtForValue(calculator, results, numberOfTimes, PiSquared)
            );
        }

        Task.WaitAll(tasks);
        foreach (var task in tasks)
        {
            task.Dispose();
        }

        PrintResultList(results);
    }

    [Test]
    public void SqrtWriteToConsoleLockOnWriteLine_On10Threads_WriteResult()
    {
        const int numberOfThreads = 10;
        var threads = new Thread[numberOfThreads];
        for (var i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(CalculateSqrt);
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        PrintResultList(results);
    }

    [Test]
    public void SqrtWriteToConsoleLockOnWriteLine_NewInstanceOn100Tasks_WriteResult()
    {
        const int numberOfTasks = 100;
        var tasks = new Task[numberOfTasks];
        var results = new List<string>();
        for (var i = 0; i < numberOfTasks; i++)
        {
            var calculator = new Calculator();
            tasks[i] = Task.Factory.StartNew(() =>
                calculator.SqrtToListLockOnList(PiSquared, results)
            );
        }

        Task.WaitAll(tasks);
        foreach (var task in tasks)
        {
            task.Dispose();
        }

        PrintResultList(results);
    }
}
