using System.Collections;
using System.Collections.Generic;



namespace ECS
{
    public class NativeArray<T> : IEnumerable<T>
    {
        private readonly T[] _data;

        public NativeArray(int length)
        {
            _data = new T[length];
        }

        public T this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        public int Length { get { return _data.Length; } }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _data.Length; i++) yield return _data[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < _data.Length; i++) yield return _data[i];
        }
    }
}
