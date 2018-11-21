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
// See the License for the specific

using System;
using System.ComponentModel;
using System.Globalization;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// A default <see cref="AK.CmdLine.Impl.IValueConverter"/> implementation.
    /// </summary>
    public class DefaultValueConverter : IValueConverter
    {
        #region Public Interface.

        /// <summary>
        /// Initialises the <see cref="AK.CmdLine.Impl.DefaultValueConverter"/> class.
        /// </summary>
        static DefaultValueConverter()
        {
            RegisterBuiltInConverters();
        }

        /// <summary>
        /// Attemps to convert the specified <paramref name="value"/> to the specified
        /// <paramref name="target"/> <see cref="System.Type"/> and returns a value indicating
        /// success.
        /// </summary>
        /// <param name="value">The String.</param>
        /// <param name="target">The target <see cref="System.Type"/>.</param>
        /// <param name="result">When this methods returns, contains the parsed value on success,
        /// otherwise; <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when the conversion was successful, otherwise;
        /// <see langword="false"/>.</returns>
        public virtual bool TryConvert(string value, Type target, out object result)
        {
            try
            {
                result = TypeDescriptor.GetConverter(target).ConvertFromString(value);
                return true;
            }
            catch(Exception e)
            {
                // Nasty, the type descriptor wraps some exceptions in, err... an Exception.
                // Rethrow if we aren't expecting the exception or inner exception.
                if(!(IsExpectedConversionException(e) ||
                    IsExpectedConversionException(e.InnerException)))
                {
                    throw;
                }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Registers a conversion method for <see cref="System.String"/> to
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TDestination">The destination <see cref="System.Type"/>.</typeparam>
        /// <param name="converter">The conversion method.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="converter"/> is <see langword="null"/>.
        /// </exception>
        public static void Register<TDestination>(Func<string, TDestination> converter)
        {
            Guard.NotNull(converter, "convertFrom");

            TypeDescriptor.AddProvider(
                new Provider(new Descriptor(new Converter<TDestination>(converter))),
                typeof(TDestination));
        }

        #endregion

        #region Private Impl.

        private static bool IsExpectedConversionException(Exception e)
        {
            return e is FormatException ||
                e is NotSupportedException ||
                e is OverflowException ||
                e is ArgumentException;
        }

        private static void RegisterBuiltInConverters()
        {
            RegisterBooleanConverter();
        }

        private static void RegisterBooleanConverter()
        {
            Register(value =>
            {
                var comparer = StringComparer.Ordinal;
                if(comparer.Equals(value, "+"))
                {
                    return true;
                }
                if(comparer.Equals(value, "-"))
                {
                    return false;
                }
                return Boolean.Parse(value);
            });
        }

        private sealed class Provider : TypeDescriptionProvider
        {
            private readonly ICustomTypeDescriptor _descriptor;

            public Provider(ICustomTypeDescriptor descriptor)
            {
                _descriptor = descriptor;
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return _descriptor;
            }
        }

        private sealed class Descriptor : CustomTypeDescriptor
        {
            private readonly TypeConverter _converter;

            public Descriptor(TypeConverter converter)
            {
                _converter = converter;
            }

            public override TypeConverter GetConverter()
            {
                return _converter;
            }
        }

        private sealed class Converter<TDesination> : TypeConverter
        {
            private readonly Func<string, TDesination> _convertFrom;

            public Converter(Func<string, TDesination> convertFrom)
            {
                _convertFrom = convertFrom;
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                CultureInfo culture, object value)
            {
                var s = value as string;
                return s != null ? _convertFrom(s) : base.ConvertFrom(context, culture, value);
            }
        }

        #endregion
    }
}