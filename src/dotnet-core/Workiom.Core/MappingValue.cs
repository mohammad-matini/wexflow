namespace Workiom.Core
{
    public enum MappingType
    {
        Static,
        Dynamic
    }

    public class MappingValue
    {
        public MappingType MappingType { get; set; }
        public string Value { get; set; }
    }
}
