using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Charlotte
{
    public class MqttMessage : DynamicObject
    {
        public string Message { get; set; }
        public string Topic { get; set; }

        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_values.ContainsKey(binder.Name))
                throw new MissingMemberException("Member not found: " + binder.Name);

            result = _values[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if(value is string)
                _values[binder.Name] = (string)value;
            else
                _values[binder.Name] = value.ToString();
            return true;
        }

        public string this[string name]
        {
            set
            {
                _values[name] = value;
            }
            get
            {
                return _values[name];
            }
        }
    }
}