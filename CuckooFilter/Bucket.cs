namespace CuckooFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Bucket
    {
        const byte nullFingerPrint = 0;
        public static readonly int BucketSize = 4;
        private readonly byte[] fingerPrints;

        public Bucket()
        {
            this.fingerPrints = new byte[BucketSize];
        }

        public bool Insert(byte fingerPrint)
        {
            for (int i = 0; i < fingerPrints.Length; i++)
            {
                if (fingerPrints[i] == nullFingerPrint)
                {
                    fingerPrints[i] = fingerPrint;
                    return true;
                }
            }
            return false;
        }

        public bool Delete(byte fingerPrint)
        {
            for (int i = 0; i < fingerPrints.Length; i++)
            {
                if (fingerPrints[i] == fingerPrint)
                {
                    fingerPrints[i] = nullFingerPrint;
                    return true;
                }
            }
            return false;
        }

        public int GetIndexOfFingerPrint(byte fingerPrint)
        {
            for (int i = 0; i < fingerPrints.Length; i++)
            {
                if (fingerPrints[i] == fingerPrint)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Reset()
        {
            for (int i = 0; i < fingerPrints.Length; i++)
            {
                fingerPrints[i] = nullFingerPrint;
            }
        }

        public byte this[int index]
        {
            get => fingerPrints[index];
            set => fingerPrints[index] = value;
        }
    }
}
