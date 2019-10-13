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
        public object Value { get; set; }
    }
}
