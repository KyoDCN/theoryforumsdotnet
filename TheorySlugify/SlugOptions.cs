using System.Collections.Generic;

namespace TheorySlugify
{
    /// <summary>
    /// Used to configure the a <see cref="SlugHelper"/> instance
    /// </summary>
    public class SlugOptions
    {
        public Dictionary<string, string> StringReplacements { get; set; }
        public bool ForceLowerCase { get; set; } = true;
        public bool CollapseWhiteSpace { get; set; } = true;
        public string DeniedCharactersRegex { get; set; } = @"[^a-zA-Z0-9\-\._]";
        public bool CollapseDashes { get; set; } = true;
        public bool TrimWhitespace { get; set; } = true;

        public SlugOptions()
        {
            StringReplacements = new Dictionary<string, string>
            {
                { " ", "-" }
            };
        }
    }
}
