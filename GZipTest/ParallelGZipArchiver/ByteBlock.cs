namespace GZipTest.ParallelGZipArchiver {
    public class ByteBlock {
        public int Id { get; private set; }
        public byte[] Buffer { get; private set; }

        public ByteBlock(int id, byte[] buffer) {
            Id = id;
            Buffer = buffer;
        }
    }
}
