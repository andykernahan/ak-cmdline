// Copyright 2011 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.ComponentModel;
using AK.CmdLine.Impl.Utility;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Defines the base descriptor class. This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class DescriptorBase
    {
        #region Fields.

        private string _shortName;
        private string _description;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets the <see cref="System.StringComparer"/> used to comparer names. This field is
        /// <see langword="readonly"/>.
        /// </summary>
        public static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns> <see langword="true"/> if the specified <see cref="System.Object"/> is equal
        /// to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj == this)
            {
                return true;
            }
            if(obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            var other = (DescriptorBase)obj;
            return NameEquals(other.Name) || NameEquals(other.ShortName);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeBuilder.New().Add(Name);
        }

        /// <summary>
        /// Returns a value indicating if the specified name matches the name of this descriptor.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see langword="true"/> if <paramref name="name"/> matches the name of this
        /// descriptor, otherwise; <see langword="false"/>.</returns>
        public bool IsNamed(string name)
        {
            return NameEquals(name);
        }

        /// <summary>
        /// When overriden in a derived class; returns a <see cref="System.String"/> that represents
        /// this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();

        /// <summary>
        /// When overriden in a derived class; gets the name of this descriptor.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the short name of this descriptor.
        /// </summary>
        public string ShortName
        {
            get
            {
                if(_shortName == null)
                {
                    _shortName = With<ShortNameAttribute>(x => x.ShortName);
                }
                return _shortName;
            }
        }

        /// <summary>
        /// Gets a description of this descriptor.
        /// </summary>
        public string Description
        {
            get
            {
                if(_description == null)
                {
                    _description = With<DescriptionAttribute>(x => x.Description);
                }
                return _description;
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; gets the attribute of type
        /// <typeparamref name="TAttribute"/> from the underlying member info.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute <see cref="System.Type"/>.</typeparam>
        /// <returns>The attribute if founds, otherwise; <see langword="null"/>.</returns>
        protected abstract TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute;

        #endregion

        #region Private Impl.

        private bool NameEquals(string s)
        {
            return NameComparer.Equals(Name, s) || (ShortName.Length > 0 && NameComparer.Equals(ShortName, s));
        }

        private string With<TAttribute>(Func<TAttribute, string> selector) where TAttribute : Attribute
        {
            var attr = GetAttribute<TAttribute>();
            return attr != null ? selector(attr) : String.Empty;
        }

        #endregion
    }
}