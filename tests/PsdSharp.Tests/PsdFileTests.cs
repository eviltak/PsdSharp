using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PsdSharp.Tests
{
    public class PsdFileTests
    {
        private const string PsdRootPath = "PSDs/";

        private readonly ITestOutputHelper output;

        public PsdFileTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [InlineData(PsdRootPath + "LayersAndMasks.psd")]
        [Theory]
        public void LayersTest(string psdFilePath)
        {
            /*
            PsdFile psdFile = new PsdFile(psdFilePath);

            output.WriteLine(string.Join("\n", psdFile.Layers.Select(layer => layer.Name)));
            */
        }
    }
}
