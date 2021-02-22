using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.ReadDTO
{
    public class APIGenericResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
