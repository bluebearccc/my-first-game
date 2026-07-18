using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    public enum GameState { Menu, Explore, Dialogue, Puzzle, Boss, Paused }

    public enum InteractableKind { Npc, Pedestal, PortalForward, PortalBack, Boss, Lore }

    public struct InteractableInfo
    {
        public InteractableKind Kind;
        public char Code;
        public InteractableInfo(InteractableKind kind, char code) { Kind = kind; Code = code; }
    }

    public enum PuzzleType { Classify, Order, Quiz }

    public class DialogueChoice
    {
        public string Text;
        public bool Correct;
        public string Feedback;
        public DialogueChoice(string text, bool correct, string feedback)
        {
            Text = text; Correct = correct; Feedback = feedback;
        }
    }

    public class DialogueLine
    {
        // Speaker code: NPC letter, "@" = player, "!" = narrator, "X" = boss
        public string Speaker;
        public string Text;
        public List<DialogueChoice> Choices;

        public DialogueLine(string speaker, string text, List<DialogueChoice> choices = null)
        {
            Speaker = speaker; Text = text; Choices = choices;
        }
    }

    public class DialogueDef
    {
        public List<DialogueLine> Lines = new List<DialogueLine>();
        // Semicolon-separated tokens: "stage+", "crystal", "journal:<id>", "item:<id>", "toast:<msg>", "puzzle", "boss", "win"
        public string Action;
    }

    public class PuzzleItem
    {
        public string Label;
        public string Category; // for Classify
        public PuzzleItem(string label, string category) { Label = label; Category = category; }
    }

    public class QuizQuestion
    {
        public string Statement;
        public List<string> Options;
        public int CorrectIndex;
        public List<string> Feedback;
    }

    public class PuzzleDef
    {
        public PuzzleType Type;
        public string Title;
        public string Instructions;
        public List<string> Categories;      // Classify
        public List<PuzzleItem> Items;       // Classify
        public List<string> Steps;           // Order (correct sequence)
        public List<QuizQuestion> Quiz;      // Quiz
    }

    public class BossRound
    {
        public string Statement;
        public List<string> Options;
        public int CorrectIndex;
        public List<string> Feedback;
    }

    public class NpcDef
    {
        public char Code;
        public string Name;
        public Color Hair;
        public Color Skin;
        public Color Shirt;
    }

    public class JournalEntry
    {
        public string Id;
        public string Title;
        public string Body;
    }

    public class ItemDef
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Icon; // book / bag / scroll / lens
        public Color Color;
    }

    public class MapDef
    {
        public string Id;
        public string Title;
        public string[] Rows;              // ASCII grid, uniform width (built via MapGrid)
        public Color Ground;
        public Color Grass;
        public Color Water;
        public Dictionary<char, NpcDef> Npcs = new Dictionary<char, NpcDef>();
        public char[] Flow;                // stage sequence, e.g. {'C','A','B','*'}
        public string[] Objectives;         // length = Flow.Length + 1
        public string RewardAction;         // applied after puzzle/boss success
    }

    // Helper for building rectangular ASCII map grids purely in code,
    // avoids hand-counting characters in literal strings.
    public class MapGrid
    {
        public int W, H;
        char[,] cells;

        public MapGrid(int w, int h, char fill = '#')
        {
            W = w; H = h;
            cells = new char[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    cells[x, y] = fill;
        }

        public void Set(int x, int y, char c)
        {
            if (x >= 0 && x < W && y >= 0 && y < H) cells[x, y] = c;
        }

        public char Get(int x, int y)
        {
            if (x >= 0 && x < W && y >= 0 && y < H) return cells[x, y];
            return '#';
        }

        public void Rect(int x, int y, int w, int h, char c)
        {
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    Set(x + i, y + j, c);
        }

        public string[] Rows()
        {
            var rows = new string[H];
            for (int y = 0; y < H; y++)
            {
                var sb = new System.Text.StringBuilder(W);
                for (int x = 0; x < W; x++) sb.Append(cells[x, y]);
                rows[y] = sb.ToString();
            }
            return rows;
        }
    }
}
