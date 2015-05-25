using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stock.Adapter
{
    class DataAdapter
    {
        public static string RealTwo(double real)
        {
            return String.Format("{0:F}", real);
        }
    }
}
