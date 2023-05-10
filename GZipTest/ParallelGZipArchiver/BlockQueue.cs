using System.Collections.Generic;
using System.Threading;

namespace GZipTest.ParallelGZipArchiver {
    public class BlockQueue {
        private readonly object _locker;
        private readonly Queue<ByteBlock> _queue;
        private bool _completed;
        private int _currentBlockId;
        private const int MaxBlockCount = 20;

        public BlockQueue() {
            _locker = new object();
            _queue = new Queue<ByteBlock>();
            _completed = false;
            _currentBlockId = 0;
        }

        public void Enqueue(byte[] buffer) {
            lock (_locker) {
                while (_queue.Count >= MaxBlockCount) {
                    Monitor.Wait(_locker);
                }
                var block = new ByteBlock(_currentBlockId, buffer);
                _queue.Enqueue(block);
                _currentBlockId++;
                Monitor.PulseAll(_locker);
            }
        }

        public void Enqueue(ByteBlock byteBlock) {
            lock (_locker) {
                while (byteBlock.Id != _currentBlockId || _queue.Count >= MaxBlockCount) {
                    Monitor.Wait(_locker);
                }

                _queue.Enqueue(byteBlock);
                _currentBlockId++;
                Monitor.PulseAll(_locker);
            }
        }

        public ByteBlock Dequeue() {
            lock (_locker) {
                while (_queue.Count == 0 && !_completed) {
                    Monitor.Wait(_locker);
                }

                if (_queue.Count == 0)
                    return null;

                var result = _queue.Dequeue();
                Monitor.PulseAll(_locker);
                return result;
            }
        }

        public void Complete() {
            lock (_locker) {
                _completed = true;
                Monitor.PulseAll(_locker);
            }
        }
    }
}
