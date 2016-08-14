namespace PsdSharp.ImageResources
{
    public class ImageResource
    {
        private PsdDocument document;

        public ImageResourceId Id { get; set; }

        public string Name { get; set; }

        public byte[] data;

        internal ImageResource(PsdDocument document, ImageResourceId id, string name, byte[] data)
        {
            this.document = document;
            this.data = data;
            Id = id;
            Name = name;
        }
    }
}