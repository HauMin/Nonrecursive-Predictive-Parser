using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nonrecursive_Predictive_Parser
{
    internal class Input
    {
        private List<Container> varList = new List<Container>();
        private Stack<string> stack = new Stack<string>();
        private string[,] _parsetable;
        private List<string> _allterList;
        private List<string> _allverList;
        private string[] userinput;
        private string[] line = File.ReadAllLines("g.txt");
        private bool _error;

        public Input()
        {
            PrintHeader();
            Start();

            new Fast(varList);
            new Follow(varList);
            var table = new Table(varList);
            _parsetable = table.GetTable();
            _allterList = table.AllterList();
            _allverList = table.AllverList();
            _error = false;
            if (!table.Ambiguitycheck())
            {
                UserInput();
                
            }
            else
            {
                Console.WriteLine("Since The Grammar Is Ambiguous, No Input Can't Be Parsed\n");
            }
            
        }

        private void Start()
        {
            foreach (string l in line)
            {
                var splitStrings = l.Split(':');
                if (splitStrings.Length == 2)
                {
                    var mainstring = splitStrings[1];
                    splitStrings[1] += " " + "\0";
                    varList.Add(new Container
                    {
                        Var = splitStrings[0],
                        Mainstring = mainstring.Split('|'),
                        SplitStrings = splitStrings[1].Split(' '),
                        Fast = new List<string>(),
                        Follow = new List<string>()
                    });
                }
                else
                {
                    break;
                }
            }
        }

        private void Output(string var, string s)
        {
            Console.WriteLine("{0} -> {1} ", var, s);
        }

        private void OutputError()
        {
            Console.WriteLine("\n\tERROR!\n");
            _error = true;
        }

        private void OutputMatch(string s)
        {
            Console.WriteLine("\n match : {0} \n", s);
        }

        private void OutputSuccess()
        {
            Console.WriteLine("\n\tSuccess!\n");
        }

        private void UserInput()
        {
            Console.Write("\nInput Your String : ");
            userinput = (Console.ReadLine() + " " + "$").Split(' ');
            Console.WriteLine("\n output:\n ");

            stack.Push("$");
            stack.Push(varList[0].Var);
            foreach (var u in userinput)
            {
                if (!_error)
                {
                    Parse(u);
                }
                else
                {
                    break;
                }
            }
        }

        private void Parse(string u)
        {
            while (stack.Count != 0)
            {
                string ele = stack.Pop();
                if (ele.Equals("~")) continue;
                
                if (ele.Equals(u) && ele.Equals("$"))
                {
                    OutputSuccess();
                }
                else if (ele.Equals(u))
                {
                    OutputMatch(u);
                    break;
                }
                
                else
                {
                    var str = GetPTableData(ele, u);
                    if (!str.Equals("\0"))
                    {
                        Output(ele, str);
                        var splitstr = str.Split(' ');

                        foreach (var s in splitstr.Reverse())
                        {
                            if (!s.Equals(""))
                            {
                                stack.Push(s);
                            }
                        }
                    }
                    else
                    {
                        OutputError();
                        break;
                    }
                }
            }
        }

        private string GetPTableData(string var, string tar)
        {
            for (var i = 0; i < _allverList.Count; i++)
            {
                if (_allverList[i].Equals(var))
                {
                    for (var j = 0; j < _allterList.Count; j++)
                    {
                        if (_allterList[j].Equals(tar))
                        {
                            return _parsetable[i, j];
                        }
                    }
                }
            }
            return "\0";
        }

       
        public void PrintHeader()
        {
            Console.WriteLine("\t\t\tNonrecursive Predictive Parser");
            Console.WriteLine("\t\t\t------------------------------\n");
        }
    }
}