using System;
using System.Collections.Generic;
using System.Linq;

namespace Nonrecursive_Predictive_Parser
{
    internal class Fast
    {
        private readonly List<Container> _varlist;
        private readonly Stack<string> _stack;
        private readonly List<string> _temp;

        public Fast(List<Container> varlist)
        {
            _varlist = varlist;
            _stack = new Stack<string>();

            foreach (var i in varlist)
            {
                _temp = new List<string>();

                FindFast(i.SplitStrings, i);
                foreach (var t in _temp)
                {
                    if (!i.Fast.Contains(t))
                    {
                        i.Fast.Add(t);
                    }
                }

                i.Fast.Reverse();

                Console.Write("first({0})->   ", i.Var);
                foreach (var f in i.Fast)
                {
                    Console.Write(f + " ");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine();
        }


        private bool FindOr(string[] s)
        {
            return s.Any(i => i.Equals("|"));
        }

        private int VarLoc(string s)
        {
            foreach (var i in _varlist.Where(i => i.Var.Equals(s)))
            {
                return _varlist.IndexOf(i);
            }
            return -1;
        }

        private bool FindVar(string s)
        {
            return _varlist.Any(i => i.Var.Equals(s));
        }

        private void FindFast(string[] s, Container iContainer)
        {
            if (!FindOr(s) && !iContainer.Var.Equals(iContainer.SplitStrings[0]))
            {
                _stack.Push(s[0]);
                GetTer(iContainer);
            }
            else
            {
                for (var i = 0; i < s.Length; i++)
                {
                    if (i == 0 && !iContainer.Var.Equals(iContainer.SplitStrings[0]))
                    {
                        _stack.Push(s[i]);
                    }
                    else if (s[i] == "|")
                    {
                        _stack.Push(s[i + 1]);
                    }
                }
                if (_stack.Count != 0)
                {
                    GetTer(iContainer);
                }
            }
        }

        private void GetTer(Container iContainer)
        {
            var st = _stack.Pop();

            if (FindVar(st))
            {
                int loc = VarLoc(st);
                if (loc != -1)
                {
                    if (!st.Equals(iContainer.Var))
                    {
                        FindFast(_varlist[loc].SplitStrings, iContainer);
                    }
                }
            }
            else
            {
                _temp.Add(st);
            }
            if (_stack.Count != 0)
            {
                GetTer(iContainer);
            }
        }
    }
}
