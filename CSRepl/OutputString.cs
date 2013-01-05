namespace CSRepl
{
    public class OutputString
    {
        public string Value { get; private set; }

        public OutputString(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}