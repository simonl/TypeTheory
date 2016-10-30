namespace Machine
{
    public interface ISpace<T>
    {
        T this[uint index] { get; set; }
    }

    public sealed class Space<T> : ISpace<T>
    {
        private readonly T[] Store;

        public Space(byte size)
        {
            Size = size;
            Store = new T[1U << Size];
        }

        private byte Size { get; set; }

        public T this[uint index]
        {
            get
            {
                index = Wrap(index, mod: Size);

                return this.Store[index];
            }
            set
            {
                index = Wrap(index, mod: Size);

                this.Store[index] = value;
            }
        }

        private uint Wrap(uint index, byte mod)
        {
            return index & ((1U << mod) - 1);
        }
    }
}