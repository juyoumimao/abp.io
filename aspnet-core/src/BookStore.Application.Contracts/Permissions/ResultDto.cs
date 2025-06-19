using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Permissions
{
    public enum StatusCode 
    {
        Success = 200,
        Error = 500,
        NotLogin = 401,
    }
    public class ResultDto<T>
    {
        public string msg { get; set; }
        public StatusCode code { get; set; }
        public T Data { get; set; }
    }
}
