using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// internal stuff
using Stuff;
using Stuff.Things;
using Stuff.Things.Actors;
using Stuff.Things.Containers;
using Interfaces;
using Meta.Exceptions;

namespace Meta.ParsingAndPrinting
{
	/// <summary>
	/// This class is responsible for managing and running the game world.
	/// </summary>
	public class GameManager
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// The <see cref="Room"/> that the player spawns in.
		/// </summary>
		private static Room firstRoom;

		/// <summary>
		/// This represents the player character.
		/// </summary>
		private static Person player;

		/// <summary>
		/// This keeps track of how many turns have passed.
		/// </summary>
		private static int turnCount = 0;

		/// <summary>
		/// Dictionary of verbs with no parameters to recognize in the player
		/// command, mapped to the appropriate function to execute.
		/// </summary>
		private static Dictionary<string, Delegate> zeroArgumentCommandMatcher = new Dictionary<string, Delegate>();

		/// <summary>
		/// Dictionary of verbs with one or more parameters. Matches a given
		/// verb to another dictionary, which matches helper words to the
		/// correct function(s). This allows "attack Bob" and "attack Bob
		/// with sword" to be two different commands, instead of trying to
		/// parse "Bob with sword" as one parameter.
		/// </summary>
		private static Dictionary<string, Dictionary<string, Delegate>> someArgumentCommandMatcher = new Dictionary<string, Dictionary<string, Delegate>>();

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Determines the outermost container that can be seen from within the given container.
		/// </summary>
		/// <param name="container">the containter inside of which to check</param>
		/// <returns>the outermost container that can be seen from within the given container</returns>
		public static Container GetTopMostVisibleContainer(Container container)
		{
			if (typeof(OpenableContainer).IsAssignableFrom(container.GetType()))
			{
				if (((OpenableContainer)container).IsOpaque() && !((OpenableContainer)container).IsOpen())
				{
					return container;
				}
			}
			if (typeof(Room).IsAssignableFrom(container.GetType()))
			{
				return container;
			}
			else if (((Thing)container).GetLocation() == null)
			{
				return null;
			}
			else
			{
				return GetTopMostVisibleContainer(((Thing)container).GetLocation());
			}
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Checks whether a given <see cref="Thing"/> is the <see cref="player"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to check</param>
		/// <returns>true if the <see cref="Thing"/> is the <see cref="player"/>; false otherwise</returns>
		public static bool IsPlayer(Thing item)
		{
			return item == player;
		}

		/// <summary>
		/// Determines whether or not the given <see cref="Thing"/> is visible
		/// to the <see cref="player"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to check if visible</param>
		/// <returns>true if the <see cref="Thing"/> is visible to the <see cref="player"/>; false otherwise</returns>
		public static bool IsVisible(Thing item)
		{
			return GetTopMostVisibleContainer(item.GetLocation()) == GetTopMostVisibleContainer(player.GetLocation());
		}

		/// <summary>
		/// Checks if a given <see cref="Actor"/> can see a given
		/// <see cref="Thing"/> from their current location(s).
		/// </summary>
		/// <param name="actor">the <see cref="Actor"/> trying to interact with something</param>
		/// <param name="item">the <see cref="Thing"/> being interacted with</param>
		/// <returns>true if the <see cref="Actor"/> can see the <see cref="Thing"/>; false otherwise</returns>
		public static bool CanSee(Actor actor, Thing item)
		{
			// if either the item's location or the actor's location is null, return false
			if (item.GetLocation() == null || actor.GetLocation() == null) {
				return false; }

			// if there are no opaque barriers between the item and the actor, return true
			else if (GetTopMostVisibleContainer(actor.GetLocation()) == GetTopMostVisibleContainer(item.GetLocation())) {
				return true; }

			// if the item is in an inventory,
			else if (typeof(Inventory) == item.GetLocation().GetType()) {

				// if the inventory belongs to the actor, return true
				if (((Inventory)item.GetLocation()).GetOwner() == actor) {
					return true; }

				else { // theft is not coded yet
					throw new Exception("Theft has not been coded for yet."); }

			} // end case "item is in an inventory"

			// if the item is a piece of clothing currently being worn
			// or is inside a piece of clothing currently being worn,
			else if (typeof(Clothes) == GetTopMostVisibleContainer(item.GetLocation()).GetType()) {

				// if the person wearing the clothing is the actor, return true
				if (((Clothes)GetTopMostVisibleContainer(item.GetLocation())).GetOwner() == actor) {
					return true; }

				// if the item is worn by someone else and it is visible, return true
				else if (((Clothes)item.GetLocation()).GetOwner().GetVisibleRecursiveContents().Contains(item)){
					return true; }

				// if it's not visible, return false
				else {
					return false; }

			} // end case "item is being worn"

			// otherwise, return false
			return false;
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// Moves a <see cref="Thing"/> into a given <see cref="Container"/>,
		/// regardless of where the <see cref="Thing"/> is before the move.
		/// Can be used to bring a <see cref="Thing"/> into play.
		/// Therefore, the <see cref="Thing"/>'s location may be null.
		/// This function checks for that possibility and handles it.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to move</param>
		/// <param name="place">the <see cref="Container"/> to put it in</param>
		public static void Move(Thing item, Container place)
		{
			if (item.GetLocation() != null) {
				item.GetLocation().RemoveThing(item); }
			item.SetLocation(place);
			place.AddThing(item);
		}

		/// <summary>
		/// Prints the <see cref="Room"/> description that appears in
		/// response to "look" or on entering a new <see cref="Room"/>.
		/// </summary>
		public static void Look()
		{
			Console.WriteLine(new String('-', 79));
			Console.WriteLine(player.GetLocation().GetName());
			Console.WriteLine(new String('-', 79));
			writeFormatted(player.GetLocation().GetDescription());
		}

		/// <summary>
		/// Prints a description of the specified <see cref="Thing"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to examine</param>
		public static void Examine(Thing item)
		{
			writeFormatted(item.GetDescription());
		}

		/// <summary>
		/// Prints out the <see cref="player"/>'s <see cref="Inventory"/>.
		/// </summary>
		public static void Inventory()
		{
			Console.WriteLine(new String('=', 79));
			Console.WriteLine((new String(' ', 32)) + "Your Inventory");
			Console.WriteLine(new String('=', 79));
			if (player.GetInventory().Count == 0) {
				Console.WriteLine("...is empty."); }
			else {
				foreach (Thing item in player.GetInventory())
					Console.WriteLine(item.GetQualifiedName()); }
		}

		/// <summary>
		/// Removes the <see cref="player"/> from a given set.
		/// This is used to prevent the <see cref="player"/> from showing up
		/// in room descriptions and the like.
		/// </summary>
		/// <param name="set"></param>
		public static void RemovePlayer(HashSet<Thing> set)
		{
			set.Remove(player);
		}

		/// <summary>
		/// This function checks to see if the <see cref="player"/> can see the
		/// <see cref="Thing"/> trying to print a message. If the
		/// <see cref="player"/> can see the <see cref="Thing"/>, the message
		/// is printed. If not, nothing happens.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> that wants to print a message</param>
		/// <param name="message">the message to be printed</param>
		public static void ReportIfVisible(Thing item, string message) 
		{
			if (IsVisible(item)) {
				writeFormatted(message); }
		}

		/// <summary>
		/// Prints a generic message reporting a simple action.
		/// </summary>
		/// <param name="actor">the <see cref="Actor"/> performing the action</param>
		/// <param name="verb">the action being performed</param>
		/// <param name="item">the <see cref="Thing"/> the action is performed on</param>
		public static void ReportIfVisible(Actor actor, VerbSet verb, Thing item)
		{
			ReportIfVisible(actor, StringManipulator.CapitalizeFirstLetter(actor.GetSpecificName()) + ' ' + actor.GetConjugatedVerb(verb) + ' ' + item.GetSpecificName() + '.');
		}

		/// <summary>
		/// Prints a generic message reporting a simple action with two parameters.
		/// </summary>
		/// <param name="actor">the <see cref="Actor"/> performing the action</param>
		/// <param name="verb">the action being performed</param>
		/// <param name="item1">the first <see cref="Thing"/> the action is performed on</param>
		/// <param name="separator">word or phrase separating the two parameters</param>
		/// <param name="item2">the second <see cref="Thing"/> the action is performed on</param>
		public static void ReportIfVisible(Actor actor, VerbSet verb, Thing item1, string separator, Thing item2)
		{
			ReportIfVisible(actor,
				StringManipulator.CapitalizeFirstLetter(actor.GetSpecificName())
				+ ' ' + actor.GetConjugatedVerb(verb)
				+ ' ' + item1.GetSpecificName()
				+ separator + item2.GetSpecificName() + '.');
		}

		//===================================================================//
		//                            Helpers                                //
		//===================================================================//

		/// <summary>
		/// Helper method for <see cref="matchCommand"/> that tries to find the
		/// value for a parameter that the player entered.
		/// Throws exceptions for types that I haven't coded yet.
		/// </summary>
		/// <param name="paramName">the name of the parameter</param>
		/// <param name="parameters">the ParameterInfo that tells what kind of parameter it is</param>
		/// <returns>an object[] containing the parameter if successful; null otherwise</returns>
		private static object[] findParam(string paramName, System.Reflection.ParameterInfo[] parameters)
		{
			// CASE 0: The parameter is a "Thing"
			if (typeof(Thing).IsAssignableFrom(parameters[0].ParameterType)) {
				// Try to find a thing with the parameter name.
				Thing item = findThingNamed(paramName);

				// If unsuccessful, return null.
				if (item != null) {
					object[] foundParam = new object[1];
					foundParam[0] = item;
					return foundParam; }
				else {
					return null; }
			} // END CASE 0: Thing

			// CASE 1: The parameter is a "Direction"
			else if (typeof(Direction) == parameters[0].ParameterType) {
				try {
					Direction dir = (Direction)Enum.Parse(typeof(Direction), paramName.ToLower());
					object[] foundParam = new object[1];
					foundParam[0] = dir;
					return foundParam; }
				catch (ArgumentException) {
					return null; }
			} // END CASE 1: Direction

			// CASE 2: The parameter is something I haven't coded for yet, lol.
			else {
				throw new Exception("I haven't coded this bit yet :P");
			} // END CASE 2: Not Coded Yet
		}

		/// <summary>
		/// Helper method <see cref="matchCommand"/> that tries to find the
		/// values for parameters that the player entered.
		/// Throws exceptions for types that I haven't coded yet.
		/// </summary>
		/// <param name="param1Name">the name of the first parameter</param>
		/// <param name="param2Name">the name of the second parameter</param>
		/// <param name="parameters">the ParameterInfo that tells what kind of parameter it is</param>
		/// <returns>an object[] containing the parameters if successful; null otherwise</returns>
		private static object[] findParams(string param1Name, string param2name, System.Reflection.ParameterInfo[] parameters)
		{
			try
			{
				object[] param1 = findParam(param1Name, parameters);
				object[] param2 = findParam(param2name, parameters);

				if (param1 == null || param2 == null) {
					return null; }

				else
				{
					object[] foundParams = new object[2];
					foundParams[0] = param1[0];
					foundParams[1] = param2[0];

					return foundParams;
				}
			}
			catch
			{
				throw new Exception("I haven't coded this bit yet :P");
			}
		}

		/// <summary>
		/// Searches the player's current location, their inventory, and their
		/// clothes for a thing with the specified name.
		/// </summary>
		/// <param name="name">the name of the thing</param>
		/// <returns>a thing with the specified name, if it is found; null if none is found</returns>
		private static Thing findThingNamed(string name)
		{
			HashSet<Thing> itemsFound = new HashSet<Thing>();
			foreach (Thing item in GetTopMostVisibleContainer(player.GetLocation()).GetVisibleRecursiveContents()) {
				if (item.IsNamed(name)) {
					itemsFound.Add(item); }
			}
			foreach (Thing item in player.GetRecursiveContents()) {
				if (item.IsNamed(name)) {
					itemsFound.Add(item); }
			}

			if (itemsFound.Count == 0) {
				return null; }
			else if (itemsFound.Count == 1) {
				return itemsFound.ElementAt(0); }
			else {
				Console.WriteLine("The specified name is ambiguous. Try being more specific?");
				Console.Write(">>> ");
				return findThingNamed(Console.ReadLine()); }
		}

		/// <summary>
		/// Attempts to parse a command entered by the player.
		/// </summary>
		/// <param name="command">the command that the player entered</param>
		/// <returns>true if the command was matched successfully; false otherwise</returns>
		private static bool matchCommand(string command)
		{
			try {
				// CASE 0: The entire command matches a single verb in the dictionary.
				//         Correct Examples: "look", "look around", "help"
				//         Incorrect Cases: this will match if the player did not include the needed parameters
				if (zeroArgumentCommandMatcher.ContainsKey(command.ToLower()) ||
					someArgumentCommandMatcher.ContainsKey(command.ToLower())) {

					// CASE 0b: The verb actually requires parameters, but the user left them out.
					if (someArgumentCommandMatcher.ContainsKey(command.ToLower()) &&
						!zeroArgumentCommandMatcher.ContainsKey(command.ToLower())) {
						// complain about the first missing parameter
						writeFormatted("I understood that you want to \""
							+ command.ToLower() + ',' +'"' + ' ' +
							"but I was expecting some more words at the end.");
						return false; }

					// Case 0a: This is a zero-parameter function. Run it normally.
					else {
						// store both the function and its parameters in local variables
						Delegate function = zeroArgumentCommandMatcher[command.ToLower()];
						System.Reflection.ParameterInfo[] parameters = function.Method.GetParameters();

						// checking really not necessay because none of the
						// no-parameter functions throw exceptions
						function.Method.Invoke(player, null); 
						return true; }

				} // END CASE 0

				// CASE 1: The entire command does not match a verb in the dictionary.
				//         Correct Cases: the function requires one or more parameters,
				//                        potententially with helper words
				//         Incorrect Cases: the command is garbage, misspelled,
				//                          or ends with extra words
				else {
					// Break up the command into words.
					string[] words = command.Split(' ');

					// CASE 1b: There was only one word, and it didn't match any known verbs.
					if (words.Length < 2) {
						// complain about unrecognized verb
						writeFormatted("I don't recognize the verb " + command + '.');
						return false; }

					// CASE 1a: There are multiple words.
					else {
						// Keep checking the first "numWordsInVerb" words to see if they make a valid verb.
						for (int numWordsInVerb = words.Length - 1; numWordsInVerb > 0; numWordsInVerb--) {
							// Create an array of the first "numWordsInVerb" words separated by spaces.
							string[] stringsToConcat = new string[(numWordsInVerb * 2) - 1];
							for (int index = 0; index < stringsToConcat.Length; index++) {
								// Put spaces between the words.
								if (index % 2 == 1) {
									stringsToConcat[index] = " "; }
								else {
									stringsToConcat[index] = words[index / 2]; }
							} // END of creating the array

							// Turn the array into a string.
							string verb = String.Concat(stringsToConcat);

							// Check if the string is a valid verb.
							if (someArgumentCommandMatcher.ContainsKey(verb.ToLower())) {
								// Try each potential implementation of the verb until you find one that works.
								for (int implementIndex = 0; implementIndex < someArgumentCommandMatcher[verb.ToLower()].Count; implementIndex++) {
									// Store the implementation, function, and parameters locally.
									KeyValuePair<string, Delegate> implementation = someArgumentCommandMatcher[verb.ToLower()].ElementAt(implementIndex);
									System.Reflection.MethodInfo function = implementation.Value.Method;
									System.Reflection.ParameterInfo[] parameters = function.GetParameters();

									// Check for the only-one-param version, denoted by an empty string.
									if (implementation.Key == "") {
										// Create an array of the string in the parameter, separated by spaces.
										string[] stringsInParam = new string[((words.Length - numWordsInVerb) * 2) - 1];
										for (int stringIndex = 0; stringIndex < stringsInParam.Length; stringIndex++) {
											// Put spaces between the words.
											if (stringIndex % 2 == 1) {
												stringsInParam[stringIndex] = " "; }
											else {
												stringsInParam[stringIndex] = words[(stringIndex / 2) + numWordsInVerb]; }
										} // END of creating the array
										string paramName = String.Concat(stringsInParam);

										object[] foundParam = findParam(paramName, parameters);
										if (foundParam != null) {
											try {
												function.Invoke(player, foundParam);
												return true; }
											catch (Exception ge) {
												if(typeof(GameException).IsAssignableFrom(ge.InnerException.GetType())) {
													Console.WriteLine(ge.InnerException.Message);
													return false; }
												else {
													throw ge; }
												} // end catch
											} // No parameters found.
									} // END one-param-only case

									// There are two parameters separated by a word.
									else {
										// Make sure there are actually enough words for the parameters.
										if (words.Length - numWordsInVerb >= 3) {
											for (int indexOfSeparator = numWordsInVerb + 1; indexOfSeparator < words.Length - 1; indexOfSeparator++) {
												if (words[indexOfSeparator] == implementation.Key) {
													// define the first param to be all the words between the verb and the separator
													string firstParam = words[numWordsInVerb];
													for (int indicesOfFirstParam = numWordsInVerb + 1; indicesOfFirstParam < indexOfSeparator; indicesOfFirstParam++) {
														firstParam += ' ' + words[indicesOfFirstParam]; }
													// define the second param to be all the words after the separator
													string secondParam = words[indexOfSeparator + 1];
													for (int indicesOfSecondParam = indexOfSeparator + 2; indicesOfSecondParam < words.Length; indicesOfSecondParam++) {
														secondParam += ' ' + words[indicesOfSecondParam]; }
													// try to find the parameters
													object[] foundParams = findParams(firstParam, secondParam, parameters);
													// if it doesn't fail, invoke the function
													if (foundParams != null) {
														try {
															function.Invoke(player, foundParams);
															return true; }
														catch (Exception ge) {
															if(typeof(GameException).IsAssignableFrom(ge.InnerException.GetType())) {
																writeFormatted(ge.InnerException.Message);
																return false; }
															else {
																throw ge; }
														} // end of catch
													} // Parameters were not found.
												} // This word is not the separator. Keep looking.
											} // Separator word not found; must be wrong implementation.
										} // If not, assume this is the wrong implementation.
									} // This implementation was the wrong one.
								} // None of the implementations worked!
								writeFormatted("I understood that you wanted to \"" + verb + ",\" but the words that came after confused me.");
								return false;
							} // This combination of words didn't match a verb.
						} // No verb was found in the first "MaxWordsInVerb" words.
						Console.WriteLine("That command made no sense to me.");
						return false;
					} // After this, code is unreachable.
				} // Still unreachable.
			} // End of try block
			catch (GameException ge) {
				Console.WriteLine(ge.Message);
				return false; }
		}

		/// <summary>
		/// Generates a string complaining about a missing parameter in a player command.
		/// </summary>
		/// <param name="parameter">the parameter that is missing</param>
		/// <param name="command">the command the player entered</param>
		/// <returns>a complaint</returns>
		private static string reportMissingParam(System.Reflection.ParameterInfo parameter, string command)
		{
			string report;
			if (typeof(Actor).IsAssignableFrom(parameter.ParameterType))
			{
				report = "Who or what are you trying to ";
			}
			else if (typeof(Direction) == parameter.ParameterType)
			{
				report = "What direction are you trying to ";
			}
			else if (typeof(Person).IsAssignableFrom(parameter.ParameterType))
			{
				report = "Who are you trying to ";
			}
			else
			{
				report = "What are you trying to ";
			}
			report += command + '?';
			return report;
		}

		/// <summary>
		/// Helper method that initializes the command matcher dictionaries.
		/// </summary>
		private static void initCommandMatcher()
		{
			//===============================================================//
			// ZERO ARGUMENT COMMANDS
			//===============================================================//
			// Look
			zeroArgumentCommandMatcher.Add("look", new NoArgumentsDelegate(Look));
			zeroArgumentCommandMatcher.Add("look around", new NoArgumentsDelegate(Look));
			// Inventory
			zeroArgumentCommandMatcher.Add("inventory", new NoArgumentsDelegate(Inventory));
			zeroArgumentCommandMatcher.Add("take inventory", new NoArgumentsDelegate(Inventory));
			zeroArgumentCommandMatcher.Add("i", new NoArgumentsDelegate(Inventory));

			//===============================================================//
			// COMMANDS WITH ONE ARGUMENT
			//===============================================================//
			// Move
			Dictionary<string, Delegate> moveDict = new Dictionary<string, Delegate>();
			moveDict.Add("", new OneDirectionDelegate(player.Move));
			someArgumentCommandMatcher.Add("go",   moveDict);
			someArgumentCommandMatcher.Add("move", moveDict);
			// Examine
			Dictionary<string, Delegate> examineDict = new Dictionary<string, Delegate>();
			examineDict.Add("", new OneThingDelegate(Examine));
			someArgumentCommandMatcher.Add("examine", examineDict);
			someArgumentCommandMatcher.Add("look at", examineDict);
			someArgumentCommandMatcher.Add("inspect", examineDict);
			someArgumentCommandMatcher.Add("x",       examineDict);
			// Take
			Dictionary<string, Delegate> takeDict = new Dictionary<string, Delegate>();
			takeDict.Add("", new OneThingDelegate(player.Take));
			someArgumentCommandMatcher.Add("take",    takeDict);
			someArgumentCommandMatcher.Add("get",     takeDict);
			someArgumentCommandMatcher.Add("procure", takeDict);
			someArgumentCommandMatcher.Add("obtain",  takeDict);
			// Drop
			Dictionary<string, Delegate> dropDict = new Dictionary<string, Delegate>();
			dropDict.Add("", new OneThingDelegate(player.Drop));
			someArgumentCommandMatcher.Add("drop",          dropDict);
			someArgumentCommandMatcher.Add("let go of",     dropDict);
			someArgumentCommandMatcher.Add("stop carrying", dropDict);
			// Open
			Dictionary<string, Delegate> openDict = new Dictionary<string, Delegate>();
			openDict.Add("", new OneThingDelegate(player.Open));
			someArgumentCommandMatcher.Add("open", openDict);
			// Close
			Dictionary<string, Delegate> closeDict = new Dictionary<string, Delegate>();
			closeDict.Add("", new OneThingDelegate(player.Close));
			someArgumentCommandMatcher.Add("close", closeDict);
			// Wear
			Dictionary<string, Delegate> wearDict = new Dictionary<string, Delegate>();
			wearDict.Add("", new OneThingDelegate(player.Wear));
			someArgumentCommandMatcher.Add("wear", wearDict);
			someArgumentCommandMatcher.Add("put on", wearDict);
			// Take Off
			Dictionary<string, Delegate> takeOffDict = new Dictionary<string, Delegate>();
			takeOffDict.Add("", new OneThingDelegate(player.TakeOff));
			someArgumentCommandMatcher.Add("take off", takeOffDict);
			someArgumentCommandMatcher.Add("remove", takeOffDict);

			//===============================================================//
			// COMMANDS WITH TWO ARGUMENTs
			//===============================================================//
			// Put Into
			Dictionary<string, Delegate> putIntoDict = new Dictionary<string, Delegate>();
			putIntoDict.Add("into", new TwoThingsDelegate(player.PutInto));
			someArgumentCommandMatcher.Add("put", putIntoDict);
		}

		/// <summary>
		/// Helper method that initializes the <see cref="player"/> character
		/// and the <see cref="Room"/>s of the game.
		/// </summary>
		private static void initPlayerAndRooms()
		{
			player = new Person("You", "How did you get this to display? It's supposed to be masked.", PronounSet.GetSecondPersonSet());
			firstRoom = new Room("Kitchen", "The kitchen is a room where people make food.");
			Room anotherRoom = new Room("Bedroom", "This is a place where people typically sleep. Its name comes from the piece of furniture, a bed, that usually occupies the room. However, while providing a place to rest is certainly one of a bedroom's important functions, it is also expected to provide several other services. As you may have already guessed, I needed a long description to test my description-displaying.");
			Room aThirdRoom = new Room("Hallway", "Just a heads up: this is the room I used for testing non-euclidean room connections. It worked.");

			Room.SetAdjacent(firstRoom, Direction.south, anotherRoom, Direction.north);
			Room.SetAdjacent(aThirdRoom, Direction.east, anotherRoom, Direction.west);
			aThirdRoom.SetAdjacent(firstRoom, Direction.southwest);

			Move(new Thing("table", "a kitchen table"), firstRoom);

			Move(new Thing("bed", "a thing that you sleep on"), anotherRoom);
			Move(new Person("Steve", "The first NPC I made for testing.", PronounSet.GetMaleSet()), anotherRoom);

			BasicContainer basket = new BasicContainer("basket", "A woven basket.");
			Move(basket, aThirdRoom);
			Move(new Thing("flower", "It's a dandylion."), basket);
			BasicOpenableContainer box = new BasicOpenableContainer("box", "I'm just going through classes I've made, instantiating them.", true);
			Move(box, aThirdRoom);
			box.Open();
			Move(new Thing("plates", "Some nice dinner plates. This one was for testing plural nouns.", isPlural: true), box);
			box.Close();
			Move(new Clothing("dress", "A cotton dress.", slots:new ClothingSlot[] { ClothingSlot.shirt, ClothingSlot.skirt}), aThirdRoom);

			Move(player, firstRoom);
		}

		/// <summary>
		/// Helper method that formats a string before printing it.
		/// </summary>
		/// <param name="str">the string to print</param>
		private static void writeFormatted(string str)
		{
			Console.WriteLine(StringManipulator.Enforce80CharLineLimit(str));
		}

		//===================================================================//
		//                              Main                                 //
		//===================================================================//

		/// <summary>
		/// This function runs the game engine. It loops around reading
		/// commands until it sees a command to quit.
		/// </summary>
		/// <param name="args">this is where commandline arguments go; I don't use any yet</param>
		static void Main(string[] args)
		{
			initPlayerAndRooms();
			initCommandMatcher();

			Look();

			string command = "nonsense";
			while (!command.Equals("quit"))
			{
				Console.Write(">>> ");
				command = Console.ReadLine();
				if (matchCommand(command))
				{
					turnCount++;
				}
			}
		}
	}
}