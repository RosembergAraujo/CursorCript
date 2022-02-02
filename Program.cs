using System;

namespace CursorCript
{
    internal class Program
    {
        private static readonly string[,] Matrix = new string[,]
        {
            {"a", "b", "c", "d", "e"},
            {"f", "g", "h", "i", "j"},
            {"k", "l", "m", "n", "o"},
            {"p", "q", "r", "s", "t"},
            {"u", "v", "w", "x", "y"},
            {"z", " ", "ç", "!", "@"}
        };
        private class Cursor
        {
            public int LinePos { get; private set; }
            public int ColumnPos { get; private set; }
            public static readonly string[] Symbols = new string[]
            {
                "-",  //0 letter divider
                "#",  //1 Line/Column divider
                ",",  //2 Cursor steps divider
                ".",  //3 Stay cursor movement
                "..", //4 Backward cursor movement
                "/"   //5 Forward cursor movement
            };
            public void MoveLineCursor(string symbol)
            {
                LinePos += MoveCursor(symbol);
            }
            public void MoveColumnCursor(string symbol)
            {
                ColumnPos += MoveCursor(symbol);
            }
            private static int MoveCursor(string symbol)
            {
                if (symbol == Symbols[4])
                {
                    return -1;
                }
                return symbol == Symbols[5] ? 1 : 0;
            }
        }
        public static void Main(string[] args)
        {
            string Decodify(string message)
            {
                var result = "";
                var cursor = new Cursor();
                var messageDivided = message.Split(
                    new[] { Cursor.Symbols[0] }, StringSplitOptions.None);
                foreach (var letter in messageDivided)
                {
                    var isLine = true;
                    var lineColumn = letter.Split(
                        new[] { Cursor.Symbols[1] }, StringSplitOptions.None);
                    foreach (var axis in lineColumn)
                    {
                        var moves = axis.Split(
                            new[] { Cursor.Symbols[2] }, StringSplitOptions.None);
                        foreach (var move in moves)
                        {
                            if (isLine) cursor.MoveLineCursor(move);
                            else cursor.MoveColumnCursor(move);
                        }
                        isLine = !isLine;
                    }
                    result += Matrix[cursor.LinePos,cursor.ColumnPos];
                }
                return result;
            }
            
            string Codify(string message)
            {
                var cursor = new Cursor();
                var difCursorLine = 0;
                var difCursorColumn = 0;
                var encryptedMessage = "";
                foreach (var letter in message)
                {
                    for (var i = 0; i < Matrix.GetLength(0); i++)
                    {
                        for (var j = 0; j < Matrix.GetLength(1); j++)
                        {
                            if (letter.ToString() == Matrix[i,j]) EncryptLetter(i,j);
                        }
                    }

                    void EncryptLetter(int line, int column)
                    {
                        difCursorLine = cursor.LinePos - line;
                        difCursorColumn = cursor.ColumnPos - column;

                        if (difCursorLine == 0) encryptedMessage+=Cursor.Symbols[3]; // "."
                        else
                        {
                            while (difCursorLine != 0)
                            {
                                if (difCursorLine > 0)
                                {
                                    encryptedMessage+=Cursor.Symbols[4];
                                    cursor.MoveLineCursor(Cursor.Symbols[4]);
                                }
                                else if (difCursorLine < 0)
                                {
                                    encryptedMessage+=Cursor.Symbols[5];
                                    cursor.MoveLineCursor(Cursor.Symbols[5]);
                                }
                                difCursorLine = cursor.LinePos - line;
                                encryptedMessage+=Cursor.Symbols[2];
                            }
                        }
                        if (encryptedMessage[encryptedMessage.Length - 1].ToString() == Cursor.Symbols[2])
                            encryptedMessage = encryptedMessage.Remove(encryptedMessage.Length - 1);
                        // Remove the move separator, if it's in the last position
                        encryptedMessage += Cursor.Symbols[1]; // Add line-column separator

                        if (difCursorColumn == 0) encryptedMessage+=Cursor.Symbols[3]; // Stay cursor
                        else
                        {
                            while (difCursorColumn != 0)
                            {
                                if (difCursorColumn > 0)
                                {
                                    encryptedMessage+=Cursor.Symbols[4]; // Cursor forward
                                    cursor.MoveColumnCursor(Cursor.Symbols[4]);
                                }
                                else if (difCursorColumn < 0)
                                {
                                    encryptedMessage+=Cursor.Symbols[5]; // Cursor backward
                                    cursor.MoveColumnCursor(Cursor.Symbols[5]);
                                }
                                difCursorColumn = cursor.ColumnPos - column;
                                encryptedMessage+=Cursor.Symbols[2];
                            }
                        }
                        
                        if (encryptedMessage[encryptedMessage.Length - 1].ToString() == Cursor.Symbols[2])
                            encryptedMessage = encryptedMessage.Remove(encryptedMessage.Length - 1);
                        // Remove the move separator, if it's in the last position
                    }
                    encryptedMessage += Cursor.Symbols[0];
                }
                if (encryptedMessage[encryptedMessage.Length - 1].ToString() == Cursor.Symbols[0])
                    encryptedMessage = encryptedMessage.Remove(encryptedMessage.Length - 1);
                // Remove the letter separator, if it's in the last position
                return encryptedMessage;
            }

            //Tests
            // Console.WriteLine(Decodify("/,/,/#/,/,/,/-..,..,..#.-/,/,/#..-.#/"));
            // Console.WriteLine(Codify("test"));
        }
    }
}