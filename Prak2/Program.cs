using System;
using System.Linq;

namespace Prak2 {
    class WordCount {
        static void Main(string[] args) {
            string[] text = {
                "Zu Dionys, dem Tyrannen, schlich ",
                "Damon, den Dolch im Gewande: ",
                "Ihn schlugen die Häscher in Bande, ",
                "'Was wolltest du mit dem Dolche? sprich!'",
                "Entgegnet ihm finster der Wüterich.",
                "'Die Stadt vom Tyrannen befreien!'",
                "'Das sollst du am Kreuze bereuen.'",

                "Ich bin, spricht jener, zu sterben bereit ",
                "Und bitte nicht um mein Leben: ",
                "Doch willst du Gnade mir geben,",
                "Ich flehe dich um drei Tage Zeit,",
                "Bis ich die Schwester dem Gatten gefreit; ",
                "Ich lasse den Freund dir als Bürgen, ",
                "Ihn magst du, entrinn' ich, erwürgen.'",

                "Da lächelt der König mit arger List",
                "Und spricht nach kurzem Bedenken: ",
                "'Drei Tage will ich dir schenken; ",
                "Doch wisse, wenn sie verstrichen, die Frist, ",
                "Eh' du zurück mir gegeben bist, ",
                "So muß er statt deiner erblassen,",
                "Doch dir ist die Strafe erlassen.'"
            };

            WordCount count1 = new WordCount("Alf", "Bart", "Charlie", "Dora", "Emil", "Bart", "Charlie", "Dora",
                "Emil", "Charlie", "Dora", "Emil", "Dora", "Emil", "Emil");
            WordCount count2 = new WordCount();
            char[] seperatoren = {'\'', ',', '.', '?', '!', ' ', ';', ':'};
            foreach (var zeile in text) {
                count2.SortedUpdate(zeile.Split(seperatoren, StringSplitOptions.RemoveEmptyEntries));
            }

            count1.Print();
            Console.WriteLine("---------");
            count1.Reverse();
            count1.Delete("Alf");
            count1.Delete("Emil");
            count1.Delete("Charlie");
            count1.Print();
            Console.WriteLine(count1["Dora"]);
            Console.WriteLine("---------");
            count2.Print(3);
            Console.WriteLine("---------");
            WordCount count3 = count2.Filter("ich");
            count3.Print();
        }

        class ListItem {
            public string Text;
            public int Count;
            public ListItem Next;

            public ListItem(string text, int count = 1) {
                Text = text;
                Count = count;
            }

            public override string ToString() {
                return $"{Text} -- {Count}";
            }
        }

        private ListItem _first;
        private ListItem _last;

        public WordCount() {
        }

        public WordCount(params string[] textList) {
            SortedUpdate(textList);
        }

        private void AddLast(string text, int count = 1) {
            ListItem listItem = new ListItem(text, count);

            //Wenn das letzte Element null ist, ist die Liste leer und damit auch das erste Element null
            if (_last == null) {
                _first = listItem;
            } else {
                _last.Next = listItem;
            }

            _last = listItem;
        }

        //Wird nicht benötigt
        private void AddFirst(string text, int count = 1) {
            ListItem listItem = new ListItem(text, count);

            //Wenn das erste Element null ist, ist die Liste leer und damit auch das letzte Element null
            if (_first == null) {
                _last = listItem;
            } else {
                listItem.Next = _first;
            }

            _first = listItem;
        }

        public void SortedUpdate(params string[] textList) {
            foreach (string text in textList) {
                SortedUpdate(text);
            }
        }

        public void SortedUpdate(string text) {
            ListItem current = _first;
            ListItem previous = null;

            //Iteriert über alle Elemente, bis die Liste zu Ende ist, oder das einzufügende Element vor dem geprüften Element eingefügt werden muss
            while (current != null && current.Text.CompareTo(text) < 0) {
                previous = current;
                current = current.Next;
            }
            
            if (current != null && current.Text == text) {
                current.Count++;
            } else {
                InsertBetween(previous, current, new ListItem(text));
            }
        }

        private void InsertBetween(ListItem itemBefore, ListItem itemAfter, ListItem itemToInsert) {
            if (itemBefore == null) {
                _first = itemToInsert;
            } else {
                itemBefore.Next = itemToInsert;
            }

            itemToInsert.Next = itemAfter;

            if (itemAfter == null) {
                _last = itemToInsert;
            }
        }

        public void Print(int min = 1) {
            for (ListItem current = _first; current != null; current = current.Next) {
                if (current.Count >= min) {
                    Console.WriteLine(current);
                }

                current = current.Next;
            }
        }

        public void Reverse() {
            ListItem previous = null;
            ListItem next;
            ListItem firstCopy = _first;

            for (ListItem current = _first; current != null; current = current.Next) {
                next = current.Next; //Schleifensteuerung: Zwischenspeichern des nächsten Elements des aktuellen Elements, damit dieses überschrieben werden kann
                current.Next = previous; //Setzen des nächsten Elements auf das vorherige
                previous = current; //Schleifensteuerung: Festlegen des aktuellen Elements als vorheriges des nächsten Durchlaufs
                current = next; //Schleifensteuerung: Festlegen des nächsten Elements als aktuelles des nächsten Durchlaufs
            }

            //Vertauschen von Listenanfang und -ende
            _first = _last;
            _last = firstCopy;
        }

        public void Delete(string text) {
            //Falls die Liste leer ist, muss auch nichts gelöscht werden.
            if (_first != null) {
                ListItem previous = null;

                for (ListItem current = _first; current != null; current = current.Next) {
                    if (current.Text == text) {
                        //Kein vorheriges Element, also soll das erste Element gelöscht werden. Damit muss der Zeiger auf des erste Element geändert werden.
                        if (previous == null) {
                            _first = current.Next;
                        } else {
                            previous.Next = current.Next;
                        }

                        if (current.Next == null) {
                            //Kein folgendes Element, also soll das letzte Element gelöscht werden. Damit muss der Zeiger auf des letzte Element geändert werden.
                            _last = previous;
                        }

                        break; //Da es keine doppelten Elemente gibt, kann man sich weitere Schleifendurchläufe sparen.
                    }

                    previous = current;
                    current = current.Next;
                }
            }
        }

        public WordCount Filter(string pattern) {
            WordCount wordCount = new WordCount();

            for (ListItem current = _first; current != null; current = current.Next) {
                if (current.Text.Contains(pattern)) {
                    //Elemente können ohne Sortierung eingefügt werden, da die ursprüngliche Liste bereits sortiert ist
                    wordCount.AddLast(current.Text, current.Count);
                }

                current = current.Next;
            }

            return wordCount;
        }

        public int this[string text] {
            get {
                for (ListItem current = _first; current != null; current = current.Next) {
                    if (current.Text == text) {
                        return current.Count;
                    }

                    current = current.Next;
                }

                return 0;
            }
        }
    }
}