using System;

namespace GZipTest.ParallelGZipArchiver {
    public class OperatorState {
        public bool IsSuccess { get; private set; }
        public string ExceptionMessage { get; private set; }

        private readonly object _locker;
        public OperatorState() {
            IsSuccess = true;
            _locker = new object();
        }

        public void HandleException(Exception ex) {
            lock (_locker) {
                if (IsSuccess) {
                    ExceptionMessage = ex.Message;
                }
                IsSuccess = false;
            }
        }
    }
}
