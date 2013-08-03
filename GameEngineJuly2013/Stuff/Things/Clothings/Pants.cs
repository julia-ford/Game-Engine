using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Meta.ParsingAndPrinting;

namespace Stuff.Things.Clothings
{
	/// <summary>
	/// Represents a pair of pants.
	/// </summary>
	class Pants : Clothing
	{
		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates some new <see cref="Pants"/> with the following
		/// specifications.
		/// </summary>
		/// 
		/// <param name="name">the name of the <see cref="Pants"/></param>
		/// 
		/// <param name="description">
		/// a brief description of the <see cref="Pants"/>
		/// </param>
		/// 
		/// <param name="isSpecific">
		/// whether or not to use "the" as the article when printing the name
		/// of the <see cref="Pants"/>
		/// </param>
		/// 
		/// <param name="isPlural">
		/// whether or not this is a plural noun
		/// </param>
		/// 
		/// <param name="isProper">
		/// whether or not an article should be used for this item
		/// </param>
		/// 
		/// <param name="canBeTaken">whether or not this can be taken</param>
		/// 
		/// <param name="canBeDropped">
		/// whether or not this can be put down after it is picked up
		/// </param>
		/// 
		/// <param name="canBeRemoved">
		/// whether or not this can be taken off once it is worn
		/// </param>
		public Pants(string name, string description, string[] parsedNames = null,
			bool isSpecific=false, bool isPlural=true, bool isProper=false,
			bool canBeTaken=true,bool canBeDropped=true,bool canBeRemoved=true)
			: base(name, description, new ClothingSlot[] {ClothingSlot.pants},
			parsedNames:parsedNames,
			isSpecific:isSpecific, isPlural:isPlural, isProper:isProper,
			canBeTaken:canBeTaken,canBeDropped:canBeDropped,canBeRemoved:canBeRemoved){ }

	}
}
