using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSONConfFileEditor.ConfModels.Enums;

namespace JSONConfFileEditor.ConfModels
{
    public class ValidationTest
    {
        //public char test { get; set; }

        //public List<List<char>> test4 { get; set; }

        //public string field;

        //public k c { get; set; }

        //public k c { get; set; }

        //public List<string> c { get; set; }

        //public List<double> d { get; set; }

        //public List<FeedbackMechanismGroupOneEnum> e { get; set; }

        //public List<Int16> f { get; set; }

        //public List<List<string>> Aa { get; set; }

        //public List<string> a { get; set; }

        //public List<string> b { get; set; }

        //public k g2 { get; set; }

        public List<k> g { get; set; }


        //public List<k> f { get; set; }

        //public List<k> f { get; set; }

        //public k obj { get; set; }

    }

    public class k // TODO fix naming issues
    {
        //public string test1 { get; set; }
        public string test3 { get; set; }

        public h test4 { get; set; }
        //public List<string> f { get; set; }


        //public h test4 { get; set; }
    }

    public class h
    {

        //public string test1 { get; set; }

        public List<bool> t { get; set; }

        //public string test3 { get; set; }
    }

}
