﻿namespace POS_API.Model
{
    public class ParsedElement
    {
        public string ElementName { get; set; }
        public string Description { get; set; }
        public List<ParsedElement> Children { get; set; } = new List<ParsedElement>();

    }

}
