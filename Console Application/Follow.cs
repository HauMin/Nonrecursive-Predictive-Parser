using System;
using System.Collections.Generic;
using System.Linq;

namespace Nonrecursive_Predictive_Parser
{
    internal class Follow
    {
        private readonly List<Container> _varlist;
        private readonly Stack<string> _stack;

        public Follow(List<Container> varlist)
        {
            _varlist = varlist;
            _stack = new Stack<string>();
            FollowFunction();

            foreach (var v in varlist)
            {
                Console.Write("follow({0})->   ", v.Var);
                foreach (var f in v.Follow)
                {
                    Console.Write(f + " ");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine();
        }

        private List<string> GetfirstList(string var)
        {
            return (from v in _varlist where v.Var.Equals(var) select v.Fast).FirstOrDefault();
        }

        private bool findESP(List<string> f)
        {
            return f.Any(i => i.Equals("~"));
        }

        private void FollowFunction()
        {
            _varlist[0].Follow.Add("$");
            foreach (var v in _varlist)
            {
                if (!v.Var.Equals(_varlist[0].Var) && v.Follow.Count != 0) continue;
                _stack.Push(v.Var);
                while (!_stack.Count.Equals(0))
                {

                    var b = _stack.Pop();
                    
                    foreach (var i in _varlist)
                    {
                       
                        for (var j = 0; j < (i.SplitStrings.Length - 1); j++)
                        {
                            if (b != i.SplitStrings[j]) continue;
                            

                            if (!i.SplitStrings[j + 1].Equals("\0") && !i.SplitStrings[j + 1].Equals("|")) // checking @Bb rule 
                            {
                                var flist = GetfirstList(i.SplitStrings[j + 1]);
                                if (flist == null)                                          //if  i.SplitStrings[j + 1] is a terminal
                                {
                                    if (!v.Follow.Contains(i.SplitStrings[j + 1]))
                                    {
                                        v.Follow.Add(i.SplitStrings[j + 1]);
                                    }
                                }
                                else if (!findESP(flist))           //if no ~
                                {
                                    foreach (var f1 in flist)
                                    {
                                        if (!v.Follow.Contains(f1))
                                        {
                                            v.Follow.Add(f1);
                                        }
                                    }
                                }
                                else if (i.Follow.Count != 0)        //appling 2nd and 3rd rule
                                {
                                    foreach (var f in flist)               
                                    {
                                        if (!f.Equals("~") && !v.Follow.Contains(f))
                                        {
                                            v.Follow.Add(f);
                                        }
                                    }
                                    foreach (var f in i.Follow)
                                    {
                                        if (!v.Follow.Contains(f))
                                        {
                                            v.Follow.Add(f);
                                        }
                                      
                                    }
                                }
                                else
                                {
                                    if (!_stack.Contains(i.Var))
                                    {
                                        _stack.Push(i.Var);
                                    }
                                }
                            }
                            else                                        // checking @B rule
                            {
                                if (i.Follow.Count != 0)
                                {
                                    foreach (var f in i.Follow)
                                    {
                                        if (!v.Follow.Contains(f))
                                        {
                                            v.Follow.Add(f);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!_stack.Contains(i.Var))
                                    {
                                        _stack.Push(i.Var);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
