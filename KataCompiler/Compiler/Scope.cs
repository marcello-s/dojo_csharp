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
    private MethodScope _current;
    private readonly IList<Assignment> _assignments;
    private readonly IDictionary<int, Constant> _constants;
    private int _constantKeyMax;

    public IEnumerable<Assignment> Assignments
    {
        get { return _assignments; }
    }
    public IDictionary<int, Constant> Constants
    {
        get { return _constants; }
    }

    public bool IsAllocationMode { get; private set; }

    public Scope()
    {
        Push();
        ResetAllocationMode();
        _assignments = new List<Assignment>();
        _constants = new Dictionary<int, Constant>();
    }

    public void Push()
    {
        _current = new MethodScope(_current);
    }

    public void Pop()
    {
        _current = _current.Parent;
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
        return _current.Define(identifier, expr);
    }

    public IExpression Lookup(string identifier)
    {
        var item = _current.Lookup(identifier);
        return item != null ? item.Expr : null;
    }

    public Assignment Assign(IExpression left, IExpression right)
    {
        var assignment = new Assignment(left, right);
        _assignments.Add(assignment);

        return assignment;
    }

    public int AddConstant(object value, ConstantType type)
    {
        if (
            !_constants.Any(c =>
                c.Value.Value.ToString() == value.ToString() && c.Value.Type == type
            )
        )
        {
            var key = _constantKeyMax;
            _constants.Add(key, new Constant(value, type));
            ++_constantKeyMax;
            return key;
        }

        return _constants
            .Single(c => c.Value.Value.ToString() == value.ToString() && c.Value.Type == type)
            .Key;
    }

    class MethodScope
    {
        private readonly MethodScope _parent;
        private readonly IList<NamedItem> _items;

        public MethodScope Parent
        {
            get { return _parent; }
        }
        public IEnumerable<NamedItem> Items
        {
            get { return _items; }
        }

        public MethodScope(MethodScope parent)
        {
            _parent = parent;
            _items = new List<NamedItem>();
        }

        public bool Define(string identifier, IExpression expr)
        {
            if (identifier == null)
            {
                identifier = string.Empty;
            }

            if (_items.Any(i => i.Name.Equals(identifier)))
            {
                return false;
            }

            _items.Add(new NamedItem(identifier, expr));
            return true;
        }

        public NamedItem Lookup(string identifier)
        {
            return LookupRecursive(this, identifier);
        }

        private static NamedItem LookupRecursive(MethodScope scope, string identifier)
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

    class NamedItem
    {
        public string Name { get; private set; }
        public IExpression Expr { get; private set; }

        public NamedItem(string name, IExpression expr)
        {
            Name = name;
            Expr = expr;
        }
    }

    public class Assignment
    {
        public IExpression Left { get; private set; }
        public IExpression Right { get; private set; }

        public Assignment(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
    }

    public class Pair
    {
        public string Key { get; private set; }
        public object Obj { get; private set; }

        public Pair(string key, object obj)
        {
            Key = key;
            Obj = obj;
        }
    }

    public class Constant
    {
        public object Value { get; private set; }
        public ConstantType Type { get; private set; }

        public Constant(object value, ConstantType type)
        {
            Value = value;
            Type = type;
        }
    }
}
