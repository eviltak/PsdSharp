namespace PsdSharp
{
    /// <summary>
    /// Defines constants for the color mode of the Photoshop document.
    /// </summary>
    public enum DocumentColorMode : short
    {
        Bitmap = 0,
        Grayscale = 1,
        Indexed = 2,
        Rgb = 3,
        Cmyk = 4,
        Multichannel = 7,
        Duotone = 8,
        Lab = 9
    }
}