using System;
using DinnerParty.Helpers;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace DinnerParty.Models.Schema
{
    public class DinnerInputType : InputObjectGraphType
    {
        public DinnerInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("title");
            Field<NonNullGraphType<StringGraphType>>("description");
            Field<NonNullGraphType<DateGraphType>>("eventDate");
            Field<NonNullGraphType<StringGraphType>>("address");
            Field<NonNullGraphType<StringGraphType>>("hostName");
            Field<NonNullGraphType<StringGraphType>>("contactPhone");
        }
    }

    public class PhoneNumberType : ScalarGraphType
    {
        public PhoneNumberType()
        {
            Name = "PhoneNumber";
        }

        public override object Serialize(object value)
        {
            return (value as PhoneNumber)?.ToFormatted();
        }

        public override object ParseValue(object value)
        {
            if (value == null) return null;
            return new PhoneNumber(value.ToString());
        }

        public override object ParseLiteral(IValue value)
        {
            var stringVal = value as StringValue;
            if (stringVal != null)
            {
                ParseValue(stringVal.Value);
            }
            return null;
        }
    }

    public class PhoneNumber
    {
        private static readonly string[] Replacements = { " ", "(", ")", "-", "+", "/" };
        private string _rawValue;

        // Make sure we can play nice with serialization
        public PhoneNumber()
        {
            Label = string.Empty;
        }

        public PhoneNumber(string rawValue)
            : this()
        {
            RawValue = rawValue;
        }

        public string Label { get; set; }

        public string RawValue
        {
            get { return _rawValue; }
            set { _rawValue = stripFormatting(value); }
        }

        public string AreaCode()
        {
            return !string.IsNullOrWhiteSpace(_rawValue) && _rawValue.Length >= 3
                ? _rawValue.Substring(0, 3)
                : null;
        }

        public string ExchangeCode()
        {
            return !string.IsNullOrWhiteSpace(_rawValue) && _rawValue.Length >= 6
                ? _rawValue.Substring(3, 3)
                : null;
        }

        public string ToFormatted()
        {
            if (string.IsNullOrEmpty(_rawValue)) return string.Empty;

            try
            {
                var first = _rawValue.Substring(0, 3);
                var second = _rawValue.Substring(3, 3);
                var third = _rawValue.Substring(6, 4);
                return $"1-{first}-{second}-{third}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var phoneNumber = obj as PhoneNumber;
            if (phoneNumber == null) return false;
            return Equals(phoneNumber);
        }

        public override string ToString()
        {
            return _rawValue;
        }

        private static string stripFormatting(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            Replacements.Each(r => value = value.Replace(r, ""));

            if (value.StartsWith("1"))
            {
                value = value.Substring(1, value.Length - 1);
            }

            return value;
        }

        public bool Equals(PhoneNumber other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._rawValue, _rawValue);
        }

        public override int GetHashCode()
        {
            return _rawValue?.GetHashCode() ?? 0;
        }
    }
}