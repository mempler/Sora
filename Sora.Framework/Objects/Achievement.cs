using System.Net;
using ImageMagick;

namespace Sora.Framework.Objects
{
    public class Achievement
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string IconURI { get; set; }
        
        public string ToOsuString() => $"{Name}+{DisplayName}+{Description}".Replace('\n', '\t');

        public byte[] GetIconImage(bool x2 = false)
        {
            var request = (HttpWebRequest) WebRequest.Create(IconURI);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var image = new MagickImage(stream)) {
                if (x2)
                    image.Resize(385, 420);
                else
                    image.Resize(193, 210);

                return image.ToByteArray(MagickFormat.Png);
            }
        }
    }
}