using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things;

namespace Interfaces
{
	/// <summary>
	/// Represents something that contains items, but that it might not make
	/// sense to be able to add or remove items from.
	/// </summary>
	public interface Searchable
	{
		/// <summary>
		/// Accessor for the contents of the <see cref="Searchable"/>.
		/// </summary>
		/// <returns>a HashSet of the contents of the <see cref="Searchable"/></returns>
		HashSet<Thing> GetContents();
		/// <summary>
		/// Accessor for the visible contents of the <see cref="Searchable"/>.
		/// </summary>
		/// <returns>a HashSet of the visible contents of the <see cref="Searchable"/></returns>
		HashSet<Thing> GetVisibleContents();
		/// <summary>
		/// Gets the contents of this <see cref="Searchable"/> and the contents of any
		/// <see cref="Searchable"/> inside this <see cref="Searchable"/>, recursively.
		/// </summary>
		/// <returns>a HashSet of the recursive contents of this container</returns>
		HashSet<Thing> GetRecursiveContents();
		/// <summary>
		/// Gets the visible contents of this <see cref="Searchable"/> and the visible
		/// contents of any <see cref="Searchable"/> inside this <see cref="Searchable"/>, recursively.
		/// </summary>
		/// <returns>a HashSet of the visible recursive contents of this <see cref="Searchable"/></returns>
		HashSet<Thing> GetVisibleRecursiveContents();
	}
}
