using System.Drawing;
using System.Text;
using System.Globalization;

string mapFolder = args[0];

CultureInfo.CurrentCulture = new CultureInfo("en-US");

Dictionary<Color, Province> Provinces = new Dictionary<Color, Province>();

string[] defines = File.ReadAllLines($"{mapFolder}/definition.csv");

foreach(string define in defines)
{
    string[] def = define.Split(';');
    Color color = Color.FromArgb(
        int.Parse(def[1]),
        int.Parse(def[2]),
        int.Parse(def[3])
    );
    Provinces.Add(color, new());
}

Bitmap provincemap;
provincemap = (global::System.Drawing.Bitmap)Bitmap.FromFile($"{mapFolder}/provinces.bmp");
Bitmap heightmap;
heightmap = (global::System.Drawing.Bitmap)Bitmap.FromFile($"{mapFolder}/heightmap.bmp");

for(int x = 0; x < provincemap.Width; x++)
{
    for(int y = 0; y < provincemap.Height; y++)
    {
        Provinces[provincemap.GetPixel(x, y)].Pixels.Add(new(x,y));
    }
}
bool nullprovince = false;
int i = 1;
foreach (KeyValuePair<Color,Province> kvp in Provinces)
{
    if (kvp.Value.Pixels.Count == 0) 
    { 
        Console.WriteLine($"{i} | {kvp.Key.ToString()}: has 0 Pixels: {kvp.Key.R:X2}{kvp.Key.G:X2}{kvp.Key.B:X2}");
        nullprovince = true;
    }
    i++;
}
if (nullprovince)
    throw new Exception("One or More provinces has 0 Pixels");
i = 1;
StringBuilder file = new();
foreach(Province province in Provinces.Values)
{
    province.calculate(heightmap);
    file.Append(province.stringify(i));
    i++;
}

File.WriteAllText($"{mapFolder}/positions.txt", file.ToString());

return 0;

class Province
{
    public List<Pixel> Pixels = new List<Pixel>();
    
    public Position City;
    public Position Unit;
    public Position Text;
    public Position Port;
    public Position TradeNode;
    public Position Fight;
    public Position TradeWind;

    public void calculate(Bitmap heightmap)
    {
        float x = 0;
        float y = 0;
        for (int i = 0; i < Pixels.Count; i++)
        {
            x += Pixels[i].x;
            y += Pixels[i].y;
        }
        x /= Pixels.Count;
        y /= Pixels.Count;
        y = MathF.Abs(y - heightmap.Height);
        float rotation = 0;
        float height = heightmap.GetPixel((int)x,(int)y).GetBrightness();
        Position a = new Position(x, y, rotation, height);
        City = a;
        Unit = a;
        Text = a;
        Port = a;
        TradeNode = a;
        Fight = a;
        TradeWind = a;
    }
    public static int MAXC = 0;
    public string stringify(int i)
    {
        MAXC++;
        return @$"{i} = {{ #{MAXC}
    position = {{
        {City.x:0}.000 {City.y:0}.000 {Unit.x:0}.000 {Unit.y:0}.000 {Text.x:0}.000 {Text.y:0}.000 {Port.x:0}.000 {Port.y:0}.000 {TradeNode.x:0}.000 {TradeNode.y:0}.000 {Fight.x:0}.000 {Fight.y:0}.000 {TradeWind.x:0}.000 {TradeWind.y:0}.000
    }}
    rotation = {{
        {City.rotation:0.0000} {Unit.rotation:0.0000} {Text.rotation:0.0000} {Port.rotation:0.0000} {TradeNode.rotation:0.0000} {Fight.rotation:0.0000} {TradeWind.rotation:0.0000}
    }}
    height = {{
        {City.height:0.0000} {Unit.height:0.0000} {Text.height:0.0000} {Port.height:0.0000} {TradeNode.height:0.0000} {Fight.height:0.0000} {TradeWind.height:0.0000}
    }}
}}
";
    }
}
class Pixel
{
    public int x;
    public int y;
    public Pixel(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
class Position
{
    public float x;
    public float y;
    public float rotation;
    public float height;
    public Position (float X, float Y, float Rotation, float Height)
    {
        x = X;
        y = Y;
        rotation = Rotation;
        height = Height;
    }
}