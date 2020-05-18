using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DDD.Core
{
    /// <summary>
    /// Represents a ValueObject in the domain (DDD).
    /// </summary>
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            return obj != null &&
                   this.GetType() == obj.GetType() &&
                   AreEqual(this, (ValueObject)obj);
        }

        public override int GetHashCode()
        {
            return GetAtomicValues()
                    .Select(x => x != null ? x.GetHashCode() : 0)
                    .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return AreEqual(left, right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !AreEqual(left, right);
        }

        private static bool AreEqual(ValueObject left, ValueObject right)
        {
            if (left is null)
                return right is null;
            else
                return right is object &&
                       left.GetAtomicValues().SequenceEqual(right.GetAtomicValues());
        }
    }
}