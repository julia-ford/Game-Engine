using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Interfaces;
using Meta.ParsingAndPrinting;
using Meta.Exceptions.OpeningAndClosingExceptions;
using Meta.Exceptions.TakingAndDroppingExceptions;

namespace Stuff.Things.Containers
{
	/// <summary>
	/// Represents a container that can be opened and closed.
	/// </summary>
	public class BasicOpenableContainer : BasicContainer, OpenableContainer
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// whether or not the contaier is currently open
		/// </summary>
		private bool isOpen;

		/// <summary>
		/// whether or not the container is opaque
		/// </summary>
		private bool isOpaque;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new container with the specified name and description
		/// that is either opaque or translucent.
		/// </summary>
		/// <param name="name">the name of the container</param>
		/// <param name="description">the description of the container</param>
		/// <param name="isOpaque">whether or not the container is opaque</param>
		public BasicOpenableContainer(
			string name, string description, bool isOpaque,
			string[] parsedNames = null,
			bool isSpecific = false, bool isPlural = false, bool isProper = false,
			bool canBeTaken = true, bool canBeDropped = true, bool isTwoHanded = false) :
			base(name, description,  parsedNames:parsedNames,
			isSpecific: isSpecific, isPlural: isPlural, isProper: isProper,
			canBeTaken:canBeTaken,canBeDropped:canBeDropped,isTwoHanded:isTwoHanded)
		{
			this.isOpen = false;
			this.isOpaque = isOpaque;
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// This override makes it so a closed, opaque container's contents
		/// are not considered visible.
		/// </summary>
		/// <returns>the visible contents of the container</returns>
		public override HashSet<Thing> GetVisibleContents()
		{
			if (this.isOpaque && !this.isOpen) {
				return new HashSet<Thing>(); }
			else {
				return base.GetVisibleContents(); }
		}

		/// <summary>
		/// Sees to it that the contents of a closed, opaque container are not considered visible.
		/// </summary>
		/// <returns>a recursive list of its contents if it is open and/or not opaque; an empty list otherwise</returns>
		public override HashSet<Thing> GetVisibleRecursiveContents()
		{
			if (this.isOpaque && !this.isOpen) {
				return new HashSet<Thing>(); }
			else {
				return base.GetVisibleRecursiveContents(); }
		}

		/// <summary>
		/// This override prevents the contsiner from providing additional information about itself if it is closed and opaque.
		/// </summary>
		/// <returns></returns>
		public override string GetName() {
			if (this.isOpaque && !this.isOpen) {
				if (this.IsEmpty()) {
					return base.GetName().Substring(
						base.GetName().IndexOf(' ') + 1); }
				else {
					return base.GetName().Substring(0,
						base.GetName().IndexOf('(') - 1); }
			} // end "if this is opaque and closed"

			else {
				return base.GetName(); }
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Determines whether or not the container is currently open.
		/// </summary>
		/// <returns>true if the container is open; false otherwise</returns>
		public bool IsOpen()
		{
			return this.isOpen;
		}

		/// <summary>
		/// Accessor for the container's opacity status.
		/// </summary>
		/// <returns>true if the container is opaque; false otherwise</returns>
		public bool IsOpaque()
		{
			return this.isOpaque;
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// Opens the container.
		/// </summary>
		/// <exception cref="OpeningAlreadyOpenContainerException">if the container is already open</exception>
		public void Open()
		{
			// if it's already open, throw an exception
			if (this.isOpen) {
				throw new OpeningAlreadyOpenContainerException(); }

			// otherwise, open the container
			else {
				this.isOpen = true; }
		}

		/// <summary>
		/// Closes the container.
		/// </summary>
		/// <exception cref="ClosingAlreadyClosedContinerException">if the container is already closed</exception>
		public void Close()
		{
			if (!this.isOpen) {
				throw new ClosingAlreadyClosedContinerException(); }

			else {
				this.isOpen = false; }
		}

	}
}
