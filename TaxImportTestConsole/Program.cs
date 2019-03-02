using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxImportLogic;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            new LogicExecuter().CallAPIAndImportData("https://api.zip-tax.com/request/v30", "K8E65AhXIga2fJuz");
        }
    }
}
