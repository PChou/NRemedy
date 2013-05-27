
using System.Text.RegularExpressions;
using ARNative;
namespace NRemedy
{
    public class GetARFormFilterDelegate
    {
        #region defined form filter

        public static bool FormFilterBase(ARForm form)
        {
            return form != null;
        }
        
        public static bool FormFilterIsRegularForm(ARForm form)
        {
            return form.formtype == 1;
        }

        public static bool FormFilterIsJoinForm(ARForm form)
        {
            return form.formtype == 2;
        }
       
        public static bool FormFilterIsViewForm(ARForm form)
        {
            return form.formtype == 3;
        }

        public static bool FormFilterIsDisplayOnlyForm(ARForm form)
        {
            return form.formtype == 4;
        }

        public static bool FormFilterIsVendorForm(ARForm form)
        {
            return form.formtype == 5;
        }

        #endregion

        public ARFormFilterDelegate getARFormFilterDelegate()
        {
            return new ARFormFilterDelegate(DefaultARFormFilter);
        }

        private bool DefaultARFormFilter(ARForm form)
        {
            return true;
        }

    }
}
