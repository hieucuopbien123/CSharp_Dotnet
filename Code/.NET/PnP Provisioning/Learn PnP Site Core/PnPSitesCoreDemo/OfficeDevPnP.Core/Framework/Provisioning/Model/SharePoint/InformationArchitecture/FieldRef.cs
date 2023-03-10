using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public partial class FieldRef : BaseModel, IEquatable<FieldRef>
    {
        #region Private Members

        private Guid _id = Guid.Empty;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets ot sets the ID of the referenced field
        /// </summary>
        public Guid Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the field link. This will not change the internal name of the field.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the Display Name of the field. Only applicable to fields associated with lists.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets if the field is Required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets if the field is Hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Declares if the FieldRef should be Removed from the list or library, optional attribute.
        /// </summary>
        public bool Remove { get; set; }

		/// <summary>
		/// Declares whether the current field reference has to be udpated on inherited content types
		/// </summary>
		public bool UpdateChildren { get; set; } = true;

#if !SP2013 && !SP2016
        /// <summary>
        /// Gets or sets if the field is shown in the display form
        /// </summary>
        public bool ShowInDisplayForm { get; set; }

        /// <summary>
        /// Gets or sets if the field is read only
        /// </summary>
        public bool ReadOnly { get; set; }
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for FieldRef class
        /// </summary>
        public FieldRef()
        {
        }

        /// <summary>
        /// Constructor for FieldRef class
        /// </summary>
        /// <param name="fieldRefName">Name of the Field Reference</param>
        public FieldRef(string fieldRefName)
        {
            this.Name = fieldRefName;
        }

        #endregion

        #region Comparison code
        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Returns HashCode</returns>
        public override int GetHashCode()
        {
            return (String.Format(
                "{0}|{1}|{2}|{3}|{4}|"
#if !SP2013 && !SP2016
                + "{5}|{6}|"
#endif
                ,
                (this.Id != null ? this.Id.GetHashCode() : 0),
                this.Required.GetHashCode(),
                this.Hidden.GetHashCode(),
                this.Remove.GetHashCode(),
                this.UpdateChildren.GetHashCode()
#if !SP2013 && !SP2016
                ,
                this.ShowInDisplayForm.GetHashCode(),
                this.ReadOnly.GetHashCode()
#endif
            ).GetHashCode());
        }

        /// <summary>
        /// Compares object with FieldRef
        /// </summary>
        /// <param name="obj">Object that represents FieldRef</param>
        /// <returns>true if the current object is equal to the FieldRef</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FieldRef))
            {
                return (false);
            }
            return (Equals((FieldRef)obj));
        }

        /// <summary>
        /// Compares FieldRef object based on Id, Required, Hidden, Remove, and UpdateChildren properties.
        /// </summary>
        /// <param name="other">FieldRef object</param>
        /// <returns>true if the FieldRef object is equal to the current object; otherwise, false.</returns>
        public bool Equals(FieldRef other)
        {
            if (other == null)
            {
                return (false);
            }

            return (this.Id == other.Id &&
                this.Required == other.Required &&
                this.Hidden == other.Hidden &&
                this.Remove == other.Remove &&
                this.UpdateChildren == other.UpdateChildren
#if !SP2013 && !SP2016
                && this.ShowInDisplayForm == other.ShowInDisplayForm
                && this.ReadOnly == other.ReadOnly
#endif
                );
        }

        #endregion
    }
}
