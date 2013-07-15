
using ARNative;
namespace NRemedy.CodeGenerator
{

    public class GetARFieldFilterDelegate 
    {
        #region defined field filter

        public static bool FieldFilterBase(ARField field)
        {
            if (field == null) return false;
            return true;
        }

        public static bool FieldFilterIsIntegerField(ARField field)
        {
            return (ARType)field.dataType == ARType.IntegerField;
        }

        public static bool FieldFilterIsRealField(ARField field)
        {
            return (ARType)field.dataType == ARType.RealField;
        }

        public static bool FieldFilterIsCharacterField(ARField field)
        {
            return (ARType)field.dataType == ARType.CharacterField;
        }

        public static bool FieldFilterIsDiaryField(ARField field)
        {
            return (ARType)field.dataType == ARType.DiaryField;
        }

        public static bool FieldFilterIsSelectionField(ARField field)
        {
            return (ARType)field.dataType == ARType.SelectionField;
        }

        public static bool FieldFilterIsDateTimeField(ARField field)
        {
            return (ARType)field.dataType == ARType.DateTimeField;
        }

        public static bool FieldFilterIsDecimalField(ARField field)
        {
            return (ARType)field.dataType == ARType.DecimalField;
        }

        public static bool FieldFilterIsAttachmentField(ARField field)
        {
            return (ARType)field.dataType == ARType.AttachmentField;
        }

        public static bool FieldFilterIsCurrencyField(ARField field)
        {
            return (ARType)field.dataType == ARType.CurrencyField;
        }

        public static bool FieldFilterIsDateOnlyField(ARField field)
        {
            return (ARType)field.dataType == ARType.DateOnlyField;
        }

        public static bool FieldFilterIsTimeOnlyField(ARField field)
        {
            return (ARType)field.dataType == ARType.TimeOnlyField;
        }

        #endregion

        public static ARFieldFilterDelegate getARFieldFilterDelegate()
        {
            return new ARFieldFilterDelegate(DefaultARFieldFilter);
        }

        private static bool DefaultARFieldFilter(ARField field)
        {
            return true;
        }
    }
}
