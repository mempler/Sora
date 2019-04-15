using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Net;
using ImageMagick;
using Newtonsoft.Json;

namespace Shared.Models
{
    public class Achievements
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BitId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string IconURI { get; set; }


        public byte[] GetIconImage(bool x2 = false)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(IconURI);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (MagickImage image = new MagickImage(stream))
            {
                if (x2)
                    image.Resize(385, 420);
                else
                    image.Resize(193, 210);
                
                return image.ToByteArray(MagickFormat.Jpg);
            }
        }
    }
}