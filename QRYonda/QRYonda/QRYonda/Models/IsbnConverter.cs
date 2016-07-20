/* 
The MIT License(MIT)

Copyright(c) 2014 Kenji Wada

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 See: https://github.com/CH3COOH/Softbuild.Data.Isbn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRYonda.Models
{
    public class IsbnConverter
    {
        public static string ConvertToISBN13(string isbn10)
        {
            if (string.IsNullOrWhiteSpace(isbn10) || isbn10.Length != 10)
            {
                throw new ArgumentException();
            }

            var list = new List<int>();
            for (int i = 0; i < 12; i++)
            {
                var s = isbn10.Insert(0, "978").Substring(i, 1);
                list.Add(int.Parse(s));
            }

            var sum = list
                .Select((e, index) => e * (((index + 1) % 2 == 1) ? 1 : 3))
                .Sum(e => e);

            var digit = string.Empty;
            var check = 10 - (sum % 10);
            if (check == 10)
            {
                digit = "0";
            }
            else
            {
                digit = check.ToString();
            }

            return string.Format("978{0}{1}", isbn10.Substring(0, 9), digit);
        }
    }

}

