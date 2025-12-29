#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Compiler;

class Scope
{
    private MethodScope? current;
    private readonly IList<Assignment> assignments;
    private readonly IDictionary<int, Constant> constants;
    private int constantKeyMax;

    public IEnumerable<Assignment> Assignments
    {
        get { return assignments; }
    }

    public IDictionary<int, Constant> Constants
    {
        get { return constants; }
    }

    public bool IsAllocationMode { get; private set; }

    public Scope()
    {
        Push();
        ResetAllocationMode();
        assignments = new List<Assignment>();
        constants = new Dictionary<int, Constant>();
    }

    public void Push()
    {
        current = new MethodScope(current);
    }

    public void Pop()
    {
        current = current?.Parent;
    }

    public void SetAllocationMode()
    {
        IsAllocationMode = true;
    }

    public void ResetAllocationMode()
    {
        IsAllocationMode = false;
    }

    public bool Define(string identifier, IExpression expr)
    {
        if (current == null)
        {
            return false;
        }

        return current.Define(identifier, expr);
    }

    public IExpression? Lookup(string identifier)
    {
        var item = current != null ? current.Lookup(identifier) : null;
        return item != null ? item.Expr : null;
    }

    public Assignment Assign(IExpression left, IExpression right)
    {
        var assignment = new Assignment(left, right);
        assignments.Add(assignment);

        return assignment;
    }

    public int AddConstant(object value, ConstantType type)
    {
        if (
            !constants.Any(c =>
                c.Value.Value.ToString() == value.ToString() && c.Value.Type == type
            )
        )
        {
            var key = constantKeyMax;
            constants.Add(key, new Constant(value, type));
            ++constantKeyMax;
            return key;
        }

        return constants
            .Single(c => c.Value.Value.ToString() == value.ToString() && c.Value.Type == type)
            .Key;
    }

    class MethodScope(MethodScope? parent)
    {
        private readonly MethodScope? parent = parent;
        private readonly IList<NamedItem> items = new List<NamedItem>();

        public MethodScope? Parent
        {
            get { return parent; }
        }

        public IEnumerable<NamedItem> Items
        {
            get { return items; }
        }

        public bool Define(string identifier, IExpression expr)
        {
            if (identifier == null)
            {
                identifier = string.Empty;
            }

            if (items.Any(i => i.Name.Equals(identifier)))
            {
                return false;
            }

            items.Add(new NamedItem(identifier, expr));
            return true;
        }

        public NamedItem? Lookup(string identifier)
        {
            return LookupRecursive(this, identifier);
        }

        private static NamedItem? LookupRecursive(MethodScope? scope, string identifier)
        {
            if (scope == null)
            {
                return null;
            }

            if (!scope.Items.Any(i => i.Name.Equals(identifier)))
            {
                return LookupRecursive(scope.Parent, identifier);
            }

            return scope.Items.Single(i => i.Name.Equals(identifier));
        }
    }

    class NamedItem(string name, IExpression expr)
    {
        public string Name { get; private set; } = name;
        public IExpression Expr { get; private set; } = expr;
    }

    public class Assignment(IExpression left, IExpression right)
    {
        public IExpression Left { get; private set; } = left;
        public IExpression Right { get; private set; } = right;
    }

    public class Pair(string key, object obj)
    {
        public string Key { get; private set; } = key;
        public object Obj { get; private set; } = obj;
    }

    public class Constant(object value, ConstantType type)
    {
        public object Value { get; private set; } = value;
        public ConstantType Type { get; private set; } = type;
    }
}
