using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace LeaseExample
{
    class SimpleEntity : TableEntity
    {
        public int SomeValue { get; set; }
    }
}
