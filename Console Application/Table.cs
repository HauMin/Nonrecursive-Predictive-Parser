using System;
using System.Collections.Generic;
using System.Linq;

namespace Nonrecursive_Predictive_Parser
{
    internal class Table
    {
        private readonly List<Container> _varlist;
        private readonly List<string> _allter;
        private readonly List<string> _allver;
        private readonly Stack<string> _stack;
        private readonly string[,] _parseTable;
        private readonly int _row;
        private readonly int _col;
        private bool _ambiguity;

        public Table(List<Container> varlist)
        {
            _varlist = varlist;
            _allter = new List<string>();
            _allver = new List<string>();
            _stack = new Stack<string>();
            _ambiguity = false;
            GetAllVar();
            GetAllTar();
            _row = _allver.Count;
            _col = _allter.Count;
            _parseTable = new string[_row, _col];


            SetNull();

            CreateTable();

            if (!_ambiguity)
            {
                PrintTable();
            }
        }

        public bool Ambiguitycheck()
        {
            return _ambiguity;
        }

        public List<string> AllverList()
        {
            return _allver;
        }

        public List<string> AllterList()
        {
            return _allter;
        }

        public string[,] GetTable()
        {
            return _parseTable;
        }

        private void PrintTable()
        {
            Console.WriteLine("Table:\n");


            foreach (var t in _allter)
            {
                Console.Write("\t{0}", t);
            }

            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < _row; i++)
            {
                Console.Write(_allver[i] + "-> ");

                for (int j = 0; j < _col; j++)
                {
                    Console.Write("\t" + _parseTable[i, j]);
                }
                Console.WriteLine("\n");
            }
        }

        private void SetNull()
        {
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    _parseTable[i, j] = "\0";
                }
            }
        }

        private bool FindVar(string s)
        {
            return _varlist.Any(i => i.Var.Equals(s));
        }


        private void GetAllTar()
        {
            foreach (var v in _varlist)
            {
                foreach (var s in v.SplitStrings)
                {
                    if (!FindVar(s) && !s.Equals("\0") && !s.Equals("|") && !s.Equals("~") && !_allter.Contains(s))
                    {
                        _allter.Add(s);
                    }
                    else if (!_allter.Contains("$") && s.Equals("~"))
                    {
                        _allter.Add("$");
                    }
                }
            }
        }

        private void SetAmbiguti(string var, string ptver, string currentver, string colname)
        {
            Console.WriteLine("\n\t" + "THIS Grammar Is Ambiguous For: \n");
            Console.WriteLine("      " + colname + "\n");
            Console.WriteLine("     ---\n");
            Console.WriteLine("{0}-> {1}\n", var, ptver);

            Console.WriteLine("{0}-> {1}\n", var, currentver);
            Console.WriteLine();
            _ambiguity = true;
        }

        private void GetAllVar()
        {
            foreach (var v in _varlist)
            {
                _allver.Add(v.Var);
            }
        }

        private bool CheckTer(string s)
        {
            return _allter.Any(s.Equals);
        }

        private bool FindFirst(List<string> tarfirstList, string s)
        {
            return tarfirstList != null && tarfirstList.Any(t => t.Equals(s));
        }

        private bool FindFollow(List<string> varfollowList, string s)
        {
            return varfollowList != null && varfollowList.Any(t => t.Equals(s));
        }

        private List<string> GetfirstList(string var)
        {
            return (from v in _varlist where v.Var.Equals(var) select v.Fast).FirstOrDefault();
        }

        private bool CheckFirstESP(List<string> firstList)
        {
            return firstList.Any(f => f.Equals("~"));
        }


        private void CreateTable()
        {
            for (int i = 0; i < _row; i++)
            {
                foreach (var m in _varlist[i].Mainstring)
                {
                    _stack.Push(m);
                    var str = m.Split(' ');
                    if (str[0].Equals(""))
                    {
                        if (str[1].Equals("~"))
                        {
                            _stack.Pop();
                        }
                        else
                        {
                            Setdata(i, str[1]);
                        }
                    }

                    else
                    {
                        Setdata(i, str[0]);
                    }
                }
                SetEsp(i);
            }
        }

        private void SetEsp(int i)
        {
            for (var j = 0; j < _col; j++)
            {
                if (!CheckFirstESP(_varlist[i].Fast)) continue;

                if (!FindFollow(_varlist[i].Follow, _allter[j])) continue;

                if (_parseTable[i, j].Equals("\0"))
                {
                    _parseTable[i, j] = "~";
                }
                else
                {
                    SetAmbiguti(_allver[i], _parseTable[i, j], "~", _allter[j]);
                }
            }
        }

        private void Setdata(int i, string s)
        {
            var str = _stack.Pop();
            var tarfirst = GetfirstList(s);

            for (int j = 0; j < _col; j++)
            {
                if (CheckTer(s) && _allter[j].Equals(s))
                {
                    if (_parseTable[i, j].Equals("\0"))
                    {
                        _parseTable[i, j] = str;
                    }
                    else
                    {
                        SetAmbiguti(_allver[i], _parseTable[i, j], str, _allter[j]);
                    }
                }
                else if (FindFirst(tarfirst, _allter[j]))
                {
                    if (_parseTable[i, j].Equals("\0"))
                    {
                        _parseTable[i, j] = str;
                    }
                    else
                    {
                        SetAmbiguti(_allver[i], _parseTable[i, j], str, _allter[j]);
                    }
                }
            }
        }
    }
}
