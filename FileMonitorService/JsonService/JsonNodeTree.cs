namespace FileMonitorService.JsonService
{
    public class JsonNodeTree
    {
        public string? PropertyName { get; set; }
        public object? Value { get; set; }
        public JsonNodeTree? Root { get; set; }
        public bool IsArray { get; set; }
        public bool IsComplex { get; set; }
        public int? ArrayIndex { get; set; } = null;
        public LinkedList<JsonNodeTree> Branches { get; set; } = new LinkedList<JsonNodeTree>();
        public bool IsMainRoot => Root == null;
        public bool IsArrayItem => ArrayIndex != null;

        public override bool Equals(object? obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
                return false;
            else
            {
                JsonNodeTree other = (JsonNodeTree)obj;
                bool sameProperties = PropertyName == other.PropertyName
                    && Value == null && other.Value == null ? true : Value?.Equals(other.Value) ?? false
                    && IsMainRoot == other.IsMainRoot
                    && IsArray == other.IsArray
                    && IsComplex == other.IsComplex
                    && ArrayIndex == other.ArrayIndex;
                return sameProperties && Branches.SequenceEqual(other.Branches);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}