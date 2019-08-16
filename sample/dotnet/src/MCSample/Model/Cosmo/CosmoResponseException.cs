using System;

namespace MCSample.Model.Cosmo
{
    public class CosmoResponseException<T> : Exception
    {
        public CosmoResponseException(T response)
        {
            Response = response;
        }

        public T Response { get; private set; }
    }
}
