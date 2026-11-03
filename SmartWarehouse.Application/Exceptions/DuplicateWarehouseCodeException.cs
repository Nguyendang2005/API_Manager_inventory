using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWarehouse.Application.Exceptions
{
    public class DuplicateWarehouseCodeException : Exception
    {

       public DuplicateWarehouseCodeException(string code)
            : base($"A warehouse with the code '{code}' already exists.")
        {
        }
    }
}
