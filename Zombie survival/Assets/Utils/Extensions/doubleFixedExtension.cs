public static class doubleFixedExtension
{
    public static string toFixed(this float v)
    {
        string r = "";

        r = v.ToString();
        r = r.Replace(',', '.');

        return r;
    }
}
