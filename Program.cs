using System.Linq;
using System.Text;

var state = new State().Next().Skip(1).FirstOrDefault();
TreeStateNode tree = new TreeStateNode();
tree.Value = state;
tree.Expand();

Console.WriteLine(state);
Console.WriteLine(tree.WinState);

public class Game
{
    public State Current { get; set; }

    public void AIPlay()
    {
        Queue<State> queue = new Queue<State>();
        queue.Enqueue(Current);

        while (queue.Count > 0)
        {
            var crr = queue.Dequeue();

            if (crr.HasEnd)
            {
                this.Current = crr;
                return;
            }

            foreach (var next in crr.Next())
            {
                queue.Enqueue(next);
            }

        }
    }
}

public struct State
{
    public bool XPlays { get; set; } = true;
    private int playCount = 0;
    private byte[] map = new byte[9];
    private sbyte[] checksum = new sbyte[8];

    public bool HasEnd
        => checksum.Any(x => x == 3 || x == -3);
    
    public bool XWin
        => checksum.Any(x => x == 3);
    
    public bool OWin
        => checksum.Any(x => x == -3);

    public State() { }

    public IEnumerable<State> Next()
    {
        int playNeeded = 9 - playCount;
        byte play = (byte)(XPlays ? 2 : 1);
        int jAttempt = 0;
        for (int i = 0; i < playNeeded; i++)
        {
            State state = new State();

            state.XPlays = !XPlays;
            state.playCount = playCount + 1;
            map.CopyTo(state.map, 0);
            checksum.CopyTo(state.checksum, 0);

            for (;jAttempt < 9; jAttempt++)
            {
                if (state.map[jAttempt] == 0)
                {
                    state.map[jAttempt] = play;
                    int y = jAttempt % 3,
                        x = jAttempt / 3;
                    sbyte check = (sbyte)(play == 2 ? 1 : -1);
                    state.checksum[y] += check;
                    state.checksum[3 + x] += check;
                    if (x == y)
                        state.checksum[6] += check;
                    if (x + y == 2)
                        state.checksum[7] += check;
                    
                    jAttempt++;
                    break;
                }
            }

            yield return state;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 9; i += 3)
        {
            sb.Append(convert(i + 0, map));
            sb.Append('│');
            sb.Append(convert(i + 1, map));
            sb.Append('│');
            sb.Append(convert(i + 2, map));
            sb.Append('\n');
            if (i != 6)
                sb.Append("─┼─┼─\n");
        }

        return sb.ToString();

        char convert(int i, byte[] map)
        {
            var value = map[i];
            switch (value)
            {
                case 0:
                    return ' ';
                case 1:
                    return 'o';
                case 2:
                    return 'x';
                default:
                    return '?';
            }
        }
    }
}

public class TreeState
{
    
}

public class TreeStateNode
{
    public State Value { get; set; }
    public int WinState { get; set; } = 0;
    public TreeStateNode Parent { get; set; }
    public List<TreeStateNode> Children { get; set; } = new List<TreeStateNode>();

    public void Expand()
    {
        var children = Value.Next();
        foreach (var child in children)
        {
            TreeStateNode newNode = new TreeStateNode();
            newNode.Value = child;
            newNode.Parent = this;
            newNode.Expand();
            this.Children.Add(newNode);
        }
        if (Value.HasEnd)
        {
            if (Value.XWin)
            {
                WinState = 1;
            }
            else if (Value.OWin)
            {
                WinState = -1;
            }
            else
            {
                WinState = 0;
            }
        }
        else
        {
            if (Value.XPlays && Children.Any(c => c.WinState == 1))
            {
                WinState = 1;
            }
            else if (!Value.XPlays && Children.Any(c => c.WinState == -1))
            {
                WinState = -1;
            }
            else if (Value.XPlays && Children.All(c => c.WinState == -1))
            {
                WinState = -1;
            }
            else if (!Value.XPlays && Children.All(c => c.WinState == 1))
            {
                WinState = 1;
            }
            else
            {
                WinState = 0;
            }
        }
    }
}