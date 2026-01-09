#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public class RulesEngine<T>(IEventAggregator events) : IHandle<T>
{
    private readonly List<Rule<T>> rules = new List<Rule<T>>();
    private int recursionLevel;
    private int handlingLevel;

    public void AddRule(Rule<T> rule)
    {
        lock (rules)
        {
            if (!rules.Contains(rule))
            {
                events.Unsubscribe(this);
                rules.Add(rule);
                recursionLevel++;
                events.Subscribe(this);
            }
        }
    }

    public void RemoveRule(Rule<T> rule)
    {
        lock (rules)
        {
            if (rules.Any() && rules.Contains(rule))
            {
                events.Unsubscribe(this);
                rules.Remove(rule);
                recursionLevel--;
                if (rules.Any())
                {
                    events.Subscribe(this);
                }
            }
        }
    }

    public void Handle(T message)
    {
        lock (rules)
        {
            // evaluate rules and publish their responses
            var responses = rules
                .Select(r => r.Evaluate(message))
                .Where(response => response != null)
                .ToList();

            if (handlingLevel >= recursionLevel)
            {
                return;
            }

            foreach (var response in responses)
            {
                handlingLevel++;
                if (response != null)
                {
                    events.Publish(response);
                }

                handlingLevel--;
            }
        }
    }
}
