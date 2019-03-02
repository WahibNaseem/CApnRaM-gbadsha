using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class markers
    {
        public string title { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string description { get; set; }

        public string number { get; set; }

    }

    public class FileViewModel
    {

        public string Path { get; set; }
        public string FileName { get; set; }
    }

}
