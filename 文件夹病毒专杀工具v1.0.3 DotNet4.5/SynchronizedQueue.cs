using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    public class SynchronizedQueue<T>
    {
        LinkedList<T> _queue = null;
        AutoResetEvent _autoResetEvent = null;

        public bool IsBlockThread
        {
            get;
            private set;
        }

        public object SyncObject { get; } = new object();

        public int Count
        {
            get
            {
                lock (SyncObject)
                {
                    return _queue.Count;
                }
            }
        }

        public SynchronizedQueue() : this(true)
        {
        }

        public SynchronizedQueue(bool isBlockThread)
        {
            _queue = new LinkedList<T>();
            InitAutoResetEvent(isBlockThread);
        }

        void InitAutoResetEvent(bool isBlockThread)
        {
            if (isBlockThread)
            {
                _autoResetEvent = new AutoResetEvent(false);
            }
            IsBlockThread = isBlockThread;
        }

        public void Clear()
        {
            lock (SyncObject)
            {
                _queue.Clear();
            }
        }
        public bool Contains(T item)
        {
            lock (SyncObject)
            {
                return _queue.Contains(item);
            }
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            lock (SyncObject)
            {
                return _queue.Contains(item, comparer);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (SyncObject)
            {
                _queue.CopyTo(array, arrayIndex);
            }
        }
        public void Enqueue(T item)
        {
            LinkedListNode<T> current = new LinkedListNode<T>(item);
            lock (SyncObject)
            {
                _queue.AddFirst(current);
            }
            if (IsBlockThread)
            {
                _autoResetEvent.Set();
            }
        }

        public T Dequeue()
        {
            LinkedListNode<T> node = null;
            while (true)
            {
                lock (SyncObject)
                {
                    node = _queue.Last;
                    if (node != null)
                    {
                        _queue.RemoveLast();
                        break;
                    }
                }
                if (!IsBlockThread)
                {
                    break;
                }
                else
                {
                    _autoResetEvent.WaitOne(1000);
                }
            }
            if (node != null)
            {
                return node.Value;
            }
            return default(T);
        }

        public LinkedList<T>.Enumerator GetEnumerator()
        {
            lock (SyncObject)
            {
                return _queue.GetEnumerator();
            }
        }

        public T[] ToArray()
        {
            lock (SyncObject)
            {
                return _queue.ToArray();
            }
        }
    }
}
