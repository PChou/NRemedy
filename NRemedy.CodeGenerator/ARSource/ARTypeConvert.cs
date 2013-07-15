using System;

namespace NRemedy.CodeGenerator
{
    public class ARTypeConvert
    {
        public static Type MappingARType(string atTypeName)
        {
            return MappingARType((ARType)Enum.Parse(typeof(ARType), atTypeName));
        }

        public static Type MappingARType(ARType arType)
        {
            return MappingARType((uint)arType);
        }

        public static Type MappingARType(uint arTypeID)
        {
            switch (arTypeID)
            {
                case 2u: return typeof(Int32);
                case 3u: return typeof(Double);
                case 4u: return typeof(String);
                //case 5u: return typeof(String);
                case 6u: return typeof(Int32);
                case 7u: return typeof(DateTime);
                case 8u: return typeof(Int32?);
                case 10u: return typeof(Decimal);
                //case 11u: return typeof(BMC.ARSystem.Attachment);
                //case 12u: return typeof(BMC.ARSystem.CurrencyValue);
                //case 13u: return typeof(BMC.ARSystem.DateValue);
                //case 14u: return typeof(BMC.ARSystem.TimeOfDayValue);
                //default: throw new InvalidCastException(string.Format("{0} is not supported data field type", arTypeID));
                default: return null;
            }

        }
    }
}
