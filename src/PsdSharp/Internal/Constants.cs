namespace PsdSharp.Internal
{
    /// <summary>
    /// A class containing all constants used in code. All "magic" numbers and strings 
    /// that are be used only once also go here.
    /// </summary>
    internal static class Constants
    {
        public const string ImageResourceSignature = "8BIM";

        /// <summary>
        /// The fixed signature that each document must have.
        /// </summary>
        public const string FileSignature = "8BPS";

        /// <summary>
        /// The minimum possible number of channels in the document.
        /// </summary>
        public const int MinChannelCount = 1;

        public const int MaxChannelCount = 56;

        public const int MinDocumentWidth = 1;
        public const int MaxDocumentWidth = 30000;

        public const int MinDocumentHeight = 1;
        public const int MaxDocumentHeight = 30000;
    }
}