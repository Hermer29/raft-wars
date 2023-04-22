namespace LanguageChanger
{
    public class ParametrizedLocalizableString
    {
        private string _value;
        
        private const string ParameterFormat = "{INDEX}";

        public ParametrizedLocalizableString(string value)
        {
            _value = value;
        }
        
        public void EnterParameter(int index, object value)
        {
            string soughtSubstring = ParameterFormat.Replace("INDEX", index.ToString());
            _value.Replace(soughtSubstring, value.ToString());
        }

        public override string ToString()
        {
            return _value;
        }
    }
}