#region copyright

/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

namespace Shared.Database.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Enums;

    public class UserStats
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }

        [Required]
        [DefaultValue(0)]
        // ReSharper disable once UnusedMember.Global
        public CountryIds CountryId { get; set; }


        // ReSharper disable once MemberCanBePrivate.Global
        public static UserStats GetUserStats(int userId)
        {
            using (SoraContext db = new SoraContext())
                return db.UserStats.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
        }

        // ReSharper disable once UnusedMember.Global
        public static UserStats GetUserStats(Users user) => GetUserStats(user.Id);
    }
}
