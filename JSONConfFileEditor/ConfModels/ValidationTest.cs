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

        //public List<string> IsEnabled { get; set; }
        
       // public bool aaaa { get; set; }
        
        //public List<bool> isen { get; set; }


        //public int b { get; set; }

        //public int d { get; set; }

        //public List<double> c { get; set; }

        //public List<List<int>> a { get; set; } = new List<List<int>> { new List<int> { 1, 2, 3, 5 }, new List<int> { 1, 2, 5 } };

        public List<List<List<double>>> b { get; set; } = new List<List<List<double>>> { new List<List<double>> { new List<double> { 1, 2, 3, 5 }, new List<double> { 1, 2, 5 } }, new List<List<double>> { new List<double> { 1, 2, 3, 5 }, new List<double> { 1, 2, 5 } } };

        //public List<List<List<h>>> b { get; set; } = new List<List<List<h>>> { new List<List<h>> { new List<h> { new h() }, new List<h> { new h()} }};


        //public string What { get; set; }

        //public List<List<string>> a { get; set; }

        //public List<char> c { get; set; }

        //public List<List<List<k>>> b { get; set; }

        //public List<k> a { get; set; }

        //List<int[]> l;

        //public List<int>[]  c { get; set; }


        /*public k Car { get; set; }
        public List<k> Cars { get; set; }*/

        //public string What { get; set; }
        //public char TestingChar { get; set; }


    }

    public class k // TODO fix naming issues
    {
        //public string test1 { get; set; }
        public int CarTitle { get; set; }

        public h Parts { get; set; } = new h();
        //public List<string> f { get; set; }


        //public h test4 { get; set; }
    }

    public class h
    {

        public string Title { get; set; }

        //public List<Whatever> IsPresent { get; set; }

        public string test3 { get; set; }
    }

    public class Whatever
    {
        public bool IsEnabled { get; set; }
    }

}
