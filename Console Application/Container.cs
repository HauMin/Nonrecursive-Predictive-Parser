using System.Collections.Generic;

namespace Nonrecursive_Predictive_Parser
{
    class Container
    {
        public string Var { get; set; }
        public List<string> Fast { get; set; }
        public string[] Mainstring { get; set; }
        public string[] SplitStrings { get; set; }      
        public List<string> Follow { get; set; }
    }
}
