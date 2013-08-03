using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Interfaces;
using Meta.ParsingAndPrinting;
using Stuff.Things;
using Stuff.Things.Actors;

namespace Stuff
{
	/// <summary>
	/// Represents a physical area in the game world. This can be a literal
	/// room in building, or a garden, or "east of the house," as long as it
	/// makes sense as a thing that the player can enter and exit.
	/// </summary>
	public class Room : Describable, Container
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// All of the rooms that are adjacent / adjoining to this one. Used for handling movement.
		/// </summary>
		private Dictionary<Direction, Room> adjacencies;

		/// <summary>
		/// All of the <see cref="Thing"/>s that the room contains.
		/// </summary>
		private HashSet<Thing> contents;

		/// <summary>
		/// The name of the room.
		/// </summary>
		private string name;

		/// <summary>
		/// a brief description of the room
		/// </summary>
		private string description;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new room with the specified name and description.
		/// </summary>
		/// <param name="name">the name of the room</param>
		/// <param name="description">a brief description of the room</param>
		public Room(string name, string description)
		{
			this.name = name;
			this.description = description;
			this.adjacencies = new Dictionary<Direction, Room>();
			this.contents = new HashSet<Thing>();
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Gets a reference for the adjacent room in the given direction.
		/// </summary>
		/// <param name="dir">the direction of the adjacent room</param>
		/// <returns>a reference to the room, if it exists; null otherwise</returns>
		public Room GetAdjacent(Direction dir)
		{
			if (!this.adjacencies.ContainsKey(dir)) { return null; }
			return this.adjacencies[dir];
		}

		/// <summary>
		/// Accessor for the contents of the room.
		/// </summary>
		/// <returns>a HashSet containing the contents of the room</returns>
		public HashSet<Thing> GetContents()
		{
			return new HashSet<Thing>(this.contents);
		}

		/// <summary>
		/// Accessor for the visible contents of the room.
		/// At this point, this is the exact same as Room.GetContents()
		/// </summary>
		/// <returns>a HashSet containing the visible contents of the room</returns>
		public HashSet<Thing> GetVisibleContents()
		{
			return new HashSet<Thing>(this.contents);
		}

		/// <summary>
		/// Gets the contents of the room, the contents of any containers in
		/// the room, and the inventories and clothes of any people in the
		/// room, recursively.
		/// </summary>
		/// <returns>a HashSet containing the recursive contents of the room</returns>
		public HashSet<Thing> GetRecursiveContents()
		{
			HashSet<Thing> roomSet = new HashSet<Thing>(this.contents);
			foreach (Thing item in this.contents)
			{
				if (typeof(Searchable).IsAssignableFrom(item.GetType()))
				{
					roomSet.UnionWith(((Searchable)item).GetRecursiveContents());
				}
			}
			return roomSet;
		}

		/// <summary>
		/// Gets the visible contents of the room, the visible contents of any
		/// containers in the room, and the visible clothes of any people in
		/// the room, recursively.
		/// </summary>
		/// <returns>a HashSet containing the visible, recursive contents of the room</returns>
		public HashSet<Thing> GetVisibleRecursiveContents()
		{
			HashSet<Thing> roomSet = new HashSet<Thing>(this.contents);
			foreach (Thing item in this.contents) {
				if (typeof(Searchable).IsAssignableFrom(item.GetType())) {
					roomSet.UnionWith(((Searchable)item).GetVisibleRecursiveContents()); }
			} // end foreach
			return roomSet;
		}

		/// <summary>
		/// Accessor for a room's name.
		/// </summary>
		/// <returns>the name of the room</returns>
		public string GetName()
		{
			return this.name;
		}
		
		/// <summary>
		/// Gets the name of the thing preceded by an appropriate article.
		/// By default, the article is always "the."
		/// </summary>
		/// <returns></returns>
		public string GetQualifiedName()
		{
			return "the " + this.name;
		}

		/// <summary>
		/// This is just a stub at this point, but eventually it will
		/// thoroughly describe a room, its contents, and its exits.
		/// </summary>
		/// <returns>a description of the room</returns>
		public string GetDescription()
		{
			string finalDescription = this.description;
			string contentsDescription = this.getContentsDescription();
			string adjacenciesDescription = this.getAdjacenciesDescription();

			if (contentsDescription != null) { finalDescription += "\n\n" + contentsDescription; }

			if (adjacenciesDescription != null) { finalDescription += "\n\n" + adjacenciesDescription; }

			return finalDescription;
		}

		/// <summary>
		/// A helper method that describes the contents of the room, if there are any.
		/// </summary>
		/// <returns>a description of the contents of the room if the room 
		/// contains some things besides the player; null if the player is the
		/// only thing in the room</returns>
		private string getContentsDescription()
		{
			// make a copy of the room contents without the player in it
			HashSet<Thing> contentsSansPlayer = new HashSet<Thing>(this.contents);
			GameManager.RemovePlayer(contentsSansPlayer);

			// return null if the room is empty.
			if (contentsSansPlayer.Count == 0) { return null; }
			else if (contentsSansPlayer.Count == 1) {
				return "You can see " + contentsSansPlayer.ElementAt(0).GetQualifiedName() + " here."; }
			else if (contentsSansPlayer.Count == 2) {
				return "You can see " + contentsSansPlayer.ElementAt(0).GetQualifiedName()
					+ " and " + contentsSansPlayer.ElementAt(1).GetQualifiedName() + " here."; }
			else {
				string returnString = "You can see ";
				for (int i = 0; i < contentsSansPlayer.Count - 1; i++) {
					returnString += contentsSansPlayer.ElementAt(i).GetQualifiedName() + ", "; }
				returnString += "and " + contentsSansPlayer.ElementAt(contentsSansPlayer.Count - 1).GetQualifiedName() + " here.";
				return returnString; }
		}

		/// <summary>
		/// Gets a description of adjacent rooms.
		/// </summary>
		/// <returns>a description of the adjacent rooms</returns>
		private string getAdjacenciesDescription()
		{
			if (this.adjacencies.Count == 0)
			{
				return null;
			}
			else if (this.adjacencies.Count == 1)
			{
				return this.getAdjacentDescription(this.adjacencies.ElementAt(0).Key, this.adjacencies.ElementAt(0).Value);
			}
			else
			{
				string adjacentDescription = this.getAdjacentDescription(this.adjacencies.ElementAt(0).Key, this.adjacencies.ElementAt(0).Value);
				for (int i = 1; i < this.adjacencies.Count; i++)
				{
					adjacentDescription += '\n' + this.getAdjacentDescription(this.adjacencies.ElementAt(i).Key, this.adjacencies.ElementAt(i).Value);
				}
				return adjacentDescription;
			}
		}

		/// <summary>
		/// Describes a single adjacent room.
		/// </summary>
		/// <param name="dir">the direction of the adjacent room</param>
		/// <param name="room">the adjacent room</param>
		/// <returns>a description of the adjacent room</returns>
		private string getAdjacentDescription(Direction dir, Room room)
		{
			if (dir == Direction.up || dir == Direction.down)
			{
				return StringManipulator.CapitalizeFirstLetter(dir.ToString()) + " from here is " + room.GetQualifiedName() + '.';
			}
			else
			{
				return "To the " + StringManipulator.CapitalizeFirstLetter(dir.ToString()) + " is " + room.GetQualifiedName() + '.';
			}
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Checks if there is an adjoining room in a given direction.
		/// </summary>
		/// <param name="dir">the direction to check</param>
		/// <returns>true if there is an adjoing room in the given direction; false otherwise</returns>
		public bool HasAdjacent(Direction dir)
		{
			return this.adjacencies.ContainsKey(dir);
		}

		/// <summary>
		/// Determines whether or not the room is empty.
		/// </summary>
		/// <returns>true if the room is empty; false if the room contains at least one thing</returns>
		public bool IsEmpty()
		{
			return this.contents.Count == 0;
		}

		/// <summary>
		/// Determines whether the room contains a specified item.
		/// </summary>
		/// <param name="item">the item to check for</param>
		/// <returns>true if the room contains the item; false if it does not</returns>
		public bool Contains(Thing item)
		{
			return this.contents.Contains(item);
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// Creates a connection from one room to another.
		/// This connection is not mutual.
		/// You can travel to the specified room, but not back.
		/// If this room already connects to another in the given direction,
		/// then that connection is replaced with the new one.
		/// </summary>
		/// <param name="room">the room to which a connection is created</param>
		/// <param name="dirToRoom">the direction in which the connection is created</param>
		public void SetAdjacent(Room room, Direction dirToRoom)
		{
			// remove existing connection, if it exists
			if (this.adjacencies.ContainsKey(dirToRoom))
			{
				this.adjacencies.Remove(dirToRoom);
			}
			// add new connection
			this.adjacencies.Add(dirToRoom, room);
		}

		/// <summary>
		/// Makes two given rooms adjoin to each other in the specified directions.
		/// Allows for non-euclidean geometry. This intentional, because sometimes I
		/// like to put non-euclidean geometry in my games.
		/// </summary>
		/// <param name="room1">the first room to connect</param>
		/// <param name="dirToRoom1">the direction to go in, from room2, to get to room1</param>
		/// <param name="room2">the second room to connect</param>
		/// <param name="dirToRoom2">the direction to go in, from room1, to get to room2</param>
		public static void SetAdjacent(Room room1, Direction dirToRoom1, Room room2, Direction dirToRoom2)
		{
			room1.SetAdjacent(room2, dirToRoom2);
			room2.SetAdjacent(room1, dirToRoom1);
		}

		/// <summary>
		/// Adds a <see cref="Thing"/> to the contents of the room. If the
		/// specified item is already in the room, throws an exception instead.
		/// </summary>
		/// <param name="item">the thing to be added to the room</param>
		public void AddThing(Thing item)
		{
			// if the room already contains that item, throw an exception
			if (this.contents.Contains(item)) {
				throw new Exception("Error: Attempted to add something to a room that it was already inside of."); }

			else { // just add the item normally
				this.contents.Add(item); }
		}

		/// <summary>
		/// Removes a <see cref="Thing"/> from the contents of the room. If the
		/// specified item is not in the room, throws an exception instead.
		/// </summary>
		/// <param name="item">the thing to be removed from the room</param>
		public void RemoveThing(Thing item)
		{
			// if the room does not contain the item, throw an exception
			if (!this.contents.Remove(item)) {
				throw new Exception("Error: Attempted to remove something from a room that did not contain it."); }
		}

	}
}
