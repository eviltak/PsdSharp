using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PsdSharp.Tests
{
    public class PsdDocumentTests
    {
        private const string PsdRootPath = "PSDs/";

        private readonly ITestOutputHelper output;

        public PsdDocumentTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [InlineData(PsdRootPath + "LayersAndMasks.psd")]
        [Theory]
        public void LayerGroupsImageResourceTest(string psdPath)
        {
            PsdDocument psdDocument = PsdDocument.Load(psdPath);

            Assert.True(psdDocument.ImageResources.ContainsKey(ImageResourceId.LayerGroups));

            output.WriteLine(psdDocument.ImageResources[ImageResourceId.LayerGroups].data.Length.ToString());
        }
    }
}
