using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Data.Entities
{
    public class getTransactHistResourceParameters
    {
        const int maxpagesize = 15; 
        public int PageNumber { get; set; } = 1;
        private int _Pagesize { get; set; } = 10;
        public int pageSize 
        {
            get => _Pagesize;
            set => _Pagesize = (value > maxpagesize) ? maxpagesize : value;
        }
    }
}
