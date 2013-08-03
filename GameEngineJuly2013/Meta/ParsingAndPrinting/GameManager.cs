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
using Stuff.Things.Clothings;
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
			// if the container is openable,
			if (typeof(OpenableContainer).IsAssignableFrom(container.GetType())) {
				// if it is opaque and closed, return it
				if (((OpenableContainer)container).IsOpaque() && !((OpenableContainer)container).IsOpen()) {
					return container; }
			} // end "if the container is openable"

			// if the container is a room, return it
			if (typeof(Room).IsAssignableFrom(container.GetType())) {
				return container; }

			// if the container's location is "null," return the container
			else if (((Thing)container).GetLocation() == null) {
				return container; }

			// otherwise, try looking at the container's location instead
			else {
				return GetTopMostVisibleContainer(((Thing)container).GetLocation()); }
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
			// if either the item's location or the actor's location is null,
			// return false
			if (item.GetLocation() == null || actor.GetLocation() == null) {
				return false; }

			// if there are no opaque barriers between the item and the actor,
			// return true
			else if (GetTopMostVisibleContainer(actor.GetLocation())
				  == GetTopMostVisibleContainer(item.GetLocation())) {
				return true; }

			// if the item is held in someone's hands,
			else if (typeof(Hands) ==
				GetTopMostVisibleContainer(item.GetLocation()).GetType()) {

				// do an explicit cast for the sake of legibility
				Hands hands = (Hands)GetTopMostVisibleContainer(
					item.GetLocation());

				// if the hands belongs to the actor, return true
				if (hands.GetOwner() == actor) {
					return true; }

				// if the actor can see the owner of the hands, return true
				else if (GetTopMostVisibleContainer(actor.GetLocation())
						== GetTopMostVisibleContainer(
							hands.GetOwner().GetLocation())){
					return true; }

				// otherwise, the hands cannot be seen,
				// so the item cannot be seen either
				return false;

			} // end case "item is in someone's hands"

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
		/// Prints a description of the specified <see cref="Thing"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to examine</param>
		public static void Examine(Thing item)
		{
			writeFormatted(item.GetDescription());
		}

		/// <summary>
		/// Prints a list of commands for the benefit of the player.
		/// </summary>
		public static void Help()
		{
			Console.WriteLine(new String('=', 79));
			Console.WriteLine(new String(' ', 40) + "Help");
			Console.WriteLine(new String('=', 79));

			Console.WriteLine(
"'help' --------------------- display a list of valid commands");
			Console.WriteLine(
"'look' --------------------- re-print the room description");
			Console.WriteLine(
"'i' ------------------------ list your clothes and held items");
			Console.WriteLine(
"'x <thing>' ---------------- examine something");
			Console.WriteLine(
"'go <direction>' ----------- move in the given direction");
			Console.WriteLine(
"'take <thing>' ------------- pick up something");
			Console.WriteLine(
"'drop <thing>' ------------- let go of something you're holding");
			Console.WriteLine(
"'put <thing> into <thing>' - place an item into a container");
			Console.WriteLine(
"'wear <thing>' ------------- put on some clothing/armor/jewelery");
			Console.WriteLine(
"'take off <thing>' --------- stop wearing some clothing/armor/jewelery");
			Console.WriteLine(
"'take off <thing>' --------- stop wearing some clothing/armor/jewelery");
		}

		/// <summary>
		/// Prints out a list of the player's held items and worn clothes.
		/// </summary>
		public static void Inventory()
		{
			Console.WriteLine(new String('=', 79));
			Console.WriteLine(new String(' ', 34) + "Your Stuff");
			Console.WriteLine(new String('=', 79));

			writeFormatted(player.DescribeAllClothes() + ' ' +
				player.DescribeCarriedItems());
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
		//                 Command and Parameter Matching                    //
		//===================================================================//

		/// <summary>
		/// Helper method for <see cref="matchCommand"/> that tries to find the
		/// value for a parameter that the player entered.
		/// Throws exceptions for types that I haven't coded yet.
		/// </summary>
		/// <param name="paramName">the name of the parameter</param>
		/// <param name="parameters">the ParameterInfo that tells what kind of parameter it is</param>
		/// <returns>an object[] containing the parameter if successful; null otherwise</returns>
		private static object[] findParam(string paramName, System.Reflection.ParameterInfo[] parameters, Delegate verb)
		{
			// CASE 0: The parameter is a "Thing"
			if (typeof(Thing).IsAssignableFrom(parameters[0].ParameterType)) {
				// Try to find a thing with the parameter name.
				Thing item = findThingNamed(paramName, verb);

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
		private static object[] findParams(string param1Name, string param2name, System.Reflection.ParameterInfo[] parameters, Delegate verb)
		{
			try
			{
				object[] param1 = findParam(param1Name, parameters, verb);
				object[] param2 = findParam(param2name, parameters, verb);

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
		private static Thing findThingNamed(string name, Delegate verb)
		{
			List<Thing> itemsFound = new List<Thing>();

			// add all the stuff in the player's general area
			foreach (Thing item in GetTopMostVisibleContainer(player.GetLocation()).GetVisibleRecursiveContents()) {
				if (item.IsNamed(name)) {
					itemsFound.Add(item); } }

			// add the stuff the player is carrying and wearing
			foreach (Thing item in player.GetRecursiveContents()) {
				if (item.IsNamed(name)) {
					itemsFound.Add(item); } }

			// if nothing was found, return null
			if (itemsFound.Count == 0) {
				return null; }

			// if exactly one thing was found, return it
			else if (itemsFound.Count == 1) {
				return itemsFound.ElementAt(0); }

			else /*item name ambiguous*/ {

				// special cases for taking
				if (verb == someArgumentCommandMatcher["take"][""]) {
					return disambiguateTaking(itemsFound, name); }

				// normal disambiguation
				else {
					return handleAmbiguousCases(itemsFound, name, verb); }

			} // end of ambiguous name stuff
		}

		/// <summary>
		/// Handles ambiguous cases for item name parsing for findThingNamed().
		/// </summary>
		/// <param name="itemsList">the list of items that matched the player command</param>
		/// <param name="name">the name the player entered in their command; needed for printing disambiguation messages</param>
		/// <param name="verb">the verb in the player command; needed because this can recurse back to findThingNamed()</param>
		/// <returns>a Thing that the player most likely meant to target</returns>
		private static Thing handleAmbiguousCases(List<Thing> itemsList, string name, Delegate verb)
		{
			// if at least some of the items are non-differentiable,
			if (!Thing.CanBeDifferentiated(itemsList)) {
				// get a list of non-generic things
				List<Thing> nonGenericThings = listNonGenericThings(itemsList);

				// if all the found items are generic,
				// return a random generic item.
				if(nonGenericThings.Count == 0) {
					Random random = new Random();
					int index = random.Next(0, itemsList.Count);
					return itemsList.ElementAt(index); }

				// if the non-generic things cannot be differentiated, throw an exception
				else if(!Thing.CanBeDifferentiated(nonGenericThings)) {
					throw new Exception("Error: Name overlap could not be resolved."); }
					
				// ask if the player meant a generic thing, or one of the specific things
				else {
					List<string> stringList = new List<string>(Thing.GetUniqueNames(nonGenericThings));
					string genericName = "a generic " + ((name.StartsWith("a ") || name.StartsWith("an ") || name.StartsWith("some ")) ? name.Substring(name.IndexOf(' ') + 1) : name).ToLower().Trim();
					stringList.Add(genericName);
					writeFormatted("Did you mean " + StringManipulator.MakeOrList(stringList.ToArray()) + '?');
					Console.WriteLine("(disambiguation)>>> ");
					string newName = Console.ReadLine();
					if (newName.ToLower().Trim() == genericName || newName.Trim().ToLower() == genericName.Substring(2)) {
						Random random = new Random();
						int index = random.Next(0,itemsList.ToArray().Length);
						return itemsList.ElementAt(index); }
					else { return findThingNamed(newName, verb); } }
			} // end "things can't all be differentiated"

			// if the items can all be differentiated,
			// prompt the user to try again.
			else /*things can be differentiated*/  {
				Console.WriteLine("Did you mean " + StringManipulator.MakeOrList(Thing.GetUniqueNames(itemsList)) + '?');
				Console.Write("(disambiguation)>>> ");
				return findThingNamed(Console.ReadLine().Trim().ToLower(), verb); }
		}

		/// <summary>
		/// Parameter disambiguation for taking something.
		/// </summary>
		/// <param name="itemsList">the list of things that matched the player command</param>
		/// <param name="name">the name the player entered in their command; needed for printing disambiguation messages</param>
		/// <param name="verb">the verb in the player command; needed because this can recurse back to findThingNamed()</param>
		/// <returns>the most likely thing the player probably meant to target</returns>
		private static Thing disambiguateTaking(List<Thing> itemsList, string name)
		{
			// give perference to things not already held
			List<Thing> notHeldList = listNonCarriedThings(itemsList);
			if (notHeldList.Count == 1) { return notHeldList[0]; }
			if (notHeldList.Count > 1) {
				// give preference to things not worn
				List<Thing> notWornList = listNonWornThings(notHeldList);
				if (notWornList.Count == 1) { return notWornList[0]; }
				if (notWornList.Count > 1) {
					// give preference to things not inside a worn thing
					List<Thing> notInsideWornList = listNonInsideWornThings(notWornList);
					if (notInsideWornList.Count == 1) { return notInsideWornList[0]; }
					if (notInsideWornList.Count > 1) {
						// give preference to things not inside carried things
						List<Thing> notInsideCarriedList = listNonInsideCarriedThings(notInsideWornList);
						if (notInsideCarriedList.Count == 1) { return notInsideCarriedList[0]; }
						if (notInsideCarriedList.Count > 1) {
							// give preference to things that can be taken
							List<Thing> canBeTakenList = listTakeableThings(notInsideCarriedList);
							if (canBeTakenList.Count == 1) { return canBeTakenList[0]; }
							if (canBeTakenList.Count > 1) {
								// give preference to things not worn by other people
								List<Thing> notWornByOthersList = listNonWornByOthersThings(canBeTakenList);
								if (notWornByOthersList.Count == 1) { return canBeTakenList[0]; }
								if (notWornByOthersList.Count > 1) {
									// give preference to things not inside things carried by other people
									List<Thing> notInsideCarriedByOthersList = listNonInsideCarriedByOthersThings(notWornByOthersList);
									if (notInsideCarriedByOthersList.Count == 1) { return notInsideCarriedByOthersList[0]; }
									if (notInsideCarriedByOthersList.Count > 1) {
										// give preference to things not inside something worn by someone else
										List<Thing> notInsideWornByOthersList = listNonInsideWornByOthersThings(notInsideCarriedByOthersList);
										if (notInsideWornByOthersList.Count == 1) { return notInsideWornByOthersList[0]; }
										if (notInsideWornByOthersList.Count > 1) {
											List<Thing> notCarriedByOtherList = listNonCarriedByOthersThings(notInsideWornByOthersList);
											if (notCarriedByOtherList.Count == 1) { return notCarriedByOtherList[0]; }
											if (notCarriedByOtherList.Count > 1) {
												return handleAmbiguousCases(notCarriedByOtherList, name, someArgumentCommandMatcher["take"][""]);
											} // else, everything not inside something worn by someone else is carried by someone else
											else {
												return handleAmbiguousCases(notInsideWornByOthersList, name, someArgumentCommandMatcher["take"][""]); }
										} // else, everything not inside something carried by someone else is inside something worn by someone else
										else {
											return handleAmbiguousCases(notInsideCarriedByOthersList, name, someArgumentCommandMatcher["take"][""]); }
									} // else, everything not worn by someone else is inside something carried by someone else
									else {
										return handleAmbiguousCases(notWornByOthersList, name, someArgumentCommandMatcher["take"][""]); }
								} // else, everything that can be taken is worn by someone else
								else {
									return handleAmbiguousCases(canBeTakenList, name, someArgumentCommandMatcher["take"][""]); }
							} // else, everything not inside something carried cannot be taken
							else {
								return handleAmbiguousCases(notInsideCarriedList, name, someArgumentCommandMatcher["take"][""]); }
						} // else, everything not inside something worn is inside something carried
						else {
							return handleAmbiguousCases(notInsideWornList, name, someArgumentCommandMatcher["take"][""]); }
					} // else, everything not worn is inside something worn
					else {
						return handleAmbiguousCases(notWornList, name, someArgumentCommandMatcher["take"][""]); }
				} // else, everything not held is worn
				else {
					return handleAmbiguousCases(notHeldList, name, someArgumentCommandMatcher["take"][""]); }
			} // else, everything is held
			else {
				return handleAmbiguousCases(itemsList, name, someArgumentCommandMatcher["take"][""]); }
		}

		/// <summary>
		/// Attempts to parse a command entered by the player.
		/// </summary>
		/// <param name="command">the command that the player entered</param>
		/// <returns>true if the command was matched successfully; false otherwise</returns>
		private static bool matchCommand(string command)
		{
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
						+ command.ToLower() + ",\" " +
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

									object[] foundParam = findParam(paramName, parameters, implementation.Value);
									if (foundParam != null) {
										try {
											function.Invoke(player, foundParam);
											return true; }
										catch (Exception ge) {
											if(typeof(GameException).IsAssignableFrom(ge.InnerException.GetType())) {
												writeFormatted(ge.InnerException.Message);
												return false; }
											else {
												throw ge.InnerException; }
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
												object[] foundParams = findParams(firstParam, secondParam, parameters, implementation.Value);
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
					writeFormatted("That command made no sense to me.");
					return false;
				} // After this, code is unreachable.
			} // Still unreachable.
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

		//===================================================================//
		//                     Narrow Lists of Things                        //
		//===================================================================//

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not carried by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listCarriedThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (player.Carries(item)) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are carried by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonCarriedThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!player.Carries(item)) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not worn by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listWornThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (player.Wears(item)) {
					newList.Add(item); } }

			return newList;
		}

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are worn by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonWornThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!player.Wears(item)) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not inside something carried by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listInsideCarriedThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (GetTopMostVisibleContainer(item.GetLocation()).GetType() == typeof(Hands)) {
					if (!player.Carries(item)) {
						if (((Hands)GetTopMostVisibleContainer(item.GetLocation())).GetOwner() == player) {
							newList.Add(item); } } } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are inside something carried by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonInsideCarriedThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (GetTopMostVisibleContainer(item.GetLocation()).GetType() != typeof(Hands)) {
					newList.Add(item); }
				else if (player.Carries(item)) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not inside something worn by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listInsideWornThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (typeof(Clothing).IsAssignableFrom(GetTopMostVisibleContainer(item.GetLocation()).GetType())) {
					if (player.Wears((Thing)GetTopMostVisibleContainer(item.GetLocation()))) {
							newList.Add(item); } } } 

			return newList;
		}

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are inside something worn by the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonInsideWornThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!typeof(Clothing).IsAssignableFrom(GetTopMostVisibleContainer(item.GetLocation()).GetType())) {
					newList.Add(item); }
				else if (!player.Wears((Thing)GetTopMostVisibleContainer(item.GetLocation()))) {
					newList.Add(item); }
			}

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not carried by the someone other than the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listCarriedByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!player.Carries(item)) {
					if (item.GetLocation().GetType() == typeof(Hands)) {
						newList.Add(item); } } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are carried by the someone other than the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonCarriedByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (player.Carries(item)) {
					newList.Add(item); }
				else if (item.GetLocation().GetType() != typeof(Hands)) {
					newList.Add(item); } } 

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not worn by the someone other than the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listWornByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!player.Wears(item)) {
					if (item.GetLocation().GetType() == typeof(Clothes)) {
						newList.Add(item); } } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are worn by the someone other than the player.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonWornByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (player.Wears(item)) {
					newList.Add(item); }
				else if (item.GetLocation().GetType() != typeof(Clothes)) {
					newList.Add(item); } } 

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not inside something carried by someone else.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listInsideCarriedByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (GetTopMostVisibleContainer(item.GetLocation()).GetType() == typeof(Hands)) {
					if (item.GetLocation().GetType() != typeof(Hands)) {
						if (((Hands)GetTopMostVisibleContainer(item.GetLocation())).GetOwner() != player) {
							newList.Add(item); } } } }

			return newList;
		}

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are inside something carried by someone else.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonInsideCarriedByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (GetTopMostVisibleContainer(item.GetLocation()).GetType() != typeof(Hands)) {
					newList.Add(item); }
				else if (item.GetLocation().GetType() == typeof(Hands)) {
					newList.Add(item); }
				else if (((Hands)GetTopMostVisibleContainer(item.GetLocation())).GetOwner() == player) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not inside something worn by someone else.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listInsideWornByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (typeof(Clothing).IsAssignableFrom(GetTopMostVisibleContainer(item.GetLocation()).GetType())) {
					if (!player.Wears((Thing)GetTopMostVisibleContainer(item.GetLocation()))) {
						if (((Thing)GetTopMostVisibleContainer(item.GetLocation())).GetLocation() != null) {
							if (typeof(Clothes).IsAssignableFrom(((Thing)GetTopMostVisibleContainer(item.GetLocation())).GetLocation().GetType())) {
								newList.Add(item); } } } } }

			return newList;
		}

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are inside something worn by someone else.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonInsideWornByOthersThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!typeof(Clothing).IsAssignableFrom(GetTopMostVisibleContainer(item.GetLocation()).GetType())) {
					newList.Add(item); }
				else if (player.Wears((Thing)GetTopMostVisibleContainer(item.GetLocation()))) {
					newList.Add(item); }
				else if (((Thing)GetTopMostVisibleContainer(item.GetLocation())).GetLocation() == null) {
					newList.Add(item); }
			}

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not owned.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listOwnedThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (item.HasOwner()) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are owned.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonOwnedThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!item.HasOwner()) {
					newList.Add(item); } }

			return newList;
		}

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not generic.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listGenericThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (item.IsGeneric()) {
					newList.Add(item); } }

			return newList;
		}

		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are generic.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonGenericThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!item.IsGeneric()) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are not clothing.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listClothingThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (typeof(Clothing).IsAssignableFrom(item.GetType())) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that are clothing.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonClothingThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!typeof(Clothing).IsAssignableFrom(item.GetType())) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that cannot be taken.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listTakeableThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (item.CanBeTaken()) {
					newList.Add(item); } }

			return newList;
		}
		
		/// <summary>
		/// Narrows a list of things down by eliminating the ones
		/// that can be taken.
		/// </summary>
		/// <param name="itemsList">the list of items to narrow down</param>
		/// <returns>a list of items that did not meet the criteria</returns>
		private static List<Thing> listNonTakeableThings(List<Thing> itemsList)
		{
			List<Thing> newList = new List<Thing>();

			foreach(Thing item in itemsList) {
				if (!item.CanBeTaken()) {
					newList.Add(item); } }

			return newList;
		}
		
		//===================================================================//
		//                            Helpers                                //
		//===================================================================//

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
			someArgumentCommandMatcher.Add("don", wearDict);
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
			player = new Person("You", "How did you get this to display? It's supposed to be masked.", PronounSet.GetSecondPersonSet(), new string[] { "me" });
			firstRoom = new Room("Kitchen", "The kitchen is a room where people make food.");
			Room anotherRoom = new Room("Bedroom", "This is a place where people typically sleep. Its name comes from the piece of furniture, a bed, that usually occupies the room. However, while providing a place to rest is certainly one of a bedroom's important functions, it is also expected to provide several other services. As you may have already guessed, I needed a long description to test my description-displaying.");
			Room aThirdRoom = new Room("Hallway", "Just a heads up: this is the room I used for testing non-euclidean room connections. It worked.");

			Room.SetAdjacent(firstRoom, Direction.south, anotherRoom, Direction.north);
			Room.SetAdjacent(aThirdRoom, Direction.east, anotherRoom, Direction.west);
			aThirdRoom.SetAdjacent(firstRoom, Direction.southwest);

			Move(new Thing("table", "a kitchen table", canBeTaken:false), firstRoom);
			Move(new Thing(), firstRoom);
			Move(new Thing(), firstRoom);

			Move(new Thing("bed", "a thing that you sleep on", canBeTaken:false), anotherRoom);
			Move(new Person("Steve", "The first NPC I made for testing.", PronounSet.GetMaleSet()), anotherRoom);

			BasicContainer basket = new BasicContainer("basket", "A woven basket.");
			Move(basket, aThirdRoom);
			Move(new Thing("flower", "It's a dandylion."), basket);
			BasicOpenableContainer box = new BasicOpenableContainer("box", "I'm just going through classes I've made, instantiating them.", true);
			Move(box, aThirdRoom);
			Move(new Thing("plates", "Some nice dinner plates. This one was for testing plural nouns.", isPlural: true), box);
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