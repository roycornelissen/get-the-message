namespace Microsoft.Extensions.Hosting;

public static class CoinFlipper
{
    public static bool FlipCoin()
    {
        return new Random().Next(2) == 0;
    }
}