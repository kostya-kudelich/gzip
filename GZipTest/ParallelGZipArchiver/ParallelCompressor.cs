using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.ParallelGZipArchiver {
    public class ParallelCompressor : BaseGZipOperator {
        private const int BlockSizeInBytes = 1024 * 1024 * 4;
        public ParallelCompressor(string inputFile, string outputFile) : base(inputFile, outputFile) { }

        public void Execute() {
            ExecuteOperation(
                () => WrappedMethod(ReadFile),
                () => WrappedMethod(WriteFile),
                () => WrappedMethod(Compress)
            );
        }

        private void ReadFile() {
            using FileStream fileStream = new FileStream(_inputFile, FileMode.Open);
            while (fileStream.Position < fileStream.Length && _operatorState.IsSuccess) {
                int bytesToRead = (int)Math.Min((fileStream.Length - fileStream.Position), BlockSizeInBytes);
                byte[] buffer = new byte[bytesToRead];
                fileStream.Read(buffer, 0, bytesToRead);
                _queueReadingBlocks.Enqueue(buffer);
            }

            _queueReadingBlocks.Complete();
        }

        private void Compress() {
            while (true && _operatorState.IsSuccess) {
                ByteBlock block = _queueReadingBlocks.Dequeue();

                if (block == null) {
                    return;
                }

                using MemoryStream memoryStream = new MemoryStream();
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
                    gZipStream.Write(block.Buffer, 0, block.Buffer.Length);
                }

                byte[] compressedBuffer = memoryStream.ToArray();
                ByteBlock compressedBufferBlock = new ByteBlock(block.Id, compressedBuffer);
                _queueWritingBlocks.Enqueue(compressedBufferBlock);
            }
        }

        private void WriteFile() {
            using FileStream targetStream = new FileStream(_outputFile + ".gz", FileMode.Append);
            while (true && _operatorState.IsSuccess) {
                ByteBlock block = _queueWritingBlocks.Dequeue();
                if (block == null) {
                    return;
                }
                BitConverter.GetBytes(block.Buffer.Length).CopyTo(block.Buffer, 4);
                targetStream.Write(block.Buffer, 0, block.Buffer.Length);
            }
        }
    }
}
