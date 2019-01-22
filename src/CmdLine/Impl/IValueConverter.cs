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

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Provides support for converting strings to other types.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Attempts to convert the specified <paramref name="value"/> to the specified
        /// <paramref name="target"/> <see cref="System.Type"/> and returns a value indicating
        /// success.
        /// </summary>
        /// <param name="value">The String.</param>
        /// <param name="target">The target <see cref="System.Type"/>.</param>
        /// <param name="result">When this methods returns, contains the parsed value on success,
        /// otherwise; <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when the conversion was successful, otherwise;
        /// <see langword="false"/>.</returns>
        bool TryConvert(string value, Type target, out object result);
    }
}