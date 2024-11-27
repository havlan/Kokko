namespace CuckooFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class CuckooFilter
    {
        private const int MaxCuckooCount = 500;
        private readonly Bucket[] buckets;
        private uint count;
        private readonly uint bucketPow;
        private readonly Util util;

        public CuckooFilter(int capacity)
        {
            var transformedCapacity = Util.GetNextPow2((uint)capacity) / Bucket.BucketSize;
            transformedCapacity = (transformedCapacity == 0 ? 1 : transformedCapacity);
            this.buckets = new Bucket[transformedCapacity];
            for (int i = 0; i < transformedCapacity; i++)
            {
                this.buckets[i] = new Bucket();
            }
            this.count = 0;
            this.bucketPow = (uint)BitOperations.TrailingZeroCount(transformedCapacity);
            this.util = new Util();
        }

        public bool Insert(byte[] data)
        {
            (var firstIndex, var fingerPrint) = this.util.GetIndexAndFingerPrint(data, this.bucketPow);

            if (this.Insert(fingerPrint, firstIndex))
            {
                return true;
            }

            var secondIndex = this.util.GetAltIndex(fingerPrint, firstIndex, this.bucketPow);
            if (this.Insert(fingerPrint, secondIndex))
            {
                return true;
            }

            return this.Reinsert(fingerPrint, this.util.Rand2(firstIndex, secondIndex));
        }

        private bool Reinsert(byte fingerPrint, uint index)
        {
            for(var k = 0; k < MaxCuckooCount; k++)
            {
                var indexInBucket = this.util.GetRandomBucket();
                (this.buckets[index][indexInBucket], fingerPrint) = (fingerPrint, this.buckets[index][indexInBucket]);
                var altIndex = this.util.GetAltIndex(fingerPrint, index, this.bucketPow);
                if (this.Insert(fingerPrint, altIndex))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Insert(byte fingerPrint, uint index)
        {
            if (this.buckets[index].Insert(fingerPrint))
            {
                this.count++;
                return true;
            }
            return false;
        }

        public bool Lookup(byte[] data)
        {
            (var firstIndex, var fingerPrint) = this.util.GetIndexAndFingerPrint(data, this.bucketPow);
            var indexOfFingerPrint = this.buckets[firstIndex].GetIndexOfFingerPrint(fingerPrint);
            if (indexOfFingerPrint > -1)
            {
                return true;
            }

            var altIndex = this.util.GetAltIndex(fingerPrint, firstIndex, this.bucketPow);
            var altIndexOfFingerPrint = this.buckets[altIndex].GetIndexOfFingerPrint(fingerPrint);
            return altIndexOfFingerPrint > -1;
        }

        public bool Delete(byte[] data)
        {
            (var firstIndex, var fingerPrint) = this.util.GetIndexAndFingerPrint(data, this.bucketPow);
            if (this.Delete(fingerPrint, firstIndex))
            {
                return true;
            }

            var altIndex = this.util.GetAltIndex(fingerPrint, firstIndex, this.bucketPow);
            return this.Delete(fingerPrint, altIndex);
        }

        private bool Delete(byte fingerPrint, uint index)
        {
            if (this.buckets[index].Delete(fingerPrint))
            {
                this.count--;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Count: {this.count}";
        }
    }
}