using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy
{
    /// <summary>
    /// The model metadata of the T and model, this is used for ARProxy to do the model binding.
    /// the provider can choose some of them to implement according to requirement
    /// </summary>
    public class ModelMeteData<TModel>
    {
        /// <summary>
        /// the model object of the TModel
        /// create|set
        /// </summary>
        public TModel Model { get; set; }

        /// <summary>
        /// The formName of the TModel,which usually decorate with ARFormAttribute
        /// create|set|delete|get
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// indicate the valid properties in TModel
        /// create|set|get|statics
        /// </summary>
        public List<PropertyAndField<TModel>> Properties { get; set; }

        /// <summary>
        /// indicate the entryid value of the Model
        /// delete|set
        /// </summary>
        public string EntryId { get; set; }

    }
}
