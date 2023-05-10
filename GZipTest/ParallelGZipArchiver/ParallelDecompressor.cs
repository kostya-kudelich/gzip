using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.ParallelGZipArchiver {
    public class ParallelDecompressor : BaseGZipOperator {
        private const int GZipBlockInfoSize = 8;
        private const int GZipBlockLengthSize = 4;
        public ParallelDecompressor(string inputFile, string outputFile) : base(inputFile, outputFile) { }

        public void Execute() {
            ExecuteOperation(
                () => WrappedMethod(ReadFile),
                () => WrappedMethod(WriteFile),
                () => WrappedMethod(Decompress)
            );
        }

        private void ReadFile() {
            int counter = 0;
            using FileStream fileStream = new FileStream(_inputFile, FileMode.Open);

            while (fileStream.Position < fileStream.Length && _operatorState.IsSuccess) {
                byte[] blockInfo = new byte[GZipBlockInfoSize];
                fileStream.Read(blockInfo, 0, blockInfo.Length);
                int blockLength = BitConverter.ToInt32(blockInfo, GZipBlockLengthSize);

                byte[] compressedBuffer = new byte[blockLength];
                blockInfo.CopyTo(compressedBuffer, 0);

                fileStream.Read(compressedBuffer, GZipBlockInfoSize, blockLength - GZipBlockInfoSize);
                int bufferSize = BitConverter.ToInt32(compressedBuffer, blockLength - GZipBlockLengthSize);

                SizedByteBlock block = new SizedByteBlock(counter++, compressedBuffer, bufferSize);
                _queueReadingBlocks.Enqueue(block);
            }

            _queueReadingBlocks.Complete();
        }

        private void Decompress() {
            while (true && _operatorState.IsSuccess) {
                SizedByteBlock block = (SizedByteBlock)_queueReadingBlocks.Dequeue();
                if (block == null) {
                    return;
                }

                using MemoryStream memoryStream = new MemoryStream(block.Buffer);
                using GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);

                byte[] buffer = new byte[block.BufferSize.Value];
                gZipStream.Read(buffer, 0, buffer.Length);
                ByteBlock decompressedBlock = new ByteBlock(block.Id, buffer);
                _queueWritingBlocks.Enqueue(decompressedBlock);
            }
        }

        private void WriteFile() {
            using FileStream fileStream = new FileStream(_outputFile, FileMode.Append);

            while (true && _operatorState.IsSuccess) {
                ByteBlock block = _queueWritingBlocks.Dequeue();
                if (block == null)
                    return;

                fileStream.Write(block.Buffer, 0, block.Buffer.Length);
            }
        }
    }
}