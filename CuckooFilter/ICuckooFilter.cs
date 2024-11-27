namespace CuckooFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICuckooFilter<T>
    {
        public void Add(T item);
        public void Delete(T item);
    }
}
