using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame
{
    class LanguageList:List<Language>
    {
        public LanguageList()
        {
            //Initialize all language supported! 123
            this.Add(new Language() { Code = "en-US", Name = "English" });
            this.Add(new Language() { Code = "vi-VN", Name = "Vietnamese" });
            this.Add(new Language() { Code = "ja-JP", Name = "Japanese" });
            this.Add(new Language() { Code = "fr-FR", Name = "French" });
        }
    }
}
