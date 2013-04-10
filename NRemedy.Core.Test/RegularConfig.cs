using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.Core.Test
{
    public class RegularConfig : ContextConfig
    {
        protected static string TestRegularFormName = "NRemedy_Test_Regular_Form";
        protected static uint TestCharacterFieldId = 20000001;
        protected static uint TestIntFieldId = 20000002;
        protected static uint TestDateTimeFieldId = 20000003;
        protected static uint TestDateFieldId = 20000004;
        protected static uint TestTimeFieldId = 2000005;
        protected static uint TestRealFieldId = 20000006;
        protected static uint TestDecimalFieldId = 20000007;

        protected static uint TestCharacterFieldIdNotExist = 745678897;

        protected static string TestCharacterFieldValue = "Hello Remedy";
        protected static string TestCharacterFieldValueChinese = "你好 Remedy";

        protected static uint TestRadioFieldId = 20000009;
    }
}
