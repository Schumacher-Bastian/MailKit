﻿//
// UniqueIdMap.cs
//
// Author: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2013-2016 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace MailKit {
	/// <summary>
	/// A unique identifier mapping.
	/// </summary>
	/// <remarks>
	/// Maps a source UID to a destination UID.
	/// </remarks>
	public class UniqueIdMapping
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.UniqueIdMapping"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="MailKit.UniqueIdMapping"/>.
		/// </remarks>
		/// <param name="src">The source UID.</param>
		/// <param name="dest">The destination UID.</param>
		public UniqueIdMapping (UniqueId src, UniqueId dest)
		{
			Destination = dest;
			Source = src;
		}

		/// <summary>
		/// Gets the unique identifier of the message in the source folder.
		/// </summary>
		/// <remarks>
		/// Gets the unique identifier of the message in the source folder.
		/// </remarks>
		/// <value>The unique identifier of the message in the source folder.</value>
		public UniqueId Source {
			get; private set;
		}

		/// <summary>
		/// Gets the unique identifier of the message in the destination folder.
		/// </summary>
		/// <remarks>
		/// Gets the unique identifier of the message in the destination folder.
		/// </remarks>
		/// <value>The unique identifier of the message in the destination folder.</value>
		public UniqueId Destination {
			get; private set;
		}
	}

	/// <summary>
	/// A mapping of unique identifiers.
	/// </summary>
	/// <remarks>
	/// A mapping of unique identifiers.
	/// </remarks>
	public class UniqueIdMap : IEnumerable<UniqueIdMapping>
	{
		/// <summary>
		/// Any empty mapping of unique identifiers.
		/// </summary>
		/// <remarks>
		/// Any empty mapping of unique identifiers.
		/// </remarks>
		public static readonly UniqueIdMap Empty = new UniqueIdMap ();

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.UniqueIdMap"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="MailKit.UniqueIdMap"/>.
		/// </remarks>
		/// <param name="source">The unique identifiers used in the source folder.</param>
		/// <param name="destination">The unique identifiers used in the destination folder.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="source"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		public UniqueIdMap (IList<UniqueId> source, IList<UniqueId> destination)
		{
			if (source == null)
				throw new ArgumentNullException ("source");

			if (destination == null)
				throw new ArgumentNullException ("destination");

			Destination = destination;
			Source = source;
		}

		UniqueIdMap ()
		{
			Destination = Source = new UniqueId[0];
		}

		/// <summary>
		/// Gets the unique identifiers used in the source folder.
		/// </summary>
		/// <remarks>
		/// Gets the unique identifiers used in the source folder.
		/// </remarks>
		/// <value>The unique identifiers used in the source folder.</value>
		public IList<UniqueId> Source {
			get; private set;
		}

		/// <summary>
		/// Gets the unique identifiers used in the destination folder.
		/// </summary>
		/// <remarks>
		/// Gets the unique identifiers used in the destination folder.
		/// </remarks>
		/// <value>The unique identifiers used in the destination folder.</value>
		public IList<UniqueId> Destination {
			get; private set;
		}

		/// <summary>
		/// Tries to get the remapped unique identifier.
		/// </summary>
		/// <remarks>
		/// Attempts to get the remapped unique identifier.
		/// </remarks>
		/// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
		/// <param name="src">The unique identifier of the message in the source folder.</param>
		/// <param name="dest">The unique identifier of the message in the destination folder.</param>
		public bool TryGetValue (UniqueId src, out UniqueId dest)
		{
			int index = Source.IndexOf (src);

			if (index == -1 || index >= Destination.Count) {
				dest = UniqueId.Invalid;
				return false;
			}

			dest = Destination[index];

			return true;
		}

		/// <summary>
		/// Gets the remapped unique identifier.
		/// </summary>
		/// <remarks>
		/// Gets the remapped unique identifier.
		/// </remarks>
		/// <param name="index">The unique identifier of the message in the source folder.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		public UniqueId this [UniqueId index] {
			get {
				UniqueId uid;

				if (!TryGetValue (index, out uid))
					throw new ArgumentOutOfRangeException ("index");

				return uid;
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <remarks>
		/// Gets the enumerator.
		/// </remarks>
		/// <returns>The enumerator.</returns>
		public IEnumerator<UniqueIdMapping> GetEnumerator ()
		{
			var dst = Destination.GetEnumerator ();
			var src = Source.GetEnumerator ();

			while (src.MoveNext () && dst.MoveNext ())
				yield return new UniqueIdMapping (src.Current, dst.Current);

			yield break;
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <remarks>
		/// Gets the enumerator.
		/// </remarks>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}