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

        public k Car { get; set; }
        public List<k> Cars { get; set; }

        //public string What { get; set; }
        //public char TestingChar { get; set; }


    }

    public class k // TODO fix naming issues
    {
        //public string test1 { get; set; }
        public string CarTitle { get; set; }

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
