namespace LanguageChanger
{
    public class ParametrizedText : LocalizableText
    {
        private string _parameter = null;
        
        public void AssignValueToParameter(string value)
        {
            _parameter = value;
            _text.text = _text.text.Replace("{n}", value);
        }

        protected override void SetText(DescriptionProvider provider)
        {
            if (_parameter != null)
            {
                //TODO: Check substring "{n}" for existence in string
                _text.text = provider[_name].Replace("{n}", _parameter);
                return;
            }
            _text.text = provider[_name];
        }
    }
}