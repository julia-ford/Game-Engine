using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stuff.Things;

namespace Interfaces
{
	/// <summary>
	/// Represents anything with a flat surface that <see cref="Thing"/>s can
	/// be deposited on.
	/// </summary>
	interface Supporter : Searchable
	{
		/// <summary>
		/// Determines whether this <see cref="Supporter"/> supports a
		/// specified <see cref="Thing"/>.
		/// </summary>
		/// 
		/// <param name="item">the item to check for</param>
		/// 
		/// <returns>
		/// true if this supports the <see cref="Thing"/>, false otherwise
		/// </returns>
		bool Supports(Thing item);

		/// <summary>
		/// Determines whether this <see cref="Supporter"/> currently supports
		/// anything.
		/// </summary>
		/// 
		/// <returns>
		/// true if the <see cref="Supporter"/> supports at least one
		/// <see cref="Thing"/>, false otherwise
		/// </returns>
		bool IsEmpty();

		/// <summary>
		/// Adds the specified <see cref="Thing"/> to the
		/// <see cref="Supporter"/>.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to add</param>
		void AddThing(Thing item);

		/// <summary>
		/// Removes the specified <see cref="Thing"/> from the
		/// <see cref="Supporter"/>.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to remove</param>
		void RemoveThing(Thing item);
	}
}
