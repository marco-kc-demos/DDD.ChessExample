using Minor.Miffy;
using System;

namespace DDD.Core.Application
{
    public interface IEventDispatcher
    {
        void Dispatch(EventMessage message);
    }
}