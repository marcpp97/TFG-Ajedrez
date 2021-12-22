
public class Posicion
{

    public int x;
    public int y;

    public Posicion(int x, int y)
    {

        this.x = x;
        this.y = y;

    }

    public bool isEqual(Posicion p)
    {

        if (p.x == x && p.y == y)
            return true;
        return false;

    }

    public string GetToString()
    {
        return getLetterFromInt(x) + "" + (y + 1);
    }

    string getLetterFromInt(int x)
    {
        switch (x)
        {
            case 0:
                return "a";
            case 1:
                return "b";
            case 2:
                return "c";
            case 3:
                return "d";
            case 4:
                return "e";
            case 5:
                return "f";
            case 6:
                return "g";
            case 7:
                return "h";
            default:
                return "";
        }
    }
}

