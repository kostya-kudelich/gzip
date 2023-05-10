namespace GZipTest.ParallelGZipArchiver {
    public class SizedByteBlock : ByteBlock {
        public int? BufferSize { get; private set; }

        public SizedByteBlock(int id, byte[] buffer, int? bufferSize = null) : base (id, buffer) {
            BufferSize = bufferSize;
        }
    }
}
