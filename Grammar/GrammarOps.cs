using Grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
	public class GrammarOps
	{
		public GrammarOps(IGrammar g)
		{
			this.g = g;
			compute_empty();
      compute_first();
		}

		public ISet<Nonterminal> EmptyNonterminals { get; } = new HashSet<Nonterminal>();
    public Dictionary<Nonterminal, HashSet<Symbol>> FirstSet { get; } = new Dictionary<Nonterminal, HashSet<Symbol>>();
    private void compute_empty()
		{
      foreach (var rule in g.Rules)
      {
        if (rule.RHS.Count == 0) EmptyNonterminals.Add(rule.LHS);
      }
      int count;
      do
      {
        count = EmptyNonterminals.Count;
        foreach (var rule in g.Rules)
          if (rule.RHS.All(x => x is Nonterminal && EmptyNonterminals.Contains(x))) EmptyNonterminals.Add(rule.LHS);
      } while (count != EmptyNonterminals.Count);

    }

    private void compute_first()
    {
      foreach (var rule in g.Rules)
      {
        FirstSet[rule.LHS] = new HashSet<Symbol>();
      }

      foreach (var rule in g.Rules)
      {
        if (rule.RHS.Count == 0)
          continue;
        Stack<Rule> stack = new Stack<Rule>();
        var right = rule.RHS[0];
        FirstSet[rule.LHS].Add(right);
        if(EmptyNonterminals.Contains(right) && rule.RHS.Count > 1)
          FirstSet[rule.LHS].Add(rule.RHS[1]);
      }

      bool change;
      do
      {
        change = false;
        foreach (var kv in FirstSet)
        {
          var nts = kv.Value.Where(x => x.GetType() == typeof(Nonterminal));
          List<Symbol> additionalSymbols = new List<Symbol>();
          foreach (var nt in nts)
          {
            foreach(var x in FirstSet[(Nonterminal)nt])
            {
              if (!kv.Value.Contains(x))
              {
                additionalSymbols.Add(x);
                change = true;
              }
            }
          }
          
          foreach(Symbol s in additionalSymbols)
            kv.Value.Add(s);
        }
      } while (change);
    }

  private IGrammar g;
	}
}
