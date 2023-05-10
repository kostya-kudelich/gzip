using System;
using System.Threading.Tasks;

namespace GZipTest.ParallelGZipArchiver {
    public class BaseGZipOperator {
        protected BlockQueue _queueReadingBlocks, _queueWritingBlocks;
        protected Task[] _parallelTasks;
        protected readonly string _inputFile, _outputFile;
        protected OperatorState _operatorState;

        public BaseGZipOperator(string inputFile, string outputFile) {
            _operatorState = new OperatorState();
            _queueReadingBlocks = new BlockQueue();
            _queueWritingBlocks = new BlockQueue();
            _parallelTasks = new Task[Environment.ProcessorCount];
            _inputFile = inputFile;
            _outputFile = outputFile;
        }

        protected void ExecuteOperation(Action readFile, Action writeFile, Action operation) {
            var readingTask = Task.Run(() => readFile());
            for (int i = 0; i < _parallelTasks.Length; i++) {
                _parallelTasks[i] = Task.Run(() => operation());
            }
            var writingTask = Task.Run(() => writeFile());

            readingTask.Wait();
            Task.WaitAll(_parallelTasks);
            _queueWritingBlocks.Complete();
            writingTask.Wait();

            if (!_operatorState.IsSuccess) {
                throw new Exception(_operatorState.ExceptionMessage);
            }
        }

        protected void HandleException(Exception ex) {
            _operatorState.HandleException(ex);
            _queueReadingBlocks.Complete();
            _queueWritingBlocks.Complete();
        }

        protected void WrappedMethod(Action method) {
            try {
                method();
            } catch (Exception ex) {
                HandleException(ex);
            }
        }
    }
}
