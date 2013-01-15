//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-11
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    public class DuplicateIDException : Exception
    {
        public uint ID { get; set; }

        public string ExternalMessage { get; set; }

        public DuplicateIDException()
        {
        }

        public DuplicateIDException(uint ID)
        {
            this.ID = ID;
        }

        public DuplicateIDException(uint ID, string Message)
        {
            this.ID = ID;
            this.ExternalMessage = Message;
        }

        public override string Message
        {
            get
            {
                return string.Format("Object already contains the field of ID : {0}.{1}", ID.ToString(), ExternalMessage);
            }
        }
    }
}
