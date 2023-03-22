using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult(string message) : base(true, message)
        {
            // :base ile miras (inherite) aldığı sınıfın constructor'ına gönderiyor
        }
        public SuccessResult() : base(true)
        {

        }
    }
}
