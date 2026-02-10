#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib;

public class ResultEnumerator(IEnumerable<IResult> children)
{
    private readonly IEnumerator<IResult> enumerator = children.GetEnumerator();

    public void Enumerate()
    {
        ChildCompleted(null, EventArgs.Empty);
    }

    private void ChildCompleted(object? sender, EventArgs e)
    {
        var previous = sender as IResult;
        if (previous != null)
        {
            previous.Completed -= ChildCompleted;
        }

        if (!enumerator.MoveNext())
        {
            return;
        }

        var next = enumerator.Current;
        if (next == null)
        {
            return;
        }

        next.Completed += ChildCompleted;
        next.Execute();
    }
}
