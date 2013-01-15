//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-10
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    public enum ARType : uint
    {
        None = 0u,              //define as $NULL$
        KeyWords = 1u,
        IntegerField = 2u,      //System.Int32
        RealField = 3u,         //System.Double
        CharacterField = 4u,    //System.String
        DiaryField = 5u,        //BMC.ARSystem.DiaryList | System.String
        SelectionField = 6u,    //System.Int32
        DateTimeField = 7u,     //System.DateTime.ToUniversalTime
        BitMask = 8u,
        Bytes = 9u,
        DecimalField = 10u,     //System.Decimal
        AttachmentField = 11u,  //ARSystem.Attachment
        CurrencyField = 12u,    //ARSystem.CurrencyValue
        DateOnlyField = 13u,    //BMC.ARSystem.DateValue.JulianDate // Date value = Oct 20, 2002
        TimeOnlyField = 14u,    //ARSystem.TimeOfDayValue (((6 * 60)  + 32) * 60) + 27; // Time of day = 6:32:27AM
        ULong = 40u,
        Coords = 41u
    }
}
