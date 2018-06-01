using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myRetail.Models
{
    interface Irepository
    {
        Product GetProduct(int id);
        Product updateProduct(int id);

    }
}
