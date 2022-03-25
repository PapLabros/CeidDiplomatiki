using Atom.Core;
using System;
using System.Windows.Media;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Provides enumeration over the available column attributes
    /// </summary>
    public class ColumnAttribute : IIdentifiable, INameable, IEquatable<ColumnAttribute>
    {
        #region Public Properties

        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The color that represents the attribute
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The id of the group.
        /// NOTE: The group id acts as a validator! Two attributes with the same group id can't be applied
        ///       to the same column!
        /// NOTE: When the <see cref="GroupId"/> is set to <see cref="null"/> then the validation isn't
        ///       applied!
        /// </summary>
        public string GroupId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ColumnAttribute() : base()
        {

        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="id">The id</param>
        /// <param name="name">The name</param>
        /// <param name="color">The color</param>
        /// <param name="groupId">
        /// The id of the group.
        /// NOTE: The group id acts as a validator! Two attributes with the same group id can't be applied
        ///       to the same column!
        /// NOTE: When the <see cref="GroupId"/> is set to <see cref="null"/> then the validation isn't
        ///       applied!
        /// </param>
        public ColumnAttribute(string id, string name, string color, string groupId = null)
        {
            Id = id;
            Name = name;
            Color = color;
            GroupId = groupId;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ColumnAttribute attribute)
                return Equals(attribute);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(ColumnAttribute other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Color, GroupId);
        }

        #endregion
    }
}
