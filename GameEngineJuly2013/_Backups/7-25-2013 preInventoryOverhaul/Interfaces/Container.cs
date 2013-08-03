using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things;

namespace Interfaces
{
	/// <summary>
	/// This interface is used for aything that can hold items, be it a
	/// <see cref="Room"/> or a <see cref="Thing"/>. The one exemption from
	/// this is a <see cref="Person"/>'s inventory and clothes.
	/// </summary>
	public interface Container : Describable, Searchable
	{
		/// <summary>
		/// Determines whether this container contains a specified <see cref="Thing"/>.
		/// </summary>
		/// <param name="item">the item to check for</param>
		/// <returns>true if this contains the item, false otherwise</returns>
		bool Contains(Thing item);
		/// <summary>
		/// Determines whether this container is empty.
		/// </summary>
		/// <returns>true if this is empty; false otherwise</returns>
		bool IsEmpty();

		/// <summary>
		/// Adds the specified <see cref="Thing"/> to the container.
		/// </summary>
		/// <param name="item">the thing to add</param>
		void AddThing(Thing item);
		/// <summary>
		/// Removes the specified <see cref="Thing"/> from the container.
		/// </summary>
		/// <param name="item">the thing to remove</param>
		void RemoveThing(Thing item);
	}
}
