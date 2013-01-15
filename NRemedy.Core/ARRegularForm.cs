//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-10
//------------------------------------------------------------------
using System;

namespace NRemedy.Dropped
{
    /// <summary>
    /// this class is dropped,and will be removed in the next version
    /// </summary>
    [Obsolete]
    public class ARRegularForm : ARBaseForm
    {
        public enum Status_Enum
        {
            New,
            Assigned,
            Fixed,
            Rejected,
            Closed
        }

        private string _Request_ID;
        private string _Submitter;
        private DateTime _Create_Date;
        private string _Assigned_To;
        private string _Last_Modifed_By;
        private DateTime _Modifed_Date;
        private Status_Enum _Status;
        private string _Short_Description;

        [AREntryKey]
        [ARField(DatabaseID = 1, DatabaseName = "Request ID", DataType = ARType.CharacterField, BinderAccess = ModelBinderAccessLevel.OnlyBind)]
        public string Request_ID
        {
            get
            {
                return _Request_ID;
            }
            protected set
            {
                _Request_ID = value;
                OnPropertyChanged("Request_ID");
            }
        }

        [ARField(DatabaseID = 2, DatabaseName = "Submitter", DataType = ARType.CharacterField)]
        public string Submitter
        {
            get
            {
                return _Submitter;
            }
            set
            {
                _Submitter = value;
                OnPropertyChanged("Submitter");
            }
        }

        [ARField(DatabaseID = 3, DatabaseName = "Create Date", DataType = ARType.DateTimeField, BinderAccess = ModelBinderAccessLevel.OnlyBind)]
        public DateTime Create_Date
        {
            get
            {
                return _Create_Date;
            }
            protected set
            {
                _Create_Date = value;
                OnPropertyChanged("Create_Date");
            }
        }

        [ARField(DatabaseID = 4, DatabaseName = "Assigned To", DataType = ARType.CharacterField)]
        public string Assigned_To
        {
            get
            {
                return _Assigned_To;
            }
            set
            {
                _Assigned_To = value;
                OnPropertyChanged("Assigned_To");
            }
        }

        [ARField(DatabaseID = 5, DatabaseName = "Last Modified By", DataType = ARType.CharacterField, BinderAccess = ModelBinderAccessLevel.OnlyBind)]
        public string Last_Modifed_By
        {
            get
            {
                return _Last_Modifed_By;
            }
            protected set
            {
                _Last_Modifed_By = value;
                OnPropertyChanged("Last_Modifed_By");
            }
        }

        [ARField(DatabaseID = 6, DatabaseName = "Modified Date", DataType = ARType.DateTimeField, BinderAccess = ModelBinderAccessLevel.OnlyBind)]
        public DateTime Modifed_Date
        {
            get
            {
                return _Modifed_Date;
            }
            protected set
            {
                _Modifed_Date = value;
                OnPropertyChanged("Modifed_Date");
            }
        }

        [ARField(DatabaseID = 7, DatabaseName = "Status", DataType = ARType.SelectionField)]
        public Status_Enum Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        [ARField(DatabaseID = 8, DatabaseName = "Short Description", DataType = ARType.CharacterField)]
        public string Short_Description
        {
            get
            {
                return _Short_Description;
            }
            set
            {
                _Short_Description = value;
                OnPropertyChanged("Short_Description");
            }
        }

        //public virtual string DefaultEntryKey
        //{
        //    get { return Request_ID; }
        //    set { Request_ID = value; }
        //}
    }
}
