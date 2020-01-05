using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LibraryTranslator : MonoBehaviour
{
	private Dictionary<char,string> subjectDictionary;
	private Dictionary<char,string> verbDictionary;
	private Dictionary<char,string> adjectDictionary;
	private Dictionary<char,string> connectorsDictionary;

	private Queue<string> connectors;

	private string state; //"SUBJECT","VERB","OBJECT"
	private bool objectSet = false;
	private bool plural = false;


	void Start ()
	{
		setupDictionaries ();
	}

	void Update ()
	{
	
	}

	public string translateText(string s){
		
		string[] array = s.Split (' ');
		string translation = "";
		foreach(string t in array){
			if (t != "") {
				translation += translate (t);
			}
		}
		return translation;
	}

	//translates a string divided by " " (spaces)
	private string translate(string s){
		connectors = new Queue<string> ();
		//put chars into queue to be processed
		Queue<char> input = new Queue<char> ();
		int periodCount = 0;
		char[] array = s.ToCharArray();
		foreach (char c in array) {
			//exclude periods and count them
			if (c == '.') {
				periodCount++;	
			} else {
				input.Enqueue (c);
			}
		}
		input.Enqueue (' ');	//enqueue space at the end

		bool negated = periodCount % 2 != 0;
		//translate each character starting with the subject
		state = "SUBJECT";
		Queue<string> output = new Queue<string> ();
		while(input.Count > 0){
			translateState (input,output,negated);
		}
		//output holds the strings for the translation
		//concatenate translation with spaces between the words
		string translation = "";
		while (output.Count > 0) {
			translation += output.Dequeue ();
			if (output.Count > 0) {
				string next = output.Peek ();
				if (next != ".") {
					translation += " ";
				} else {
					translation += output.Dequeue ();
					translation += " ";
				}
			}
		}

		return translation;
	}

	//forward the translation of the caracter c to the respective method chosen by the current state
	private void translateState(Queue<char> input,Queue<string> output,bool negated){
		switch (state) {
		case "SUBJECT":
			translateSubject (input, output);
			break;
		case "VERB":
			translateVerb (input, output,negated);
			break;
		case "OBJECT":
			translateObject (input, output);
			break;
		default:
			throw new KeyNotFoundException ();
		}
	}

	private void translateSubject(Queue<char> input,Queue<string> output){
		char c = input.Dequeue ();
		if (c == ',') {
			output.Enqueue ("You should");
			state = "VERB";
			return;
		} else {
			//c is a letter between a-z
			if(!subjectDictionary.ContainsKey(c)){
				string key = "" + c;
				if (c == ' ')
					key = "space";
				print("key error with key: " + key);
			}
			output.Enqueue(subjectDictionary[c]);
			//see if next letter is a comma
			c = input.Peek();
			if (c == ',') {
				c = input.Dequeue ();
				if ((char)input.Peek () == ',') {
					//enqueue "and" and remove following commas
					output.Enqueue ("or");
					removeCommas (input);
				} else {
					//following character is not a comma
					output.Enqueue ("and");
				}
				state = "SUBJECT";
				plural = true;
				return;
			}
		}
		state = "VERB";
	}

	private void translateVerb(Queue<char> input,Queue<string> output,bool negated){
		char c = (char)input.Dequeue ();
		if (c == ' ') {
			output.Enqueue (".");
			state = "SUBJECT";
			plural = false;
		} else if (c == ',') {
				if ((char)input.Peek () == ',') {
					//enqueue "and" and remove following commas
					output.Enqueue ("or");
					removeCommas (input);
				} else {
					//following character is not a comma
					output.Enqueue ("and");
				}
				state = "VERB";
				return;
				//c = (char)input.Dequeue ();
		} else {
			if (c == ' ') {
				state = "SUBJECT";
				plural = false;
			} else {
				//character is a letter between a-z
				//negate if needed
				if (negated) {
					output.Enqueue ("don't");
				}

				string verb = conjugateVerb (verbDictionary [c], output);
				output.Enqueue (verb);
				addVerbConnector (verb);
				if (input.Peek () == ',') {
					state = "VERB";
				} else {
					state = "OBJECT";
				}
			}
		}
	}

	private void translateObject(Queue<char> input,Queue<string> output){
		char c = (char)input.Dequeue ();
		if (c == ' ') {
			output.Enqueue (".");
			state = "SUBJECT";
			plural = false;
			objectSet = false;
		} else {
			if (c == ',') {
				if ((char)input.Peek () == ',') {
					//enqueue "and" and remove following commas
					output.Enqueue ("or");
					removeCommas (input);
				} else {
					//following character is not a comma
					output.Enqueue("and");
				}
				c = (char)input.Dequeue ();
			}
			if (c == ' ') {
				state = "SUBJECT";
				plural = false;
				objectSet = false;
			} else {
				if (objectSet) {
					output.Enqueue (connectors.Dequeue ());
				}
				//character is a letter between a-z
				output.Enqueue (adjectDictionary [c]);
				connectors.Enqueue (connectorsDictionary [c]);
				state = "OBJECT";
				objectSet = true;
			}
		}
	
	} 

	private void removeCommas(Queue<char> q){
		while ((char)q.Peek () == ',') {
			q.Dequeue ();
		}
	}

	private string conjugateVerb(string verb,Queue<string> output){
		//get subject of the sentence
		string subject = output.Peek();
		bool be = false;
		if (verb == "be") {
			be = true;
			verb = conjugateBe (subject);
		}
		if (subject == "He" || subject == "She" || subject == "It" || subject == "Everybody" || subject == "Nature") {
			switch (verb) {
			case "have":
				verb = "has";
				break;
			case "touch":
				verb = "touches";
				break;
			case "think about":
				verb = "thinks about";
				break;
			case "dream about": 
				verb = "dreams about";
				break;
			default:
				if(!be && !plural) verb += "s";
				break;
			}
		}
		return verb;
	}

	private string conjugateBe(string subject){
		switch (subject) {
		case "I":
			return "am";
		case "He": 
		case "She": 
		case "It": 
		case "Everybody":
		case "Nature":
			return "is";
		case "You should":
			return "be";
		default: return "are";
		}
	}

	private void addVerbConnector(string verb){
		switch (verb) {
		case "ask":
			connectors.Enqueue ("about");
			break;
		case "remove":
			connectors.Enqueue ("from");
			break;
		}
	}

	private void setupDictionaries(){
		subjectDictionary = new Dictionary<char, string> ();
		subjectDictionary.Add ('a', "I");
		subjectDictionary.Add ('b', "You");
		subjectDictionary.Add ('c', "He");
		subjectDictionary.Add ('d', "She");
		subjectDictionary.Add ('e', "It");
		subjectDictionary.Add ('f', "We");
		subjectDictionary.Add ('g', "Everybody");
		subjectDictionary.Add ('h', "They");
		subjectDictionary.Add ('i', "Men");
		subjectDictionary.Add ('j', "Women");
		subjectDictionary.Add ('k', "Animals");
		subjectDictionary.Add ('l', "Plants");
		subjectDictionary.Add ('m', "Nature");
		subjectDictionary.Add ('n', "Drivers");
		subjectDictionary.Add ('o', "Writers");
		subjectDictionary.Add ('p', "Programmers");
		subjectDictionary.Add ('q', "Readers");
		subjectDictionary.Add ('r', "Leaders");
		subjectDictionary.Add ('s', "Librarians");
		subjectDictionary.Add ('t', "Painters");
		subjectDictionary.Add ('u', "I");
		subjectDictionary.Add ('v', "You");
		subjectDictionary.Add ('w', "Musicians");
		subjectDictionary.Add ('x', "Philosophers");
		subjectDictionary.Add ('y', "Scientists");
		subjectDictionary.Add ('z', "Spirits");

		verbDictionary = new Dictionary<char, string> ();
		verbDictionary.Add ('a', "have");
		verbDictionary.Add ('b', "see");
		verbDictionary.Add ('c', "touch");
		verbDictionary.Add ('d', "be");
		verbDictionary.Add ('e', "think about");
		verbDictionary.Add ('f', "dream about");
		verbDictionary.Add ('g', "hear");
		verbDictionary.Add ('h', "taste");
		verbDictionary.Add ('i', "understand");
		verbDictionary.Add ('j', "like");
		verbDictionary.Add ('k', "dislike");
		verbDictionary.Add ('l', "love");
		verbDictionary.Add ('m', "hate");
		verbDictionary.Add ('n', "play");
		verbDictionary.Add ('o', "paint");
		verbDictionary.Add ('p', "realize");
		verbDictionary.Add ('q', "read");
		verbDictionary.Add ('r', "walk");
		verbDictionary.Add ('s', "feel");
		verbDictionary.Add ('t', "fight");
		verbDictionary.Add ('u', "believe");
		verbDictionary.Add ('v', "ask");
		verbDictionary.Add ('w', "answer");
		verbDictionary.Add ('x', "remove");
		verbDictionary.Add ('y', "turn");
		verbDictionary.Add ('z', "waste");

		adjectDictionary = new Dictionary<char, string> ();
		adjectDictionary.Add ('a', "him");
		adjectDictionary.Add ('b', "her");
		adjectDictionary.Add ('c', "a human");
		adjectDictionary.Add ('d', "water");
		adjectDictionary.Add ('e', "the word");
		adjectDictionary.Add ('f', "children");
		adjectDictionary.Add ('g', "it");
		adjectDictionary.Add ('h', "colors");
		adjectDictionary.Add ('i', "food");
		adjectDictionary.Add ('j', "life");
		adjectDictionary.Add ('k', "the universe");
		adjectDictionary.Add ('l', "everything");
		adjectDictionary.Add ('m', "the answer");
		adjectDictionary.Add ('n', "love");
		adjectDictionary.Add ('o', "a book");
		adjectDictionary.Add ('p', "the library");
		adjectDictionary.Add ('q', "music");
		adjectDictionary.Add ('r', "creation");
		adjectDictionary.Add ('s', "me");
		adjectDictionary.Add ('t', "you");
		adjectDictionary.Add ('u', "the beauty");
		adjectDictionary.Add ('v', "the light");
		adjectDictionary.Add ('w', "poverty");
		adjectDictionary.Add ('x', "wisdom");
		adjectDictionary.Add ('y', "injustice");
		adjectDictionary.Add ('z', "value");

		connectorsDictionary = new Dictionary<char, string> ();
		connectorsDictionary.Add ('a', "with");
		connectorsDictionary.Add ('b', "with");
		connectorsDictionary.Add ('c', "and");
		connectorsDictionary.Add ('d', "for");
		connectorsDictionary.Add ('e', "of");
		connectorsDictionary.Add ('f', "of");
		connectorsDictionary.Add ('g', "but not");
		connectorsDictionary.Add ('h', "in");
		connectorsDictionary.Add ('i', "for");
		connectorsDictionary.Add ('j', "of");
		connectorsDictionary.Add ('k', "of");
		connectorsDictionary.Add ('l', "for");
		connectorsDictionary.Add ('m', "to");
		connectorsDictionary.Add ('n', "of");
		connectorsDictionary.Add ('o', "about");
		connectorsDictionary.Add ('p', "of");
		connectorsDictionary.Add ('q', "by");
		connectorsDictionary.Add ('r', "of");
		connectorsDictionary.Add ('s', "and");
		connectorsDictionary.Add ('t', "and");
		connectorsDictionary.Add ('u', "of");
		connectorsDictionary.Add ('v', "of");
		connectorsDictionary.Add ('w', "because of");
		connectorsDictionary.Add ('x', "and");
		connectorsDictionary.Add ('y', "of");
		connectorsDictionary.Add ('z', "of");
	}
}

