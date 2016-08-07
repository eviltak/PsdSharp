using Xunit;
using Xunit.Sdk;

namespace PsdSharp.Tests
{
    public class PsdFileTests
    {
        private const string PsdRootPath = "PSDs/";
        
        [InlineData(PsdRootPath + "LayersAndMasks.psd")]
        [Theory]
        public void LayersTest(string psdFilePath)
        {
            PsdFile psdFile = new PsdFile(psdFilePath);
        }
    }
}
