namespace CuckooFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO.Hashing;
    using System.Text;
    using System.Threading.Tasks;

    public class Util
    {
        private readonly uint[] altHash;
        private readonly uint[] masks;
        private const int seed = 199933;
        private readonly Random random;

        public Util()
        {
            this.altHash = new uint[256];
            this.masks = new uint[64];
            this.random = new Random();

            for(var i = 0; i < this.altHash.Length; i++)
            {
                this.altHash[i] = (uint)(XxHash64.HashToUInt64([(byte)i], seed));
            }

            for(var i = 0; i < this.masks.Length; i++)
            {
                this.masks[i] = (uint)((1 << i) - 1);
            }
        }

        public static uint GetNextPow2(uint n)
        {
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            n |= n >> 32;
            n++;
            return n;
        }

        public (uint, byte) GetIndexAndFingerPrint(byte[] data, uint bucketPow)
        {
            var hash = XxHash64.HashToUInt64(data);
            var fingerPrint = Util.GetFingerPrint(hash);
            var i1 = (uint)(hash >> 32) & masks[bucketPow];
            return (i1, GetFingerPrint(fingerPrint));
        }

        public uint GetAltIndex(byte fingerPrint, uint index, uint bucketPow)
        {
            var mask = masks[bucketPow];
            var hash = altHash[fingerPrint] & mask;
            return (index & mask) ^ hash;
        }

        private static byte GetFingerPrint(ulong hash)
        {
            return (byte)(hash % 255 + 1);
        }

        public uint Rand2(uint firstIndex, uint secondIndex)
        {
            if (this.random.Next(2) == 0) 
            {
                return firstIndex;
            }
            else
            {
                return secondIndex;
            }
        }

        public int GetRandomBucket()
        {
            return this.random.Next(Bucket.BucketSize);
        }
    }
}
